#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandEndTxn : IMessage<CommandEndTxn>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandEndTxn> _parser = new pb::MessageParser<CommandEndTxn>(() => new CommandEndTxn());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandEndTxn> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[65]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandEndTxn() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandEndTxn(CommandEndTxn other) : this() {
        _hasBits0 = other._hasBits0;
        requestId_ = other.requestId_;
        txnidLeastBits_ = other.txnidLeastBits_;
        txnidMostBits_ = other.txnidMostBits_;
        txnAction_ = other.txnAction_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandEndTxn Clone() {
        return new CommandEndTxn(this);
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 1;
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

    /// <summary>Field number for the "txnid_least_bits" field.</summary>
    public const int TxnidLeastBitsFieldNumber = 2;
    private readonly static ulong TxnidLeastBitsDefaultValue = 0UL;

    private ulong txnidLeastBits_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong TxnidLeastBits {
        get { if ((_hasBits0 & 2) != 0) { return txnidLeastBits_; } else { return TxnidLeastBitsDefaultValue; } }
        set {
            _hasBits0 |= 2;
            txnidLeastBits_ = value;
        }
    }
    /// <summary>Gets whether the "txnid_least_bits" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTxnidLeastBits {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "txnid_least_bits" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTxnidLeastBits() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "txnid_most_bits" field.</summary>
    public const int TxnidMostBitsFieldNumber = 3;
    private readonly static ulong TxnidMostBitsDefaultValue = 0UL;

    private ulong txnidMostBits_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong TxnidMostBits {
        get { if ((_hasBits0 & 4) != 0) { return txnidMostBits_; } else { return TxnidMostBitsDefaultValue; } }
        set {
            _hasBits0 |= 4;
            txnidMostBits_ = value;
        }
    }
    /// <summary>Gets whether the "txnid_most_bits" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTxnidMostBits {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "txnid_most_bits" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTxnidMostBits() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "txn_action" field.</summary>
    public const int TxnActionFieldNumber = 4;
    private readonly static global::DotPulsar.Internal.PulsarApi.TxnAction TxnActionDefaultValue = global::DotPulsar.Internal.PulsarApi.TxnAction.Commit;

    private global::DotPulsar.Internal.PulsarApi.TxnAction txnAction_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.TxnAction TxnAction {
        get { if ((_hasBits0 & 8) != 0) { return txnAction_; } else { return TxnActionDefaultValue; } }
        set {
            _hasBits0 |= 8;
            txnAction_ = value;
        }
    }
    /// <summary>Gets whether the "txn_action" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTxnAction {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "txn_action" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTxnAction() {
        _hasBits0 &= ~8;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandEndTxn);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandEndTxn other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (RequestId != other.RequestId) return false;
        if (TxnidLeastBits != other.TxnidLeastBits) return false;
        if (TxnidMostBits != other.TxnidMostBits) return false;
        if (TxnAction != other.TxnAction) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasTxnidLeastBits) hash ^= TxnidLeastBits.GetHashCode();
        if (HasTxnidMostBits) hash ^= TxnidMostBits.GetHashCode();
        if (HasTxnAction) hash ^= TxnAction.GetHashCode();
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
      if (HasRequestId) {
        output.WriteRawTag(8);
        output.WriteUInt64(RequestId);
      }
      if (HasTxnidLeastBits) {
        output.WriteRawTag(16);
        output.WriteUInt64(TxnidLeastBits);
      }
      if (HasTxnidMostBits) {
        output.WriteRawTag(24);
        output.WriteUInt64(TxnidMostBits);
      }
      if (HasTxnAction) {
        output.WriteRawTag(32);
        output.WriteEnum((int) TxnAction);
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
        if (HasRequestId) {
            output.WriteRawTag(8);
            output.WriteUInt64(RequestId);
        }
        if (HasTxnidLeastBits) {
            output.WriteRawTag(16);
            output.WriteUInt64(TxnidLeastBits);
        }
        if (HasTxnidMostBits) {
            output.WriteRawTag(24);
            output.WriteUInt64(TxnidMostBits);
        }
        if (HasTxnAction) {
            output.WriteRawTag(32);
            output.WriteEnum((int) TxnAction);
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
        if (HasRequestId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(RequestId);
        }
        if (HasTxnidLeastBits) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(TxnidLeastBits);
        }
        if (HasTxnidMostBits) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(TxnidMostBits);
        }
        if (HasTxnAction) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) TxnAction);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandEndTxn other) {
        if (other == null) {
            return;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.HasTxnidLeastBits) {
            TxnidLeastBits = other.TxnidLeastBits;
        }
        if (other.HasTxnidMostBits) {
            TxnidMostBits = other.TxnidMostBits;
        }
        if (other.HasTxnAction) {
            TxnAction = other.TxnAction;
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
            RequestId = input.ReadUInt64();
            break;
          }
          case 16: {
            TxnidLeastBits = input.ReadUInt64();
            break;
          }
          case 24: {
            TxnidMostBits = input.ReadUInt64();
            break;
          }
          case 32: {
            TxnAction = (global::DotPulsar.Internal.PulsarApi.TxnAction) input.ReadEnum();
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
                    RequestId = input.ReadUInt64();
                    break;
                }
                case 16: {
                    TxnidLeastBits = input.ReadUInt64();
                    break;
                }
                case 24: {
                    TxnidMostBits = input.ReadUInt64();
                    break;
                }
                case 32: {
                    TxnAction = (global::DotPulsar.Internal.PulsarApi.TxnAction) input.ReadEnum();
                    break;
                }
            }
        }
    }
#endif

}
