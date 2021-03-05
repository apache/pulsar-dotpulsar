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

namespace DotPulsar.Schemas
{
    using DotPulsar.Abstractions;
    using DotPulsar.Exceptions;
    using System;
    using System.Buffers;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Schema definition for Float messages.
    /// </summary>
    public sealed class FloatSchema : ISchema<float>
    {
        public FloatSchema()
            => SchemaInfo = new SchemaInfo("Float", Array.Empty<byte>(), SchemaType.Float, ImmutableDictionary<string, string>.Empty);

        public SchemaInfo SchemaInfo { get; }

        public float Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
        {
            if (bytes.Length != 4)
                throw new SchemaSerializationException($"{nameof(FloatSchema)} expected to decode 4 bytes, but received {bytes} bytes");

            var array = bytes.ToArray();

            if (BitConverter.IsLittleEndian)
                Array.Reverse(array);

            return MemoryMarshal.Read<float>(array);
        }

        public ReadOnlySequence<byte> Encode(float message)
        {
            var array = BitConverter.GetBytes(message);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(array);

            return new(array);
        }
    }
}
