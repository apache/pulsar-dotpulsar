#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandGetTopicsOfNamespaceResponse : IMessage<CommandGetTopicsOfNamespaceResponse>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandGetTopicsOfNamespaceResponse> _parser = new pb::MessageParser<CommandGetTopicsOfNamespaceResponse>(() => new CommandGetTopicsOfNamespaceResponse());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandGetTopicsOfNamespaceResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[47]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandGetTopicsOfNamespaceResponse() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandGetTopicsOfNamespaceResponse(CommandGetTopicsOfNamespaceResponse other) : this() {
        _hasBits0 = other._hasBits0;
        requestId_ = other.requestId_;
        topics_ = other.topics_.Clone();
        filtered_ = other.filtered_;
        topicsHash_ = other.topicsHash_;
        changed_ = other.changed_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandGetTopicsOfNamespaceResponse Clone() {
        return new CommandGetTopicsOfNamespaceResponse(this);
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

    /// <summary>Field number for the "topics" field.</summary>
    public const int TopicsFieldNumber = 2;
    private static readonly pb::FieldCodec<string> _repeated_topics_codec
        = pb::FieldCodec.ForString(18);
    private readonly pbc::RepeatedField<string> topics_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<string> Topics {
        get { return topics_; }
    }

    /// <summary>Field number for the "filtered" field.</summary>
    public const int FilteredFieldNumber = 3;
    private readonly static bool FilteredDefaultValue = false;

    private bool filtered_;
    /// <summary>
    /// true iff the topic list was filtered by the pattern supplied by the client
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Filtered {
        get { if ((_hasBits0 & 2) != 0) { return filtered_; } else { return FilteredDefaultValue; } }
        set {
            _hasBits0 |= 2;
            filtered_ = value;
        }
    }
    /// <summary>Gets whether the "filtered" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasFiltered {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "filtered" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearFiltered() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "topics_hash" field.</summary>
    public const int TopicsHashFieldNumber = 4;
    private readonly static string TopicsHashDefaultValue = "";

    private string topicsHash_;
    /// <summary>
    /// hash computed from the names of matching topics
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string TopicsHash {
        get { return topicsHash_ ?? TopicsHashDefaultValue; }
        set {
            topicsHash_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "topics_hash" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTopicsHash {
        get { return topicsHash_ != null; }
    }
    /// <summary>Clears the value of the "topics_hash" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTopicsHash() {
        topicsHash_ = null;
    }

    /// <summary>Field number for the "changed" field.</summary>
    public const int ChangedFieldNumber = 5;
    private readonly static bool ChangedDefaultValue = true;

    private bool changed_;
    /// <summary>
    /// if false, topics is empty and the list of matching topics has not changed
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Changed {
        get { if ((_hasBits0 & 4) != 0) { return changed_; } else { return ChangedDefaultValue; } }
        set {
            _hasBits0 |= 4;
            changed_ = value;
        }
    }
    /// <summary>Gets whether the "changed" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasChanged {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "changed" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearChanged() {
        _hasBits0 &= ~4;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandGetTopicsOfNamespaceResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandGetTopicsOfNamespaceResponse other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (RequestId != other.RequestId) return false;
        if(!topics_.Equals(other.topics_)) return false;
        if (Filtered != other.Filtered) return false;
        if (TopicsHash != other.TopicsHash) return false;
        if (Changed != other.Changed) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        hash ^= topics_.GetHashCode();
        if (HasFiltered) hash ^= Filtered.GetHashCode();
        if (HasTopicsHash) hash ^= TopicsHash.GetHashCode();
        if (HasChanged) hash ^= Changed.GetHashCode();
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
      topics_.WriteTo(output, _repeated_topics_codec);
      if (HasFiltered) {
        output.WriteRawTag(24);
        output.WriteBool(Filtered);
      }
      if (HasTopicsHash) {
        output.WriteRawTag(34);
        output.WriteString(TopicsHash);
      }
      if (HasChanged) {
        output.WriteRawTag(40);
        output.WriteBool(Changed);
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
        topics_.WriteTo(ref output, _repeated_topics_codec);
        if (HasFiltered) {
            output.WriteRawTag(24);
            output.WriteBool(Filtered);
        }
        if (HasTopicsHash) {
            output.WriteRawTag(34);
            output.WriteString(TopicsHash);
        }
        if (HasChanged) {
            output.WriteRawTag(40);
            output.WriteBool(Changed);
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
        size += topics_.CalculateSize(_repeated_topics_codec);
        if (HasFiltered) {
            size += 1 + 1;
        }
        if (HasTopicsHash) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(TopicsHash);
        }
        if (HasChanged) {
            size += 1 + 1;
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandGetTopicsOfNamespaceResponse other) {
        if (other == null) {
            return;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        topics_.Add(other.topics_);
        if (other.HasFiltered) {
            Filtered = other.Filtered;
        }
        if (other.HasTopicsHash) {
            TopicsHash = other.TopicsHash;
        }
        if (other.HasChanged) {
            Changed = other.Changed;
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
          case 18: {
            topics_.AddEntriesFrom(input, _repeated_topics_codec);
            break;
          }
          case 24: {
            Filtered = input.ReadBool();
            break;
          }
          case 34: {
            TopicsHash = input.ReadString();
            break;
          }
          case 40: {
            Changed = input.ReadBool();
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
                case 18: {
                    topics_.AddEntriesFrom(ref input, _repeated_topics_codec);
                    break;
                }
                case 24: {
                    Filtered = input.ReadBool();
                    break;
                }
                case 34: {
                    TopicsHash = input.ReadString();
                    break;
                }
                case 40: {
                    Changed = input.ReadBool();
                    break;
                }
            }
        }
    }
#endif

}
