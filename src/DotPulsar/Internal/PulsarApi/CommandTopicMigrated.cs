#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandTopicMigrated : IMessage<CommandTopicMigrated>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandTopicMigrated> _parser = new pb::MessageParser<CommandTopicMigrated>(() => new CommandTopicMigrated());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandTopicMigrated> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[33]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandTopicMigrated() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandTopicMigrated(CommandTopicMigrated other) : this() {
        _hasBits0 = other._hasBits0;
        resourceId_ = other.resourceId_;
        resourceType_ = other.resourceType_;
        brokerServiceUrl_ = other.brokerServiceUrl_;
        brokerServiceUrlTls_ = other.brokerServiceUrlTls_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandTopicMigrated Clone() {
        return new CommandTopicMigrated(this);
    }

    /// <summary>Field number for the "resource_id" field.</summary>
    public const int ResourceIdFieldNumber = 1;
    private readonly static ulong ResourceIdDefaultValue = 0UL;

    private ulong resourceId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong ResourceId {
        get { if ((_hasBits0 & 1) != 0) { return resourceId_; } else { return ResourceIdDefaultValue; } }
        set {
            _hasBits0 |= 1;
            resourceId_ = value;
        }
    }
    /// <summary>Gets whether the "resource_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasResourceId {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "resource_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearResourceId() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "resource_type" field.</summary>
    public const int ResourceTypeFieldNumber = 2;
    private readonly static global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated.Types.ResourceType ResourceTypeDefaultValue = global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated.Types.ResourceType.Producer;

    private global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated.Types.ResourceType resourceType_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated.Types.ResourceType ResourceType {
        get { if ((_hasBits0 & 2) != 0) { return resourceType_; } else { return ResourceTypeDefaultValue; } }
        set {
            _hasBits0 |= 2;
            resourceType_ = value;
        }
    }
    /// <summary>Gets whether the "resource_type" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasResourceType {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "resource_type" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearResourceType() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "brokerServiceUrl" field.</summary>
    public const int BrokerServiceUrlFieldNumber = 3;
    private readonly static string BrokerServiceUrlDefaultValue = "";

    private string brokerServiceUrl_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string BrokerServiceUrl {
        get { return brokerServiceUrl_ ?? BrokerServiceUrlDefaultValue; }
        set {
            brokerServiceUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "brokerServiceUrl" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasBrokerServiceUrl {
        get { return brokerServiceUrl_ != null; }
    }
    /// <summary>Clears the value of the "brokerServiceUrl" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearBrokerServiceUrl() {
        brokerServiceUrl_ = null;
    }

    /// <summary>Field number for the "brokerServiceUrlTls" field.</summary>
    public const int BrokerServiceUrlTlsFieldNumber = 4;
    private readonly static string BrokerServiceUrlTlsDefaultValue = "";

    private string brokerServiceUrlTls_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string BrokerServiceUrlTls {
        get { return brokerServiceUrlTls_ ?? BrokerServiceUrlTlsDefaultValue; }
        set {
            brokerServiceUrlTls_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "brokerServiceUrlTls" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasBrokerServiceUrlTls {
        get { return brokerServiceUrlTls_ != null; }
    }
    /// <summary>Clears the value of the "brokerServiceUrlTls" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearBrokerServiceUrlTls() {
        brokerServiceUrlTls_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandTopicMigrated);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandTopicMigrated other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ResourceId != other.ResourceId) return false;
        if (ResourceType != other.ResourceType) return false;
        if (BrokerServiceUrl != other.BrokerServiceUrl) return false;
        if (BrokerServiceUrlTls != other.BrokerServiceUrlTls) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasResourceId) hash ^= ResourceId.GetHashCode();
        if (HasResourceType) hash ^= ResourceType.GetHashCode();
        if (HasBrokerServiceUrl) hash ^= BrokerServiceUrl.GetHashCode();
        if (HasBrokerServiceUrlTls) hash ^= BrokerServiceUrlTls.GetHashCode();
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
      if (HasResourceId) {
        output.WriteRawTag(8);
        output.WriteUInt64(ResourceId);
      }
      if (HasResourceType) {
        output.WriteRawTag(16);
        output.WriteEnum((int) ResourceType);
      }
      if (HasBrokerServiceUrl) {
        output.WriteRawTag(26);
        output.WriteString(BrokerServiceUrl);
      }
      if (HasBrokerServiceUrlTls) {
        output.WriteRawTag(34);
        output.WriteString(BrokerServiceUrlTls);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
#endif
    }

#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
        if (HasResourceId) {
            output.WriteRawTag(8);
            output.WriteUInt64(ResourceId);
        }
        if (HasResourceType) {
            output.WriteRawTag(16);
            output.WriteEnum((int) ResourceType);
        }
        if (HasBrokerServiceUrl) {
            output.WriteRawTag(26);
            output.WriteString(BrokerServiceUrl);
        }
        if (HasBrokerServiceUrlTls) {
            output.WriteRawTag(34);
            output.WriteString(BrokerServiceUrlTls);
        }
        if (_unknownFields != null) {
            _unknownFields.WriteTo(ref output);
        }
    }
#endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
        int size = 0;
        if (HasResourceId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ResourceId);
        }
        if (HasResourceType) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ResourceType);
        }
        if (HasBrokerServiceUrl) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(BrokerServiceUrl);
        }
        if (HasBrokerServiceUrlTls) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(BrokerServiceUrlTls);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandTopicMigrated other) {
        if (other == null) {
            return;
        }
        if (other.HasResourceId) {
            ResourceId = other.ResourceId;
        }
        if (other.HasResourceType) {
            ResourceType = other.ResourceType;
        }
        if (other.HasBrokerServiceUrl) {
            BrokerServiceUrl = other.BrokerServiceUrl;
        }
        if (other.HasBrokerServiceUrlTls) {
            BrokerServiceUrlTls = other.BrokerServiceUrlTls;
        }
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
          case 8: {
            ResourceId = input.ReadUInt64();
            break;
          }
          case 16: {
            ResourceType = (global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated.Types.ResourceType) input.ReadEnum();
            break;
          }
          case 26: {
            BrokerServiceUrl = input.ReadString();
            break;
          }
          case 34: {
            BrokerServiceUrlTls = input.ReadString();
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
                case 8: {
                    ResourceId = input.ReadUInt64();
                    break;
                }
                case 16: {
                    ResourceType = (global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated.Types.ResourceType) input.ReadEnum();
                    break;
                }
                case 26: {
                    BrokerServiceUrl = input.ReadString();
                    break;
                }
                case 34: {
                    BrokerServiceUrlTls = input.ReadString();
                    break;
                }
            }
        }
    }
#endif

    #region Nested types
    /// <summary>Container for nested types declared in the CommandTopicMigrated message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Types {
        public enum ResourceType {
            [pbr::OriginalName("Producer")] Producer = 0,
            [pbr::OriginalName("Consumer")] Consumer = 1,
        }

    }
    #endregion

}
