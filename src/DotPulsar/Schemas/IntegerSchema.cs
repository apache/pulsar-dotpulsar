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

namespace DotPulsar.Schemas;

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using System;
using System.Buffers;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

/// <summary>
/// Schema definition for Integer (int32) messages.
/// </summary>
public sealed class IntegerSchema : ISchema<int>
{
    public IntegerSchema()
        => SchemaInfo = new SchemaInfo("INT32", Array.Empty<byte>(), SchemaType.Int32, ImmutableDictionary<string, string>.Empty);

    public SchemaInfo SchemaInfo { get; }

    public int Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {
        if (bytes.Length != 4)
            throw new SchemaSerializationException($"{nameof(IntegerSchema)} expected to decode 4 bytes, but received {bytes} bytes");

        var array = bytes.ToArray();

        if (BitConverter.IsLittleEndian)
            Array.Reverse(array);

        return MemoryMarshal.Read<int>(array);
    }

    public ReadOnlySequence<byte> Encode(int message)
    {
        var array = BitConverter.GetBytes(message);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(array);

        return new(array);
    }
}
