#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandRedeliverUnacknowledgedMessages : IMessage<CommandRedeliverUnacknowledgedMessages>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandRedeliverUnacknowledgedMessages> _parser = new pb::MessageParser<CommandRedeliverUnacknowledgedMessages>(() => new CommandRedeliverUnacknowledgedMessages());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandRedeliverUnacknowledgedMessages> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[36]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandRedeliverUnacknowledgedMessages() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandRedeliverUnacknowledgedMessages(CommandRedeliverUnacknowledgedMessages other) : this() {
        _hasBits0 = other._hasBits0;
        consumerId_ = other.consumerId_;
        messageIds_ = other.messageIds_.Clone();
        consumerEpoch_ = other.consumerEpoch_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandRedeliverUnacknowledgedMessages Clone() {
        return new CommandRedeliverUnacknowledgedMessages(this);
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

    /// <summary>Field number for the "message_ids" field.</summary>
    public const int MessageIdsFieldNumber = 2;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.MessageIdData> _repeated_messageIds_codec
        = pb::FieldCodec.ForMessage(18, global::DotPulsar.Internal.PulsarApi.MessageIdData.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.MessageIdData> messageIds_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.MessageIdData>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.MessageIdData> MessageIds {
        get { return messageIds_; }
    }

    /// <summary>Field number for the "consumer_epoch" field.</summary>
    public const int ConsumerEpochFieldNumber = 3;
    private readonly static ulong ConsumerEpochDefaultValue = 0UL;

    private ulong consumerEpoch_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong ConsumerEpoch {
        get { if ((_hasBits0 & 2) != 0) { return consumerEpoch_; } else { return ConsumerEpochDefaultValue; } }
        set {
            _hasBits0 |= 2;
            consumerEpoch_ = value;
        }
    }
    /// <summary>Gets whether the "consumer_epoch" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasConsumerEpoch {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "consumer_epoch" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearConsumerEpoch() {
        _hasBits0 &= ~2;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandRedeliverUnacknowledgedMessages);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandRedeliverUnacknowledgedMessages other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ConsumerId != other.ConsumerId) return false;
        if(!messageIds_.Equals(other.messageIds_)) return false;
        if (ConsumerEpoch != other.ConsumerEpoch) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasConsumerId) hash ^= ConsumerId.GetHashCode();
        hash ^= messageIds_.GetHashCode();
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
      messageIds_.WriteTo(output, _repeated_messageIds_codec);
      if (HasConsumerEpoch) {
        output.WriteRawTag(24);
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
        messageIds_.WriteTo(ref output, _repeated_messageIds_codec);
        if (HasConsumerEpoch) {
            output.WriteRawTag(24);
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
        size += messageIds_.CalculateSize(_repeated_messageIds_codec);
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
    public void MergeFrom(CommandRedeliverUnacknowledgedMessages other) {
        if (other == null) {
            return;
        }
        if (other.HasConsumerId) {
            ConsumerId = other.ConsumerId;
        }
        messageIds_.Add(other.messageIds_);
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
            messageIds_.AddEntriesFrom(input, _repeated_messageIds_codec);
            break;
          }
          case 24: {
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
                    messageIds_.AddEntriesFrom(ref input, _repeated_messageIds_codec);
                    break;
                }
                case 24: {
                    ConsumerEpoch = input.ReadUInt64();
                    break;
                }
            }
        }
    }
#endif

}
