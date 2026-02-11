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
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Buffers;

[Trait("Category", "Unit")]
public sealed class ProtobufSchemaTests
{
    [Fact]
    public void Constructor_GivenValidProtobufMessage_ShouldCreateSchema()
    {
        //Act
        var schema = new ProtobufSchema<StringValue>();

        //Assert
        schema.SchemaInfo.ShouldNotBeNull();
    }

    [Fact]
    public void SchemaInfo_ShouldHaveProtobufNativeSchemaType()
    {
        //Arrange
        var schema = new ProtobufSchema<StringValue>();

        //Act
        var schemaInfo = schema.SchemaInfo;

        //Assert
        schemaInfo.Type.ShouldBe(SchemaType.ProtobufNative);
    }

    [Fact]
    public void SchemaInfo_ShouldHaveCorrectName()
    {
        //Arrange
        var schema = new ProtobufSchema<StringValue>();

        //Act
        var schemaInfo = schema.SchemaInfo;

        //Assert
        schemaInfo.Name.ShouldBe("StringValue");
    }

    [Fact]
    public void SchemaInfo_ShouldHaveNonEmptySchemaData()
    {
        //Arrange
        var schema = new ProtobufSchema<StringValue>();

        //Act
        var schemaInfo = schema.SchemaInfo;

        //Assert
        schemaInfo.Data.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Encode_GivenValidMessage_ShouldReturnBytes()
    {
        //Arrange
        var schema = new ProtobufSchema<StringValue>();
        var message = new StringValue { Value = "Hello Pulsar" };

        //Act
        var bytes = schema.Encode(message);

        //Assert
        bytes.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Decode_GivenValidBytes_ShouldReturnCorrectMessage()
    {
        //Arrange
        var schema = new ProtobufSchema<StringValue>();
        var expected = new StringValue { Value = "Hello Pulsar" };
        var protobufBytes = expected.ToByteArray();
        var bytes = new ReadOnlySequence<byte>(protobufBytes);

        //Act
        var actual = schema.Decode(bytes);

        //Assert
        actual.Value.ShouldBe(expected.Value);
    }

    [Fact]
    public void EncodeDecode_GivenValidMessage_ShouldRoundTrip()
    {
        //Arrange
        var schema = new ProtobufSchema<StringValue>();
        var expected = new StringValue { Value = "Round trip test" };

        //Act
        var bytes = schema.Encode(expected);
        var actual = schema.Decode(bytes);

        //Assert
        actual.Value.ShouldBe(expected.Value);
    }

    [Fact]
    public void EncodeDecode_GivenInt32Value_ShouldRoundTrip()
    {
        //Arrange
        var schema = new ProtobufSchema<Int32Value>();
        var expected = new Int32Value { Value = 42 };

        //Act
        var bytes = schema.Encode(expected);
        var actual = schema.Decode(bytes);

        //Assert
        actual.Value.ShouldBe(expected.Value);
    }

    [Fact]
    public void EncodeDecode_GivenEmptyMessage_ShouldRoundTrip()
    {
        //Arrange
        var schema = new ProtobufSchema<StringValue>();
        var expected = new StringValue { Value = "" };

        //Act
        var bytes = schema.Encode(expected);
        var actual = schema.Decode(bytes);

        //Assert
        actual.Value.ShouldBe(expected.Value);
    }

    [Fact]
    public void Decode_GivenEmptyBytes_ShouldReturnDefaultMessage()
    {
        //Arrange
        var schema = new ProtobufSchema<StringValue>();
        var bytes = new ReadOnlySequence<byte>(Array.Empty<byte>());

        //Act
        var actual = schema.Decode(bytes);

        //Assert
        actual.Value.ShouldBe("");
    }
}
