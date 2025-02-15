namespace DotPulsar.Schemas;

using Avro.Generic;
using Avro.IO;
using Avro.Reflect;
using Avro.Specific;
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
using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

public sealed class AvroISpecificRecordSchema<T> : ISchema<T> where T : ISpecificRecord
{
    private Avro.Schema? avroSchema;
    private DatumWriter<T> avroWriter;
    private DatumReader<T> avroReader;
    public SchemaInfo SchemaInfo { get; }
    public AvroISpecificRecordSchema()
    {
        Type type = typeof(T);
        try
        {
            avroSchema = type.GetField("_SCHEMA")?.GetValue(null) as Avro.Schema;
            if (avroSchema is null)
            {
                throw new AvroISpecificRecordSchemaException(string.Format("The static field named '_SCHEMA' must not be null in type: {0}", type));
            }
        }
        catch (Exception e)
        {

            throw new AvroISpecificRecordSchemaException(string.Format("The Type: {0} must implement a static field named '_SCHEMA' and of type Avro.Schema", type), e);
        }
        SchemaInfo = new SchemaInfo(avroSchema.Name, Encoding.UTF8.GetBytes(avroSchema.ToString()), SchemaType.Avro, new Dictionary<string, string>());
        avroWriter = new SpecificDatumWriter<T>(avroSchema);
        avroReader = new SpecificDatumReader<T>(avroSchema, avroSchema);
    }
    public T Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {

        using var stream = new MemoryStream(bytes.ToArray());
#pragma warning disable CS8604
        return avroReader.Read(default, new BinaryDecoder(stream));
#pragma warning restore CS8604
    }

    public ReadOnlySequence<byte> Encode(T message)
    {
        using var stream = new MemoryStream();
        avroWriter.Write(message, new BinaryEncoder(stream));
        return new ReadOnlySequence<byte>(stream.ToArray());
    }
}
