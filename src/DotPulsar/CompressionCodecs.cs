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

namespace DotPulsar;

using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Compression;
using System.Buffers;

/// <summary>
/// Explicit registration of compression codecs.
/// </summary>
/// <remarks>
/// By default DotPulsar discovers LZ4 (K4os.Compression.LZ4), Snappy (IronSnappy) and Zstd (ZstdSharp.Port or ZstdNet) via reflection.
/// That discovery does not work in trimmed or Native AOT applications, because the codec assemblies are not statically referenced.
/// Applications that are trimmed or compiled ahead-of-time should register their codecs explicitly with this class before
/// creating producers, consumers or readers. Explicitly registered codecs always take precedence over discovered codecs.
/// Reflection-based discovery can be disabled with the 'DotPulsar.Compression.EnableReflectionDiscovery' runtime feature switch.
/// </remarks>
public static class CompressionCodecs
{
    /// <summary>
    /// Register a codec from a pair of thread-safe compress and decompress functions.
    /// </summary>
    /// <param name="compressionType">The compression type the codec implements.</param>
    /// <param name="compress">Compresses the input. Must be thread-safe.</param>
    /// <param name="decompress">Decompresses the input. The second argument is the expected decompressed size. Must be thread-safe.</param>
    /// <exception cref="CompressionException">Thrown if the codec fails a compress/decompress round-trip test.</exception>
    public static void Register(
        CompressionType compressionType,
        Func<ReadOnlySequence<byte>, ReadOnlySequence<byte>> compress,
        Func<ReadOnlySequence<byte>, int, ReadOnlySequence<byte>> decompress)
    {
        if (compress is null)
            throw new ArgumentNullException(nameof(compress));
        if (decompress is null)
            throw new ArgumentNullException(nameof(decompress));

        Register(compressionType, () => new Compressor(compress), () => new Decompressor(decompress));
    }

    /// <summary>
    /// Register a codec from factories creating compressor and decompressor instances.
    /// </summary>
    /// <remarks>
    /// Use this overload for codecs with state that is not thread-safe (like a Zstd compression context).
    /// A new instance is created for each producer and consumer/reader and is disposed when it is no longer used.
    /// </remarks>
    /// <param name="compressionType">The compression type the codec implements.</param>
    /// <param name="createCompressor">Creates a new compressor.</param>
    /// <param name="createDecompressor">Creates a new decompressor.</param>
    /// <exception cref="CompressionException">Thrown if the codec fails a compress/decompress round-trip test.</exception>
    public static void Register(CompressionType compressionType, Func<ICompress> createCompressor, Func<IDecompress> createDecompressor)
    {
        if (compressionType == CompressionType.None)
            throw new ArgumentException("A codec cannot be registered for CompressionType.None", nameof(compressionType));
        if (createCompressor is null)
            throw new ArgumentNullException(nameof(createCompressor));
        if (createDecompressor is null)
            throw new ArgumentNullException(nameof(createDecompressor));

        var type = (Internal.PulsarApi.CompressionType) compressionType;
        var compressorFactory = new CompressorFactory(type, createCompressor);
        var decompressorFactory = new DecompressorFactory(type, createDecompressor);

        bool works;
        try
        {
            works = CompressionTester.TestCompression(compressorFactory, decompressorFactory);
        }
        catch (Exception exception)
        {
            throw new CompressionException($"The {compressionType} codec failed a compress/decompress round-trip test: {exception.Message}");
        }

        if (!works)
            throw new CompressionException($"The {compressionType} codec failed a compress/decompress round-trip test");

        CompressionFactories.Register(compressorFactory, decompressorFactory);
    }
}
