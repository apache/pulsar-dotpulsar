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

namespace DotPulsar.Tests.Schemas.TestSamples.JsonModels;

using System.Text.Json.Serialization;

public sealed class NullableFieldModel
{
    public string Name { get; set; } = string.Empty;
    public int? OptionalAge { get; set; }
    public double? OptionalScore { get; set; }
}

public sealed class NestedModel
{
    public string Title { get; set; } = string.Empty;
    public PersonModel Author { get; set; } = new();
}

public sealed class CollectionModel
{
    public string Name { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public int[] Scores { get; set; } = [];
}

public sealed class JsonPropertyNameModel
{
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;
}

public sealed class JsonIgnoreModel
{
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public string Secret { get; set; } = string.Empty;
}

public enum Priority
{
    Low,
    Medium,
    High
}

public sealed class EnumModel
{
    public string Name { get; set; } = string.Empty;
    public Priority Priority { get; set; }
}

public sealed class AllTypesModel
{
    public string StringField { get; set; } = string.Empty;
    public int IntField { get; set; }
    public long LongField { get; set; }
    public float FloatField { get; set; }
    public double DoubleField { get; set; }
    public bool BoolField { get; set; }
    public decimal DecimalField { get; set; }
}

public sealed class DictionaryModel
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, int> Metadata { get; set; } = new();
}
