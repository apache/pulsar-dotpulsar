#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class KeySharedMeta : IMessage<KeySharedMeta>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<KeySharedMeta> _parser = new pb::MessageParser<KeySharedMeta>(() => new KeySharedMeta());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<KeySharedMeta> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[15]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public KeySharedMeta() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public KeySharedMeta(KeySharedMeta other) : this() {
        _hasBits0 = other._hasBits0;
        keySharedMode_ = other.keySharedMode_;
        hashRanges_ = other.hashRanges_.Clone();
        allowOutOfOrderDelivery_ = other.allowOutOfOrderDelivery_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public KeySharedMeta Clone() {
        return new KeySharedMeta(this);
    }

    /// <summary>Field number for the "keySharedMode" field.</summary>
    public const int KeySharedModeFieldNumber = 1;
    private readonly static global::DotPulsar.Internal.PulsarApi.KeySharedMode KeySharedModeDefaultValue = global::DotPulsar.Internal.PulsarApi.KeySharedMode.AutoSplit;

    private global::DotPulsar.Internal.PulsarApi.KeySharedMode keySharedMode_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.KeySharedMode KeySharedMode {
        get { if ((_hasBits0 & 1) != 0) { return keySharedMode_; } else { return KeySharedModeDefaultValue; } }
        set {
            _hasBits0 |= 1;
            keySharedMode_ = value;
        }
    }
    /// <summary>Gets whether the "keySharedMode" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasKeySharedMode {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "keySharedMode" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearKeySharedMode() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "hashRanges" field.</summary>
    public const int HashRangesFieldNumber = 3;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.IntRange> _repeated_hashRanges_codec
        = pb::FieldCodec.ForMessage(26, global::DotPulsar.Internal.PulsarApi.IntRange.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.IntRange> hashRanges_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.IntRange>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.IntRange> HashRanges {
        get { return hashRanges_; }
    }

    /// <summary>Field number for the "allowOutOfOrderDelivery" field.</summary>
    public const int AllowOutOfOrderDeliveryFieldNumber = 4;
    private readonly static bool AllowOutOfOrderDeliveryDefaultValue = false;

    private bool allowOutOfOrderDelivery_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool AllowOutOfOrderDelivery {
        get { if ((_hasBits0 & 2) != 0) { return allowOutOfOrderDelivery_; } else { return AllowOutOfOrderDeliveryDefaultValue; } }
        set {
            _hasBits0 |= 2;
            allowOutOfOrderDelivery_ = value;
        }
    }
    /// <summary>Gets whether the "allowOutOfOrderDelivery" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasAllowOutOfOrderDelivery {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "allowOutOfOrderDelivery" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearAllowOutOfOrderDelivery() {
        _hasBits0 &= ~2;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as KeySharedMeta);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(KeySharedMeta other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (KeySharedMode != other.KeySharedMode) return false;
        if(!hashRanges_.Equals(other.hashRanges_)) return false;
        if (AllowOutOfOrderDelivery != other.AllowOutOfOrderDelivery) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasKeySharedMode) hash ^= KeySharedMode.GetHashCode();
        hash ^= hashRanges_.GetHashCode();
        if (HasAllowOutOfOrderDelivery) hash ^= AllowOutOfOrderDelivery.GetHashCode();
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
      if (HasKeySharedMode) {
        output.WriteRawTag(8);
        output.WriteEnum((int) KeySharedMode);
      }
      hashRanges_.WriteTo(output, _repeated_hashRanges_codec);
      if (HasAllowOutOfOrderDelivery) {
        output.WriteRawTag(32);
        output.WriteBool(AllowOutOfOrderDelivery);
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
        if (HasKeySharedMode) {
            output.WriteRawTag(8);
            output.WriteEnum((int) KeySharedMode);
        }
        hashRanges_.WriteTo(ref output, _repeated_hashRanges_codec);
        if (HasAllowOutOfOrderDelivery) {
            output.WriteRawTag(32);
            output.WriteBool(AllowOutOfOrderDelivery);
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
        if (HasKeySharedMode) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) KeySharedMode);
        }
        size += hashRanges_.CalculateSize(_repeated_hashRanges_codec);
        if (HasAllowOutOfOrderDelivery) {
            size += 1 + 1;
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(KeySharedMeta other) {
        if (other == null) {
            return;
        }
        if (other.HasKeySharedMode) {
            KeySharedMode = other.KeySharedMode;
        }
        hashRanges_.Add(other.hashRanges_);
        if (other.HasAllowOutOfOrderDelivery) {
            AllowOutOfOrderDelivery = other.AllowOutOfOrderDelivery;
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
            KeySharedMode = (global::DotPulsar.Internal.PulsarApi.KeySharedMode) input.ReadEnum();
            break;
          }
          case 26: {
            hashRanges_.AddEntriesFrom(input, _repeated_hashRanges_codec);
            break;
          }
          case 32: {
            AllowOutOfOrderDelivery = input.ReadBool();
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
                    KeySharedMode = (global::DotPulsar.Internal.PulsarApi.KeySharedMode) input.ReadEnum();
                    break;
                }
                case 26: {
                    hashRanges_.AddEntriesFrom(ref input, _repeated_hashRanges_codec);
                    break;
                }
                case 32: {
                    AllowOutOfOrderDelivery = input.ReadBool();
                    break;
                }
            }
        }
    }
#endif

}
