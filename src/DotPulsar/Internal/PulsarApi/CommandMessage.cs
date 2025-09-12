#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandMessage : IMessage<CommandMessage>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandMessage> _parser = new pb::MessageParser<CommandMessage>(() => new CommandMessage());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandMessage> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[25]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandMessage() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandMessage(CommandMessage other) : this() {
        _hasBits0 = other._hasBits0;
        consumerId_ = other.consumerId_;
        messageId_ = other.messageId_ != null ? other.messageId_.Clone() : null;
        redeliveryCount_ = other.redeliveryCount_;
        ackSet_ = other.ackSet_.Clone();
        consumerEpoch_ = other.consumerEpoch_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandMessage Clone() {
        return new CommandMessage(this);
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

    /// <summary>Field number for the "message_id" field.</summary>
    public const int MessageIdFieldNumber = 2;
    private global::DotPulsar.Internal.PulsarApi.MessageIdData messageId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.MessageIdData MessageId {
        get { return messageId_; }
        set {
            messageId_ = value;
        }
    }

    /// <summary>Field number for the "redelivery_count" field.</summary>
    public const int RedeliveryCountFieldNumber = 3;
    private readonly static uint RedeliveryCountDefaultValue = 0;

    private uint redeliveryCount_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint RedeliveryCount {
        get { if ((_hasBits0 & 2) != 0) { return redeliveryCount_; } else { return RedeliveryCountDefaultValue; } }
        set {
            _hasBits0 |= 2;
            redeliveryCount_ = value;
        }
    }
    /// <summary>Gets whether the "redelivery_count" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasRedeliveryCount {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "redelivery_count" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearRedeliveryCount() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "ack_set" field.</summary>
    public const int AckSetFieldNumber = 4;
    private static readonly pb::FieldCodec<long> _repeated_ackSet_codec
        = pb::FieldCodec.ForInt64(32);
    private readonly pbc::RepeatedField<long> ackSet_ = new pbc::RepeatedField<long>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<long> AckSet {
        get { return ackSet_; }
    }

    /// <summary>Field number for the "consumer_epoch" field.</summary>
    public const int ConsumerEpochFieldNumber = 5;
    private readonly static ulong ConsumerEpochDefaultValue = 0UL;

    private ulong consumerEpoch_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong ConsumerEpoch {
        get { if ((_hasBits0 & 4) != 0) { return consumerEpoch_; } else { return ConsumerEpochDefaultValue; } }
        set {
            _hasBits0 |= 4;
            consumerEpoch_ = value;
        }
    }
    /// <summary>Gets whether the "consumer_epoch" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasConsumerEpoch {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "consumer_epoch" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearConsumerEpoch() {
        _hasBits0 &= ~4;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandMessage);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandMessage other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ConsumerId != other.ConsumerId) return false;
        if (!object.Equals(MessageId, other.MessageId)) return false;
        if (RedeliveryCount != other.RedeliveryCount) return false;
        if(!ackSet_.Equals(other.ackSet_)) return false;
        if (ConsumerEpoch != other.ConsumerEpoch) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasConsumerId) hash ^= ConsumerId.GetHashCode();
        if (messageId_ != null) hash ^= MessageId.GetHashCode();
        if (HasRedeliveryCount) hash ^= RedeliveryCount.GetHashCode();
        hash ^= ackSet_.GetHashCode();
        if (HasConsumerEpoch) hash ^= ConsumerEpoch.GetHashCode();
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
      if (messageId_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(MessageId);
      }
      if (HasRedeliveryCount) {
        output.WriteRawTag(24);
        output.WriteUInt32(RedeliveryCount);
      }
      ackSet_.WriteTo(output, _repeated_ackSet_codec);
      if (HasConsumerEpoch) {
        output.WriteRawTag(40);
        output.WriteUInt64(ConsumerEpoch);
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
        if (messageId_ != null) {
            output.WriteRawTag(18);
            output.WriteMessage(MessageId);
        }
        if (HasRedeliveryCount) {
            output.WriteRawTag(24);
            output.WriteUInt32(RedeliveryCount);
        }
        ackSet_.WriteTo(ref output, _repeated_ackSet_codec);
        if (HasConsumerEpoch) {
            output.WriteRawTag(40);
            output.WriteUInt64(ConsumerEpoch);
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
        if (messageId_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(MessageId);
        }
        if (HasRedeliveryCount) {
            size += 1 + pb::CodedOutputStream.ComputeUInt32Size(RedeliveryCount);
        }
        size += ackSet_.CalculateSize(_repeated_ackSet_codec);
        if (HasConsumerEpoch) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ConsumerEpoch);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandMessage other) {
        if (other == null) {
            return;
        }
        if (other.HasConsumerId) {
            ConsumerId = other.ConsumerId;
        }
        if (other.messageId_ != null) {
            if (messageId_ == null) {
                MessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            MessageId.MergeFrom(other.MessageId);
        }
        if (other.HasRedeliveryCount) {
            RedeliveryCount = other.RedeliveryCount;
        }
        ackSet_.Add(other.ackSet_);
        if (other.HasConsumerEpoch) {
            ConsumerEpoch = other.ConsumerEpoch;
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
          case 18: {
            if (messageId_ == null) {
              MessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            input.ReadMessage(MessageId);
            break;
          }
          case 24: {
            RedeliveryCount = input.ReadUInt32();
            break;
          }
          case 34:
          case 32: {
            ackSet_.AddEntriesFrom(input, _repeated_ackSet_codec);
            break;
          }
          case 40: {
            ConsumerEpoch = input.ReadUInt64();
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
                case 18: {
                    if (messageId_ == null) {
                        MessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
                    }
                    input.ReadMessage(MessageId);
                    break;
                }
                case 24: {
                    RedeliveryCount = input.ReadUInt32();
                    break;
                }
                case 34:
                case 32: {
                    ackSet_.AddEntriesFrom(ref input, _repeated_ackSet_codec);
                    break;
                }
                case 40: {
                    ConsumerEpoch = input.ReadUInt64();
                    break;
                }
            }
        }
    }
#endif

}
