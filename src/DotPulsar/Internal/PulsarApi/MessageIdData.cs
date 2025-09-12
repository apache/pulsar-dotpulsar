#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class MessageIdData : IMessage<MessageIdData>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<MessageIdData> _parser = new pb::MessageParser<MessageIdData>(() => new MessageIdData());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<MessageIdData> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public MessageIdData() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public MessageIdData(MessageIdData other) : this() {
        _hasBits0 = other._hasBits0;
        ledgerId_ = other.ledgerId_;
        entryId_ = other.entryId_;
        partition_ = other.partition_;
        batchIndex_ = other.batchIndex_;
        ackSet_ = other.ackSet_.Clone();
        batchSize_ = other.batchSize_;
        firstChunkMessageId_ = other.firstChunkMessageId_ != null ? other.firstChunkMessageId_.Clone() : null;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public MessageIdData Clone() {
        return new MessageIdData(this);
    }

    /// <summary>Field number for the "ledgerId" field.</summary>
    public const int LedgerIdFieldNumber = 1;
    private readonly static ulong LedgerIdDefaultValue = 0UL;

    private ulong ledgerId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong LedgerId {
        get { if ((_hasBits0 & 1) != 0) { return ledgerId_; } else { return LedgerIdDefaultValue; } }
        set {
            _hasBits0 |= 1;
            ledgerId_ = value;
        }
    }
    /// <summary>Gets whether the "ledgerId" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasLedgerId {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "ledgerId" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearLedgerId() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "entryId" field.</summary>
    public const int EntryIdFieldNumber = 2;
    private readonly static ulong EntryIdDefaultValue = 0UL;

    private ulong entryId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong EntryId {
        get { if ((_hasBits0 & 2) != 0) { return entryId_; } else { return EntryIdDefaultValue; } }
        set {
            _hasBits0 |= 2;
            entryId_ = value;
        }
    }
    /// <summary>Gets whether the "entryId" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasEntryId {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "entryId" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearEntryId() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "partition" field.</summary>
    public const int PartitionFieldNumber = 3;
    private readonly static int PartitionDefaultValue = -1;

    private int partition_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int Partition {
        get { if ((_hasBits0 & 4) != 0) { return partition_; } else { return PartitionDefaultValue; } }
        set {
            _hasBits0 |= 4;
            partition_ = value;
        }
    }
    /// <summary>Gets whether the "partition" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasPartition {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "partition" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearPartition() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "batch_index" field.</summary>
    public const int BatchIndexFieldNumber = 4;
    private readonly static int BatchIndexDefaultValue = -1;

    private int batchIndex_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int BatchIndex {
        get { if ((_hasBits0 & 8) != 0) { return batchIndex_; } else { return BatchIndexDefaultValue; } }
        set {
            _hasBits0 |= 8;
            batchIndex_ = value;
        }
    }
    /// <summary>Gets whether the "batch_index" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasBatchIndex {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "batch_index" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearBatchIndex() {
        _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "ack_set" field.</summary>
    public const int AckSetFieldNumber = 5;
    private static readonly pb::FieldCodec<long> _repeated_ackSet_codec
        = pb::FieldCodec.ForInt64(40);
    private readonly pbc::RepeatedField<long> ackSet_ = new pbc::RepeatedField<long>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<long> AckSet {
        get { return ackSet_; }
    }

    /// <summary>Field number for the "batch_size" field.</summary>
    public const int BatchSizeFieldNumber = 6;
    private readonly static int BatchSizeDefaultValue = 0;

    private int batchSize_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int BatchSize {
        get { if ((_hasBits0 & 16) != 0) { return batchSize_; } else { return BatchSizeDefaultValue; } }
        set {
            _hasBits0 |= 16;
            batchSize_ = value;
        }
    }
    /// <summary>Gets whether the "batch_size" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasBatchSize {
        get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "batch_size" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearBatchSize() {
        _hasBits0 &= ~16;
    }

    /// <summary>Field number for the "first_chunk_message_id" field.</summary>
    public const int FirstChunkMessageIdFieldNumber = 7;
    private global::DotPulsar.Internal.PulsarApi.MessageIdData firstChunkMessageId_;
    /// <summary>
    /// For the chunk message id, we need to specify the first chunk message id.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.MessageIdData FirstChunkMessageId {
        get { return firstChunkMessageId_; }
        set {
            firstChunkMessageId_ = value;
        }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as MessageIdData);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(MessageIdData other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (LedgerId != other.LedgerId) return false;
        if (EntryId != other.EntryId) return false;
        if (Partition != other.Partition) return false;
        if (BatchIndex != other.BatchIndex) return false;
        if(!ackSet_.Equals(other.ackSet_)) return false;
        if (BatchSize != other.BatchSize) return false;
        if (!object.Equals(FirstChunkMessageId, other.FirstChunkMessageId)) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasLedgerId) hash ^= LedgerId.GetHashCode();
        if (HasEntryId) hash ^= EntryId.GetHashCode();
        if (HasPartition) hash ^= Partition.GetHashCode();
        if (HasBatchIndex) hash ^= BatchIndex.GetHashCode();
        hash ^= ackSet_.GetHashCode();
        if (HasBatchSize) hash ^= BatchSize.GetHashCode();
        if (firstChunkMessageId_ != null) hash ^= FirstChunkMessageId.GetHashCode();
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
      if (HasLedgerId) {
        output.WriteRawTag(8);
        output.WriteUInt64(LedgerId);
      }
      if (HasEntryId) {
        output.WriteRawTag(16);
        output.WriteUInt64(EntryId);
      }
      if (HasPartition) {
        output.WriteRawTag(24);
        output.WriteInt32(Partition);
      }
      if (HasBatchIndex) {
        output.WriteRawTag(32);
        output.WriteInt32(BatchIndex);
      }
      ackSet_.WriteTo(output, _repeated_ackSet_codec);
      if (HasBatchSize) {
        output.WriteRawTag(48);
        output.WriteInt32(BatchSize);
      }
      if (firstChunkMessageId_ != null) {
        output.WriteRawTag(58);
        output.WriteMessage(FirstChunkMessageId);
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
        if (HasLedgerId) {
            output.WriteRawTag(8);
            output.WriteUInt64(LedgerId);
        }
        if (HasEntryId) {
            output.WriteRawTag(16);
            output.WriteUInt64(EntryId);
        }
        if (HasPartition) {
            output.WriteRawTag(24);
            output.WriteInt32(Partition);
        }
        if (HasBatchIndex) {
            output.WriteRawTag(32);
            output.WriteInt32(BatchIndex);
        }
        ackSet_.WriteTo(ref output, _repeated_ackSet_codec);
        if (HasBatchSize) {
            output.WriteRawTag(48);
            output.WriteInt32(BatchSize);
        }
        if (firstChunkMessageId_ != null) {
            output.WriteRawTag(58);
            output.WriteMessage(FirstChunkMessageId);
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
        if (HasLedgerId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(LedgerId);
        }
        if (HasEntryId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(EntryId);
        }
        if (HasPartition) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(Partition);
        }
        if (HasBatchIndex) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(BatchIndex);
        }
        size += ackSet_.CalculateSize(_repeated_ackSet_codec);
        if (HasBatchSize) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(BatchSize);
        }
        if (firstChunkMessageId_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(FirstChunkMessageId);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(MessageIdData other) {
        if (other == null) {
            return;
        }
        if (other.HasLedgerId) {
            LedgerId = other.LedgerId;
        }
        if (other.HasEntryId) {
            EntryId = other.EntryId;
        }
        if (other.HasPartition) {
            Partition = other.Partition;
        }
        if (other.HasBatchIndex) {
            BatchIndex = other.BatchIndex;
        }
        ackSet_.Add(other.ackSet_);
        if (other.HasBatchSize) {
            BatchSize = other.BatchSize;
        }
        if (other.firstChunkMessageId_ != null) {
            if (firstChunkMessageId_ == null) {
                FirstChunkMessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            FirstChunkMessageId.MergeFrom(other.FirstChunkMessageId);
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
            LedgerId = input.ReadUInt64();
            break;
          }
          case 16: {
            EntryId = input.ReadUInt64();
            break;
          }
          case 24: {
            Partition = input.ReadInt32();
            break;
          }
          case 32: {
            BatchIndex = input.ReadInt32();
            break;
          }
          case 42:
          case 40: {
            ackSet_.AddEntriesFrom(input, _repeated_ackSet_codec);
            break;
          }
          case 48: {
            BatchSize = input.ReadInt32();
            break;
          }
          case 58: {
            if (firstChunkMessageId_ == null) {
              FirstChunkMessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            input.ReadMessage(FirstChunkMessageId);
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
                    LedgerId = input.ReadUInt64();
                    break;
                }
                case 16: {
                    EntryId = input.ReadUInt64();
                    break;
                }
                case 24: {
                    Partition = input.ReadInt32();
                    break;
                }
                case 32: {
                    BatchIndex = input.ReadInt32();
                    break;
                }
                case 42:
                case 40: {
                    ackSet_.AddEntriesFrom(ref input, _repeated_ackSet_codec);
                    break;
                }
                case 48: {
                    BatchSize = input.ReadInt32();
                    break;
                }
                case 58: {
                    if (firstChunkMessageId_ == null) {
                        FirstChunkMessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
                    }
                    input.ReadMessage(FirstChunkMessageId);
                    break;
                }
            }
        }
    }
#endif

}
