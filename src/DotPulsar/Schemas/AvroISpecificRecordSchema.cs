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
using System.Reflection;
using System.Text;

public sealed class AvroISpecificRecordSchema<T> : ISchema<T>
{
    private const string SchemaField = "_SCHEMA";
    private const string AvroSchemaFullName = "Avro.Schema";
    private const string AvroISpecificRecordFullName = "Avro.Specific.ISpecificRecord";

    private static readonly Type _typeT;
    private static readonly object _avroSchema;
    private static readonly MethodInfo _avroWriterWriteMethod;
    private static readonly MethodInfo _avroReaderReadMethod;
    private static readonly TypeInfo _binaryEncoderTypeInfo;
    private static readonly TypeInfo _binaryDecoderTypeInfo;
    private static readonly Type _avroWriterTypeInfo;
    private static readonly Type _avroReaderTypeInfo;
    private static readonly SchemaInfo _schemaInfo;
    private static readonly Exception _constructorException;

    private readonly object _avroWriter;
    private readonly object _avroReader;

    public SchemaInfo SchemaInfo => _schemaInfo;

#pragma warning disable CS8618 // Supressed because if there is an init error the non-static constructor will throw it instead. This is done in case of if there is a wrong implementation of ISpecificRecord in T in order not to stop the whole runtime.
    static AvroISpecificRecordSchema()
#pragma warning restore CS8618 // Supressed because if there is an init error the non-static constructor will throw it instead. This is done in case of if there is a wrong implementation of ISpecificRecord in T in order not to stop the whole runtime.
    {
        _typeT = typeof(T);

        try
        {
            if (!_typeT.GetInterfaces().Any(i => i.FullName == AvroISpecificRecordFullName))
                throw new SchemaException($"The type '{_typeT}' must implement '{AvroISpecificRecordFullName}'");

            _avroSchema = _typeT.GetField(SchemaField)?.GetValue(null) ?? throw new SchemaException($"The static field '{SchemaField}' must not be null in type '{_typeT}'");

            var avroSchemaType = _avroSchema.GetType();
            if (!avroSchemaType.ImplementsBaseTypeFullName(AvroSchemaFullName))
                throw new SchemaException($"The static field '{SchemaField}' must be of type '{AvroSchemaFullName}'");
            
            TryLoadStatic(out Type avroWriterType, out Type avroReaderType, out TypeInfo binaryEncoderType, out TypeInfo binaryDecoderType, out MethodInfo avroWriterMethod, out MethodInfo avroReaderMethod);
            _avroWriterTypeInfo = avroWriterType;
            _avroReaderTypeInfo = avroReaderType;
            _binaryEncoderTypeInfo = binaryEncoderType;
            _binaryDecoderTypeInfo = binaryDecoderType;
            _avroWriterWriteMethod = avroWriterMethod;
            _avroReaderReadMethod = avroReaderMethod;

            var schemaName = (string) (avroSchemaType.GetProperty("Name")?.GetValue(_avroSchema) ?? string.Empty);
            var schemaData = (string) (avroSchemaType.GetMethod("ToString", Type.EmptyTypes)?.Invoke(_avroSchema, null) ?? throw new SchemaException($"Schema 'ToString()' must not return null for type '{_typeT}'"));
            _schemaInfo = new SchemaInfo(schemaName, Encoding.UTF8.GetBytes(schemaData), SchemaType.Avro, new Dictionary<string, string>());
        }
        catch (Exception exception)
        {
            _constructorException = exception;
        }
    }

    public AvroISpecificRecordSchema()
    {
        if (_constructorException is not null)
            throw _constructorException;

        TryLoad(out object avroWriter, out object avroReader);
        _avroWriter = avroWriter;
        _avroReader = avroReader;
    }

    public T Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {
        using var stream = new MemoryStream(bytes.ToArray());
        T? def = default;
        return (T) (_avroReaderReadMethod.Invoke(_avroReader, [def, GetBinaryDecoder(stream)]) ?? throw new SchemaSerializationException($"Could not deserialize object of type '{_typeT}'"));
    }

    public ReadOnlySequence<byte> Encode(T message)
    {
        using var stream = new MemoryStream();
        _avroWriterWriteMethod.Invoke(_avroWriter, [message, GetBinaryEncoder(stream)]);
        return new ReadOnlySequence<byte>(stream.ToArray());
    }

