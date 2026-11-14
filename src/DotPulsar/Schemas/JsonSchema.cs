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
using System.Text.Json.Serialization.Metadata;

/// <summary>
/// Schema definition for JSON encoded messages.
/// </summary>
/// <remarks>
/// For trimmed and Native AOT applications, use the constructors taking a <see cref="JsonTypeInfo{T}"/>
/// (for example from a source-generated <see cref="System.Text.Json.Serialization.JsonSerializerContext"/>).
/// </remarks>
public sealed class JsonSchema<T> : ISchema<T>
{
    internal const string RequiresUnreferencedCodeMessage = "JSON serialization and schema generation for arbitrary types use reflection. Use the overload taking a JsonTypeInfo<T> (for example from a JsonSerializerContext) in trimmed applications.";
    internal const string RequiresDynamicCodeMessage = "JSON serialization of arbitrary types might need runtime code generation. Use the overload taking a JsonTypeInfo<T> (for example from a JsonSerializerContext) in Native AOT applications.";

    private readonly JsonTypeInfo<T> _typeInfo;

#if NET
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(RequiresDynamicCodeMessage)]
#endif
    public JsonSchema() : this(new JsonSerializerOptions()) { }

#if NET
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(RequiresDynamicCodeMessage)]
#endif
    public JsonSchema(JsonSerializerOptions options)
    {
        _typeInfo = GetTypeInfo(options);
        var schemaData = Encoding.UTF8.GetBytes(JsonSchemaDefinitionGenerator.Generate<T>(options));
        SchemaInfo = CreateSchemaInfo(schemaData);
    }

#if NET
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(RequiresDynamicCodeMessage)]
#endif
    public JsonSchema(JsonSerializerOptions options, string jsonSchemaDefinition)
    {
        _typeInfo = GetTypeInfo(options);
        SchemaInfo = CreateSchemaInfo(Encoding.UTF8.GetBytes(jsonSchemaDefinition));
    }

    /// <summary>
    /// Create a JSON schema using System.Text.Json contract metadata. The schema definition is generated from the metadata.
    /// </summary>
    public JsonSchema(JsonTypeInfo<T> typeInfo)
    {
        _typeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
        var schemaData = Encoding.UTF8.GetBytes(JsonSchemaDefinitionGenerator.Generate(typeInfo));
        SchemaInfo = CreateSchemaInfo(schemaData);
    }

    /// <summary>
    /// Create a JSON schema using System.Text.Json contract metadata and an explicit schema definition.
    /// </summary>
    public JsonSchema(JsonTypeInfo<T> typeInfo, string jsonSchemaDefinition)
    {
        _typeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
        SchemaInfo = CreateSchemaInfo(Encoding.UTF8.GetBytes(jsonSchemaDefinition));
    }

    public SchemaInfo SchemaInfo { get; }

    public T Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {
        try
        {
            var array = bytes.ToArray();
            return JsonSerializer.Deserialize(array, _typeInfo)
                ?? throw new SchemaSerializationException($"Failed to deserialize JSON to type '{typeof(T).Name}'");
        }
        catch (JsonException exception)
        {
            throw new SchemaSerializationException($"Failed to deserialize JSON to type '{typeof(T).Name}'", exception);
        }
    }

    public ReadOnlySequence<byte> Encode(T message)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(message, _typeInfo);
        return new ReadOnlySequence<byte>(bytes);
    }

    private static SchemaInfo CreateSchemaInfo(byte[] schemaData)
        => new(typeof(T).Name, schemaData, SchemaType.Json, ImmutableDictionary<string, string>.Empty);

#if NET
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode(RequiresDynamicCodeMessage)]
#endif
    private static JsonTypeInfo<T> GetTypeInfo(JsonSerializerOptions options)
    {
        // Matches what JsonSerializer.Serialize(value, options) does on first use: lock the options and
        // fall back to the reflection-based resolver if none is configured.
        options.MakeReadOnly(populateMissingResolver: true);
        return (JsonTypeInfo<T>) options.GetTypeInfo(typeof(T));
    }
}
