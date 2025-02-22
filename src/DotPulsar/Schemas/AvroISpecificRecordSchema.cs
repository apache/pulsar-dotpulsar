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
using DotPulsar.Internal.Extensions;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public sealed class AvroISpecificRecordSchema<T> : ISchema<T>
{
    private readonly Type _typeT;
    private readonly object _avroSchema;
    private readonly object _avroWriter;
    private readonly MethodInfo _avroWriterWriteMethod;
    private readonly object _avroReader;
    private readonly MethodInfo _avroReaderReadMethod;
    private readonly TypeInfo _binaryEncoderTypeInfo;
    private readonly TypeInfo _binaryDecoderTypeInfo;

    public SchemaInfo SchemaInfo { get; }
    public AvroISpecificRecordSchema()
    {
        const string schemaFullName = "Avro.Schema";
        const string ISpecificRecordFullName = "Avro.Specific.ISpecificRecord";
        _typeT = typeof(T);
        string SchemaName;
        string SchemaData;
        try
        {
            if (!_typeT.GetInterfaces().Any(i => i.FullName == ISpecificRecordFullName))
                throw new SchemaException(string.Format("The type {0} must implement {1}", _typeT, ISpecificRecordFullName));
            _avroSchema = _typeT.GetField("_SCHEMA")?.GetValue(null) ??
                throw new SchemaException(string.Format("The static field named '_SCHEMA' must not be null in type: {0}", _typeT));
            Type avroSchemaType = _avroSchema.GetType();
            if (!avroSchemaType.ImplementsBaseTypeFullName(schemaFullName))
            {
                throw new Exception(string.Format("field '_SCHEMA' must be of type {0}", schemaFullName));
            }
            SchemaName = (string) (avroSchemaType.GetProperty("Name")?.GetValue(_avroSchema) ?? string.Empty);
            SchemaData = (string) (avroSchemaType
                .GetMethod("ToString", Type.EmptyTypes)
                ?.Invoke(_avroSchema, null) ??
                throw new Exception(string.Format("Schema toString() must not return null for type {0}", _typeT)));
            TryLoad(out object avroWriter, out object avroReader, out TypeInfo binaryEncoderType, out TypeInfo binaryDecoderType, out MethodInfo avroWriterMethod, out MethodInfo avroReaderMethod);
            _avroWriter = avroWriter;
            _avroReader = avroReader;
            _binaryEncoderTypeInfo = binaryEncoderType;
            _binaryDecoderTypeInfo = binaryDecoderType;
            _avroWriterWriteMethod = avroWriterMethod;
            _avroReaderReadMethod = avroReaderMethod;
        }
        catch (Exception e)
        {

            throw new SchemaException(string.Format("There has been an exception while creating schema for type: {0}", _typeT), e);
        }
        SchemaInfo = new SchemaInfo(SchemaName,
            Encoding.UTF8.GetBytes(SchemaData),
            SchemaType.Avro,
            new Dictionary<string, string>());
    }
    public T Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {
        using var stream = new MemoryStream(bytes.ToArray());
        T? def = default;
        return (T) (_avroReaderReadMethod.Invoke(_avroReader, [def, GetBinaryDecoder(stream)]) ?? throw new SchemaSerializationException(string.Format("Could not Deserialize object of type {0}", _typeT)));
    }

    public ReadOnlySequence<byte> Encode(T message)
    {
        using var stream = new MemoryStream();
        _avroWriterWriteMethod.Invoke(_avroWriter, [message, GetBinaryEncoder(stream)]);
        return new ReadOnlySequence<byte>(stream.ToArray());
    }
    private void TryLoad(out object avroWriter,
        out object avroReader,
        out TypeInfo binaryEncoderType,
        out TypeInfo binaryDecoderType,
        out MethodInfo avroWriterMethod,
        out MethodInfo avroReaderMethod)
    {
        var assembly = Assembly.Load("Avro");
        var definedTypes = assembly.DefinedTypes.ToArray();
        avroWriter = LoadSpecificDatumWriter(definedTypes);
        avroReader = LoadSpecificDatumReader(definedTypes);
        binaryEncoderType = LoadBinaryEncoderType(definedTypes);
        binaryDecoderType = LoadBinaryDecoderType(definedTypes);
        avroWriterMethod = LoadSpecificDatumWriterMethod(avroWriter.GetType().GetMethods());
        avroReaderMethod = LoadSpecificDatumReaderMethod(avroReader.GetType().GetMethods());
    }
    private object LoadSpecificDatumWriter(IEnumerable<TypeInfo> types)
    {
        const string fullName = "Avro.Specific.SpecificDatumWriter`1";
        foreach (var type in types)
        {
            if (type.FullName is null || !type.FullName.Equals(fullName))
                continue;
            if (type.IsPublic && type.IsClass)
            {
                var SpecificWritter = type.AsType().MakeGenericType(typeof(T));
                return Activator.CreateInstance(SpecificWritter, _avroSchema) ?? throw new Exception();
            }
            break;
        }
        throw new Exception($"{fullName} as a public class was not found");
    }
    private object LoadSpecificDatumReader(IEnumerable<TypeInfo> types)
    {
        const string fullName = "Avro.Specific.SpecificDatumReader`1";
        foreach (var type in types)
        {
            if (type.FullName is null || !type.FullName.Equals(fullName))
                continue;
            if (type.IsPublic && type.IsClass)
            {
                var SpecificReader = type.AsType().MakeGenericType(typeof(T));
                return Activator.CreateInstance(SpecificReader, _avroSchema, _avroSchema) ?? throw new Exception();
            }
            break;
        }
        throw new Exception($"{fullName} as a public class was not found");
    }
    private MethodInfo LoadSpecificDatumReaderMethod(IEnumerable<MethodInfo> methods)
    {
        const string name = "Read";
        const string secondParamFullname = "Avro.IO.Decoder";
        foreach (var method in methods)
        {
            if (method.Name != name || method.ReturnType != typeof(T))
                continue;

            var parameters = method.GetParameters();
            if (parameters.Length != 2)
                continue;

            if (parameters[0].ParameterType != typeof(T) ||
                !parameters[1].ParameterType.FullName.Equals(secondParamFullname))
                continue;

            return method;
        }
        throw new Exception($"A method with the name '{name}' matching the delegate was not found");
    }
    private MethodInfo LoadSpecificDatumWriterMethod(IEnumerable<MethodInfo> methods)
    {
        const string name = "Write";
        const string secondParamFullname = "Avro.IO.Encoder";
        foreach (var method in methods)
        {
            if (method.Name != name || method.ReturnType != typeof(void))
                continue;

            var parameters = method.GetParameters();
            if (parameters.Length != 2)
                continue;

            if (parameters[0].ParameterType != typeof(T) ||
                !parameters[1].ParameterType.FullName.Equals(secondParamFullname))
                continue;

            return method;
        }

        throw new Exception($"A method with the name '{name}' matching the delegate was not found");
    }
    private TypeInfo LoadBinaryEncoderType(IEnumerable<TypeInfo> types)
    {
        const string fullName = "Avro.IO.BinaryEncoder";

        foreach (var type in types)
        {
            if (type.FullName is null || !type.FullName.Equals(fullName))
                continue;

            if (type.IsPublic && type.IsClass)
                return type;

            break;
        }

        throw new Exception($"{fullName} as a public class was not found");
    }
    private TypeInfo LoadBinaryDecoderType(IEnumerable<TypeInfo> types)
    {
        const string fullName = "Avro.IO.BinaryDecoder";

        foreach (var type in types)
        {
            if (type.FullName is null || !type.FullName.Equals(fullName))
                continue;

            if (type.IsPublic && type.IsClass)
                return type;

            break;
        }

        throw new Exception($"{fullName} as a public class was not found");
    }
    private object GetBinaryEncoder(MemoryStream stream)
    {
        return Activator.CreateInstance(_binaryEncoderTypeInfo, stream) ?? throw new Exception("There was a problem while instanciating BinaryEncoder");
    }
    private object GetBinaryDecoder(MemoryStream stream)
    {
        return Activator.CreateInstance(_binaryDecoderTypeInfo, stream) ?? throw new Exception("There was a problem while instanciating BinaryDecoder");
    }

}
