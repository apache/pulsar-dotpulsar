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

using DotPulsar.Schemas;
using DotPulsar.Tests.Schemas.TestSamples.JsonModels;
using System.Buffers;
using System.Text.Json;

[Trait("Category", "Unit")]
public sealed class JsonSchemaTests
{
    [Fact]
    public void Constructor_GivenNoArguments_ShouldCreateSchemaWithDefaultOptions()
    {
        //Act
        var schema = new JsonSchema<PersonModel>();

        //Assert
        schema.SchemaInfo.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_GivenCustomOptions_ShouldCreateSchemaWithCustomOptions()
    {
        //Arrange
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        //Act
        var schema = new JsonSchema<PersonModel>(options);

        //Assert
        schema.SchemaInfo.ShouldNotBeNull();
    }

    [Fact]
    public void SchemaInfo_ShouldHaveJsonSchemaType()
    {
        //Arrange
        var schema = new JsonSchema<PersonModel>();

        //Act
        var schemaInfo = schema.SchemaInfo;

        //Assert
        schemaInfo.Type.ShouldBe(SchemaType.Json);
    }

    [Fact]
    public void SchemaInfo_ShouldHaveCorrectName()
    {
        //Arrange
        var schema = new JsonSchema<PersonModel>();

        //Act
        var schemaInfo = schema.SchemaInfo;

        //Assert
        schemaInfo.Name.ShouldBe("PersonModel");
    }

    [Fact]
    public void Encode_GivenValidModel_ShouldReturnJsonBytes()
    {
        //Arrange
        var schema = new JsonSchema<PersonModel>();
        var model = new PersonModel { Name = "Alice", Age = 30 };

        //Act
        var bytes = schema.Encode(model);

        //Assert
        bytes.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Decode_GivenValidJsonBytes_ShouldReturnCorrectModel()
    {
        //Arrange
        var schema = new JsonSchema<PersonModel>();
        var expected = new PersonModel { Name = "Bob", Age = 25 };
        var json = JsonSerializer.SerializeToUtf8Bytes(expected);
        var bytes = new ReadOnlySequence<byte>(json);

        //Act
        var actual = schema.Decode(bytes);

        //Assert
        actual.Name.ShouldBe(expected.Name);
        actual.Age.ShouldBe(expected.Age);
    }

    [Fact]
    public void EncodeDecode_GivenValidModel_ShouldRoundTrip()
    {
        //Arrange
        var schema = new JsonSchema<PersonModel>();
        var expected = new PersonModel { Name = "Charlie", Age = 42 };

        //Act
        var bytes = schema.Encode(expected);
        var actual = schema.Decode(bytes);

        //Assert
        actual.Name.ShouldBe(expected.Name);
        actual.Age.ShouldBe(expected.Age);
    }

    [Fact]
    public void EncodeDecode_GivenCamelCaseOptions_ShouldRoundTrip()
    {
        //Arrange
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var schema = new JsonSchema<PersonModel>(options);
        var expected = new PersonModel { Name = "Diana", Age = 35 };

        //Act
        var bytes = schema.Encode(expected);
        var actual = schema.Decode(bytes);

        //Assert
        actual.Name.ShouldBe(expected.Name);
        actual.Age.ShouldBe(expected.Age);
    }

    [Fact]
    public void Encode_GivenCamelCaseOptions_ShouldProduceCamelCaseJson()
    {
        //Arrange
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var schema = new JsonSchema<PersonModel>(options);
        var model = new PersonModel { Name = "Eve", Age = 28 };

        //Act
        var bytes = schema.Encode(model);
        var json = System.Text.Encoding.UTF8.GetString(bytes.ToArray());

        //Assert
        json.ShouldContain("\"name\"", Case.Sensitive);
        json.ShouldContain("\"age\"", Case.Sensitive);
        json.ShouldNotContain("\"Name\"", Case.Sensitive);
        json.ShouldNotContain("\"Age\"", Case.Sensitive);
    }

    [Fact]
    public void Constructor_GivenSchemaDefinition_ShouldUseItAsSchemaData()
    {
        //Arrange
        var definition = "{\"type\":\"record\",\"name\":\"PersonModel\"}";
        var options = new JsonSerializerOptions();

        //Act
        var schema = new JsonSchema<PersonModel>(options, definition);

        //Assert
        var schemaData = System.Text.Encoding.UTF8.GetString(schema.SchemaInfo.Data);
        schemaData.ShouldBe(definition);
    }

    [Fact]
    public void Decode_GivenEmptyBytes_ShouldThrowSchemaSerializationException()
    {
        //Arrange
        var schema = new JsonSchema<PersonModel>();
        var bytes = new ReadOnlySequence<byte>(Array.Empty<byte>());

        //Act
        var exception = Record.Exception(() => schema.Decode(bytes));

        //Assert
        exception.ShouldBeOfType<DotPulsar.Exceptions.SchemaSerializationException>();
    }
}
