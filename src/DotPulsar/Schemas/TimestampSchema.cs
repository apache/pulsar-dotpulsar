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
using System.Buffers;
using System.Collections.Immutable;

/// <summary>
/// Schema definition for Timestamp (DateTime) messages.
/// </summary>
public sealed class TimestampSchema : ISchema<DateTime>
{
    static TimestampSchema()
    {
        Timestamp = new TimestampSchema(new SchemaInfo("Timestamp", Array.Empty<byte>(), SchemaType.Timestamp, ImmutableDictionary<string, string>.Empty));
        Date = new TimestampSchema(new SchemaInfo("Date", Array.Empty<byte>(), SchemaType.Date, ImmutableDictionary<string, string>.Empty));
    }

    public static TimestampSchema Timestamp { get; }
    public static TimestampSchema Date { get; }

    public TimestampSchema(SchemaInfo schemaInfo)
        => SchemaInfo = schemaInfo;

    public SchemaInfo SchemaInfo { get; }

    public DateTime Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {
        if (bytes.Length != 8)
            throw new SchemaSerializationException($"{nameof(TimestampSchema)} expected to decode 8 bytes, but received {bytes} bytes");

        var milliseconds = Schema.Int64.Decode(bytes);

        if (milliseconds < -62135596800000)
            return DateTime.MinValue;

        if (milliseconds > 253402300799999)
            return DateTime.MaxValue;

        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime;
    }

    public ReadOnlySequence<byte> Encode(DateTime message)
    {
        var milliseconds = new DateTimeOffset(message).ToUnixTimeMilliseconds();
        return Schema.Int64.Encode(milliseconds);
    }
}
