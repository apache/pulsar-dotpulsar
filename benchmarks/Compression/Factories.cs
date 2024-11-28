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

namespace Compression;

using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Compression;

public static class Factories
{
    static Factories()
    {
        if (!Lz4Compression.TryLoading(out var compressor, out var decompressor))
            throw new Exception("Could not load K4os.Compression.LZ4");
        K4aosCompressionLz4CompressorFactory = compressor!;
        K4aosCompressionLz4DecompressorFactory = decompressor!;

        if (!SnappyCompression.TryLoading(out compressor, out decompressor))
            throw new Exception("Could not load IronSnappy");
        IronSnappyCompressorFactory = compressor!;
        IronSnappyDecompressorFactory = decompressor!;

        if (!BuiltinZlibCompression.TryLoading(out compressor, out decompressor))
            throw new Exception("Could not load BuiltinZlibCompression");
        BuiltinZlibCompressionCompressorFactory = compressor!;
        BuiltinZlibCompressionDecompressorFactory = decompressor!;

        if (!ZlibCompression.TryLoading(out compressor, out decompressor))
            throw new Exception("Could not load DotNetZip");
        DotNetZipCompressorFactory = compressor!;
        DotNetZipDecompressorFactory = decompressor!;

        if (!ZstdCompression.TryLoading(out compressor, out decompressor))
            throw new Exception("Could not load ZstdNet");
        ZstdNetCompressorFactory = compressor!;
        ZstdNetDecompressorFactory = decompressor!;

        if (!ZstdSharpCompression.TryLoading(out compressor, out decompressor))
            throw new Exception("Could not load ZstdSharp");
        ZstdSharpCompressorFactory = compressor!;
        ZstdSharpDecompressorFactory = decompressor!;
    }

    public static ICompressorFactory K4aosCompressionLz4CompressorFactory { get; }
    public static IDecompressorFactory K4aosCompressionLz4DecompressorFactory { get; }

    public static ICompressorFactory IronSnappyCompressorFactory { get; }
    public static IDecompressorFactory IronSnappyDecompressorFactory { get; }

    public static ICompressorFactory BuiltinZlibCompressionCompressorFactory { get; }
    public static IDecompressorFactory BuiltinZlibCompressionDecompressorFactory { get; }

    public static ICompressorFactory DotNetZipCompressorFactory { get; }
    public static IDecompressorFactory DotNetZipDecompressorFactory { get; }

    public static ICompressorFactory ZstdNetCompressorFactory { get; }
    public static IDecompressorFactory ZstdNetDecompressorFactory { get; }

    public static ICompressorFactory ZstdSharpCompressorFactory { get; }
    public static IDecompressorFactory ZstdSharpDecompressorFactory { get; }
}
