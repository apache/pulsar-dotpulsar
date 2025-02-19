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

namespace DotPulsar.Tests.Schemas;

using Avro.Specific;
using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Schemas;
using System.Buffers;

[Trait("Category", "Unit")]
public sealed class AvroISpecificRecordSchemaTests
{

    [Fact]
    public void Schema_ClassNotImplementingISpecific_ShouldThrowException()
    {
        var ex = Record.Exception(Schema.AvroISpecificRecord<AvroBlankSampleModel>);
        ex.ShouldBeOfType<SchemaException>();
    }
    [Fact]
    public void Schema_ClassStaticSchemaFIeldWrongType_ShouldThrowException()
    {
        var ex = Record.Exception(Schema.AvroISpecificRecord<AvroSampleModelWithWrongSCHEMAField>);
        ex.ShouldBeOfType<SchemaException>();
    }
    [Fact]
    public void Schema_ClassImplementsISpecificRecordCorrectly_ShouldReturnSchema()
    {
        var pulsarSchema = Schema.AvroISpecificRecord<AvroSampleModel>();
        pulsarSchema.ShouldBeOfType<AvroISpecificRecordSchema<AvroSampleModel>>();
    }
    [Fact]
    public void Schema_GivenDataAndImplementsEncodeProperly_ShouldReturnReadOnlySequenceOfBytes()
    {
        var pulsarSchema = Schema.AvroISpecificRecord<AvroSampleModel>();
        var res = pulsarSchema.Encode(new AvroSampleModel() { Name = "Jon", Surname = "Klinaku", Age = 27 });
        res.ShouldBeOfType<ReadOnlySequence<byte>>();
    }
    [Fact]
    public void Schema_GivenDataAndImplementsDecodeProperly_ShouldReturnCorrectObject()
    {
        var pulsarSchema = Schema.AvroISpecificRecord<AvroSampleModel>();
        ReadOnlySequence<byte> bytes = new ReadOnlySequence<byte>([16, 77, 97, 114, 105, 103, 111, 110, 97, 8, 67, 101, 116, 97, 58]);
        var res = pulsarSchema.Decode(bytes);
        res.ShouldBeOfType<AvroSampleModel>();
        res.Name.ShouldBe("Marigona");
        res.Surname.ShouldBe("Ceta");
        res.Age.ShouldBe(29);
    }

    private class AvroSampleModel : ISpecificRecord
    {
        public static readonly Avro.Schema _SCHEMA = Avro.Schema.Parse(@"
        {
            ""type"": ""record"",
            ""name"": ""AvroSampleModel"",
            ""fields"": [
                { ""name"": ""Name"", ""type"": ""string"" },
                { ""name"": ""Surname"", ""type"": ""string"" },
                { ""name"": ""Age"", ""type"": ""int"" }
            ]
        }");

        public virtual Avro.Schema Schema => _SCHEMA;
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public object Get(int fieldPos)
        {
            return fieldPos switch
            {
                0 => Name,
                1 => Surname,
                2 => Age,
                _ => throw new ArgumentOutOfRangeException(nameof(fieldPos), "Invalid field position")
            };
        }

        public void Put(int fieldPos, object value)
        {
            switch (fieldPos)
            {
                case 0:
                    Name = value as string ?? throw new ArgumentException("Name must be a string");
                    break;
                case 1:
                    Surname = value as string ?? throw new ArgumentException("Name must be a string");
                    break;
                case 2:
                    Age = value is int intValue ? intValue : throw new ArgumentException("Age must be an int");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldPos), "Invalid field position");
            }
        }
    }
    private class AvroBlankSampleModel
    {
    }
    private class AvroSampleModelWithWrongSCHEMAField : ISpecificRecord
    {
        public static string _SCHEMA = "WRONG!";
        public Avro.Schema Schema => throw new NotImplementedException();

        public object Get(int fieldPos)
        {
            throw new NotImplementedException();
        }

        public void Put(int fieldPos, object fieldValue)
        {
            throw new NotImplementedException();
        }
    }

}
