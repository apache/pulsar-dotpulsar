#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandGetLastMessageIdResponse : IMessage<CommandGetLastMessageIdResponse>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandGetLastMessageIdResponse> _parser = new pb::MessageParser<CommandGetLastMessageIdResponse>(() => new CommandGetLastMessageIdResponse());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandGetLastMessageIdResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[45]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandGetLastMessageIdResponse() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandGetLastMessageIdResponse(CommandGetLastMessageIdResponse other) : this() {
        _hasBits0 = other._hasBits0;
        lastMessageId_ = other.lastMessageId_ != null ? other.lastMessageId_.Clone() : null;
        requestId_ = other.requestId_;
        consumerMarkDeletePosition_ = other.consumerMarkDeletePosition_ != null ? other.consumerMarkDeletePosition_.Clone() : null;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandGetLastMessageIdResponse Clone() {
        return new CommandGetLastMessageIdResponse(this);
    }

    /// <summary>Field number for the "last_message_id" field.</summary>
    public const int LastMessageIdFieldNumber = 1;
    private global::DotPulsar.Internal.PulsarApi.MessageIdData lastMessageId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.MessageIdData LastMessageId {
        get { return lastMessageId_; }
        set {
            lastMessageId_ = value;
        }
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 2;
    private readonly static ulong RequestIdDefaultValue = 0UL;

    private ulong requestId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong RequestId {
        get { if ((_hasBits0 & 1) != 0) { return requestId_; } else { return RequestIdDefaultValue; } }
        set {
            _hasBits0 |= 1;
            requestId_ = value;
        }
    }
    /// <summary>Gets whether the "request_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasRequestId {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "request_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearRequestId() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "consumer_mark_delete_position" field.</summary>
    public const int ConsumerMarkDeletePositionFieldNumber = 3;
    private global::DotPulsar.Internal.PulsarApi.MessageIdData consumerMarkDeletePosition_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.MessageIdData ConsumerMarkDeletePosition {
        get { return consumerMarkDeletePosition_; }
        set {
            consumerMarkDeletePosition_ = value;
        }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandGetLastMessageIdResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandGetLastMessageIdResponse other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (!object.Equals(LastMessageId, other.LastMessageId)) return false;
        if (RequestId != other.RequestId) return false;
        if (!object.Equals(ConsumerMarkDeletePosition, other.ConsumerMarkDeletePosition)) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (lastMessageId_ != null) hash ^= LastMessageId.GetHashCode();
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (consumerMarkDeletePosition_ != null) hash ^= ConsumerMarkDeletePosition.GetHashCode();
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
      if (lastMessageId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(LastMessageId);
      }
      if (HasRequestId) {
        output.WriteRawTag(16);
        output.WriteUInt64(RequestId);
      }
      if (consumerMarkDeletePosition_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(ConsumerMarkDeletePosition);
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
        if (lastMessageId_ != null) {
            output.WriteRawTag(10);
            output.WriteMessage(LastMessageId);
        }
        if (HasRequestId) {
            output.WriteRawTag(16);
            output.WriteUInt64(RequestId);
        }
        if (consumerMarkDeletePosition_ != null) {
            output.WriteRawTag(26);
            output.WriteMessage(ConsumerMarkDeletePosition);
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
        if (lastMessageId_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(LastMessageId);
        }
        if (HasRequestId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(RequestId);
        }
        if (consumerMarkDeletePosition_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(ConsumerMarkDeletePosition);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandGetLastMessageIdResponse other) {
        if (other == null) {
            return;
        }
        if (other.lastMessageId_ != null) {
            if (lastMessageId_ == null) {
                LastMessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            LastMessageId.MergeFrom(other.LastMessageId);
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.consumerMarkDeletePosition_ != null) {
            if (consumerMarkDeletePosition_ == null) {
                ConsumerMarkDeletePosition = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            ConsumerMarkDeletePosition.MergeFrom(other.ConsumerMarkDeletePosition);
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
          case 10: {
            if (lastMessageId_ == null) {
              LastMessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            input.ReadMessage(LastMessageId);
            break;
          }
          case 16: {
            RequestId = input.ReadUInt64();
            break;
          }
          case 26: {
            if (consumerMarkDeletePosition_ == null) {
              ConsumerMarkDeletePosition = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            input.ReadMessage(ConsumerMarkDeletePosition);
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
                    if (lastMessageId_ == null) {
                        LastMessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
                    }
                    input.ReadMessage(LastMessageId);
                    break;
                }
                case 16: {
                    RequestId = input.ReadUInt64();
                    break;
                }
                case 26: {
                    if (consumerMarkDeletePosition_ == null) {
                        ConsumerMarkDeletePosition = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
                    }
                    input.ReadMessage(ConsumerMarkDeletePosition);
                    break;
                }
            }
        }
    }
#endif

}
