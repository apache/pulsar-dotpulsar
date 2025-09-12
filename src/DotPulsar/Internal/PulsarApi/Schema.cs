#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class Schema : IMessage<Schema>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<Schema> _parser = new pb::MessageParser<Schema>(() => new Schema());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<Schema> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Schema() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Schema(Schema other) : this() {
        _hasBits0 = other._hasBits0;
        name_ = other.name_;
        schemaData_ = other.schemaData_;
        type_ = other.type_;
        properties_ = other.properties_.Clone();
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public Schema Clone() {
        return new Schema(this);
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 1;
    private readonly static string NameDefaultValue = "";

    private string name_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Name {
        get { return name_ ?? NameDefaultValue; }
        set {
            name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "name" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasName {
        get { return name_ != null; }
    }
    /// <summary>Clears the value of the "name" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearName() {
        name_ = null;
    }

    /// <summary>Field number for the "schema_data" field.</summary>
    public const int SchemaDataFieldNumber = 3;
    private readonly static pb::ByteString SchemaDataDefaultValue = pb::ByteString.Empty;

    private pb::ByteString schemaData_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString SchemaData {
        get { return schemaData_ ?? SchemaDataDefaultValue; }
        set {
            schemaData_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "schema_data" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSchemaData {
        get { return schemaData_ != null; }
    }
    /// <summary>Clears the value of the "schema_data" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSchemaData() {
        schemaData_ = null;
    }

    /// <summary>Field number for the "type" field.</summary>
    public const int TypeFieldNumber = 4;
    private readonly static global::DotPulsar.Internal.PulsarApi.Schema.Types.Type TypeDefaultValue = global::DotPulsar.Internal.PulsarApi.Schema.Types.Type.None;

    private global::DotPulsar.Internal.PulsarApi.Schema.Types.Type type_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.Schema.Types.Type Type {
        get { if ((_hasBits0 & 1) != 0) { return type_; } else { return TypeDefaultValue; } }
        set {
            _hasBits0 |= 1;
            type_ = value;
        }
    }
    /// <summary>Gets whether the "type" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasType {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "type" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearType() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "properties" field.</summary>
    public const int PropertiesFieldNumber = 5;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.KeyValue> _repeated_properties_codec
        = pb::FieldCodec.ForMessage(42, global::DotPulsar.Internal.PulsarApi.KeyValue.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> properties_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> Properties {
        get { return properties_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as Schema);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(Schema other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (Name != other.Name) return false;
        if (SchemaData != other.SchemaData) return false;
        if (Type != other.Type) return false;
        if(!properties_.Equals(other.properties_)) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasName) hash ^= Name.GetHashCode();
        if (HasSchemaData) hash ^= SchemaData.GetHashCode();
        if (HasType) hash ^= Type.GetHashCode();
        hash ^= properties_.GetHashCode();
        if (_unknownFields != null) {
            hash ^= _unknownFields.GetHashCode();
        }
        return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
        return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        output.WriteRawMessage(this);
#else
      if (HasName) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (HasSchemaData) {
        output.WriteRawTag(26);
        output.WriteBytes(SchemaData);
      }
      if (HasType) {
        output.WriteRawTag(32);
        output.WriteEnum((int) Type);
      }
      properties_.WriteTo(output, _repeated_properties_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
        if (HasName) {
            output.WriteRawTag(10);
            output.WriteString(Name);
        }
        if (HasSchemaData) {
            output.WriteRawTag(26);
            output.WriteBytes(SchemaData);
        }
        if (HasType) {
            output.WriteRawTag(32);
            output.WriteEnum((int) Type);
        }
        properties_.WriteTo(ref output, _repeated_properties_codec);
        if (_unknownFields != null) {
            _unknownFields.WriteTo(ref output);
        }
    }
#endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
        int size = 0;
        if (HasName) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
        }
        if (HasSchemaData) {
            size += 1 + pb::CodedOutputStream.ComputeBytesSize(SchemaData);
        }
        if (HasType) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Type);
        }
        size += properties_.CalculateSize(_repeated_properties_codec);
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(Schema other) {
        if (other == null) {
            return;
        }
        if (other.HasName) {
            Name = other.Name;
        }
        if (other.HasSchemaData) {
            SchemaData = other.SchemaData;
        }
        if (other.HasType) {
            Type = other.Type;
        }
        properties_.Add(other.properties_);
        _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        input.ReadRawMessage(this);
#else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
      if ((tag & 7) == 4) {
        // Abort on any end group tag.
        return;
      }
      switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 26: {
            SchemaData = input.ReadBytes();
            break;
          }
          case 32: {
            Type = (global::DotPulsar.Internal.PulsarApi.Schema.Types.Type) input.ReadEnum();
            break;
          }
          case 42: {
            properties_.AddEntriesFrom(input, _repeated_properties_codec);
            break;
          }
        }
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
        uint tag;
        while ((tag = input.ReadTag()) != 0) {
            if ((tag & 7) == 4) {
                // Abort on any end group tag.
                return;
            }
            switch(tag) {
                default:
                    _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
                    break;
                case 10: {
                    Name = input.ReadString();
                    break;
                }
                case 26: {
                    SchemaData = input.ReadBytes();
                    break;
                }
                case 32: {
                    Type = (global::DotPulsar.Internal.PulsarApi.Schema.Types.Type) input.ReadEnum();
                    break;
                }
                case 42: {
                    properties_.AddEntriesFrom(ref input, _repeated_properties_codec);
                    break;
                }
            }
        }
    }
#endif

    #region Nested types
    /// <summary>Container for nested types declared in the Schema message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Types {
        public enum Type {
            [pbr::OriginalName("None")] None = 0,
            [pbr::OriginalName("String")] String = 1,
            [pbr::OriginalName("Json")] Json = 2,
            [pbr::OriginalName("Protobuf")] Protobuf = 3,
            [pbr::OriginalName("Avro")] Avro = 4,
            [pbr::OriginalName("Bool")] Bool = 5,
            [pbr::OriginalName("Int8")] Int8 = 6,
            [pbr::OriginalName("Int16")] Int16 = 7,
            [pbr::OriginalName("Int32")] Int32 = 8,
            [pbr::OriginalName("Int64")] Int64 = 9,
            [pbr::OriginalName("Float")] Float = 10,
            [pbr::OriginalName("Double")] Double = 11,
            [pbr::OriginalName("Date")] Date = 12,
            [pbr::OriginalName("Time")] Time = 13,
            [pbr::OriginalName("Timestamp")] Timestamp = 14,
            [pbr::OriginalName("KeyValue")] KeyValue = 15,
            [pbr::OriginalName("Instant")] Instant = 16,
            [pbr::OriginalName("LocalDate")] LocalDate = 17,
            [pbr::OriginalName("LocalTime")] LocalTime = 18,
            [pbr::OriginalName("LocalDateTime")] LocalDateTime = 19,
            [pbr::OriginalName("ProtobufNative")] ProtobufNative = 20,
            [pbr::OriginalName("AutoConsume")] AutoConsume = 21,
            [pbr::OriginalName("External")] External = 22,
        }

    }
    #endregion

}
