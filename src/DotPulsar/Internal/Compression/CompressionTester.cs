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

namespace DotPulsar.Internal.Compression;

using DotPulsar.Internal.Abstractions;
using System.Buffers;

public static class CompressionTester
{
    public static bool TestCompression(ICompressorFactory compressorFactory, IDecompressorFactory decompressorFactory)
    {
        using var compressor = compressorFactory.Create();
        using var decompressor = decompressorFactory.Create();
        var data = System.Text.Encoding.UTF8.GetBytes("Test data");
        var compressedData = compressor.Compress(new ReadOnlySequence<byte>(data));
        var decompressed = decompressor.Decompress(compressedData, data.Length).ToArray();
        return data.SequenceEqual(decompressed);
    }
}
