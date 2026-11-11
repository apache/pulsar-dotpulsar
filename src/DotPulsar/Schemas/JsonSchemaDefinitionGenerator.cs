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

using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

/// <summary>
/// Generates Avro-style JSON schema definitions from .NET types.
/// Used by <see cref="JsonSchema{T}"/> to produce broker-compatible schema data.
/// </summary>
public static class JsonSchemaDefinitionGenerator
{
    private const string RequiresUnreferencedCodeMessage = "Generating a schema definition from a Type uses reflection over its properties. Use the overload taking a JsonTypeInfo in trimmed applications.";

    /// <summary>
    /// Generate an Avro-style schema definition for the given type.
    /// </summary>
#if NET
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
#endif
    public static string Generate<T>(JsonSerializerOptions? options = null)
        => Generate(typeof(T), options);

    /// <summary>
    /// Generate an Avro-style schema definition for the given type.
    /// </summary>
#if NET
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
#endif
    public static string Generate(Type type, JsonSerializerOptions? options = null)
    {
        var namingPolicy = options?.PropertyNamingPolicy;
        var sb = new StringBuilder();
        ReflectionGenerator.GenerateRecord(type, namingPolicy, sb);
        return sb.ToString();
    }

    /// <summary>
    /// Generate an Avro-style schema definition from System.Text.Json contract metadata.
    /// </summary>
    /// <remarks>
    /// This overload is compatible with trimming and Native AOT when the metadata comes from a source-generated
    /// <see cref="JsonSerializerContext"/>. Field names, ignored properties and ordering follow the serializer's contract,
    /// and nested types are resolved through <see cref="JsonTypeInfo.Options"/>.
    /// </remarks>
    public static string Generate(JsonTypeInfo typeInfo)
    {
        if (typeInfo is null)
            throw new ArgumentNullException(nameof(typeInfo));

        var sb = new StringBuilder();
        GenerateRecord(typeInfo, sb);
        return sb.ToString();
    }

    private static void GenerateRecord(JsonTypeInfo typeInfo, StringBuilder sb)
    {
        var type = typeInfo.Type;
        sb.Append("{\"type\":\"record\",\"name\":\"");
        sb.Append(type.Name);
        sb.Append("\",\"namespace\":\"");
        sb.Append(type.Namespace ?? string.Empty);
        sb.Append("\",\"fields\":[");

        if (typeInfo.Kind == JsonTypeInfoKind.Object)
        {
            var first = true;

            foreach (var property in typeInfo.Properties)
            {
                // Properties with [JsonIgnore] stay in the contract (for name conflict detection) but have no accessors
                if (property.Get is null && property.Set is null)
                    continue;

                if (!first)
                    sb.Append(',');

                first = false;

                var avroType = MapToAvroType(property.PropertyType, typeInfo.Options);

                sb.Append("{\"name\":\"");
                sb.Append(property.Name);
                sb.Append("\",\"type\":");
                sb.Append(avroType);
                sb.Append('}');
            }
        }

        sb.Append("]}");
    }

    private static string MapToAvroType(Type type, JsonSerializerOptions options)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType is not null)
            return "[\"null\"," + MapToAvroType(underlyingType, options) + "]";

        var primitive = MapPrimitive(type);
        if (primitive is not null)
            return primitive;

        if (type.IsEnum)
            return MapEnum(type);

        var typeInfo = options.GetTypeInfo(type);

        switch (typeInfo.Kind)
        {
            case JsonTypeInfoKind.Enumerable when GetElementType(typeInfo) is { } itemType:
                return "{\"type\":\"array\",\"items\":" + MapToAvroType(itemType, options) + "}";
            case JsonTypeInfoKind.Dictionary when GetElementType(typeInfo) is { } valueType:
                return "{\"type\":\"map\",\"values\":" + MapToAvroType(valueType, options) + "}";
            case JsonTypeInfoKind.Object:
                var nestedSb = new StringBuilder();
                GenerateRecord(typeInfo, nestedSb);
                return nestedSb.ToString();
            default:
                return "\"string\"";
        }
    }

    private static Type? GetElementType(JsonTypeInfo typeInfo)
    {
#if NET9_0_OR_GREATER || NETSTANDARD
        return typeInfo.ElementType;
#else
        // JsonTypeInfo.ElementType is only available from System.Text.Json 9 (the in-box version on .NET 8 is 8). Fall back to inspecting the type
        // with APIs that are safe for trimming: arrays, then the last generic argument of the type or a base type
        // (the item type of collections and the value type of dictionaries).
        var type = typeInfo.Type;
        if (type.IsArray)
            return type.GetElementType();

        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.IsGenericType)
            {
                var arguments = current.GetGenericArguments();
                return arguments[arguments.Length - 1];
            }
        }

        return null;
