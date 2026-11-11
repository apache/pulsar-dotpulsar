/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Extensions;
using DotPulsar.Internal.Compression;
using Google.Protobuf.WellKnownTypes;
using IronSnappy;
using K4os.Compression.LZ4;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

var failures = 0;

void Check(string name, bool passed)
{
    Console.WriteLine($"{(passed ? "PASS" : "FAIL")}: {name}");
    if (!passed)
        failures++;
}

// False when published as Native AOT, and also when running a PublishAot project on the JIT (the SDK sets the switch to match)
var dynamicCodeSupported = RuntimeFeature.IsDynamicCodeSupported;
Console.WriteLine($"Dynamic code supported: {dynamicCodeSupported}");

// Reflection-based codec discovery must be off (and trimmed away) under Native AOT
if (!dynamicCodeSupported)
    Check("Reflection-based compression discovery is disabled", !CompressionFactories.IsReflectionDiscoveryEnabled);

RegisterCodecs();

foreach (var type in new[] { CompressionType.Lz4, CompressionType.Snappy, CompressionType.Zlib, CompressionType.Zstd })
{
    var pulsarType = (DotPulsar.Internal.PulsarApi.CompressionType) type;
    var compressor = CompressionFactories.CompressorFactories().SingleOrDefault(f => f.CompressionType == pulsarType);
    var decompressor = CompressionFactories.DecompressorFactories().SingleOrDefault(f => f.CompressionType == pulsarType);
    Check($"{type} codec round-trips", compressor is not null && decompressor is not null && CompressionTester.TestCompression(compressor, decompressor));
}

var jsonSchema = Schema.Json(SmokeJsonContext.Default.Order);
var order = new Order { Id = 42, Lines = ["a", "b"], Status = OrderStatus.Shipped };
var decodedOrder = jsonSchema.Decode(jsonSchema.Encode(order));
Check("JSON schema round-trips", decodedOrder.Id == 42 && decodedOrder.Lines.SequenceEqual(order.Lines) && decodedOrder.Status == OrderStatus.Shipped);
Check("JSON schema definition is generated", System.Text.Encoding.UTF8.GetString(jsonSchema.SchemaInfo.Data).Contains("\"symbols\":[\"New\",\"Shipped\"]"));

var protobufSchema = Schema.Protobuf<Timestamp>();
var timestamp = Timestamp.FromDateTimeOffset(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero));
Check("Protobuf schema round-trips", protobufSchema.Decode(protobufSchema.Encode(timestamp)).Equals(timestamp));

var serviceUrl = Environment.GetEnvironmentVariable("PULSAR_SERVICE_URL");
if (string.IsNullOrEmpty(serviceUrl))
{
    Console.WriteLine("SKIP: broker round-trip (PULSAR_SERVICE_URL is not set)");
}
else
{
    await using var client = PulsarClient.Builder().ServiceUrl(new Uri(serviceUrl)).Build();

    foreach (var type in new[] { CompressionType.None, CompressionType.Lz4, CompressionType.Snappy, CompressionType.Zlib, CompressionType.Zstd })
    {
        try
        {
            Check($"Broker round-trip with {type} compression", await BrokerRoundTrip(client, jsonSchema, type));
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            Check($"Broker round-trip with {type} compression", false);
        }
    }
}

Console.WriteLine(failures == 0 ? "All checks passed" : $"{failures} check(s) failed");
return failures == 0 ? 0 : 1;

static async Task<bool> BrokerRoundTrip(IPulsarClient client, ISchema<Order> schema, CompressionType compressionType)
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
    var topic = $"persistent://public/default/aot-smoke-{compressionType}-{Guid.NewGuid():N}";

    await using var consumer = client.NewConsumer(schema)
        .SubscriptionName("aot-smoke")
        .Topic(topic)
        .InitialPosition(SubscriptionInitialPosition.Earliest)
        .Create();

    await using var producer = client.NewProducer(schema)
        .Topic(topic)
        .CompressionType(compressionType)
        .Create();

    // Large, repetitive payload so compression is actually applied
    var sent = new Order { Id = 7, Lines = Enumerable.Repeat("compressible line", 200).ToList(), Status = OrderStatus.New };
    await producer.Send(sent, cts.Token);

    var message = await consumer.Receive(cts.Token);
    await consumer.Acknowledge(message, cts.Token);
    var received = message.Value();

    return received.Id == sent.Id && received.Lines.SequenceEqual(sent.Lines) && received.Status == sent.Status;
}

// Explicit codec registration, as trimmed and Native AOT applications must do
static void RegisterCodecs()
{
    CompressionCodecs.Register(
        CompressionType.Lz4,
        data =>
        {
            var source = data.ToArray();
            var target = new byte[LZ4Codec.MaximumOutputSize(source.Length)];
            var length = LZ4Codec.Encode(source, 0, source.Length, target, 0, target.Length, LZ4Level.L00_FAST);
            if (length < 0)
                throw new CompressionException("LZ4 compression failed");
            return new ReadOnlySequence<byte>(target, 0, length);
        },
        (data, decompressedSize) =>
        {
            var source = data.ToArray();
            var target = new byte[decompressedSize];
            var length = LZ4Codec.Decode(source, 0, source.Length, target, 0, target.Length);
            if (length != decompressedSize)
                throw new CompressionException("LZ4 decompression failed");
            return new ReadOnlySequence<byte>(target);
        });

    CompressionCodecs.Register(
        CompressionType.Snappy,
        data => new ReadOnlySequence<byte>(Snappy.Encode(data.ToArray())),
        (data, _) => new ReadOnlySequence<byte>(Snappy.Decode(data.ToArray())));

    // ZstdSharp compressors/decompressors are not thread-safe, so create one per producer/consumer
    CompressionCodecs.Register(
        CompressionType.Zstd,
        () =>
        {
            var compressor = new ZstdSharp.Compressor();
            return new Compressor(data => new ReadOnlySequence<byte>(compressor.Wrap(data.ToArray()).ToArray()), compressor);
        },
        () =>
        {
            var decompressor = new ZstdSharp.Decompressor();
            return new Decompressor((data, decompressedSize) => new ReadOnlySequence<byte>(decompressor.Unwrap(data.ToArray(), decompressedSize).ToArray()), decompressor);
        });
}

public enum OrderStatus { New, Shipped }

public sealed class Order
{
    public int Id { get; set; }
    public List<string> Lines { get; set; } = [];
    public OrderStatus Status { get; set; }
}

[JsonSerializable(typeof(Order))]
public sealed partial class SmokeJsonContext : JsonSerializerContext;
