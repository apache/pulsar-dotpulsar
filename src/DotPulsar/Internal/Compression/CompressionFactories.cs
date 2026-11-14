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
using DotPulsar.Internal.PulsarApi;
using System.Diagnostics.CodeAnalysis;
#if NET
using System.Runtime.CompilerServices;
#endif

public static class CompressionFactories
{
    private const string ReflectionDiscoverySwitch = "DotPulsar.Compression.EnableReflectionDiscovery";

    private static readonly object _lock = new();
    private static readonly Lazy<(ICompressorFactory[] Compressors, IDecompressorFactory[] Decompressors)> _discovered = new(Discover);
    private static readonly Dictionary<CompressionType, (ICompressorFactory Compressor, IDecompressorFactory Decompressor)> _registered = [];

    /// <summary>
    /// Whether codecs are discovered via reflection. Enabled unless the 'DotPulsar.Compression.EnableReflectionDiscovery' switch is set to false
    /// or the application is running as Native AOT.
    /// When trimming, setting the switch to false (with Trim="true") removes the reflection-based discovery from the application.
    /// </summary>
    /// <remarks>
    /// Discovery does not need dynamic code, but Native AOT applications are always trimmed, so the codec assemblies (which are not
    /// statically referenced) are never present and discovery cannot succeed. RuntimeFeature.IsDynamicCodeSupported is a constant false
    /// under Native AOT, which lets the compiler remove the discovery code without the application having to set the switch.
    /// </remarks>
#if NET9_0_OR_GREATER
    [FeatureSwitchDefinition(ReflectionDiscoverySwitch)]
#endif
    public static bool IsReflectionDiscoveryEnabled
#if NET
        => RuntimeFeature.IsDynamicCodeSupported &&
           (!AppContext.TryGetSwitch(ReflectionDiscoverySwitch, out var isEnabled) || isEnabled);
#else
        => !AppContext.TryGetSwitch(ReflectionDiscoverySwitch, out var isEnabled) || isEnabled;
#endif

    public static void Register(ICompressorFactory compressorFactory, IDecompressorFactory decompressorFactory)
    {
        if (compressorFactory.CompressionType != decompressorFactory.CompressionType)
            throw new ArgumentException("The compressor and decompressor factories must have the same compression type");

        lock (_lock)
        {
            _registered[compressorFactory.CompressionType] = (compressorFactory, decompressorFactory);
        }
    }

    public static IEnumerable<ICompressorFactory> CompressorFactories()
    {
        lock (_lock)
        {
            return _registered.Values
                .Select(r => r.Compressor)
                .Concat(_discovered.Value.Compressors.Where(f => !_registered.ContainsKey(f.CompressionType)))
                .ToArray();
        }
    }

    public static IEnumerable<IDecompressorFactory> DecompressorFactories()
    {
        lock (_lock)
        {
            return _registered.Values
                .Select(r => r.Decompressor)
                .Concat(_discovered.Value.Decompressors.Where(f => !_registered.ContainsKey(f.CompressionType)))
                .ToArray();
        }
    }

    private static (ICompressorFactory[], IDecompressorFactory[]) Discover()
    {
        var compressorFactories = new List<ICompressorFactory>();
        var decompressorFactories = new List<IDecompressorFactory>();

        void Add(ICompressorFactory? compressorFactory, IDecompressorFactory? decompressorFactory)
        {
            compressorFactories.Add(compressorFactory!);
            decompressorFactories.Add(decompressorFactory!);
        }

#if NET6_0_OR_GREATER
        // ZLibStream is part of the BCL, so no reflection is needed.
        if (BuiltinZlibCompression.TryLoading(out var zlibCompressorFactory, out var zlibDecompressorFactory))
            Add(zlibCompressorFactory, zlibDecompressorFactory);
#endif

        if (IsReflectionDiscoveryEnabled)
            DiscoverViaReflection(Add);

        return (compressorFactories.ToArray(), decompressorFactories.ToArray());
    }

#if NET5_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Reflection-based discovery is optional, failures are handled, and it can be removed via the 'DotPulsar.Compression.EnableReflectionDiscovery' feature switch. Trimmed applications should use CompressionCodecs.Register.")]
#endif
    private static void DiscoverViaReflection(Action<ICompressorFactory?, IDecompressorFactory?> add)
    {
        if (Lz4Compression.TryLoading(out var compressorFactory, out var decompressorFactory))
            add(compressorFactory, decompressorFactory);

        if (SnappyCompression.TryLoading(out compressorFactory, out decompressorFactory))
            add(compressorFactory, decompressorFactory);

#if !NET6_0_OR_GREATER
        if (BuiltinZlibCompression.TryLoading(out compressorFactory, out decompressorFactory))
            add(compressorFactory, decompressorFactory);
#endif

        if (ZstdSharpCompression.TryLoading(out compressorFactory, out decompressorFactory))
            add(compressorFactory, decompressorFactory);
        else if (ZstdCompression.TryLoading(out compressorFactory, out decompressorFactory))
            add(compressorFactory, decompressorFactory);
    }
}
