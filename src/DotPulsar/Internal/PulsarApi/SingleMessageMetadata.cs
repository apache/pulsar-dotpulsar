#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class SingleMessageMetadata : IMessage<SingleMessageMetadata>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<SingleMessageMetadata> _parser = new pb::MessageParser<SingleMessageMetadata>(() => new SingleMessageMetadata());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<SingleMessageMetadata> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[7]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SingleMessageMetadata() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SingleMessageMetadata(SingleMessageMetadata other) : this() {
        _hasBits0 = other._hasBits0;
        properties_ = other.properties_.Clone();
        partitionKey_ = other.partitionKey_;
        payloadSize_ = other.payloadSize_;
        compactedOut_ = other.compactedOut_;
        eventTime_ = other.eventTime_;
        partitionKeyB64Encoded_ = other.partitionKeyB64Encoded_;
        orderingKey_ = other.orderingKey_;
        sequenceId_ = other.sequenceId_;
        nullValue_ = other.nullValue_;
        nullPartitionKey_ = other.nullPartitionKey_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SingleMessageMetadata Clone() {
        return new SingleMessageMetadata(this);
    }

    /// <summary>Field number for the "properties" field.</summary>
    public const int PropertiesFieldNumber = 1;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.KeyValue> _repeated_properties_codec
        = pb::FieldCodec.ForMessage(10, global::DotPulsar.Internal.PulsarApi.KeyValue.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> properties_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> Properties {
        get { return properties_; }
    }

    /// <summary>Field number for the "partition_key" field.</summary>
    public const int PartitionKeyFieldNumber = 2;
    private readonly static string PartitionKeyDefaultValue = "";

    private string partitionKey_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string PartitionKey {
        get { return partitionKey_ ?? PartitionKeyDefaultValue; }
        set {
            partitionKey_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "partition_key" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasPartitionKey {
        get { return partitionKey_ != null; }
    }
    /// <summary>Clears the value of the "partition_key" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearPartitionKey() {
        partitionKey_ = null;
    }

    /// <summary>Field number for the "payload_size" field.</summary>
    public const int PayloadSizeFieldNumber = 3;
    private readonly static int PayloadSizeDefaultValue = 0;

    private int payloadSize_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int PayloadSize {
        get { if ((_hasBits0 & 1) != 0) { return payloadSize_; } else { return PayloadSizeDefaultValue; } }
        set {
            _hasBits0 |= 1;
            payloadSize_ = value;
        }
    }
    /// <summary>Gets whether the "payload_size" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasPayloadSize {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "payload_size" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearPayloadSize() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "compacted_out" field.</summary>
    public const int CompactedOutFieldNumber = 4;
    private readonly static bool CompactedOutDefaultValue = false;

    private bool compactedOut_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool CompactedOut {
        get { if ((_hasBits0 & 2) != 0) { return compactedOut_; } else { return CompactedOutDefaultValue; } }
        set {
            _hasBits0 |= 2;
            compactedOut_ = value;
        }
    }
    /// <summary>Gets whether the "compacted_out" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasCompactedOut {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "compacted_out" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearCompactedOut() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "event_time" field.</summary>
    public const int EventTimeFieldNumber = 5;
    private readonly static ulong EventTimeDefaultValue = 0UL;

    private ulong eventTime_;
    /// <summary>
    /// the timestamp that this event occurs. it is typically set by applications.
    /// if this field is omitted, `publish_time` can be used for the purpose of `event_time`.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong EventTime {
        get { if ((_hasBits0 & 4) != 0) { return eventTime_; } else { return EventTimeDefaultValue; } }
        set {
            _hasBits0 |= 4;
            eventTime_ = value;
        }
    }
    /// <summary>Gets whether the "event_time" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasEventTime {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "event_time" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearEventTime() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "partition_key_b64_encoded" field.</summary>
    public const int PartitionKeyB64EncodedFieldNumber = 6;
    private readonly static bool PartitionKeyB64EncodedDefaultValue = false;

    private bool partitionKeyB64Encoded_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool PartitionKeyB64Encoded {
        get { if ((_hasBits0 & 8) != 0) { return partitionKeyB64Encoded_; } else { return PartitionKeyB64EncodedDefaultValue; } }
        set {
            _hasBits0 |= 8;
            partitionKeyB64Encoded_ = value;
        }
    }
    /// <summary>Gets whether the "partition_key_b64_encoded" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasPartitionKeyB64Encoded {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "partition_key_b64_encoded" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearPartitionKeyB64Encoded() {
        _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "ordering_key" field.</summary>
    public const int OrderingKeyFieldNumber = 7;
    private readonly static pb::ByteString OrderingKeyDefaultValue = pb::ByteString.Empty;

    private pb::ByteString orderingKey_;
    /// <summary>
    /// Specific a key to overwrite the message key which used for ordering dispatch in Key_Shared mode.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString OrderingKey {
        get { return orderingKey_ ?? OrderingKeyDefaultValue; }
        set {
            orderingKey_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "ordering_key" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasOrderingKey {
        get { return orderingKey_ != null; }
    }
    /// <summary>Clears the value of the "ordering_key" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearOrderingKey() {
        orderingKey_ = null;
    }

    /// <summary>Field number for the "sequence_id" field.</summary>
    public const int SequenceIdFieldNumber = 8;
    private readonly static ulong SequenceIdDefaultValue = 0UL;

    private ulong sequenceId_;
    /// <summary>
    /// Allows consumer retrieve the sequence id that the producer set.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong SequenceId {
        get { if ((_hasBits0 & 16) != 0) { return sequenceId_; } else { return SequenceIdDefaultValue; } }
        set {
            _hasBits0 |= 16;
            sequenceId_ = value;
        }
    }
    /// <summary>Gets whether the "sequence_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSequenceId {
        get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "sequence_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSequenceId() {
        _hasBits0 &= ~16;
    }

    /// <summary>Field number for the "null_value" field.</summary>
    public const int NullValueFieldNumber = 9;
    private readonly static bool NullValueDefaultValue = false;

    private bool nullValue_;
    /// <summary>
    /// Indicate if the message payload value is set
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool NullValue {
        get { if ((_hasBits0 & 32) != 0) { return nullValue_; } else { return NullValueDefaultValue; } }
        set {
            _hasBits0 |= 32;
            nullValue_ = value;
        }
    }
    /// <summary>Gets whether the "null_value" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNullValue {
        get { return (_hasBits0 & 32) != 0; }
    }
    /// <summary>Clears the value of the "null_value" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNullValue() {
        _hasBits0 &= ~32;
    }

    /// <summary>Field number for the "null_partition_key" field.</summary>
    public const int NullPartitionKeyFieldNumber = 10;
    private readonly static bool NullPartitionKeyDefaultValue = false;

    private bool nullPartitionKey_;
    /// <summary>
    /// Indicate if the message partition key is set
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool NullPartitionKey {
        get { if ((_hasBits0 & 64) != 0) { return nullPartitionKey_; } else { return NullPartitionKeyDefaultValue; } }
        set {
            _hasBits0 |= 64;
            nullPartitionKey_ = value;
        }
    }
    /// <summary>Gets whether the "null_partition_key" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNullPartitionKey {
        get { return (_hasBits0 & 64) != 0; }
    }
    /// <summary>Clears the value of the "null_partition_key" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNullPartitionKey() {
        _hasBits0 &= ~64;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as SingleMessageMetadata);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(SingleMessageMetadata other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if(!properties_.Equals(other.properties_)) return false;
        if (PartitionKey != other.PartitionKey) return false;
        if (PayloadSize != other.PayloadSize) return false;
        if (CompactedOut != other.CompactedOut) return false;
        if (EventTime != other.EventTime) return false;
        if (PartitionKeyB64Encoded != other.PartitionKeyB64Encoded) return false;
        if (OrderingKey != other.OrderingKey) return false;
        if (SequenceId != other.SequenceId) return false;
        if (NullValue != other.NullValue) return false;
        if (NullPartitionKey != other.NullPartitionKey) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        hash ^= properties_.GetHashCode();
        if (HasPartitionKey) hash ^= PartitionKey.GetHashCode();
        if (HasPayloadSize) hash ^= PayloadSize.GetHashCode();
        if (HasCompactedOut) hash ^= CompactedOut.GetHashCode();
        if (HasEventTime) hash ^= EventTime.GetHashCode();
        if (HasPartitionKeyB64Encoded) hash ^= PartitionKeyB64Encoded.GetHashCode();
        if (HasOrderingKey) hash ^= OrderingKey.GetHashCode();
        if (HasSequenceId) hash ^= SequenceId.GetHashCode();
        if (HasNullValue) hash ^= NullValue.GetHashCode();
        if (HasNullPartitionKey) hash ^= NullPartitionKey.GetHashCode();
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
      properties_.WriteTo(output, _repeated_properties_codec);
      if (HasPartitionKey) {
        output.WriteRawTag(18);
        output.WriteString(PartitionKey);
      }
      if (HasPayloadSize) {
        output.WriteRawTag(24);
        output.WriteInt32(PayloadSize);
      }
      if (HasCompactedOut) {
        output.WriteRawTag(32);
        output.WriteBool(CompactedOut);
      }
      if (HasEventTime) {
        output.WriteRawTag(40);
        output.WriteUInt64(EventTime);
      }
      if (HasPartitionKeyB64Encoded) {
        output.WriteRawTag(48);
        output.WriteBool(PartitionKeyB64Encoded);
      }
      if (HasOrderingKey) {
        output.WriteRawTag(58);
        output.WriteBytes(OrderingKey);
      }
      if (HasSequenceId) {
        output.WriteRawTag(64);
        output.WriteUInt64(SequenceId);
      }
      if (HasNullValue) {
        output.WriteRawTag(72);
        output.WriteBool(NullValue);
      }
      if (HasNullPartitionKey) {
        output.WriteRawTag(80);
        output.WriteBool(NullPartitionKey);
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
        properties_.WriteTo(ref output, _repeated_properties_codec);
        if (HasPartitionKey) {
            output.WriteRawTag(18);
            output.WriteString(PartitionKey);
        }
        if (HasPayloadSize) {
            output.WriteRawTag(24);
            output.WriteInt32(PayloadSize);
        }
        if (HasCompactedOut) {
            output.WriteRawTag(32);
            output.WriteBool(CompactedOut);
        }
        if (HasEventTime) {
            output.WriteRawTag(40);
            output.WriteUInt64(EventTime);
        }
        if (HasPartitionKeyB64Encoded) {
            output.WriteRawTag(48);
            output.WriteBool(PartitionKeyB64Encoded);
        }
        if (HasOrderingKey) {
            output.WriteRawTag(58);
            output.WriteBytes(OrderingKey);
        }
        if (HasSequenceId) {
            output.WriteRawTag(64);
            output.WriteUInt64(SequenceId);
        }
        if (HasNullValue) {
            output.WriteRawTag(72);
            output.WriteBool(NullValue);
        }
        if (HasNullPartitionKey) {
            output.WriteRawTag(80);
            output.WriteBool(NullPartitionKey);
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
        size += properties_.CalculateSize(_repeated_properties_codec);
        if (HasPartitionKey) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(PartitionKey);
        }
        if (HasPayloadSize) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(PayloadSize);
        }
        if (HasCompactedOut) {
            size += 1 + 1;
        }
        if (HasEventTime) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(EventTime);
        }
        if (HasPartitionKeyB64Encoded) {
            size += 1 + 1;
        }
        if (HasOrderingKey) {
            size += 1 + pb::CodedOutputStream.ComputeBytesSize(OrderingKey);
        }
        if (HasSequenceId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(SequenceId);
        }
        if (HasNullValue) {
            size += 1 + 1;
        }
        if (HasNullPartitionKey) {
            size += 1 + 1;
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(SingleMessageMetadata other) {
        if (other == null) {
            return;
        }
        properties_.Add(other.properties_);
        if (other.HasPartitionKey) {
            PartitionKey = other.PartitionKey;
        }
        if (other.HasPayloadSize) {
            PayloadSize = other.PayloadSize;
        }
        if (other.HasCompactedOut) {
            CompactedOut = other.CompactedOut;
        }
        if (other.HasEventTime) {
            EventTime = other.EventTime;
        }
        if (other.HasPartitionKeyB64Encoded) {
            PartitionKeyB64Encoded = other.PartitionKeyB64Encoded;
        }
        if (other.HasOrderingKey) {
            OrderingKey = other.OrderingKey;
        }
        if (other.HasSequenceId) {
            SequenceId = other.SequenceId;
        }
        if (other.HasNullValue) {
            NullValue = other.NullValue;
        }
        if (other.HasNullPartitionKey) {
            NullPartitionKey = other.NullPartitionKey;
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
            properties_.AddEntriesFrom(input, _repeated_properties_codec);
            break;
          }
          case 18: {
            PartitionKey = input.ReadString();
            break;
          }
          case 24: {
            PayloadSize = input.ReadInt32();
            break;
          }
          case 32: {
            CompactedOut = input.ReadBool();
            break;
          }
          case 40: {
            EventTime = input.ReadUInt64();
            break;
          }
          case 48: {
            PartitionKeyB64Encoded = input.ReadBool();
            break;
          }
          case 58: {
            OrderingKey = input.ReadBytes();
            break;
          }
          case 64: {
            SequenceId = input.ReadUInt64();
            break;
          }
          case 72: {
            NullValue = input.ReadBool();
            break;
          }
          case 80: {
            NullPartitionKey = input.ReadBool();
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
                    properties_.AddEntriesFrom(ref input, _repeated_properties_codec);
                    break;
                }
                case 18: {
                    PartitionKey = input.ReadString();
                    break;
                }
                case 24: {
                    PayloadSize = input.ReadInt32();
                    break;
                }
                case 32: {
                    CompactedOut = input.ReadBool();
                    break;
                }
                case 40: {
                    EventTime = input.ReadUInt64();
                    break;
                }
                case 48: {
                    PartitionKeyB64Encoded = input.ReadBool();
                    break;
                }
                case 58: {
                    OrderingKey = input.ReadBytes();
                    break;
                }
                case 64: {
                    SequenceId = input.ReadUInt64();
                    break;
                }
                case 72: {
                    NullValue = input.ReadBool();
                    break;
                }
                case 80: {
                    NullPartitionKey = input.ReadBool();
                    break;
                }
            }
        }
    }
#endif

}
