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
using System.Reflection;
using System.Text;

public sealed class AvroGenericRecordSchema<T> : ISchema<T>
{
    private const string AvroSchemafullName = "Avro.Schema";
    private const string AvroGenericRecordFullName = "Avro.Generic.GenericRecord";

    private static readonly Type _typeT;
    private static readonly MethodInfo _avroReaderReadMethod;
    private static readonly MethodInfo _avroWriterWriteMethod;
    private static readonly TypeInfo _binaryDecoderTypeInfo;
    private static readonly TypeInfo _binaryEncoderTypeInfo;
    private static readonly Type _avroReaderTypeInfo;
    private static readonly Type _avroWriterTypeInfo;
    private static readonly Exception _constructorException;
    private static readonly TypeInfo _schemaTypeInfo;

    private readonly object _avroReader;
    private readonly object _avroWriter;
    private readonly object avroSchema;

    public SchemaInfo SchemaInfo { get; }

#pragma warning disable CS8618 // Supressed because if there is an init error the non-static constructor will throw it instead. This is done in case of if there is a problem loading GenericRecord in order not to stop the whole runtime.
    static AvroGenericRecordSchema()
#pragma warning restore CS8618 // Supressed because if there is an init error the non-static constructor will throw it instead. This is done in case of if there is a problem loading GenericRecord in order not to stop the whole runtime.
    {
        _typeT = typeof(T);

        try
        {
            if (_typeT.FullName is not AvroGenericRecordFullName)
                throw new SchemaException($"The type '{_typeT}' must be '{AvroGenericRecordFullName}'");

            TryLoadStatic(out Type avroReaderType, out Type avroWriterType, out TypeInfo binaryDecoderType, out TypeInfo binaryEncoderType, out MethodInfo avroReaderMethod, out MethodInfo avroWriterMethod, out TypeInfo schemaType);
            _avroReaderTypeInfo = avroReaderType;
            _avroWriterTypeInfo = avroWriterType;
            _binaryDecoderTypeInfo = binaryDecoderType;
            _binaryEncoderTypeInfo = binaryEncoderType;
            _avroReaderReadMethod = avroReaderMethod;
            _avroWriterWriteMethod = avroWriterMethod;
            _schemaTypeInfo = schemaType;
        }
        catch (Exception exception)
        {
            _constructorException = exception;
        }
    }

    public AvroGenericRecordSchema(string jsonAvroSchema)
    {
        if (_constructorException is not null)
            throw _constructorException;

        var schemaParseMethod = LoadAvroSchemaParseMethod(_schemaTypeInfo.GetMethods());
        try
        {
            avroSchema = schemaParseMethod.Invoke(null, [jsonAvroSchema]) ?? throw new SchemaException($"Could not create schema from jsonSchema '{jsonAvroSchema}'");
        }
        catch (Exception exception)
        {
            throw new SchemaException($"Could not create schema from jsonSchema '{jsonAvroSchema}'", exception);
        }
        var schemaName = (string) (_schemaTypeInfo.GetProperty("Name")?.GetValue(avroSchema) ?? string.Empty);
        var schemaData = (string) (_schemaTypeInfo.GetMethod("ToString", Type.EmptyTypes)?.Invoke(avroSchema, null) ?? throw new SchemaException($"Schema 'ToString()' must not return null for type '{_typeT}'"));
        SchemaInfo = new SchemaInfo(schemaName, Encoding.UTF8.GetBytes(schemaData), SchemaType.Avro, new Dictionary<string, string>());

        TryLoad(out object avroReader, out object avroWriter);
        _avroReader = avroReader;
        _avroWriter = avroWriter;
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
        out Type avroReader,
        out Type avroWriter,
        out TypeInfo binaryDecoderType,
        out TypeInfo binaryEncoderType,
        out MethodInfo avroReaderMethod,
        out MethodInfo avroWriterMethod,
        out TypeInfo schemaType)
    {
        var assembly = Assembly.Load("Avro");
        var definedTypes = assembly.DefinedTypes.ToArray();
        avroReader = LoadGenericDatumReaderType(definedTypes);
        avroWriter = LoadGenericDatumWriterType(definedTypes);
        schemaType = LoadSchemaType(definedTypes);
        binaryDecoderType = LoadBinaryDecoderType(definedTypes);
        binaryEncoderType = LoadBinaryEncoderType(definedTypes);
        avroReaderMethod = LoadGenericDatumReaderMethod(avroReader.GetMethods());
        avroWriterMethod = LoadGenericDatumWriterMethod(avroWriter.GetMethods());
    }

    private void TryLoad(out object avroReader, out object avroWriter)
    {
        avroReader = LoadGenericDatumReader();
        avroWriter = LoadGenericDatumWriter();
    }

    private static Type LoadGenericDatumReaderType(IEnumerable<TypeInfo> types)
    {
        const string fullName = "Avro.Generic.GenericDatumReader`1";

        foreach (var type in types)
        {
            if (type.IsPublic && type.IsGenericType && type.FullName is not null && type.FullName.Equals(fullName))
                return type.MakeGenericType(typeof(T));
        }

        throw new SchemaException($"'{fullName}' as a generic public class was not found");
    }

    private static Type LoadGenericDatumWriterType(IEnumerable<TypeInfo> types)
    {
        const string fullName = "Avro.Generic.GenericDatumWriter`1";

        foreach (var type in types)
        {
            if (type.IsPublic && type.IsGenericType && type.FullName is not null && type.FullName.Equals(fullName))
                return type.MakeGenericType(typeof(T));
        }

        throw new SchemaException($"'{fullName}' as a generic public class was not found");
    }

    private static TypeInfo LoadSchemaType(IEnumerable<TypeInfo> types)
    {
        foreach (var type in types)
        {
            if (type.IsPublic && type.IsClass && !type.IsGenericType && type.FullName is not null && type.FullName.Equals(AvroSchemafullName))
                return type;
        }

        throw new SchemaException($"'{AvroSchemafullName}' as a generic public class was not found");
    }

    private static MethodInfo LoadAvroSchemaParseMethod(IEnumerable<MethodInfo> methods)
    {
        const string name = "Parse";

        foreach (var method in methods)
        {
            if (method.Name != name || method.ReturnType.FullName != AvroSchemafullName || !method.IsStatic)
                continue;

            var parameters = method.GetParameters();
            if (parameters.Length != 1)
                continue;

            if (parameters[0].ParameterType != typeof(string))
                continue;

            return method;
        }

        throw new SchemaException($"A method with the name '{name}' matching the delegate was not found");
    }

    private object LoadGenericDatumReader()
        => Activator.CreateInstance(_avroReaderTypeInfo, avroSchema, avroSchema) ?? throw new SchemaException("Could not create GenericDatumReader");

    private object LoadGenericDatumWriter()
        => Activator.CreateInstance(_avroWriterTypeInfo, avroSchema) ?? throw new SchemaException("Could not create GenericDatumWriter");

    private static MethodInfo LoadGenericDatumReaderMethod(IEnumerable<MethodInfo> methods)
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

    private static MethodInfo LoadGenericDatumWriterMethod(IEnumerable<MethodInfo> methods)
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

    private static object GetBinaryEncoder(MemoryStream stream)
        => Activator.CreateInstance(_binaryEncoderTypeInfo, stream) ?? throw new SchemaException("There was a problem while instantiating BinaryEncoder");

    private static object GetBinaryDecoder(MemoryStream stream)
        => Activator.CreateInstance(_binaryDecoderTypeInfo, stream) ?? throw new SchemaException("There was a problem while instantiating BinaryDecoder");
}
