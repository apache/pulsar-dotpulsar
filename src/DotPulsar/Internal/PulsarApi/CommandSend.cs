#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandSend : IMessage<CommandSend>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandSend> _parser = new pb::MessageParser<CommandSend>(() => new CommandSend());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandSend> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[22]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandSend() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandSend(CommandSend other) : this() {
        _hasBits0 = other._hasBits0;
        producerId_ = other.producerId_;
        sequenceId_ = other.sequenceId_;
        numMessages_ = other.numMessages_;
        txnidLeastBits_ = other.txnidLeastBits_;
        txnidMostBits_ = other.txnidMostBits_;
        highestSequenceId_ = other.highestSequenceId_;
        isChunk_ = other.isChunk_;
        marker_ = other.marker_;
        messageId_ = other.messageId_ != null ? other.messageId_.Clone() : null;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandSend Clone() {
        return new CommandSend(this);
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

    /// <summary>Field number for the "num_messages" field.</summary>
    public const int NumMessagesFieldNumber = 3;
    private readonly static int NumMessagesDefaultValue = 1;

    private int numMessages_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int NumMessages {
        get { if ((_hasBits0 & 4) != 0) { return numMessages_; } else { return NumMessagesDefaultValue; } }
        set {
            _hasBits0 |= 4;
            numMessages_ = value;
        }
    }
    /// <summary>Gets whether the "num_messages" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNumMessages {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "num_messages" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNumMessages() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "txnid_least_bits" field.</summary>
    public const int TxnidLeastBitsFieldNumber = 4;
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
    public const int TxnidMostBitsFieldNumber = 5;
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

    /// <summary>Field number for the "highest_sequence_id" field.</summary>
    public const int HighestSequenceIdFieldNumber = 6;
    private readonly static ulong HighestSequenceIdDefaultValue = 0UL;

    private ulong highestSequenceId_;
    /// <summary>
    ///&#x2F; Add highest sequence id to support batch message with external sequence id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong HighestSequenceId {
        get { if ((_hasBits0 & 32) != 0) { return highestSequenceId_; } else { return HighestSequenceIdDefaultValue; } }
        set {
            _hasBits0 |= 32;
            highestSequenceId_ = value;
        }
    }
    /// <summary>Gets whether the "highest_sequence_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasHighestSequenceId {
        get { return (_hasBits0 & 32) != 0; }
    }
    /// <summary>Clears the value of the "highest_sequence_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearHighestSequenceId() {
        _hasBits0 &= ~32;
    }

    /// <summary>Field number for the "is_chunk" field.</summary>
    public const int IsChunkFieldNumber = 7;
    private readonly static bool IsChunkDefaultValue = false;

    private bool isChunk_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool IsChunk {
        get { if ((_hasBits0 & 64) != 0) { return isChunk_; } else { return IsChunkDefaultValue; } }
        set {
            _hasBits0 |= 64;
            isChunk_ = value;
        }
    }
    /// <summary>Gets whether the "is_chunk" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasIsChunk {
        get { return (_hasBits0 & 64) != 0; }
    }
    /// <summary>Clears the value of the "is_chunk" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearIsChunk() {
        _hasBits0 &= ~64;
    }

    /// <summary>Field number for the "marker" field.</summary>
    public const int MarkerFieldNumber = 8;
    private readonly static bool MarkerDefaultValue = false;

    private bool marker_;
    /// <summary>
    /// Specify if the message being published is a Pulsar marker or not
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Marker {
        get { if ((_hasBits0 & 128) != 0) { return marker_; } else { return MarkerDefaultValue; } }
        set {
            _hasBits0 |= 128;
            marker_ = value;
        }
    }
    /// <summary>Gets whether the "marker" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMarker {
        get { return (_hasBits0 & 128) != 0; }
    }
    /// <summary>Clears the value of the "marker" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMarker() {
        _hasBits0 &= ~128;
    }

    /// <summary>Field number for the "message_id" field.</summary>
    public const int MessageIdFieldNumber = 9;
    private global::DotPulsar.Internal.PulsarApi.MessageIdData messageId_;
    /// <summary>
    /// Message id of this message, currently is used in replicator for shadow topic.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.MessageIdData MessageId {
        get { return messageId_; }
        set {
            messageId_ = value;
        }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandSend);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandSend other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ProducerId != other.ProducerId) return false;
        if (SequenceId != other.SequenceId) return false;
        if (NumMessages != other.NumMessages) return false;
        if (TxnidLeastBits != other.TxnidLeastBits) return false;
        if (TxnidMostBits != other.TxnidMostBits) return false;
        if (HighestSequenceId != other.HighestSequenceId) return false;
        if (IsChunk != other.IsChunk) return false;
        if (Marker != other.Marker) return false;
        if (!object.Equals(MessageId, other.MessageId)) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasProducerId) hash ^= ProducerId.GetHashCode();
        if (HasSequenceId) hash ^= SequenceId.GetHashCode();
        if (HasNumMessages) hash ^= NumMessages.GetHashCode();
        if (HasTxnidLeastBits) hash ^= TxnidLeastBits.GetHashCode();
        if (HasTxnidMostBits) hash ^= TxnidMostBits.GetHashCode();
        if (HasHighestSequenceId) hash ^= HighestSequenceId.GetHashCode();
        if (HasIsChunk) hash ^= IsChunk.GetHashCode();
        if (HasMarker) hash ^= Marker.GetHashCode();
        if (messageId_ != null) hash ^= MessageId.GetHashCode();
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
      if (HasNumMessages) {
        output.WriteRawTag(24);
        output.WriteInt32(NumMessages);
      }
      if (HasTxnidLeastBits) {
        output.WriteRawTag(32);
        output.WriteUInt64(TxnidLeastBits);
      }
      if (HasTxnidMostBits) {
        output.WriteRawTag(40);
        output.WriteUInt64(TxnidMostBits);
      }
      if (HasHighestSequenceId) {
        output.WriteRawTag(48);
        output.WriteUInt64(HighestSequenceId);
      }
      if (HasIsChunk) {
        output.WriteRawTag(56);
        output.WriteBool(IsChunk);
      }
      if (HasMarker) {
        output.WriteRawTag(64);
        output.WriteBool(Marker);
      }
      if (messageId_ != null) {
        output.WriteRawTag(74);
        output.WriteMessage(MessageId);
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
        if (HasNumMessages) {
            output.WriteRawTag(24);
            output.WriteInt32(NumMessages);
        }
        if (HasTxnidLeastBits) {
            output.WriteRawTag(32);
            output.WriteUInt64(TxnidLeastBits);
        }
        if (HasTxnidMostBits) {
            output.WriteRawTag(40);
            output.WriteUInt64(TxnidMostBits);
        }
        if (HasHighestSequenceId) {
            output.WriteRawTag(48);
            output.WriteUInt64(HighestSequenceId);
        }
        if (HasIsChunk) {
            output.WriteRawTag(56);
            output.WriteBool(IsChunk);
        }
        if (HasMarker) {
            output.WriteRawTag(64);
            output.WriteBool(Marker);
        }
        if (messageId_ != null) {
            output.WriteRawTag(74);
            output.WriteMessage(MessageId);
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
        if (HasNumMessages) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(NumMessages);
        }
        if (HasTxnidLeastBits) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(TxnidLeastBits);
        }
        if (HasTxnidMostBits) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(TxnidMostBits);
        }
        if (HasHighestSequenceId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(HighestSequenceId);
        }
        if (HasIsChunk) {
            size += 1 + 1;
        }
        if (HasMarker) {
            size += 1 + 1;
        }
        if (messageId_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(MessageId);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandSend other) {
        if (other == null) {
            return;
        }
        if (other.HasProducerId) {
            ProducerId = other.ProducerId;
        }
        if (other.HasSequenceId) {
            SequenceId = other.SequenceId;
        }
        if (other.HasNumMessages) {
            NumMessages = other.NumMessages;
        }
        if (other.HasTxnidLeastBits) {
            TxnidLeastBits = other.TxnidLeastBits;
        }
        if (other.HasTxnidMostBits) {
            TxnidMostBits = other.TxnidMostBits;
        }
        if (other.HasHighestSequenceId) {
            HighestSequenceId = other.HighestSequenceId;
        }
        if (other.HasIsChunk) {
            IsChunk = other.IsChunk;
        }
        if (other.HasMarker) {
            Marker = other.Marker;
        }
        if (other.messageId_ != null) {
            if (messageId_ == null) {
                MessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            MessageId.MergeFrom(other.MessageId);
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
          case 24: {
            NumMessages = input.ReadInt32();
            break;
          }
          case 32: {
            TxnidLeastBits = input.ReadUInt64();
            break;
          }
          case 40: {
            TxnidMostBits = input.ReadUInt64();
            break;
          }
          case 48: {
            HighestSequenceId = input.ReadUInt64();
            break;
          }
          case 56: {
            IsChunk = input.ReadBool();
            break;
          }
          case 64: {
            Marker = input.ReadBool();
            break;
          }
          case 74: {
            if (messageId_ == null) {
              MessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            input.ReadMessage(MessageId);
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
                case 24: {
                    NumMessages = input.ReadInt32();
                    break;
                }
                case 32: {
                    TxnidLeastBits = input.ReadUInt64();
                    break;
                }
                case 40: {
                    TxnidMostBits = input.ReadUInt64();
                    break;
                }
                case 48: {
                    HighestSequenceId = input.ReadUInt64();
                    break;
                }
                case 56: {
                    IsChunk = input.ReadBool();
                    break;
                }
                case 64: {
                    Marker = input.ReadBool();
                    break;
                }
                case 74: {
                    if (messageId_ == null) {
                        MessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
                    }
                    input.ReadMessage(MessageId);
                    break;
                }
            }
        }
    }
#endif

}
