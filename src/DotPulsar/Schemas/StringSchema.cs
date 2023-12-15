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
using System.Buffers;
using System.Text;

/// <summary>
/// Schema definition for UTF-8 (default), UTF-16 (unicode) or US-ASCII encoded strings.
/// </summary>
public sealed class StringSchema : ISchema<string>
{
    private const string CharSetKey = "__charset";
    private const string Utf8Encoding = "UTF-8";
    private const string UnicodeEncoding = "UTF-16";
    private const string AsciiEncoding = "US-ASCII";

    static StringSchema()
    {
        UTF8 = new StringSchema(Encoding.UTF8);
        Unicode = new StringSchema(Encoding.Unicode);
        ASCII = new StringSchema(Encoding.ASCII);
    }

    /// <summary>
    /// Schema definition for UTF-8 encoded strings.
    /// </summary>
    public static StringSchema UTF8 { get; }

    /// <summary>
    /// Schema definition for UTF-16 encoded strings.
    /// </summary>
    public static StringSchema Unicode { get; }

    /// <summary>
    /// Schema definition for US-ASCII encoded strings.
    /// </summary>
    public static StringSchema ASCII { get; }

    private static string GetCharSet(string encodingName)
    {
        return encodingName switch
        {
            "Unicode (UTF-8)" => Utf8Encoding,
            "Unicode" => UnicodeEncoding,
            "US-ASCII" => AsciiEncoding,
            _ => throw new Exception($"Encoding '{encodingName}' is not supported!")
        };
    }

    private static StringSchema GetSchema(string charSet)
    {
        return charSet switch
        {
            Utf8Encoding => UTF8,
            UnicodeEncoding => Unicode,
            AsciiEncoding => ASCII,
            _ => throw new Exception($"CharSet '{charSet}' is not supported!")
        };
    }

    public static StringSchema From(SchemaInfo schemaInfo)
    {
        if (schemaInfo.Type != SchemaType.String)
            throw new Exception("Not a string schema!");

        if (schemaInfo.Properties.TryGetValue(CharSetKey, out var charset))
            return GetSchema(charset);
        else
            return UTF8;
    }

    private readonly Encoding _encoding;

    public StringSchema(Encoding encoding)
    {
        _encoding = encoding;

        var properties = new Dictionary<string, string>
        {
            { CharSetKey, GetCharSet(encoding.EncodingName) }
        };

        SchemaInfo = new SchemaInfo("String", Array.Empty<byte>(), SchemaType.String, properties);
    }

    public SchemaInfo SchemaInfo { get; }

    public string Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion)
        => _encoding.GetString(bytes.ToArray());

    public ReadOnlySequence<byte> Encode(string message)
        => new(_encoding.GetBytes(message));
}
