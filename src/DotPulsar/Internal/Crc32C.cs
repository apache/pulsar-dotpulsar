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

using System.Buffers;

public static class Crc32C
{
    private const uint _generator = 0x82F63B78u;

    private static readonly uint[] _lookup;

    static Crc32C()
    {
        _lookup = new uint[16 * 256];

        for (uint i = 0; i < 256; i++)
        {
            var entry = i;

            for (var j = 0; j < 16; j++)
            {
                for (var k = 0; k < 8; k++)
                    entry = (entry & 1) == 1 ? _generator ^ (entry >> 1) : entry >> 1;

                _lookup[j * 256 + i] = entry;
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
                    checksum = _lookup[(byte) (checksum ^ currentByte)] ^ (checksum >> 8);
                    continue;
                }

                var offSetBase = offset * 256;

                if (offset > 11)
                    block[offset] = _lookup[offSetBase + ((byte) (checksum >> (8 * (15 - offset))) ^ currentByte)];
                else
                    block[offset] = _lookup[offSetBase + currentByte];

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
