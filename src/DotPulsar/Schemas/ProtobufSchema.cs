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
using Google.Protobuf;
using System.Buffers;
using System.Collections.Immutable;

/// <summary>
/// Schema definition for Protobuf encoded messages.
/// </summary>
public sealed class ProtobufSchema<T> : ISchema<T> where T : Google.Protobuf.IMessage<T>, new()
{
    public ProtobufSchema()
    {
        var instance = new T();
        var name = instance.Descriptor.Name;
        var data = instance.Descriptor.File.SerializedData.ToByteArray();

        SchemaInfo = new SchemaInfo(name, data, SchemaType.ProtobufNative, ImmutableDictionary<string, string>.Empty);
    }

    public SchemaInfo SchemaInfo { get; }

    public T Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {
        try
        {
            var message = new T();
            message.MergeFrom(bytes.ToArray());
            return message;
        }
        catch (InvalidProtocolBufferException exception)
        {
            throw new Exceptions.SchemaSerializationException($"Failed to decode Protobuf message of type '{typeof(T).Name}'", exception);
        }
    }

    public ReadOnlySequence<byte> Encode(T message)
    {
        var bytes = message.ToByteArray();
        return new ReadOnlySequence<byte>(bytes);
    }
}
