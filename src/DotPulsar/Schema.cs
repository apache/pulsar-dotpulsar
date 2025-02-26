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

using DotPulsar.Schemas;

/// <summary>
/// Message schema definitions.
/// </summary>
public static class Schema
{
    static Schema()
    {
        ByteSequence = new ByteSequenceSchema();
        ByteArray = new ByteArraySchema();
        String = StringSchema.UTF8;
        Boolean = new BooleanSchema();
        Int8 = new ByteSchema();
        Int16 = new ShortSchema();
        Int32 = new IntegerSchema();
        Int64 = new LongSchema();
        Float = new FloatSchema();
        TimeStamp = TimestampSchema.Timestamp;
        Date = TimestampSchema.Date;
        Time = new TimeSchema();
    }

    /// <summary>
    /// Raw bytes schema using ReadOnlySequence of bytes.
    /// </summary>
    public static ByteSequenceSchema ByteSequence { get; }

    /// <summary>
    /// Raw bytes schema using byte[].
    /// </summary>
    public static ByteArraySchema ByteArray { get; }

    /// <summary>
    /// UTF-8 schema.
    /// </summary>
    public static StringSchema String { get; }

    /// <summary>
    /// Boolean schema.
    /// </summary>
    public static BooleanSchema Boolean { get; }

    /// <summary>
    /// Byte schema.
    /// </summary>
    public static ByteSchema Int8 { get; }

    /// <summary>
    /// Short schema.
    /// </summary>
    public static ShortSchema Int16 { get; }

    /// <summary>
    /// Integer schema.
    /// </summary>
    public static IntegerSchema Int32 { get; }

    /// <summary>
    /// Long schema.
    /// </summary>
    public static LongSchema Int64 { get; }

    /// <summary>
    /// Float schema.
    /// </summary>
    public static FloatSchema Float { get; }

    /// <summary>
    /// Timestamp schema using DateTime.
    /// </summary>
    public static TimestampSchema TimeStamp { get; }

    /// <summary>
    /// Date schema using DateTime.
    /// </summary>
    public static TimestampSchema Date { get; }

    /// <summary>
    /// Time schema using TimeSpan.
    /// </summary>
    public static TimeSchema Time { get; }

    /// <summary>
    /// Avro schema for classes that use ISpecificRecord
    /// </summary>
    public static AvroISpecificRecordSchema<T> AvroISpecificRecord<T>() => new();

    /// <summary>
    /// Avro schema for generic avro record, always give Avro.Generic.GenericRecord
    /// </summary>
    public static AvroGenericRecordSchema<T> avroGenericRecordSchema<T>(string jsonAvroSchema) => new(jsonAvroSchema);
}
