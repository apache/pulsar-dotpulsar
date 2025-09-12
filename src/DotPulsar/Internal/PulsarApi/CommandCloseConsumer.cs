#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandCloseConsumer : IMessage<CommandCloseConsumer>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandCloseConsumer> _parser = new pb::MessageParser<CommandCloseConsumer>(() => new CommandCloseConsumer());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandCloseConsumer> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[35]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandCloseConsumer() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandCloseConsumer(CommandCloseConsumer other) : this() {
        _hasBits0 = other._hasBits0;
        consumerId_ = other.consumerId_;
        requestId_ = other.requestId_;
        assignedBrokerServiceUrl_ = other.assignedBrokerServiceUrl_;
        assignedBrokerServiceUrlTls_ = other.assignedBrokerServiceUrlTls_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandCloseConsumer Clone() {
        return new CommandCloseConsumer(this);
    }

    /// <summary>Field number for the "consumer_id" field.</summary>
    public const int ConsumerIdFieldNumber = 1;
    private readonly static ulong ConsumerIdDefaultValue = 0UL;

    private ulong consumerId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong ConsumerId {
        get { if ((_hasBits0 & 1) != 0) { return consumerId_; } else { return ConsumerIdDefaultValue; } }
        set {
            _hasBits0 |= 1;
            consumerId_ = value;
        }
    }
    /// <summary>Gets whether the "consumer_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasConsumerId {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "consumer_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearConsumerId() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 2;
    private readonly static ulong RequestIdDefaultValue = 0UL;

    private ulong requestId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong RequestId {
        get { if ((_hasBits0 & 2) != 0) { return requestId_; } else { return RequestIdDefaultValue; } }
        set {
            _hasBits0 |= 2;
            requestId_ = value;
        }
    }
    /// <summary>Gets whether the "request_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasRequestId {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "request_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearRequestId() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "assignedBrokerServiceUrl" field.</summary>
    public const int AssignedBrokerServiceUrlFieldNumber = 3;
    private readonly static string AssignedBrokerServiceUrlDefaultValue = "";

    private string assignedBrokerServiceUrl_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string AssignedBrokerServiceUrl {
        get { return assignedBrokerServiceUrl_ ?? AssignedBrokerServiceUrlDefaultValue; }
        set {
            assignedBrokerServiceUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "assignedBrokerServiceUrl" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasAssignedBrokerServiceUrl {
        get { return assignedBrokerServiceUrl_ != null; }
    }
    /// <summary>Clears the value of the "assignedBrokerServiceUrl" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearAssignedBrokerServiceUrl() {
        assignedBrokerServiceUrl_ = null;
    }

    /// <summary>Field number for the "assignedBrokerServiceUrlTls" field.</summary>
    public const int AssignedBrokerServiceUrlTlsFieldNumber = 4;
    private readonly static string AssignedBrokerServiceUrlTlsDefaultValue = "";

    private string assignedBrokerServiceUrlTls_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string AssignedBrokerServiceUrlTls {
        get { return assignedBrokerServiceUrlTls_ ?? AssignedBrokerServiceUrlTlsDefaultValue; }
        set {
            assignedBrokerServiceUrlTls_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "assignedBrokerServiceUrlTls" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasAssignedBrokerServiceUrlTls {
        get { return assignedBrokerServiceUrlTls_ != null; }
    }
    /// <summary>Clears the value of the "assignedBrokerServiceUrlTls" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearAssignedBrokerServiceUrlTls() {
        assignedBrokerServiceUrlTls_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandCloseConsumer);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandCloseConsumer other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ConsumerId != other.ConsumerId) return false;
        if (RequestId != other.RequestId) return false;
        if (AssignedBrokerServiceUrl != other.AssignedBrokerServiceUrl) return false;
        if (AssignedBrokerServiceUrlTls != other.AssignedBrokerServiceUrlTls) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasConsumerId) hash ^= ConsumerId.GetHashCode();
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasAssignedBrokerServiceUrl) hash ^= AssignedBrokerServiceUrl.GetHashCode();
        if (HasAssignedBrokerServiceUrlTls) hash ^= AssignedBrokerServiceUrlTls.GetHashCode();
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
      if (HasConsumerId) {
        output.WriteRawTag(8);
        output.WriteUInt64(ConsumerId);
      }
      if (HasRequestId) {
        output.WriteRawTag(16);
        output.WriteUInt64(RequestId);
      }
      if (HasAssignedBrokerServiceUrl) {
        output.WriteRawTag(26);
        output.WriteString(AssignedBrokerServiceUrl);
      }
      if (HasAssignedBrokerServiceUrlTls) {
        output.WriteRawTag(34);
        output.WriteString(AssignedBrokerServiceUrlTls);
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
        if (HasConsumerId) {
            output.WriteRawTag(8);
            output.WriteUInt64(ConsumerId);
        }
        if (HasRequestId) {
            output.WriteRawTag(16);
            output.WriteUInt64(RequestId);
        }
        if (HasAssignedBrokerServiceUrl) {
            output.WriteRawTag(26);
            output.WriteString(AssignedBrokerServiceUrl);
        }
        if (HasAssignedBrokerServiceUrlTls) {
            output.WriteRawTag(34);
            output.WriteString(AssignedBrokerServiceUrlTls);
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
        if (HasConsumerId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ConsumerId);
        }
        if (HasRequestId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(RequestId);
        }
        if (HasAssignedBrokerServiceUrl) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(AssignedBrokerServiceUrl);
        }
        if (HasAssignedBrokerServiceUrlTls) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(AssignedBrokerServiceUrlTls);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandCloseConsumer other) {
        if (other == null) {
            return;
        }
        if (other.HasConsumerId) {
            ConsumerId = other.ConsumerId;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.HasAssignedBrokerServiceUrl) {
            AssignedBrokerServiceUrl = other.AssignedBrokerServiceUrl;
        }
        if (other.HasAssignedBrokerServiceUrlTls) {
            AssignedBrokerServiceUrlTls = other.AssignedBrokerServiceUrlTls;
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
            ConsumerId = input.ReadUInt64();
            break;
          }
          case 16: {
            RequestId = input.ReadUInt64();
            break;
          }
          case 26: {
            AssignedBrokerServiceUrl = input.ReadString();
            break;
          }
          case 34: {
            AssignedBrokerServiceUrlTls = input.ReadString();
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
                    ConsumerId = input.ReadUInt64();
                    break;
                }
                case 16: {
                    RequestId = input.ReadUInt64();
                    break;
                }
                case 26: {
                    AssignedBrokerServiceUrl = input.ReadString();
                    break;
                }
                case 34: {
                    AssignedBrokerServiceUrlTls = input.ReadString();
                    break;
                }
            }
        }
    }
#endif

}