#endif
    }

    private static string? MapPrimitive(Type type)
    {
        if (type == typeof(string))
            return "\"string\"";

        if (type == typeof(int))
            return "\"int\"";

        if (type == typeof(long))
            return "\"long\"";

        if (type == typeof(float))
            return "\"float\"";

        if (type == typeof(double))
            return "\"double\"";

        if (type == typeof(bool))
            return "\"boolean\"";

        if (type == typeof(byte[]))
            return "\"bytes\"";

        if (type == typeof(decimal))
            return "\"double\"";

        if (type == typeof(short))
            return "\"int\"";

        if (type == typeof(byte))
            return "\"int\"";

        if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
            return "\"string\"";

        if (type == typeof(Guid))
            return "\"string\"";

        return null;
    }

    private static string MapEnum(Type type)
    {
        var names = Enum.GetNames(type);
        var enumSb = new StringBuilder();
        enumSb.Append("{\"type\":\"enum\",\"name\":\"");
        enumSb.Append(type.Name);
        enumSb.Append("\",\"symbols\":[");
        for (var i = 0; i < names.Length; i++)
        {
            if (i > 0)
                enumSb.Append(',');
            enumSb.Append('"');
            enumSb.Append(names[i]);
            enumSb.Append('"');
        }
        enumSb.Append("]}");
        return enumSb.ToString();
    }

#if NET
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
#endif
    private static class ReflectionGenerator
    {
        public static void GenerateRecord(Type type, JsonNamingPolicy? namingPolicy, StringBuilder sb)
        {
            sb.Append("{\"type\":\"record\",\"name\":\"");
            sb.Append(type.Name);
            sb.Append("\",\"namespace\":\"");
            sb.Append(type.Namespace ?? string.Empty);
            sb.Append("\",\"fields\":[");

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var first = true;

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<JsonIgnoreAttribute>() is not null)
                    continue;

                if (!first)
                    sb.Append(',');

                first = false;

                var fieldName = ResolveFieldName(property, namingPolicy);
                var avroType = MapToAvroType(property.PropertyType, namingPolicy, sb, fieldNameOnly: true);

                sb.Append("{\"name\":\"");
                sb.Append(fieldName);
                sb.Append("\",\"type\":");
                sb.Append(avroType);
                sb.Append('}');
            }

            sb.Append("]}");
        }

        private static string ResolveFieldName(PropertyInfo property, JsonNamingPolicy? namingPolicy)
        {
            var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (jsonPropertyName is not null)
                return jsonPropertyName.Name;

            return namingPolicy?.ConvertName(property.Name) ?? property.Name;
        }

        private static string MapToAvroType(Type type, JsonNamingPolicy? namingPolicy, StringBuilder sb, bool fieldNameOnly)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType is not null)
            {
                var innerAvro = MapToAvroType(underlyingType, namingPolicy, sb, fieldNameOnly);
                return "[\"null\"," + innerAvro + "]";
            }

            var primitive = MapPrimitive(type);
            if (primitive is not null)
                return primitive;

            if (type.IsEnum)
                return MapEnum(type);

            if (type.IsArray)
            {
                var elementType = type.GetElementType()!;
                var itemType = MapToAvroType(elementType, namingPolicy, sb, fieldNameOnly);
                return "{\"type\":\"array\",\"items\":" + itemType + "}";
            }

            if (type.IsGenericType)
            {
                var genericDef = type.GetGenericTypeDefinition();

                if (genericDef == typeof(List<>) ||
                    genericDef == typeof(IList<>) ||
                    genericDef == typeof(IEnumerable<>) ||
                    genericDef == typeof(ICollection<>) ||
                    genericDef == typeof(IReadOnlyList<>) ||
                    genericDef == typeof(IReadOnlyCollection<>))
                {
                    var itemType = MapToAvroType(type.GetGenericArguments()[0], namingPolicy, sb, fieldNameOnly);
                    return "{\"type\":\"array\",\"items\":" + itemType + "}";
                }

                if (genericDef == typeof(Dictionary<,>) ||
                    genericDef == typeof(IDictionary<,>) ||
                    genericDef == typeof(IReadOnlyDictionary<,>))
                {
                    var valueType = MapToAvroType(type.GetGenericArguments()[1], namingPolicy, sb, fieldNameOnly);
                    return "{\"type\":\"map\",\"values\":" + valueType + "}";
                }
            }

            // Nested record type — generate inline
            if (type.IsClass || type.IsValueType && !type.IsPrimitive)
            {
                var nestedSb = new StringBuilder();
                GenerateRecord(type, namingPolicy, nestedSb);
                return nestedSb.ToString();
            }

            return "\"string\"";
        }
    }
}
