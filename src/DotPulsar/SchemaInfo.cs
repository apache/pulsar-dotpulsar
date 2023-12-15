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

namespace DotPulsar;

/// <summary>
/// Information about the schema.
/// </summary>
public sealed class SchemaInfo
{
    internal SchemaInfo(Internal.PulsarApi.Schema schema)
        => PulsarSchema = schema;

    public SchemaInfo(string name, byte[] data, SchemaType type, IReadOnlyDictionary<string, string> properties)
    {
        PulsarSchema = new Internal.PulsarApi.Schema
        {
            Name = name,
            SchemaData = data,
            Type = (Internal.PulsarApi.Schema.SchemaType) type,
        };

        foreach (var property in properties)
        {
            var keyValue = new Internal.PulsarApi.KeyValue
            {
                Key = property.Key,
                Value = property.Value
            };

            PulsarSchema.Properties.Add(keyValue);
        }
    }

    internal Internal.PulsarApi.Schema PulsarSchema { get; }

    /// <summary>
    /// The name of the schema.
    /// </summary>
    public string Name => PulsarSchema.Name;

    /// <summary>
    /// The data of the schema.
    /// </summary>
    public byte[] Data => PulsarSchema.SchemaData;

    /// <summary>
    /// The type of the schema.
    /// </summary>
    public SchemaType Type => (SchemaType) PulsarSchema.Type;

    /// <summary>
    /// The properties of the schema.
    /// </summary>
    public IReadOnlyDictionary<string, string> Properties => PulsarSchema.Properties.ToDictionary(p => p.Key, p => p.Value);
}
