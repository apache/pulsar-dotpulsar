using DotPulsar.Internal.PulsarApi;
using System;
using System.Buffers;
using System.IO;

namespace DotPulsar.Internal
{
    public static class Serializer
    {
        public static T Deserialize<T>(ReadOnlySequence<byte> sequence)
        {
            using var ms = new MemoryStream(sequence.ToArray()); //TODO Fix this when protobuf-net start supporting sequences or .NET supports creating a stream from a sequence
            return ProtoBuf.Serializer.Deserialize<T>(ms);
        }

        public static ReadOnlySequence<byte> Serialize(BaseCommand command)
        {
            var commandBytes = Serialize<BaseCommand>(command);
            var commandSizeBytes = ToBigEndianBytes((uint)commandBytes.Length);
            var totalSizeBytes = ToBigEndianBytes((uint)commandBytes.Length + 4);

            return new SequenceBuilder<byte>()
                .Append(totalSizeBytes)
                .Append(commandSizeBytes)
                .Append(commandBytes)
                .Build();
        }

        public static ReadOnlySequence<byte> Serialize(BaseCommand command, PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> payload)
        {
            var commandBytes = Serialize<BaseCommand>(command);
            var commandSizeBytes = ToBigEndianBytes((uint)commandBytes.Length);

            var metadataBytes = Serialize(metadata);
            var metadataSizeBytes = ToBigEndianBytes((uint)metadataBytes.Length);

            var sb = new SequenceBuilder<byte>().Append(metadataSizeBytes).Append(metadataBytes).Append(payload);
            var checksum = Crc32C.Calculate(sb.Build());

            return sb.Prepend(ToBigEndianBytes(checksum))
                .Prepend(Constants.MagicNumber)
                .Prepend(commandBytes)
                .Prepend(commandSizeBytes)
                .Prepend(ToBigEndianBytes((uint)sb.Length))
                .Build();
        }

        public static byte[] ToBigEndianBytes(uint integer)
        {
            var union = new UIntUnion(integer);
            if (BitConverter.IsLittleEndian)
                return new[] { union.B3, union.B2, union.B1, union.B0 };
            else
                return new[] { union.B0, union.B1, union.B2, union.B3 };
        }

        private static byte[] Serialize<T>(T item)
        {
            using var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, item);
            return ms.ToArray();
        }
    }
}
