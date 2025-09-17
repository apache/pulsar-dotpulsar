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

using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using System.Buffers;
using System.Reflection;

public static class Lz4Compression
{
    public delegate int Decode(byte[] source, int sourceOffset, int sourceLength, byte[] target, int targetOffset, int targetLength);
    public delegate int Encode(byte[] source, int sourceOffset, int sourceLength, byte[] target, int targetOffset, int targetLength, int level);
    public delegate int MaximumOutputSize(int length);

    public static bool TryLoading(out ICompressorFactory? compressorFactory, out IDecompressorFactory? decompressorFactory)
    {
        try
        {
            var assembly = Assembly.Load("K4os.Compression.LZ4");

            var definedTypes = assembly.DefinedTypes.ToArray();

            var lz4Codec = FindLZ4Codec(definedTypes);
            var lz4Level = FindLZ4Level(definedTypes);

            var methods = lz4Codec.GetMethods(BindingFlags.Public | BindingFlags.Static);

            var decode = FindDecode(methods);
            var encode = FindEncode(methods, lz4Level);
            var maximumOutputSize = FindMaximumOutputSize(methods);

            compressorFactory = new CompressorFactory(Pulsar.Proto.CompressionType.Lz4, () => new Compressor(CreateCompressor(encode, maximumOutputSize)));
            decompressorFactory = new DecompressorFactory(Pulsar.Proto.CompressionType.Lz4, () => new Decompressor(CreateDecompressor(decode)));

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

    private static TypeInfo FindLZ4Codec(IEnumerable<TypeInfo> types)
    {
        const string fullName = "K4os.Compression.LZ4.LZ4Codec";

        foreach (var type in types)
        {
            if (type.FullName is null || !type.FullName.Equals(fullName))
                continue;

            if (type.IsPublic && type.IsClass && type.IsAbstract && type.IsSealed)
                return type;

            break;
        }

        throw new Exception($"{fullName} as a public and static class was not found");
    }

    private static TypeInfo FindLZ4Level(IEnumerable<TypeInfo> types)
    {
        const string fullName = "K4os.Compression.LZ4.LZ4Level";

        foreach (var type in types)
        {
            if (type.FullName is null || !type.FullName.Equals(fullName))
                continue;

            if (type.IsPublic && type.IsEnum && Enum.GetUnderlyingType(type) == typeof(int))
                return type;

            break;
        }

        throw new Exception($"{fullName} as a public enum with an int backing was not found");
    }

    private static Decode FindDecode(MethodInfo[] methods)
    {
        const string name = "Decode";

        foreach (var method in methods)
        {
            if (method.Name != name || method.ReturnType != typeof(int))
                continue;

            var parameters = method.GetParameters();
            if (parameters.Length != 6)
                continue;

            if (parameters[0].ParameterType != typeof(byte[]) ||
                parameters[1].ParameterType != typeof(int) ||
                parameters[2].ParameterType != typeof(int) ||
                parameters[3].ParameterType != typeof(byte[]) ||
                parameters[4].ParameterType != typeof(int) ||
                parameters[5].ParameterType != typeof(int))
                continue;

            return (Decode) method.CreateDelegate(typeof(Decode));
        }

        throw new Exception($"A method with the name '{name}' matching the delegate was not found");
    }

    private static Encode FindEncode(MethodInfo[] methods, Type lz4Level)
    {
        const string name = "Encode";

        foreach (var method in methods)
        {
            if (method.Name != name || method.ReturnType != typeof(int))
                continue;

            var parameters = method.GetParameters();
            if (parameters.Length != 7)
                continue;

            if (parameters[0].ParameterType != typeof(byte[]) ||
                parameters[1].ParameterType != typeof(int) ||
                parameters[2].ParameterType != typeof(int) ||
                parameters[3].ParameterType != typeof(byte[]) ||
                parameters[4].ParameterType != typeof(int) ||
                parameters[5].ParameterType != typeof(int) ||
                parameters[6].ParameterType != lz4Level)
                continue;

            return (Encode) method.CreateDelegate(typeof(Encode));
        }

        throw new Exception($"A method with the name '{name}' matching the delegate was not found");
    }

    private static MaximumOutputSize FindMaximumOutputSize(MethodInfo[] methods)
    {
        const string name = "MaximumOutputSize";

        foreach (var method in methods)
        {
            if (method.Name != name || method.ReturnType != typeof(int))
                continue;

            var parameters = method.GetParameters();
            if (parameters.Length != 1)
                continue;

            if (parameters[0].ParameterType != typeof(int))
                continue;

            return (MaximumOutputSize) method.CreateDelegate(typeof(MaximumOutputSize));
        }

        throw new Exception($"A method with the name '{name}' matching the delegate was not found");
    }

    private static Func<ReadOnlySequence<byte>, int, ReadOnlySequence<byte>> CreateDecompressor(Decode decompress)
    {
        return (source, size) =>
        {
            var decompressed = new byte[size];
            var sourceBytes = source.ToArray();
            var bytesDecompressed = decompress(sourceBytes, 0, sourceBytes.Length, decompressed, 0, decompressed.Length);
            if (size == bytesDecompressed)
                return new ReadOnlySequence<byte>(decompressed);

            throw new CompressionException($"LZ4Codec.Decode returned {bytesDecompressed} but expected {size}");
        };
    }

    private static Func<ReadOnlySequence<byte>, ReadOnlySequence<byte>> CreateCompressor(Encode compress, MaximumOutputSize maximumOutputSize)
    {
        return (source) =>
        {
            var sourceBytes = source.ToArray();
            var compressed = new byte[maximumOutputSize(sourceBytes.Length)];
            var bytesCompressed = compress(sourceBytes, 0, sourceBytes.Length, compressed, 0, compressed.Length, 0);
            if (bytesCompressed == -1)
                throw new CompressionException($"LZ4Codec.Encode returned -1 when compressing {sourceBytes.Length} bytes");

            return new ReadOnlySequence<byte>(compressed, 0, bytesCompressed);
        };
    }
}
