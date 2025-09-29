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

public static class MurmurHash3
{
    private const int UintSize = sizeof(uint);

    public static uint Hash32(ReadOnlySpan<byte> buffer, uint seed)
    {
        var length = buffer.Length;
        var hash = seed;
        var quotient = Math.DivRem(length, UintSize, out int remainder);
        var i = 0;
        var blocks = quotient * UintSize;
        while (i < blocks)
        {
#if NETSTANDARD2_0
            var value = BitConverter.ToUInt32(buffer.Slice(i, UintSize).ToArray(), 0);
#else
            var value = BitConverter.ToUInt32(buffer.Slice(i, UintSize));
#endif
            Round32(ref value, ref hash);
            hash = RotateLeft(hash, 13);
            hash *= 5;
            hash += 0xe6546b64;
            i += UintSize;
        }

        if (remainder > 0)
        {
            var remaining = PartialBytesToUInt32(buffer.Slice(i, remainder));
            Round32(ref remaining, ref hash);
        }

        hash ^= (uint) length;
        hash ^= hash >> 16;
        hash *= 0x85ebca6b;
        hash ^= hash >> 13;
        hash *= 0xc2b2ae35;
        hash ^= hash >> 16;
        return hash;
    }

    private static uint RotateLeft(uint value, int offset) => (value << offset) | (value >> (32 - offset));

    private static void Round32(ref uint value, ref uint hash)
    {
        value *= 0xcc9e2d51;
        value = RotateLeft(value, 15);
        value *= 0x1b873593;
        hash ^= value;
    }

#if NETSTANDARD2_0
    private static uint PartialBytesToUInt32(ReadOnlySpan<byte> remainingBytes)
    {
        var buffer = new byte[UintSize];
        remainingBytes.CopyTo(buffer);
        return BitConverter.ToUInt32(buffer, 0);
    }
#else
    private static uint PartialBytesToUInt32(ReadOnlySpan<byte> remainingBytes)
    {
        Span<byte> buffer = stackalloc byte[UintSize];
        remainingBytes.CopyTo(buffer);
        return BitConverter.ToUInt32(buffer);
    }
#endif
}
