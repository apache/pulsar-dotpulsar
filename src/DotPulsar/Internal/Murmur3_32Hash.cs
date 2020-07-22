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

namespace DotPulsar.Internal
{
    using System.IO;
    /// <summary>
    /// Implementation of the MurmurHash3 non-cryptographic hash function.
    /// </summary>
    public class Murmur3_32Hash
    {
        private static Murmur3_32Hash Instance { get; } = new Murmur3_32Hash();

        private static readonly uint CHUNK_SIZE = 4;
        private static readonly uint C1 = 0xcc9e2d51;
        private static readonly uint C2 = 0x1b873593;
        private readonly uint _seed;

        private Murmur3_32Hash(uint seed = 0)
        {
            _seed = seed;
        }

        public static int Hash(byte[] b)
        {
            return Instance.MakeHash(b);
        }

        public int MakeHash(byte[] b)
        {
            return MakeHash0(b) & int.MaxValue;
        }

        private int MakeHash0(byte[] bytes)
        {
            uint len = (uint) bytes.Length;
            uint reminder = len % CHUNK_SIZE;
            uint chunkCount = len / CHUNK_SIZE;
            uint h1 = _seed;

            BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes));

            uint k1;
            while (chunkCount-- != 0)
            {
                k1 = binaryReader.ReadUInt32();

                k1 = MixK1(k1);
                h1 = MixH1(h1, k1);
            }

            k1 = 0;
            for (int i = 0; i < reminder; i++)
            {
                k1 ^= (uint) ((binaryReader.ReadByte() & 255) << (i * 8));
            }

            h1 ^= MixK1(k1);
            h1 ^= len;
            h1 = Fmix(h1);

            return (int) h1;
        }

        private uint Fmix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;

            return h;
        }

        private uint MixK1(uint k1)
        {
            k1 *= C1;
            k1 = (k1 << 15) | (k1 >> 15);
            k1 *= C2;
            return k1;
        }

        private uint MixH1(uint h1, uint k1)
        {
            h1 ^= k1;
            h1 = (h1 << 13) | (h1 >> 13);
            return h1 * 5 + 0xe6546b64;
        }
    }
}
