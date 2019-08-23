using System.Buffers;

namespace DotPulsar.Internal
{
    public static class Crc32C
    {
        private const uint Generator = 0x82F63B78u;

        private static readonly uint[] Lookup;

        static Crc32C()
        {
            Lookup = new uint[16 * 256];

            for (uint i = 0; i < 256; i++)
            {
                var entry = i;
                for (var j = 0; j < 16; j++)
                {
                    for (var k = 0; k < 8; k++)
                    {
                        entry = (entry & 1) == 1 ? Generator ^ (entry >> 1) : (entry >> 1);
                    }
                    Lookup[(j * 256) + i] = entry;
                }
            }
        }

        public static uint Calculate(ReadOnlySequence<byte> sequence)
        {
            var block = new uint[16];
            var checksum = uint.MaxValue;
            var remaningBytes = sequence.Length;
            var readingBlock = remaningBytes >= 16;
            var offset = 15;

            foreach (var memory in sequence)
            {
                var span = memory.Span;
                for (var i = 0; i < span.Length; ++i)
                {
                    var currentByte = span[i];

                    if (!readingBlock)
                    {
                        checksum = Lookup[(byte)(checksum ^ currentByte)] ^ checksum >> 8;
                        continue;
                    }

                    var offSetBase = offset * 256;

                    if (offset > 11)
                        block[offset] = Lookup[offSetBase + ((byte)(checksum >> (8 * (15 - offset))) ^ currentByte)];
                    else
                        block[offset] = Lookup[offSetBase + currentByte];

                    --remaningBytes;

                    if (offset == 0)
                    {
                        offset = 15;
                        readingBlock = remaningBytes >= 16;
                        checksum = 0;
                        for (var j = 0; j < block.Length; ++j)
                            checksum ^= block[j];
                    }
                    else
                    {
                        --offset;
                    }
                }
            }

            return checksum ^ uint.MaxValue;
        }
    }
}
