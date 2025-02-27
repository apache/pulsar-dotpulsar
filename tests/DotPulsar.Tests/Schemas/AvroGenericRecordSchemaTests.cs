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

using Avro.Generic;
using DotPulsar.Exceptions;
using DotPulsar.Tests.Schemas.TestSamples.AvroModels;
using System.Buffers;

[Trait("Category", "Unit")]
public sealed class AvroGenericRecordSchemaTests
{
    [Fact]
    public void Constructor_GivenModelIsNotGenericRecord_ShouldThrowException()
    {
        var exception = Record.Exception(() => Schema.AvroGenericRecordSchema<EmptyModel>(""));
        exception.ShouldBeOfType<SchemaException>();
    }

    [Fact]
    public void Constructor_GivenGenericWithUnParsableSchema_ShouldThrowException()
    {
        var exception = Record.Exception(() => Schema.AvroGenericRecordSchema<GenericRecord>(""));
        exception.ShouldBeOfType<SchemaException>();
    }

    [Fact]
    public void Constructor_GivenGenericRecordWithParsableSchema_ShouldReturnSchema()
    {
        var exception = Record.Exception(() => Schema.AvroGenericRecordSchema<GenericRecord>(ValidModel._SCHEMA.ToString()));
        exception.ShouldBeNull();
    }

    [Fact]
    public void Encode_GivenValidModel_ShouldReturnCorrectBytes()
    {
        //Arrange
        var schema = Schema.AvroGenericRecordSchema<GenericRecord>(ValidModel._SCHEMA.ToString());
        var expected = new ReadOnlySequence<byte>([6, 66, 101, 110, 14, 75, 108, 105, 110, 97, 107, 117, 44]);
        var model = new GenericRecord((Avro.RecordSchema) ValidModel._SCHEMA);
        model.Add("Name", "Ben");
        model.Add("Surname", "Klinaku");
        model.Add("Age", 22);
        //Act
        var actual = schema.Encode(model);

        //Assert
        actual.ToArray().ShouldBe(expected.ToArray());
    }

    [Fact]
    public void Decode_GivenValidBytes_ShouldReturnCorrectModel()
    {
        //Arrange
        var schema = Schema.AvroGenericRecordSchema<GenericRecord>(ValidModel._SCHEMA.ToString());
        var bytes = new ReadOnlySequence<byte>([8, 65, 100, 101, 97, 14, 75, 108, 105, 110, 97, 107, 117, 50]);
        var expected = new ValidModel { Name = "Adea", Surname = "Klinaku", Age = 25 };

        //Act
        var actual = schema.Decode(bytes);
        //Assert
        actual["Name"].ShouldBe(expected.Name);
        actual["Surname"].ShouldBe(expected.Surname);
        actual["Age"].ShouldBe(expected.Age);
    }
}
