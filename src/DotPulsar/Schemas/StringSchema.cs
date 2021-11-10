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
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Schema definition for UTF-8 (default), UTF-16 (unicode) or US-ASCII encoded strings.
/// </summary>
public sealed class StringSchema : ISchema<string>
{
    private const string _charSetKey = "__charset";
    private const string _utf8 = "UTF-8";
    private const string _unicode = "UTF-16";
    private const string _ascii = "US-ASCII";

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
            "Unicode (UTF-8)" => _utf8,
            "Unicode" => _unicode,
            "US-ASCII" => _ascii,
            _ => throw new Exception($"Encoding '{encodingName}' is not supported!")
        };
    }

    private static StringSchema GetSchema(string charSet)
    {
        return charSet switch
        {
            _utf8 => UTF8,
            _unicode => Unicode,
            _ascii => ASCII,
            _ => throw new Exception($"CharSet '{charSet}' is not supported!")
        };
    }

    public static StringSchema From(SchemaInfo schemaInfo)
    {
        if (schemaInfo.Type != SchemaType.String)
            throw new Exception("Not a string schema!");

        if (schemaInfo.Properties.TryGetValue(_charSetKey, out var charset))
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
                { _charSetKey, GetCharSet(encoding.EncodingName) }
            };

        SchemaInfo = new SchemaInfo("String", Array.Empty<byte>(), SchemaType.String, properties);
    }

    public SchemaInfo SchemaInfo { get; }

    public string Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion)
        => _encoding.GetString(bytes.ToArray());

    public ReadOnlySequence<byte> Encode(string message)
        => new(_encoding.GetBytes(message));
}