    private static void TryLoadStatic(
        out Type avroWriter,
        out Type avroReader,
        out TypeInfo binaryEncoderType,
        out TypeInfo binaryDecoderType,
        out MethodInfo avroWriterMethod,
        out MethodInfo avroReaderMethod)
    {
        var assembly = Assembly.Load("Avro");
        var definedTypes = assembly.DefinedTypes.ToArray();
        avroWriter = LoadSpecificDatumWriterType(definedTypes);
        avroReader = LoadSpecificDatumReaderType(definedTypes);
        binaryEncoderType = LoadBinaryEncoderType(definedTypes);
        binaryDecoderType = LoadBinaryDecoderType(definedTypes);
        avroWriterMethod = LoadSpecificDatumWriterMethod(avroWriter.GetMethods());
        avroReaderMethod = LoadSpecificDatumReaderMethod(avroReader.GetMethods());
    }

    private void TryLoad(out object avroWriter, out object avroReader)
    {
        avroWriter = LoadSpecificDatumWriter();
        avroReader = LoadSpecificDatumReader();
    }

    private static Type LoadSpecificDatumWriterType(IEnumerable<TypeInfo> types)
    {
        const string fullName = "Avro.Specific.SpecificDatumWriter`1";

        foreach (var type in types)
        {
            if (type.IsPublic && type.IsClass && type.IsGenericType && type.FullName is not null && type.FullName.Equals(fullName))
                return type.MakeGenericType(typeof(T));
        }

        throw new SchemaException($"'{fullName}' as a generic public class was not found");
    }

    private static Type LoadSpecificDatumReaderType(IEnumerable<TypeInfo> types)
    {
        const string fullName = "Avro.Specific.SpecificDatumReader`1";

        foreach (var type in types)
        {
            if (type.IsPublic && type.IsClass && type.IsGenericType && type.FullName is not null && type.FullName.Equals(fullName))
                return type.MakeGenericType(typeof(T));
        }

        throw new SchemaException($"'{fullName}' as a generic public class was not found");
    }

    private object LoadSpecificDatumWriter()
        => Activator.CreateInstance(_avroWriterTypeInfo, _avroSchema) ?? throw new SchemaException("Could not create SpecificDatumWriter");

    private object LoadSpecificDatumReader()
        => Activator.CreateInstance(_avroReaderTypeInfo, _avroSchema, _avroSchema) ?? throw new SchemaException("Could not create SpecificDatumReader");

    private static MethodInfo LoadSpecificDatumReaderMethod(IEnumerable<MethodInfo> methods)
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

            var param1Fullname = parameters[1].ParameterType.FullName;
            if (param1Fullname is null)
                continue;

            if (parameters[0].ParameterType != typeof(T) || !param1Fullname.Equals(secondParamFullname))
                continue;

            return method;
        }

        throw new SchemaException($"A method with the name '{name}' matching the delegate was not found");
    }

    private static MethodInfo LoadSpecificDatumWriterMethod(IEnumerable<MethodInfo> methods)
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

            var param1Fullname = parameters[1].ParameterType.FullName;
            if (param1Fullname is null)
                continue;

            if (parameters[0].ParameterType != typeof(T) || !param1Fullname.Equals(secondParamFullname))
                continue;

            return method;
        }

        throw new SchemaException($"A method with the name '{name}' matching the delegate was not found");
    }

    private static TypeInfo LoadBinaryEncoderType(IEnumerable<TypeInfo> types)
    {
        const string fullName = "Avro.IO.BinaryEncoder";

        foreach (var type in types)
        {
            if (type.IsPublic && type.IsClass && type.FullName is not null && type.FullName.Equals(fullName))
                return type;
        }

        throw new SchemaException($"'{fullName}' as a public class was not found");
    }

    private static TypeInfo LoadBinaryDecoderType(IEnumerable<TypeInfo> types)
    {
        const string fullName = "Avro.IO.BinaryDecoder";

        foreach (var type in types)
        {
            if (type.IsPublic && type.IsClass && type.FullName is not null && type.FullName.Equals(fullName))
                return type;
        }

        throw new SchemaException($"'{fullName}' as a public class was not found");
    }

    private static object GetBinaryEncoder(MemoryStream stream)
        => Activator.CreateInstance(_binaryEncoderTypeInfo, stream) ?? throw new SchemaException("There was a problem while instantiating BinaryEncoder");

    private static object GetBinaryDecoder(MemoryStream stream)
        => Activator.CreateInstance(_binaryDecoderTypeInfo, stream) ?? throw new SchemaException("There was a problem while instantiating BinaryDecoder");
}
