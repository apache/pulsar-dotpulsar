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

public static class CompressionFactories
{
    private static readonly List<ICompressorFactory> _compressorFactories;
    private static readonly List<IDecompressorFactory> _decompressorFactories;

    static CompressionFactories()
    {
        _compressorFactories = new List<ICompressorFactory>();
        _decompressorFactories = new List<IDecompressorFactory>();

        LoadSupportForLz4();
        LoadSupportForSnappy();
        LoadSupportForZlib();
        LoadSupportForZstd();
    }

    private static void LoadSupportForLz4()
    {
        if (Lz4Compression.TryLoading(out var compressorFactory, out var decompressorFactory))
            Add(compressorFactory, decompressorFactory);
    }

    private static void LoadSupportForSnappy()
    {
        if (SnappyCompression.TryLoading(out var compressorFactory, out var decompressorFactory))
            Add(compressorFactory, decompressorFactory);
    }

    private static void LoadSupportForZlib()
    {
        if (ZlibCompression.TryLoading(out var compressorFactory, out var decompressorFactory))
            Add(compressorFactory, decompressorFactory);
    }

    private static void LoadSupportForZstd()
    {
        if (ZstdSharpCompression.TryLoading(out var compressorFactory, out var decompressorFactory))
            Add(compressorFactory, decompressorFactory);
        else if (ZstdCompression.TryLoading(out compressorFactory, out decompressorFactory))
            Add(compressorFactory, decompressorFactory);
    }

    private static void Add(ICompressorFactory? compressorFactory, IDecompressorFactory? decompressorFactory)
    {
        _compressorFactories.Add(compressorFactory!);
        _decompressorFactories.Add(decompressorFactory!);
    }

    public static IEnumerable<ICompressorFactory> CompressorFactories()
        => _compressorFactories;

    public static IEnumerable<IDecompressorFactory> DecompressorFactories()
        => _decompressorFactories;
}
