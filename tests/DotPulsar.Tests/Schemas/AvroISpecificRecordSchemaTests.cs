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

using DotPulsar.Exceptions;
using DotPulsar.Tests.Schemas.TestSamples.AvroModels;
using System.Buffers;

[Trait("Category", "Unit")]
public sealed class AvroISpecificRecordSchemaTests
{
    [Fact]
    public void Constructor_GivenModelThatDoNotImplementISpecificRecord_ShouldThrowException()
    {
        var exception = Record.Exception(Schema.AvroISpecificRecord<AvroBlankSampleModel>);
        exception.ShouldBeOfType<SchemaException>();
    }

    [Fact]
    public void Constructor_GivenModelWithWrongSchemaField_ShouldThrowException()
    {
        var exception = Record.Exception(Schema.AvroISpecificRecord<AvroSampleModelWithWrongSchemaField>);
        exception.ShouldBeOfType<SchemaException>();
    }

    [Fact]
    public void Constructor_GivenValidModel_ShouldReturnSchema()
    {
        var exception = Record.Exception(Schema.AvroISpecificRecord<AvroSampleModel>);
        exception.ShouldBeNull();
    }

    [Fact]
    public void Encode_GivenValidModel_ShouldReturnCorrectBytes()
    {
        //Arrange
        var schema = Schema.AvroISpecificRecord<AvroSampleModel>();
        var expected = new ReadOnlySequence<byte>([16, 77, 97, 114, 105, 103, 111, 110, 97, 8, 67, 101, 116, 97, 58]);
        var model = new AvroSampleModel { Name = "Marigona", Surname = "Ceta", Age = 29 };

        //Act
        var actual = schema.Encode(model);

        //Assert
        actual.ToArray().ShouldBe(expected.ToArray());
    }

    [Fact]
    public void Decode_GivenValidBytes_ShouldReturnCorrectModel()
    {
        //Arrange
        var schema = Schema.AvroISpecificRecord<AvroSampleModel>();
        var bytes = new ReadOnlySequence<byte>([16, 77, 97, 114, 105, 103, 111, 110, 97, 8, 67, 101, 116, 97, 58]);
        var expected = new AvroSampleModel { Name = "Marigona", Surname = "Ceta", Age = 29 };

        //Act
        var actual = schema.Decode(bytes);

        //Assert
        actual.Name.ShouldBe(expected.Name);
        actual.Surname.ShouldBe(expected.Surname);
        actual.Age.ShouldBe(expected.Age);
    }
}
