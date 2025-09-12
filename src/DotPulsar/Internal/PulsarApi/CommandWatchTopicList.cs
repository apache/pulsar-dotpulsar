#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandWatchTopicList : IMessage<CommandWatchTopicList>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandWatchTopicList> _parser = new pb::MessageParser<CommandWatchTopicList>(() => new CommandWatchTopicList());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandWatchTopicList> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[48]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandWatchTopicList() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandWatchTopicList(CommandWatchTopicList other) : this() {
        _hasBits0 = other._hasBits0;
        requestId_ = other.requestId_;
        watcherId_ = other.watcherId_;
        namespace_ = other.namespace_;
        topicsPattern_ = other.topicsPattern_;
        topicsHash_ = other.topicsHash_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandWatchTopicList Clone() {
        return new CommandWatchTopicList(this);
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

    /// <summary>Field number for the "watcher_id" field.</summary>
    public const int WatcherIdFieldNumber = 2;
    private readonly static ulong WatcherIdDefaultValue = 0UL;

    private ulong watcherId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong WatcherId {
        get { if ((_hasBits0 & 2) != 0) { return watcherId_; } else { return WatcherIdDefaultValue; } }
        set {
            _hasBits0 |= 2;
            watcherId_ = value;
        }
    }
    /// <summary>Gets whether the "watcher_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasWatcherId {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "watcher_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearWatcherId() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "namespace" field.</summary>
    public const int NamespaceFieldNumber = 3;
    private readonly static string NamespaceDefaultValue = "";

    private string namespace_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Namespace {
        get { return namespace_ ?? NamespaceDefaultValue; }
        set {
            namespace_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "namespace" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNamespace {
        get { return namespace_ != null; }
    }
    /// <summary>Clears the value of the "namespace" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNamespace() {
        namespace_ = null;
    }

    /// <summary>Field number for the "topics_pattern" field.</summary>
    public const int TopicsPatternFieldNumber = 4;
    private readonly static string TopicsPatternDefaultValue = "";

    private string topicsPattern_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string TopicsPattern {
        get { return topicsPattern_ ?? TopicsPatternDefaultValue; }
        set {
            topicsPattern_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "topics_pattern" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTopicsPattern {
        get { return topicsPattern_ != null; }
    }
    /// <summary>Clears the value of the "topics_pattern" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTopicsPattern() {
        topicsPattern_ = null;
    }

    /// <summary>Field number for the "topics_hash" field.</summary>
    public const int TopicsHashFieldNumber = 5;
    private readonly static string TopicsHashDefaultValue = "";

    private string topicsHash_;
    /// <summary>
    /// Only present when the client reconnects:
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

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandWatchTopicList);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandWatchTopicList other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (RequestId != other.RequestId) return false;
        if (WatcherId != other.WatcherId) return false;
        if (Namespace != other.Namespace) return false;
        if (TopicsPattern != other.TopicsPattern) return false;
        if (TopicsHash != other.TopicsHash) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasWatcherId) hash ^= WatcherId.GetHashCode();
        if (HasNamespace) hash ^= Namespace.GetHashCode();
        if (HasTopicsPattern) hash ^= TopicsPattern.GetHashCode();
        if (HasTopicsHash) hash ^= TopicsHash.GetHashCode();
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
      if (HasWatcherId) {
        output.WriteRawTag(16);
        output.WriteUInt64(WatcherId);
      }
      if (HasNamespace) {
        output.WriteRawTag(26);
        output.WriteString(Namespace);
      }
      if (HasTopicsPattern) {
        output.WriteRawTag(34);
        output.WriteString(TopicsPattern);
      }
      if (HasTopicsHash) {
        output.WriteRawTag(42);
        output.WriteString(TopicsHash);
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
        if (HasWatcherId) {
            output.WriteRawTag(16);
            output.WriteUInt64(WatcherId);
        }
        if (HasNamespace) {
            output.WriteRawTag(26);
            output.WriteString(Namespace);
        }
        if (HasTopicsPattern) {
            output.WriteRawTag(34);
            output.WriteString(TopicsPattern);
        }
        if (HasTopicsHash) {
            output.WriteRawTag(42);
            output.WriteString(TopicsHash);
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
        if (HasWatcherId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(WatcherId);
        }
        if (HasNamespace) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Namespace);
        }
        if (HasTopicsPattern) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(TopicsPattern);
        }
        if (HasTopicsHash) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(TopicsHash);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandWatchTopicList other) {
        if (other == null) {
            return;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.HasWatcherId) {
            WatcherId = other.WatcherId;
        }
        if (other.HasNamespace) {
            Namespace = other.Namespace;
        }
        if (other.HasTopicsPattern) {
            TopicsPattern = other.TopicsPattern;
        }
        if (other.HasTopicsHash) {
            TopicsHash = other.TopicsHash;
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
            WatcherId = input.ReadUInt64();
            break;
          }
          case 26: {
            Namespace = input.ReadString();
            break;
          }
          case 34: {
            TopicsPattern = input.ReadString();
            break;
          }
          case 42: {
            TopicsHash = input.ReadString();
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
                    WatcherId = input.ReadUInt64();
                    break;
                }
                case 26: {
                    Namespace = input.ReadString();
                    break;
                }
                case 34: {
                    TopicsPattern = input.ReadString();
                    break;
                }
                case 42: {
                    TopicsHash = input.ReadString();
                    break;
                }
            }
        }
    }
#endif

}
