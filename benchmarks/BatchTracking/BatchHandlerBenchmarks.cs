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

namespace BatchTracking;

using BenchmarkDotNet.Attributes;
using DotPulsar.Internal;
using DotPulsar.Internal.PulsarApi;
using Google.Protobuf;
using System.Buffers;
using System.Buffers.Binary;

[MemoryDiagnoser]
[ShortRunJob]
public class BatchLookupBenchmarks
{
    private BatchHandler<byte[]> _handler = null!;
    private MessageIdData _target = null!;

    [Params(1, 100, 1000, 10000)]
    public int OutstandingBatches { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _handler = BatchSetup.CreateHandler();
        var data = BatchSetup.CreateData(2);
        var metadata = BatchSetup.CreateMetadata(2);

        for (var entry = 0; entry < OutstandingBatches; ++entry)
        {
            var id = new MessageIdData { LedgerId = 1, EntryId = (ulong) entry, Partition = 0, BatchIndex = -1 };
            _handler.Add(id, 0, metadata, data);
        }

        // Keep the last batch incomplete so repeated invocations exercise lookup at the end of the tracker.
        _target = new MessageIdData { LedgerId = 1, EntryId = (ulong) (OutstandingBatches - 1), Partition = 0, BatchIndex = 0 };
    }

    [Benchmark]
    public MessageIdData? AcknowledgeIndex() => _handler.Acknowledge(_target);
}

[MemoryDiagnoser]
[ShortRunJob]
public class BatchCompletionBenchmarks
{
    private BatchHandler<byte[]> _handler = null!;
    private MessageIdData _target = null!;

    [Params(10, 100, 1000)]
    public int BatchSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _handler = BatchSetup.CreateHandler();
        _handler.Add(
            new MessageIdData { LedgerId = 1, EntryId = 1, Partition = 0, BatchIndex = -1 },
            0,
            BatchSetup.CreateMetadata(BatchSize),
            BatchSetup.CreateData(BatchSize));

        for (var index = 0; index < BatchSize - 1; ++index)
        {
            _handler.Acknowledge(new MessageIdData { LedgerId = 1, EntryId = 1, Partition = 0, BatchIndex = index });
        }

        // Repeatedly ACK a previously acknowledged index while the final one remains outstanding.
        _target = new MessageIdData { LedgerId = 1, EntryId = 1, Partition = 0, BatchIndex = BatchSize - 2 };
    }

    [Benchmark]
    public MessageIdData? AcknowledgeIndex() => _handler.Acknowledge(_target);
}

[MemoryDiagnoser]
[ShortRunJob]
public class BatchContentionBenchmarks
{
    private BatchHandler<byte[]> _handler = null!;
    private MessageIdData _target = null!;

    [Params(100, 1000)]
    public int OutstandingBatches { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _handler = BatchSetup.CreateHandler();
        var data = BatchSetup.CreateData(2);
        var metadata = BatchSetup.CreateMetadata(2);

        for (var entry = 0; entry < OutstandingBatches; ++entry)
        {
            var id = new MessageIdData { LedgerId = 1, EntryId = (ulong) entry, Partition = 0, BatchIndex = -1 };
            _handler.Add(id, 0, metadata, data);
        }

        // Drain the queue so the receive worker measures polling against the shared lock.
        while (_handler.GetNext() is not null) { }
        _target = new MessageIdData { LedgerId = 1, EntryId = (ulong) (OutstandingBatches - 1), Partition = 0, BatchIndex = 0 };
    }

    [Benchmark]
    public void ConcurrentAcknowledgementsAndReceives()
    {
        Parallel.For(0, 8, worker =>
        {
            for (var i = 0; i < 256; ++i)
            {
                if (worker == 0)
                    _handler.GetNext();
                else
                    _handler.Acknowledge(_target);
            }
        });
    }
}

internal static class BatchSetup
{
    public static BatchHandler<byte[]> CreateHandler()
        => new(true, new MessageFactory<byte[]>(DotPulsar.Schema.ByteArray));

    public static MessageMetadata CreateMetadata(int count)
        => new() { ProducerName = "benchmark", NumMessagesInBatch = count, PublishTime = 1, SequenceId = 1 };

    public static ReadOnlySequence<byte> CreateData(int count)
    {
        var singleMetadata = new SingleMessageMetadata { PayloadSize = 0 }.ToByteArray();
        var data = new byte[count * (4 + singleMetadata.Length)];
        for (var index = 0; index < count; ++index)
        {
            var offset = index * (4 + singleMetadata.Length);
            BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(offset, 4), (uint) singleMetadata.Length);
            singleMetadata.CopyTo(data.AsSpan(offset + 4));
        }

        return new ReadOnlySequence<byte>(data);
    }
}
