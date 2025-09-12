#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandAddSubscriptionToTxn : IMessage<CommandAddSubscriptionToTxn>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandAddSubscriptionToTxn> _parser = new pb::MessageParser<CommandAddSubscriptionToTxn>(() => new CommandAddSubscriptionToTxn());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandAddSubscriptionToTxn> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[63]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAddSubscriptionToTxn() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAddSubscriptionToTxn(CommandAddSubscriptionToTxn other) : this() {
        _hasBits0 = other._hasBits0;
        requestId_ = other.requestId_;
        txnidLeastBits_ = other.txnidLeastBits_;
        txnidMostBits_ = other.txnidMostBits_;
        subscription_ = other.subscription_.Clone();
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAddSubscriptionToTxn Clone() {
        return new CommandAddSubscriptionToTxn(this);
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

    /// <summary>Field number for the "subscription" field.</summary>
    public const int SubscriptionFieldNumber = 4;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.Subscription> _repeated_subscription_codec
        = pb::FieldCodec.ForMessage(34, global::DotPulsar.Internal.PulsarApi.Subscription.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.Subscription> subscription_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.Subscription>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.Subscription> Subscription {
        get { return subscription_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandAddSubscriptionToTxn);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandAddSubscriptionToTxn other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (RequestId != other.RequestId) return false;
        if (TxnidLeastBits != other.TxnidLeastBits) return false;
        if (TxnidMostBits != other.TxnidMostBits) return false;
        if(!subscription_.Equals(other.subscription_)) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasTxnidLeastBits) hash ^= TxnidLeastBits.GetHashCode();
        if (HasTxnidMostBits) hash ^= TxnidMostBits.GetHashCode();
        hash ^= subscription_.GetHashCode();
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
      subscription_.WriteTo(output, _repeated_subscription_codec);
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
        subscription_.WriteTo(ref output, _repeated_subscription_codec);
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
        size += subscription_.CalculateSize(_repeated_subscription_codec);
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandAddSubscriptionToTxn other) {
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
        subscription_.Add(other.subscription_);
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
          case 34: {
            subscription_.AddEntriesFrom(input, _repeated_subscription_codec);
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
                case 34: {
                    subscription_.AddEntriesFrom(ref input, _repeated_subscription_codec);
                    break;
                }
            }
        }
    }
#endif

}
