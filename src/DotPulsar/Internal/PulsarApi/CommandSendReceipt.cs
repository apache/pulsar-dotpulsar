#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandSendReceipt : IMessage<CommandSendReceipt>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandSendReceipt> _parser = new pb::MessageParser<CommandSendReceipt>(() => new CommandSendReceipt());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandSendReceipt> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[23]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandSendReceipt() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandSendReceipt(CommandSendReceipt other) : this() {
        _hasBits0 = other._hasBits0;
        producerId_ = other.producerId_;
        sequenceId_ = other.sequenceId_;
        messageId_ = other.messageId_ != null ? other.messageId_.Clone() : null;
        highestSequenceId_ = other.highestSequenceId_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandSendReceipt Clone() {
        return new CommandSendReceipt(this);
    }

    /// <summary>Field number for the "producer_id" field.</summary>
    public const int ProducerIdFieldNumber = 1;
    private readonly static ulong ProducerIdDefaultValue = 0UL;

    private ulong producerId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong ProducerId {
        get { if ((_hasBits0 & 1) != 0) { return producerId_; } else { return ProducerIdDefaultValue; } }
        set {
            _hasBits0 |= 1;
            producerId_ = value;
        }
    }
    /// <summary>Gets whether the "producer_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProducerId {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "producer_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProducerId() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "sequence_id" field.</summary>
    public const int SequenceIdFieldNumber = 2;
    private readonly static ulong SequenceIdDefaultValue = 0UL;

    private ulong sequenceId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong SequenceId {
        get { if ((_hasBits0 & 2) != 0) { return sequenceId_; } else { return SequenceIdDefaultValue; } }
        set {
            _hasBits0 |= 2;
            sequenceId_ = value;
        }
    }
    /// <summary>Gets whether the "sequence_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSequenceId {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "sequence_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSequenceId() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "message_id" field.</summary>
    public const int MessageIdFieldNumber = 3;
    private global::DotPulsar.Internal.PulsarApi.MessageIdData messageId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.MessageIdData MessageId {
        get { return messageId_; }
        set {
            messageId_ = value;
        }
    }

    /// <summary>Field number for the "highest_sequence_id" field.</summary>
    public const int HighestSequenceIdFieldNumber = 4;
    private readonly static ulong HighestSequenceIdDefaultValue = 0UL;

    private ulong highestSequenceId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong HighestSequenceId {
        get { if ((_hasBits0 & 4) != 0) { return highestSequenceId_; } else { return HighestSequenceIdDefaultValue; } }
        set {
            _hasBits0 |= 4;
            highestSequenceId_ = value;
        }
    }
    /// <summary>Gets whether the "highest_sequence_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasHighestSequenceId {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "highest_sequence_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearHighestSequenceId() {
        _hasBits0 &= ~4;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandSendReceipt);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandSendReceipt other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ProducerId != other.ProducerId) return false;
        if (SequenceId != other.SequenceId) return false;
        if (!object.Equals(MessageId, other.MessageId)) return false;
        if (HighestSequenceId != other.HighestSequenceId) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasProducerId) hash ^= ProducerId.GetHashCode();
        if (HasSequenceId) hash ^= SequenceId.GetHashCode();
        if (messageId_ != null) hash ^= MessageId.GetHashCode();
        if (HasHighestSequenceId) hash ^= HighestSequenceId.GetHashCode();
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
      if (HasProducerId) {
        output.WriteRawTag(8);
        output.WriteUInt64(ProducerId);
      }
      if (HasSequenceId) {
        output.WriteRawTag(16);
        output.WriteUInt64(SequenceId);
      }
      if (messageId_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(MessageId);
      }
      if (HasHighestSequenceId) {
        output.WriteRawTag(32);
        output.WriteUInt64(HighestSequenceId);
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
        if (HasProducerId) {
            output.WriteRawTag(8);
            output.WriteUInt64(ProducerId);
        }
        if (HasSequenceId) {
            output.WriteRawTag(16);
            output.WriteUInt64(SequenceId);
        }
        if (messageId_ != null) {
            output.WriteRawTag(26);
            output.WriteMessage(MessageId);
        }
        if (HasHighestSequenceId) {
            output.WriteRawTag(32);
            output.WriteUInt64(HighestSequenceId);
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
        if (HasProducerId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ProducerId);
        }
        if (HasSequenceId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(SequenceId);
        }
        if (messageId_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(MessageId);
        }
        if (HasHighestSequenceId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(HighestSequenceId);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandSendReceipt other) {
        if (other == null) {
            return;
        }
        if (other.HasProducerId) {
            ProducerId = other.ProducerId;
        }
        if (other.HasSequenceId) {
            SequenceId = other.SequenceId;
        }
        if (other.messageId_ != null) {
            if (messageId_ == null) {
                MessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            MessageId.MergeFrom(other.MessageId);
        }
        if (other.HasHighestSequenceId) {
            HighestSequenceId = other.HighestSequenceId;
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
            ProducerId = input.ReadUInt64();
            break;
          }
          case 16: {
            SequenceId = input.ReadUInt64();
            break;
          }
          case 26: {
            if (messageId_ == null) {
              MessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            input.ReadMessage(MessageId);
            break;
          }
          case 32: {
            HighestSequenceId = input.ReadUInt64();
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
                    ProducerId = input.ReadUInt64();
                    break;
                }
                case 16: {
                    SequenceId = input.ReadUInt64();
                    break;
                }
                case 26: {
                    if (messageId_ == null) {
                        MessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
                    }
                    input.ReadMessage(MessageId);
                    break;
                }
                case 32: {
                    HighestSequenceId = input.ReadUInt64();
                    break;
                }
            }
        }
    }
#endif

}
