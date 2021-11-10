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

/// <summary>
/// Schema definition for Time (TimeSpan) messages.
/// </summary>
public sealed class TimeSchema : ISchema<TimeSpan>
{
    public TimeSchema()
        => SchemaInfo = new SchemaInfo("Time", Array.Empty<byte>(), SchemaType.Time, ImmutableDictionary<string, string>.Empty);

    public SchemaInfo SchemaInfo { get; }

    public TimeSpan Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {
        if (bytes.Length != 8)
            throw new SchemaSerializationException($"{nameof(TimestampSchema)} expected to decode 8 bytes, but received {bytes} bytes");

        var milliseconds = Schema.Int64.Decode(bytes);
        return TimeSpan.FromMilliseconds(milliseconds);
    }

    public ReadOnlySequence<byte> Encode(TimeSpan message)
    {
        var milliseconds = (long) message.TotalMilliseconds;
        return Schema.Int64.Encode(milliseconds);
    }
}
