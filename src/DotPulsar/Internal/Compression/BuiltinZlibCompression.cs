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
using System.Reflection;

public static class BuiltinZlibCompression
{
    public static bool TryLoading(out ICompressorFactory? compressorFactory, out IDecompressorFactory? decompressorFactory)
    {
        try
        {
            var assembly = Assembly.Load("System.IO.Compression");
            var types = assembly.GetTypes();

            // Find the ZLibStream
            var zlibStreamType = types.FirstOrDefault(t => t.FullName == "System.IO.Compression.ZLibStream");

            // Find the enum values for the ZLibStream constructors
            var compressionLevelType = types.FirstOrDefault(t => t.FullName == "System.IO.Compression.CompressionLevel");
            var compressionLevelOptimal = compressionLevelType?.GetEnumValues().GetValue(0);
            var compressionModeType = types.FirstOrDefault(t => t.FullName == "System.IO.Compression.CompressionMode");
            var compressionModeDecompress = compressionModeType?.GetEnumValues().GetValue(0);

            compressorFactory = new CompressorFactory(PulsarApi.CompressionType.Zlib, () => new Compressor(CreateCompressor(zlibStreamType, compressionLevelOptimal)));
            decompressorFactory = new DecompressorFactory(PulsarApi.CompressionType.Zlib, () => new Decompressor(CreateDecompressor(zlibStreamType, compressionModeDecompress)));

            return CompressionTester.TestCompression(compressorFactory, decompressorFactory);
        }
        catch
        {
            // Ignore
        }

        compressorFactory = null;
        decompressorFactory = null;

        return false;
    }

    private static Func<ReadOnlySequence<byte>, ReadOnlySequence<byte>> CreateCompressor(Type? zlibStreamType, object? compressionLevelOptimal)
        => data =>
        {
            using var dataStream = new MemoryStream(data.ToArray());
            using var compressedStream = new MemoryStream();
            using var compressor = (Stream) Activator.CreateInstance(zlibStreamType!, [compressedStream, compressionLevelOptimal!])!;
            dataStream.CopyTo(compressor);
            compressor.Close();
            return new ReadOnlySequence<byte>(compressedStream.ToArray());
        };

    private static Func<ReadOnlySequence<byte>, int, ReadOnlySequence<byte>> CreateDecompressor(Type? zlibStreamType, object? compressionModeDecompress)
        => (data, _) =>
        {
            using var dataStream = new MemoryStream(data.ToArray());
            using var decompressedStream = new MemoryStream();
            using var decompressor = (Stream) Activator.CreateInstance(zlibStreamType!, [dataStream, compressionModeDecompress!])!;
            decompressor.CopyTo(decompressedStream);
            decompressor.Close();
            decompressedStream.Position = 0;
            return new ReadOnlySequence<byte>(decompressedStream.ToArray());
        };
}
