#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandWatchTopicUpdate : IMessage<CommandWatchTopicUpdate>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandWatchTopicUpdate> _parser = new pb::MessageParser<CommandWatchTopicUpdate>(() => new CommandWatchTopicUpdate());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandWatchTopicUpdate> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[50]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandWatchTopicUpdate() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandWatchTopicUpdate(CommandWatchTopicUpdate other) : this() {
        _hasBits0 = other._hasBits0;
        watcherId_ = other.watcherId_;
        newTopics_ = other.newTopics_.Clone();
        deletedTopics_ = other.deletedTopics_.Clone();
        topicsHash_ = other.topicsHash_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandWatchTopicUpdate Clone() {
        return new CommandWatchTopicUpdate(this);
    }

    /// <summary>Field number for the "watcher_id" field.</summary>
    public const int WatcherIdFieldNumber = 1;
    private readonly static ulong WatcherIdDefaultValue = 0UL;

    private ulong watcherId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong WatcherId {
        get { if ((_hasBits0 & 1) != 0) { return watcherId_; } else { return WatcherIdDefaultValue; } }
        set {
            _hasBits0 |= 1;
            watcherId_ = value;
        }
    }
    /// <summary>Gets whether the "watcher_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasWatcherId {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "watcher_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearWatcherId() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "new_topics" field.</summary>
    public const int NewTopicsFieldNumber = 2;
    private static readonly pb::FieldCodec<string> _repeated_newTopics_codec
        = pb::FieldCodec.ForString(18);
    private readonly pbc::RepeatedField<string> newTopics_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<string> NewTopics {
        get { return newTopics_; }
    }

    /// <summary>Field number for the "deleted_topics" field.</summary>
    public const int DeletedTopicsFieldNumber = 3;
    private static readonly pb::FieldCodec<string> _repeated_deletedTopics_codec
        = pb::FieldCodec.ForString(26);
    private readonly pbc::RepeatedField<string> deletedTopics_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<string> DeletedTopics {
        get { return deletedTopics_; }
    }

    /// <summary>Field number for the "topics_hash" field.</summary>
    public const int TopicsHashFieldNumber = 4;
    private readonly static string TopicsHashDefaultValue = "";

    private string topicsHash_;
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
        return Equals(other as CommandWatchTopicUpdate);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandWatchTopicUpdate other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (WatcherId != other.WatcherId) return false;
        if(!newTopics_.Equals(other.newTopics_)) return false;
        if(!deletedTopics_.Equals(other.deletedTopics_)) return false;
        if (TopicsHash != other.TopicsHash) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasWatcherId) hash ^= WatcherId.GetHashCode();
        hash ^= newTopics_.GetHashCode();
        hash ^= deletedTopics_.GetHashCode();
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
      if (HasWatcherId) {
        output.WriteRawTag(8);
        output.WriteUInt64(WatcherId);
      }
      newTopics_.WriteTo(output, _repeated_newTopics_codec);
      deletedTopics_.WriteTo(output, _repeated_deletedTopics_codec);
      if (HasTopicsHash) {
        output.WriteRawTag(34);
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
        if (HasWatcherId) {
            output.WriteRawTag(8);
            output.WriteUInt64(WatcherId);
        }
        newTopics_.WriteTo(ref output, _repeated_newTopics_codec);
        deletedTopics_.WriteTo(ref output, _repeated_deletedTopics_codec);
        if (HasTopicsHash) {
            output.WriteRawTag(34);
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
        if (HasWatcherId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(WatcherId);
        }
        size += newTopics_.CalculateSize(_repeated_newTopics_codec);
        size += deletedTopics_.CalculateSize(_repeated_deletedTopics_codec);
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
    public void MergeFrom(CommandWatchTopicUpdate other) {
        if (other == null) {
            return;
        }
        if (other.HasWatcherId) {
            WatcherId = other.WatcherId;
        }
        newTopics_.Add(other.newTopics_);
        deletedTopics_.Add(other.deletedTopics_);
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
            WatcherId = input.ReadUInt64();
            break;
          }
          case 18: {
            newTopics_.AddEntriesFrom(input, _repeated_newTopics_codec);
            break;
          }
          case 26: {
            deletedTopics_.AddEntriesFrom(input, _repeated_deletedTopics_codec);
            break;
          }
          case 34: {
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
                    WatcherId = input.ReadUInt64();
                    break;
                }
                case 18: {
                    newTopics_.AddEntriesFrom(ref input, _repeated_newTopics_codec);
                    break;
                }
                case 26: {
                    deletedTopics_.AddEntriesFrom(ref input, _repeated_deletedTopics_codec);
                    break;
                }
                case 34: {
                    TopicsHash = input.ReadString();
                    break;
                }
            }
        }
    }
#endif

}
