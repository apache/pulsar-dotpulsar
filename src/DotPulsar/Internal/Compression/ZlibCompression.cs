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

namespace DotPulsar.Internal.Compression
{
    using DotPulsar.Internal.Abstractions;
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ZlibCompression
    {
        public delegate byte[] CompressBuffer(byte[] source);
        public delegate byte[] UncompressBuffer(byte[] source);

        public static bool TryLoading(out ICompressorFactory? compressorFactory, out IDecompressorFactory? decompressorFactory)
        {
            try
            {
                var assembly = Assembly.Load("DotNetZip");

                var definedTypes = assembly.DefinedTypes.ToArray();

                var ZlibStream = FindZlibStream(definedTypes);

                var methods = ZlibStream.GetMethods(BindingFlags.Public | BindingFlags.Static);

                var compressBuffer = FindCompressBuffer(methods);
                var uncompressBuffer = FindUncompressBuffer(methods);

                compressorFactory = new CompressorFactory(() => new Compressor(CreateCompressor(compressBuffer)));
                decompressorFactory = new DecompressorFactory(() => new Decompressor(CreateDecompressor(uncompressBuffer)));
                return true;
            }
            catch
            {
                // Ignore
            }

            compressorFactory = null;
            decompressorFactory = null;

            return false;
        }

        private static TypeInfo FindZlibStream(IEnumerable<TypeInfo> types)
        {
            const string fullName = "Ionic.Zlib.ZlibStream";

            foreach (var type in types)
            {
                if (type.FullName is null || !type.FullName.Equals(fullName))
                    continue;

                if (type.IsPublic && type.IsClass)
                    return type;

                break;
            }

            throw new Exception($"{fullName} as a public class was not found");
        }

        private static CompressBuffer FindCompressBuffer(MethodInfo[] methods)
        {
            const string name = "CompressBuffer";

            foreach (var method in methods)
            {
                if (method.Name != name || method.ReturnType != typeof(byte[]))
                    continue;

                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType != typeof(byte[]))
                    continue;

                return (CompressBuffer) method.CreateDelegate(typeof(CompressBuffer));
            }

            throw new Exception($"A method with the name '{name}' matching the delegate was not found");
        }

        private static UncompressBuffer FindUncompressBuffer(MethodInfo[] methods)
        {
            const string name = "UncompressBuffer";

            foreach (var method in methods)
            {
                if (method.Name != name || method.ReturnType != typeof(byte[]))
                    continue;

                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType != typeof(byte[]))
                    continue;

                return (UncompressBuffer) method.CreateDelegate(typeof(UncompressBuffer));
            }

            throw new Exception($"A method with the name '{name}' matching the delegate was not found");
        }

        private static Func<ReadOnlySequence<byte>, int, ReadOnlySequence<byte>> CreateDecompressor(UncompressBuffer decompress)
            => (source, size) => new ReadOnlySequence<byte>(decompress(source.ToArray()));

        private static Func<ReadOnlySequence<byte>, ReadOnlySequence<byte>> CreateCompressor(CompressBuffer compress)
            => (source) => new ReadOnlySequence<byte>(compress(source.ToArray()));
    }
}
