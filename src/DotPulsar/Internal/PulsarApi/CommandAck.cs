#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandAck : IMessage<CommandAck>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandAck> _parser = new pb::MessageParser<CommandAck>(() => new CommandAck());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandAck> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[26]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAck() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAck(CommandAck other) : this() {
        _hasBits0 = other._hasBits0;
        consumerId_ = other.consumerId_;
        ackType_ = other.ackType_;
        messageId_ = other.messageId_.Clone();
        validationError_ = other.validationError_;
        properties_ = other.properties_.Clone();
        txnidLeastBits_ = other.txnidLeastBits_;
        txnidMostBits_ = other.txnidMostBits_;
        requestId_ = other.requestId_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAck Clone() {
        return new CommandAck(this);
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

    /// <summary>Field number for the "ack_type" field.</summary>
    public const int AckTypeFieldNumber = 2;
    private readonly static global::DotPulsar.Internal.PulsarApi.CommandAck.Types.AckType AckTypeDefaultValue = global::DotPulsar.Internal.PulsarApi.CommandAck.Types.AckType.Individual;

    private global::DotPulsar.Internal.PulsarApi.CommandAck.Types.AckType ackType_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandAck.Types.AckType AckType {
        get { if ((_hasBits0 & 2) != 0) { return ackType_; } else { return AckTypeDefaultValue; } }
        set {
            _hasBits0 |= 2;
            ackType_ = value;
        }
    }
    /// <summary>Gets whether the "ack_type" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasAckType {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "ack_type" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearAckType() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "message_id" field.</summary>
    public const int MessageIdFieldNumber = 3;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.MessageIdData> _repeated_messageId_codec
        = pb::FieldCodec.ForMessage(26, global::DotPulsar.Internal.PulsarApi.MessageIdData.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.MessageIdData> messageId_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.MessageIdData>();
    /// <summary>
    /// In case of individual acks, the client can pass a list of message ids
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.MessageIdData> MessageId {
        get { return messageId_; }
    }

    /// <summary>Field number for the "validation_error" field.</summary>
    public const int ValidationErrorFieldNumber = 4;
    private readonly static global::DotPulsar.Internal.PulsarApi.CommandAck.Types.ValidationError ValidationErrorDefaultValue = global::DotPulsar.Internal.PulsarApi.CommandAck.Types.ValidationError.UncompressedSizeCorruption;

    private global::DotPulsar.Internal.PulsarApi.CommandAck.Types.ValidationError validationError_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandAck.Types.ValidationError ValidationError {
        get { if ((_hasBits0 & 4) != 0) { return validationError_; } else { return ValidationErrorDefaultValue; } }
        set {
            _hasBits0 |= 4;
            validationError_ = value;
        }
    }
    /// <summary>Gets whether the "validation_error" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasValidationError {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "validation_error" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearValidationError() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "properties" field.</summary>
    public const int PropertiesFieldNumber = 5;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.KeyLongValue> _repeated_properties_codec
        = pb::FieldCodec.ForMessage(42, global::DotPulsar.Internal.PulsarApi.KeyLongValue.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyLongValue> properties_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyLongValue>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyLongValue> Properties {
        get { return properties_; }
    }

    /// <summary>Field number for the "txnid_least_bits" field.</summary>
    public const int TxnidLeastBitsFieldNumber = 6;
    private readonly static ulong TxnidLeastBitsDefaultValue = 0UL;

    private ulong txnidLeastBits_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong TxnidLeastBits {
        get { if ((_hasBits0 & 8) != 0) { return txnidLeastBits_; } else { return TxnidLeastBitsDefaultValue; } }
        set {
            _hasBits0 |= 8;
            txnidLeastBits_ = value;
        }
    }
    /// <summary>Gets whether the "txnid_least_bits" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTxnidLeastBits {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "txnid_least_bits" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTxnidLeastBits() {
        _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "txnid_most_bits" field.</summary>
    public const int TxnidMostBitsFieldNumber = 7;
    private readonly static ulong TxnidMostBitsDefaultValue = 0UL;

    private ulong txnidMostBits_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong TxnidMostBits {
        get { if ((_hasBits0 & 16) != 0) { return txnidMostBits_; } else { return TxnidMostBitsDefaultValue; } }
        set {
            _hasBits0 |= 16;
            txnidMostBits_ = value;
        }
    }
    /// <summary>Gets whether the "txnid_most_bits" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTxnidMostBits {
        get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "txnid_most_bits" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTxnidMostBits() {
        _hasBits0 &= ~16;
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 8;
    private readonly static ulong RequestIdDefaultValue = 0UL;

    private ulong requestId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong RequestId {
        get { if ((_hasBits0 & 32) != 0) { return requestId_; } else { return RequestIdDefaultValue; } }
        set {
            _hasBits0 |= 32;
            requestId_ = value;
        }
    }
    /// <summary>Gets whether the "request_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasRequestId {
        get { return (_hasBits0 & 32) != 0; }
    }
    /// <summary>Clears the value of the "request_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearRequestId() {
        _hasBits0 &= ~32;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandAck);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandAck other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ConsumerId != other.ConsumerId) return false;
        if (AckType != other.AckType) return false;
        if(!messageId_.Equals(other.messageId_)) return false;
        if (ValidationError != other.ValidationError) return false;
        if(!properties_.Equals(other.properties_)) return false;
        if (TxnidLeastBits != other.TxnidLeastBits) return false;
        if (TxnidMostBits != other.TxnidMostBits) return false;
        if (RequestId != other.RequestId) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasConsumerId) hash ^= ConsumerId.GetHashCode();
        if (HasAckType) hash ^= AckType.GetHashCode();
        hash ^= messageId_.GetHashCode();
        if (HasValidationError) hash ^= ValidationError.GetHashCode();
        hash ^= properties_.GetHashCode();
        if (HasTxnidLeastBits) hash ^= TxnidLeastBits.GetHashCode();
        if (HasTxnidMostBits) hash ^= TxnidMostBits.GetHashCode();
        if (HasRequestId) hash ^= RequestId.GetHashCode();
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
      if (HasAckType) {
        output.WriteRawTag(16);
        output.WriteEnum((int) AckType);
      }
      messageId_.WriteTo(output, _repeated_messageId_codec);
      if (HasValidationError) {
        output.WriteRawTag(32);
        output.WriteEnum((int) ValidationError);
      }
      properties_.WriteTo(output, _repeated_properties_codec);
      if (HasTxnidLeastBits) {
        output.WriteRawTag(48);
        output.WriteUInt64(TxnidLeastBits);
      }
      if (HasTxnidMostBits) {
        output.WriteRawTag(56);
        output.WriteUInt64(TxnidMostBits);
      }
      if (HasRequestId) {
        output.WriteRawTag(64);
        output.WriteUInt64(RequestId);
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
        if (HasAckType) {
            output.WriteRawTag(16);
            output.WriteEnum((int) AckType);
        }
        messageId_.WriteTo(ref output, _repeated_messageId_codec);
        if (HasValidationError) {
            output.WriteRawTag(32);
            output.WriteEnum((int) ValidationError);
        }
        properties_.WriteTo(ref output, _repeated_properties_codec);
        if (HasTxnidLeastBits) {
            output.WriteRawTag(48);
            output.WriteUInt64(TxnidLeastBits);
        }
        if (HasTxnidMostBits) {
            output.WriteRawTag(56);
            output.WriteUInt64(TxnidMostBits);
        }
        if (HasRequestId) {
            output.WriteRawTag(64);
            output.WriteUInt64(RequestId);
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
        if (HasAckType) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) AckType);
        }
        size += messageId_.CalculateSize(_repeated_messageId_codec);
        if (HasValidationError) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ValidationError);
        }
        size += properties_.CalculateSize(_repeated_properties_codec);
        if (HasTxnidLeastBits) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(TxnidLeastBits);
        }
        if (HasTxnidMostBits) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(TxnidMostBits);
        }
        if (HasRequestId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(RequestId);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandAck other) {
        if (other == null) {
            return;
        }
        if (other.HasConsumerId) {
            ConsumerId = other.ConsumerId;
        }
        if (other.HasAckType) {
            AckType = other.AckType;
        }
        messageId_.Add(other.messageId_);
        if (other.HasValidationError) {
            ValidationError = other.ValidationError;
        }
        properties_.Add(other.properties_);
        if (other.HasTxnidLeastBits) {
            TxnidLeastBits = other.TxnidLeastBits;
        }
        if (other.HasTxnidMostBits) {
            TxnidMostBits = other.TxnidMostBits;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
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
            AckType = (global::DotPulsar.Internal.PulsarApi.CommandAck.Types.AckType) input.ReadEnum();
            break;
          }
          case 26: {
            messageId_.AddEntriesFrom(input, _repeated_messageId_codec);
            break;
          }
          case 32: {
            ValidationError = (global::DotPulsar.Internal.PulsarApi.CommandAck.Types.ValidationError) input.ReadEnum();
            break;
          }
          case 42: {
            properties_.AddEntriesFrom(input, _repeated_properties_codec);
            break;
          }
          case 48: {
            TxnidLeastBits = input.ReadUInt64();
            break;
          }
          case 56: {
            TxnidMostBits = input.ReadUInt64();
            break;
          }
          case 64: {
            RequestId = input.ReadUInt64();
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
                    AckType = (global::DotPulsar.Internal.PulsarApi.CommandAck.Types.AckType) input.ReadEnum();
                    break;
                }
                case 26: {
                    messageId_.AddEntriesFrom(ref input, _repeated_messageId_codec);
                    break;
                }
                case 32: {
                    ValidationError = (global::DotPulsar.Internal.PulsarApi.CommandAck.Types.ValidationError) input.ReadEnum();
                    break;
                }
                case 42: {
                    properties_.AddEntriesFrom(ref input, _repeated_properties_codec);
                    break;
                }
                case 48: {
                    TxnidLeastBits = input.ReadUInt64();
                    break;
                }
                case 56: {
                    TxnidMostBits = input.ReadUInt64();
                    break;
                }
                case 64: {
                    RequestId = input.ReadUInt64();
                    break;
                }
            }
        }
    }
#endif

    #region Nested types
    /// <summary>Container for nested types declared in the CommandAck message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Types {
        public enum AckType {
            [pbr::OriginalName("Individual")] Individual = 0,
            [pbr::OriginalName("Cumulative")] Cumulative = 1,
        }

        /// <summary>
        /// Acks can contain a flag to indicate the consumer
        /// received an invalid message that got discarded
        /// before being passed on to the application.
        /// </summary>
        public enum ValidationError {
            [pbr::OriginalName("UncompressedSizeCorruption")] UncompressedSizeCorruption = 0,
            [pbr::OriginalName("DecompressionError")] DecompressionError = 1,
            [pbr::OriginalName("ChecksumMismatch")] ChecksumMismatch = 2,
            [pbr::OriginalName("BatchDeSerializeError")] BatchDeSerializeError = 3,
            [pbr::OriginalName("DecryptionError")] DecryptionError = 4,
        }

    }
    #endregion

}
