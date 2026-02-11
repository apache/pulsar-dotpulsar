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
using System.Text;
using System.Text.Json;

/// <summary>
/// Schema definition for JSON encoded messages.
/// </summary>
public sealed class JsonSchema<T> : ISchema<T>
{
    private readonly JsonSerializerOptions _options;

    public JsonSchema()
    {
        _options = new JsonSerializerOptions();
        SchemaInfo = new SchemaInfo(typeof(T).Name, Array.Empty<byte>(), SchemaType.Json, ImmutableDictionary<string, string>.Empty);
    }

    public JsonSchema(JsonSerializerOptions options)
    {
        _options = options;
        SchemaInfo = new SchemaInfo(typeof(T).Name, Array.Empty<byte>(), SchemaType.Json, ImmutableDictionary<string, string>.Empty);
    }

    public JsonSchema(JsonSerializerOptions options, string jsonSchemaDefinition)
    {
        _options = options;
        var schemaData = Encoding.UTF8.GetBytes(jsonSchemaDefinition);
        SchemaInfo = new SchemaInfo(typeof(T).Name, schemaData, SchemaType.Json, ImmutableDictionary<string, string>.Empty);
    }

    public SchemaInfo SchemaInfo { get; }

    public T Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {
        try
        {
            var array = bytes.ToArray();
            return JsonSerializer.Deserialize<T>(array, _options)
                ?? throw new SchemaSerializationException($"Failed to deserialize JSON to type '{typeof(T).Name}'");
        }
        catch (JsonException exception)
        {
            throw new SchemaSerializationException($"Failed to deserialize JSON to type '{typeof(T).Name}'", exception);
        }
    }

    public ReadOnlySequence<byte> Encode(T message)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(message, _options);
        return new ReadOnlySequence<byte>(bytes);
    }
}
