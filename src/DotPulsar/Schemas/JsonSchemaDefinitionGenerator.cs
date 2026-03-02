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

/// <summary>
/// Generates Avro-style JSON schema definitions from .NET types.
/// Used by <see cref="JsonSchema{T}"/> to produce broker-compatible schema data.
/// </summary>
public static class JsonSchemaDefinitionGenerator
{
    /// <summary>
    /// Generate an Avro-style schema definition for the given type.
    /// </summary>
    public static string Generate<T>(JsonSerializerOptions? options = null)
        => Generate(typeof(T), options);

    /// <summary>
    /// Generate an Avro-style schema definition for the given type.
    /// </summary>
    public static string Generate(Type type, JsonSerializerOptions? options = null)
    {
        var namingPolicy = options?.PropertyNamingPolicy;
        var sb = new StringBuilder();
        GenerateRecord(type, namingPolicy, sb);
        return sb.ToString();
    }

    private static void GenerateRecord(Type type, JsonNamingPolicy? namingPolicy, StringBuilder sb)
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

        if (type.IsEnum)
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
