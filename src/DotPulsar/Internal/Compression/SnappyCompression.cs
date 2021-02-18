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

    public static class SnappyCompression
    {
        public delegate byte[] Decode(ReadOnlySpan<byte> source);
        public delegate byte[] Encode(ReadOnlySpan<byte> source);

        public static bool TryLoading(out ICompressorFactory? compressorFactory, out IDecompressorFactory? decompressorFactory)
        {
            try
            {
                var assembly = Assembly.Load("IronSnappy");

                var definedTypes = assembly.DefinedTypes.ToArray();

                var snappy = FindSnappy(definedTypes);

                var methods = snappy.GetMethods(BindingFlags.Public | BindingFlags.Static);

                var decode = FindDecode(methods);
                var encode = FindEncode(methods);

                compressorFactory = new CompressorFactory(() => new Compressor(CreateCompressor(encode)));
                decompressorFactory = new DecompressorFactory(() => new Decompressor(CreateDecompressor(decode)));
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

        private static TypeInfo FindSnappy(IEnumerable<TypeInfo> types)
        {
            const string fullName = "IronSnappy.Snappy";

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

        private static Decode FindDecode(MethodInfo[] methods)
        {
            const string name = "Decode";

            foreach (var method in methods)
            {
                if (method.Name != name || method.ReturnType != typeof(byte[]))
                    continue;

                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType != typeof(ReadOnlySpan<byte>))
                    continue;

                return (Decode) method.CreateDelegate(typeof(Decode));
            }

            throw new Exception($"A method with the name '{name}' matching the delegate was not found");
        }

        private static Encode FindEncode(MethodInfo[] methods)
        {
            const string name = "Encode";

            foreach (var method in methods)
            {
                if (method.Name != name || method.ReturnType != typeof(byte[]))
                    continue;

                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    continue;

                if (parameters[0].ParameterType != typeof(ReadOnlySpan<byte>))
                    continue;

                return (Encode) method.CreateDelegate(typeof(Encode));
            }

            throw new Exception($"A method with the name '{name}' matching the delegate was not found");
        }

        private static Func<ReadOnlySequence<byte>, int, ReadOnlySequence<byte>> CreateDecompressor(Decode decompress)
            => (source, size) => new ReadOnlySequence<byte>(decompress(source.ToArray()));

        private static Func<ReadOnlySequence<byte>, ReadOnlySequence<byte>> CreateCompressor(Encode compress)
            => (source) => new ReadOnlySequence<byte>(compress(source.ToArray()));
    }
}
