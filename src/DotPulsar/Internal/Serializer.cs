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

namespace DotPulsar.Internal;

using Pulsar.Proto;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using System.Buffers;

public static class Serializer
{
    public static T Deserialize<T>(ReadOnlySequence<byte> sequence)
    {
        var message = PulsarApiReflection.Descriptor.FindTypeByName<MessageDescriptor>(typeof(T).Name).Parser.ParseFrom(sequence);
        return (T) message;
    }

    public static ReadOnlySequence<byte> Serialize(BaseCommand command)
    {
        var commandBytes = Serialize<BaseCommand>(command);
        var commandSizeBytes = ToBigEndianBytes((uint) commandBytes.Length);
        var totalSizeBytes = ToBigEndianBytes((uint) commandBytes.Length + 4);

        return new SequenceBuilder<byte>()
            .Append(totalSizeBytes)
            .Append(commandSizeBytes)
            .Append(commandBytes)
            .Build();
    }

    public static ReadOnlySequence<byte> Serialize(BaseCommand command, MessageMetadata metadata, ReadOnlySequence<byte> payload)
    {
        var commandBytes = Serialize<BaseCommand>(command);
        var commandSizeBytes = ToBigEndianBytes((uint) commandBytes.Length);

        var metadataBytes = Serialize(metadata);
        var metadataSizeBytes = ToBigEndianBytes((uint) metadataBytes.Length);

        var sb = new SequenceBuilder<byte>().Append(metadataSizeBytes).Append(metadataBytes).Append(payload);
        var checksum = Crc32C.Calculate(sb.Build());

        return sb.Prepend(ToBigEndianBytes(checksum))
            .Prepend(Constants.MagicNumber)
            .Prepend(commandBytes)
            .Prepend(commandSizeBytes)
            .Prepend(ToBigEndianBytes((uint) sb.Length))
            .Build();
    }

    public static byte[] ToBigEndianBytes(uint integer)
    {
        var union = new UIntUnion(integer);

        return BitConverter.IsLittleEndian
            ? [union.B3, union.B2, union.B1, union.B0]
            : [union.B0, union.B1, union.B2, union.B3];
    }

    private static byte[] Serialize<T>(T item) where T : IMessage => item.ToByteArray();
}
