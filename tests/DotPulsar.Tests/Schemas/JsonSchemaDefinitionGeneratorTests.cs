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
using System.Text.Json;

[Trait("Category", "Unit")]
public sealed class JsonSchemaDefinitionGeneratorTests
{
    [Fact]
    public void Generate_GivenSimpleType_ShouldProduceValidAvroSchema()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<PersonModel>();

        //Assert
        schema.ShouldContain("\"type\":\"record\"");
        schema.ShouldContain("\"name\":\"PersonModel\"");
        schema.ShouldContain("\"namespace\":\"DotPulsar.Tests.Schemas.TestSamples.JsonModels\"");
        schema.ShouldContain("\"fields\":");
    }

    [Fact]
    public void Generate_GivenSimpleType_ShouldIncludeAllPublicProperties()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<PersonModel>();

        //Assert
        schema.ShouldContain("\"name\":\"Name\"");
        schema.ShouldContain("\"name\":\"Age\"");
    }

    [Fact]
    public void Generate_GivenStringProperty_ShouldMapToAvroString()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<PersonModel>();
        var parsed = JsonDocument.Parse(schema);
        var fields = parsed.RootElement.GetProperty("fields");
        var nameField = FindField(fields, "Name");

        //Assert
        nameField.GetProperty("type").GetString().ShouldBe("string");
    }

    [Fact]
    public void Generate_GivenIntProperty_ShouldMapToAvroInt()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<PersonModel>();
        var parsed = JsonDocument.Parse(schema);
        var fields = parsed.RootElement.GetProperty("fields");
        var ageField = FindField(fields, "Age");

        //Assert
        ageField.GetProperty("type").GetString().ShouldBe("int");
    }

    [Fact]
    public void Generate_GivenAllPrimitiveTypes_ShouldMapCorrectly()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<AllTypesModel>();
        var parsed = JsonDocument.Parse(schema);
        var fields = parsed.RootElement.GetProperty("fields");

        //Assert
        FindField(fields, "StringField").GetProperty("type").GetString().ShouldBe("string");
        FindField(fields, "IntField").GetProperty("type").GetString().ShouldBe("int");
        FindField(fields, "LongField").GetProperty("type").GetString().ShouldBe("long");
        FindField(fields, "FloatField").GetProperty("type").GetString().ShouldBe("float");
        FindField(fields, "DoubleField").GetProperty("type").GetString().ShouldBe("double");
        FindField(fields, "BoolField").GetProperty("type").GetString().ShouldBe("boolean");
        FindField(fields, "DecimalField").GetProperty("type").GetString().ShouldBe("double");
    }

    [Fact]
    public void Generate_GivenNullableProperty_ShouldMapToAvroUnion()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<NullableFieldModel>();
        var parsed = JsonDocument.Parse(schema);
        var fields = parsed.RootElement.GetProperty("fields");
        var optionalAge = FindField(fields, "OptionalAge");

        //Assert
        var type = optionalAge.GetProperty("type");
        type.ValueKind.ShouldBe(JsonValueKind.Array);
        type[0].GetString().ShouldBe("null");
        type[1].GetString().ShouldBe("int");
    }

    [Fact]
    public void Generate_GivenCamelCaseNamingPolicy_ShouldUseCamelCaseFieldNames()
    {
        //Arrange
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<PersonModel>(options);

        //Assert — verify field names are camelCase, parse to check precisely
        var parsed = JsonDocument.Parse(schema);
        var fields = parsed.RootElement.GetProperty("fields");
        var fieldNames = new List<string>();
        foreach (var field in fields.EnumerateArray())
            fieldNames.Add(field.GetProperty("name").GetString()!);

        fieldNames.ShouldContain("name");
        fieldNames.ShouldContain("age");
        fieldNames.ShouldNotContain("Name");
        fieldNames.ShouldNotContain("Age");
    }

    [Fact]
    public void Generate_GivenJsonPropertyNameAttribute_ShouldUseAttributeValue()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<JsonPropertyNameModel>();

        //Assert
        schema.ShouldContain("\"name\":\"display_name\"");
        schema.ShouldContain("\"name\":\"created_at\"");
        schema.ShouldNotContain("\"name\":\"DisplayName\"");
    }

    [Fact]
    public void Generate_GivenJsonPropertyNameAttribute_ShouldTakePrecedenceOverNamingPolicy()
    {
        //Arrange
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<JsonPropertyNameModel>(options);

        //Assert — attribute wins over naming policy
        schema.ShouldContain("\"name\":\"display_name\"");
        schema.ShouldContain("\"name\":\"created_at\"");
    }

    [Fact]
    public void Generate_GivenJsonIgnoreAttribute_ShouldExcludeProperty()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<JsonIgnoreModel>();

        //Assert
        schema.ShouldContain("\"name\":\"Name\"");
        schema.ShouldNotContain("Secret");
    }

    [Fact]
    public void Generate_GivenEnumProperty_ShouldMapToAvroEnum()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<EnumModel>();
        var parsed = JsonDocument.Parse(schema);
        var fields = parsed.RootElement.GetProperty("fields");
        var priorityField = FindField(fields, "Priority");

        //Assert
        var type = priorityField.GetProperty("type");
        type.GetProperty("type").GetString().ShouldBe("enum");
        type.GetProperty("name").GetString().ShouldBe("Priority");
        var symbols = type.GetProperty("symbols");
        symbols[0].GetString().ShouldBe("Low");
        symbols[1].GetString().ShouldBe("Medium");
        symbols[2].GetString().ShouldBe("High");
    }

    [Fact]
    public void Generate_GivenNestedType_ShouldGenerateInlineRecord()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<NestedModel>();
        var parsed = JsonDocument.Parse(schema);
        var fields = parsed.RootElement.GetProperty("fields");
        var authorField = FindField(fields, "Author");

        //Assert
        var type = authorField.GetProperty("type");
        type.GetProperty("type").GetString().ShouldBe("record");
        type.GetProperty("name").GetString().ShouldBe("PersonModel");
    }

    [Fact]
    public void Generate_GivenListProperty_ShouldMapToAvroArray()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<CollectionModel>();
        var parsed = JsonDocument.Parse(schema);
        var fields = parsed.RootElement.GetProperty("fields");
        var tagsField = FindField(fields, "Tags");

        //Assert
        var type = tagsField.GetProperty("type");
        type.GetProperty("type").GetString().ShouldBe("array");
        type.GetProperty("items").GetString().ShouldBe("string");
    }

    [Fact]
    public void Generate_GivenArrayProperty_ShouldMapToAvroArray()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<CollectionModel>();
        var parsed = JsonDocument.Parse(schema);
        var fields = parsed.RootElement.GetProperty("fields");
        var scoresField = FindField(fields, "Scores");

        //Assert
        var type = scoresField.GetProperty("type");
        type.GetProperty("type").GetString().ShouldBe("array");
        type.GetProperty("items").GetString().ShouldBe("int");
    }

    [Fact]
    public void Generate_GivenDictionaryProperty_ShouldMapToAvroMap()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<DictionaryModel>();
        var parsed = JsonDocument.Parse(schema);
        var fields = parsed.RootElement.GetProperty("fields");
        var metadataField = FindField(fields, "Metadata");

        //Assert
        var type = metadataField.GetProperty("type");
        type.GetProperty("type").GetString().ShouldBe("map");
        type.GetProperty("values").GetString().ShouldBe("int");
    }

    [Fact]
    public void Generate_ShouldProduceValidJson()
    {
        //Act
        var schema = JsonSchemaDefinitionGenerator.Generate<PersonModel>();

        //Assert — should not throw
        var parsed = JsonDocument.Parse(schema);
        parsed.RootElement.ValueKind.ShouldBe(JsonValueKind.Object);
    }

    private static JsonElement FindField(JsonElement fields, string name)
    {
        foreach (var field in fields.EnumerateArray())
        {
            if (field.GetProperty("name").GetString() == name)
                return field;
        }

        throw new InvalidOperationException($"Field '{name}' not found in schema fields");
    }
}
