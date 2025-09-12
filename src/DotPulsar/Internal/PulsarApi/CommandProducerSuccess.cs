#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

/// <summary>
///&#x2F; Response from CommandProducer
/// </summary>
[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandProducerSuccess : IMessage<CommandProducerSuccess>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandProducerSuccess> _parser = new pb::MessageParser<CommandProducerSuccess>(() => new CommandProducerSuccess());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandProducerSuccess> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[38]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandProducerSuccess() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandProducerSuccess(CommandProducerSuccess other) : this() {
        _hasBits0 = other._hasBits0;
        requestId_ = other.requestId_;
        producerName_ = other.producerName_;
        lastSequenceId_ = other.lastSequenceId_;
        schemaVersion_ = other.schemaVersion_;
        topicEpoch_ = other.topicEpoch_;
        producerReady_ = other.producerReady_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandProducerSuccess Clone() {
        return new CommandProducerSuccess(this);
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

    /// <summary>Field number for the "producer_name" field.</summary>
    public const int ProducerNameFieldNumber = 2;
    private readonly static string ProducerNameDefaultValue = "";

    private string producerName_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ProducerName {
        get { return producerName_ ?? ProducerNameDefaultValue; }
        set {
            producerName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "producer_name" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProducerName {
        get { return producerName_ != null; }
    }
    /// <summary>Clears the value of the "producer_name" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProducerName() {
        producerName_ = null;
    }

    /// <summary>Field number for the "last_sequence_id" field.</summary>
    public const int LastSequenceIdFieldNumber = 3;
    private readonly static long LastSequenceIdDefaultValue = -1L;

    private long lastSequenceId_;
    /// <summary>
    /// The last sequence id that was stored by this producer in the previous session
    /// This will only be meaningful if deduplication has been enabled.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public long LastSequenceId {
        get { if ((_hasBits0 & 2) != 0) { return lastSequenceId_; } else { return LastSequenceIdDefaultValue; } }
        set {
            _hasBits0 |= 2;
            lastSequenceId_ = value;
        }
    }
    /// <summary>Gets whether the "last_sequence_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasLastSequenceId {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "last_sequence_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearLastSequenceId() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "schema_version" field.</summary>
    public const int SchemaVersionFieldNumber = 4;
    private readonly static pb::ByteString SchemaVersionDefaultValue = pb::ByteString.Empty;

    private pb::ByteString schemaVersion_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString SchemaVersion {
        get { return schemaVersion_ ?? SchemaVersionDefaultValue; }
        set {
            schemaVersion_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "schema_version" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSchemaVersion {
        get { return schemaVersion_ != null; }
    }
    /// <summary>Clears the value of the "schema_version" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSchemaVersion() {
        schemaVersion_ = null;
    }

    /// <summary>Field number for the "topic_epoch" field.</summary>
    public const int TopicEpochFieldNumber = 5;
    private readonly static ulong TopicEpochDefaultValue = 0UL;

    private ulong topicEpoch_;
    /// <summary>
    /// The topic epoch assigned by the broker. This field will only be set if we
    /// were requiring exclusive access when creating the producer.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong TopicEpoch {
        get { if ((_hasBits0 & 4) != 0) { return topicEpoch_; } else { return TopicEpochDefaultValue; } }
        set {
            _hasBits0 |= 4;
            topicEpoch_ = value;
        }
    }
    /// <summary>Gets whether the "topic_epoch" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTopicEpoch {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "topic_epoch" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTopicEpoch() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "producer_ready" field.</summary>
    public const int ProducerReadyFieldNumber = 6;
    private readonly static bool ProducerReadyDefaultValue = true;

    private bool producerReady_;
    /// <summary>
    /// If producer is not "ready", the client will avoid to timeout the request
    /// for creating the producer. Instead it will wait indefinitely until it gets
    /// a subsequent  `CommandProducerSuccess` with `producer_ready==true`.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool ProducerReady {
        get { if ((_hasBits0 & 8) != 0) { return producerReady_; } else { return ProducerReadyDefaultValue; } }
        set {
            _hasBits0 |= 8;
            producerReady_ = value;
        }
    }
    /// <summary>Gets whether the "producer_ready" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProducerReady {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "producer_ready" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProducerReady() {
        _hasBits0 &= ~8;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandProducerSuccess);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandProducerSuccess other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (RequestId != other.RequestId) return false;
        if (ProducerName != other.ProducerName) return false;
        if (LastSequenceId != other.LastSequenceId) return false;
        if (SchemaVersion != other.SchemaVersion) return false;
        if (TopicEpoch != other.TopicEpoch) return false;
        if (ProducerReady != other.ProducerReady) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasProducerName) hash ^= ProducerName.GetHashCode();
        if (HasLastSequenceId) hash ^= LastSequenceId.GetHashCode();
        if (HasSchemaVersion) hash ^= SchemaVersion.GetHashCode();
        if (HasTopicEpoch) hash ^= TopicEpoch.GetHashCode();
        if (HasProducerReady) hash ^= ProducerReady.GetHashCode();
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
      if (HasProducerName) {
        output.WriteRawTag(18);
        output.WriteString(ProducerName);
      }
      if (HasLastSequenceId) {
        output.WriteRawTag(24);
        output.WriteInt64(LastSequenceId);
      }
      if (HasSchemaVersion) {
        output.WriteRawTag(34);
        output.WriteBytes(SchemaVersion);
      }
      if (HasTopicEpoch) {
        output.WriteRawTag(40);
        output.WriteUInt64(TopicEpoch);
      }
      if (HasProducerReady) {
        output.WriteRawTag(48);
        output.WriteBool(ProducerReady);
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
        if (HasProducerName) {
            output.WriteRawTag(18);
            output.WriteString(ProducerName);
        }
        if (HasLastSequenceId) {
            output.WriteRawTag(24);
            output.WriteInt64(LastSequenceId);
        }
        if (HasSchemaVersion) {
            output.WriteRawTag(34);
            output.WriteBytes(SchemaVersion);
        }
        if (HasTopicEpoch) {
            output.WriteRawTag(40);
            output.WriteUInt64(TopicEpoch);
        }
        if (HasProducerReady) {
            output.WriteRawTag(48);
            output.WriteBool(ProducerReady);
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
        if (HasProducerName) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ProducerName);
        }
        if (HasLastSequenceId) {
            size += 1 + pb::CodedOutputStream.ComputeInt64Size(LastSequenceId);
        }
        if (HasSchemaVersion) {
            size += 1 + pb::CodedOutputStream.ComputeBytesSize(SchemaVersion);
        }
        if (HasTopicEpoch) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(TopicEpoch);
        }
        if (HasProducerReady) {
            size += 1 + 1;
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandProducerSuccess other) {
        if (other == null) {
            return;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.HasProducerName) {
            ProducerName = other.ProducerName;
        }
        if (other.HasLastSequenceId) {
            LastSequenceId = other.LastSequenceId;
        }
        if (other.HasSchemaVersion) {
            SchemaVersion = other.SchemaVersion;
        }
        if (other.HasTopicEpoch) {
            TopicEpoch = other.TopicEpoch;
        }
        if (other.HasProducerReady) {
            ProducerReady = other.ProducerReady;
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
            ProducerName = input.ReadString();
            break;
          }
          case 24: {
            LastSequenceId = input.ReadInt64();
            break;
          }
          case 34: {
            SchemaVersion = input.ReadBytes();
            break;
          }
          case 40: {
            TopicEpoch = input.ReadUInt64();
            break;
          }
          case 48: {
            ProducerReady = input.ReadBool();
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
                    ProducerName = input.ReadString();
                    break;
                }
                case 24: {
                    LastSequenceId = input.ReadInt64();
                    break;
                }
                case 34: {
                    SchemaVersion = input.ReadBytes();
                    break;
                }
                case 40: {
                    TopicEpoch = input.ReadUInt64();
                    break;
                }
                case 48: {
                    ProducerReady = input.ReadBool();
                    break;
                }
            }
        }
    }
#endif

}
