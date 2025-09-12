#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandPartitionedTopicMetadata : IMessage<CommandPartitionedTopicMetadata>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandPartitionedTopicMetadata> _parser = new pb::MessageParser<CommandPartitionedTopicMetadata>(() => new CommandPartitionedTopicMetadata());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandPartitionedTopicMetadata> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[17]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandPartitionedTopicMetadata() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandPartitionedTopicMetadata(CommandPartitionedTopicMetadata other) : this() {
        _hasBits0 = other._hasBits0;
        topic_ = other.topic_;
        requestId_ = other.requestId_;
        originalPrincipal_ = other.originalPrincipal_;
        originalAuthData_ = other.originalAuthData_;
        originalAuthMethod_ = other.originalAuthMethod_;
        metadataAutoCreationEnabled_ = other.metadataAutoCreationEnabled_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandPartitionedTopicMetadata Clone() {
        return new CommandPartitionedTopicMetadata(this);
    }

    /// <summary>Field number for the "topic" field.</summary>
    public const int TopicFieldNumber = 1;
    private readonly static string TopicDefaultValue = "";

    private string topic_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Topic {
        get { return topic_ ?? TopicDefaultValue; }
        set {
            topic_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "topic" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTopic {
        get { return topic_ != null; }
    }
    /// <summary>Clears the value of the "topic" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTopic() {
        topic_ = null;
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 2;
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

    /// <summary>Field number for the "original_principal" field.</summary>
    public const int OriginalPrincipalFieldNumber = 3;
    private readonly static string OriginalPrincipalDefaultValue = "";

    private string originalPrincipal_;
    /// <summary>
    /// TODO - Remove original_principal, original_auth_data, original_auth_method
    /// Original principal that was verified by
    /// a Pulsar proxy.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string OriginalPrincipal {
        get { return originalPrincipal_ ?? OriginalPrincipalDefaultValue; }
        set {
            originalPrincipal_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "original_principal" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasOriginalPrincipal {
        get { return originalPrincipal_ != null; }
    }
    /// <summary>Clears the value of the "original_principal" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearOriginalPrincipal() {
        originalPrincipal_ = null;
    }

    /// <summary>Field number for the "original_auth_data" field.</summary>
    public const int OriginalAuthDataFieldNumber = 4;
    private readonly static string OriginalAuthDataDefaultValue = "";

    private string originalAuthData_;
    /// <summary>
    /// Original auth role and auth Method that was passed
    /// to the proxy.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string OriginalAuthData {
        get { return originalAuthData_ ?? OriginalAuthDataDefaultValue; }
        set {
            originalAuthData_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "original_auth_data" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasOriginalAuthData {
        get { return originalAuthData_ != null; }
    }
    /// <summary>Clears the value of the "original_auth_data" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearOriginalAuthData() {
        originalAuthData_ = null;
    }

    /// <summary>Field number for the "original_auth_method" field.</summary>
    public const int OriginalAuthMethodFieldNumber = 5;
    private readonly static string OriginalAuthMethodDefaultValue = "";

    private string originalAuthMethod_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string OriginalAuthMethod {
        get { return originalAuthMethod_ ?? OriginalAuthMethodDefaultValue; }
        set {
            originalAuthMethod_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "original_auth_method" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasOriginalAuthMethod {
        get { return originalAuthMethod_ != null; }
    }
    /// <summary>Clears the value of the "original_auth_method" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearOriginalAuthMethod() {
        originalAuthMethod_ = null;
    }

    /// <summary>Field number for the "metadata_auto_creation_enabled" field.</summary>
    public const int MetadataAutoCreationEnabledFieldNumber = 6;
    private readonly static bool MetadataAutoCreationEnabledDefaultValue = true;

    private bool metadataAutoCreationEnabled_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool MetadataAutoCreationEnabled {
        get { if ((_hasBits0 & 2) != 0) { return metadataAutoCreationEnabled_; } else { return MetadataAutoCreationEnabledDefaultValue; } }
        set {
            _hasBits0 |= 2;
            metadataAutoCreationEnabled_ = value;
        }
    }
    /// <summary>Gets whether the "metadata_auto_creation_enabled" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMetadataAutoCreationEnabled {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "metadata_auto_creation_enabled" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMetadataAutoCreationEnabled() {
        _hasBits0 &= ~2;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandPartitionedTopicMetadata);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandPartitionedTopicMetadata other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (Topic != other.Topic) return false;
        if (RequestId != other.RequestId) return false;
        if (OriginalPrincipal != other.OriginalPrincipal) return false;
        if (OriginalAuthData != other.OriginalAuthData) return false;
        if (OriginalAuthMethod != other.OriginalAuthMethod) return false;
        if (MetadataAutoCreationEnabled != other.MetadataAutoCreationEnabled) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasTopic) hash ^= Topic.GetHashCode();
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasOriginalPrincipal) hash ^= OriginalPrincipal.GetHashCode();
        if (HasOriginalAuthData) hash ^= OriginalAuthData.GetHashCode();
        if (HasOriginalAuthMethod) hash ^= OriginalAuthMethod.GetHashCode();
        if (HasMetadataAutoCreationEnabled) hash ^= MetadataAutoCreationEnabled.GetHashCode();
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
      if (HasTopic) {
        output.WriteRawTag(10);
        output.WriteString(Topic);
      }
      if (HasRequestId) {
        output.WriteRawTag(16);
        output.WriteUInt64(RequestId);
      }
      if (HasOriginalPrincipal) {
        output.WriteRawTag(26);
        output.WriteString(OriginalPrincipal);
      }
      if (HasOriginalAuthData) {
        output.WriteRawTag(34);
        output.WriteString(OriginalAuthData);
      }
      if (HasOriginalAuthMethod) {
        output.WriteRawTag(42);
        output.WriteString(OriginalAuthMethod);
      }
      if (HasMetadataAutoCreationEnabled) {
        output.WriteRawTag(48);
        output.WriteBool(MetadataAutoCreationEnabled);
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
        if (HasTopic) {
            output.WriteRawTag(10);
            output.WriteString(Topic);
        }
        if (HasRequestId) {
            output.WriteRawTag(16);
            output.WriteUInt64(RequestId);
        }
        if (HasOriginalPrincipal) {
            output.WriteRawTag(26);
            output.WriteString(OriginalPrincipal);
        }
        if (HasOriginalAuthData) {
            output.WriteRawTag(34);
            output.WriteString(OriginalAuthData);
        }
        if (HasOriginalAuthMethod) {
            output.WriteRawTag(42);
            output.WriteString(OriginalAuthMethod);
        }
        if (HasMetadataAutoCreationEnabled) {
            output.WriteRawTag(48);
            output.WriteBool(MetadataAutoCreationEnabled);
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
        if (HasTopic) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Topic);
        }
        if (HasRequestId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(RequestId);
        }
        if (HasOriginalPrincipal) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(OriginalPrincipal);
        }
        if (HasOriginalAuthData) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(OriginalAuthData);
        }
        if (HasOriginalAuthMethod) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(OriginalAuthMethod);
        }
        if (HasMetadataAutoCreationEnabled) {
            size += 1 + 1;
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandPartitionedTopicMetadata other) {
        if (other == null) {
            return;
        }
        if (other.HasTopic) {
            Topic = other.Topic;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.HasOriginalPrincipal) {
            OriginalPrincipal = other.OriginalPrincipal;
        }
        if (other.HasOriginalAuthData) {
            OriginalAuthData = other.OriginalAuthData;
        }
        if (other.HasOriginalAuthMethod) {
            OriginalAuthMethod = other.OriginalAuthMethod;
        }
        if (other.HasMetadataAutoCreationEnabled) {
            MetadataAutoCreationEnabled = other.MetadataAutoCreationEnabled;
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
            Topic = input.ReadString();
            break;
          }
          case 16: {
            RequestId = input.ReadUInt64();
            break;
          }
          case 26: {
            OriginalPrincipal = input.ReadString();
            break;
          }
          case 34: {
            OriginalAuthData = input.ReadString();
            break;
          }
          case 42: {
            OriginalAuthMethod = input.ReadString();
            break;
          }
          case 48: {
            MetadataAutoCreationEnabled = input.ReadBool();
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
                    Topic = input.ReadString();
                    break;
                }
                case 16: {
                    RequestId = input.ReadUInt64();
                    break;
                }
                case 26: {
                    OriginalPrincipal = input.ReadString();
                    break;
                }
                case 34: {
                    OriginalAuthData = input.ReadString();
                    break;
                }
                case 42: {
                    OriginalAuthMethod = input.ReadString();
                    break;
                }
                case 48: {
                    MetadataAutoCreationEnabled = input.ReadBool();
                    break;
                }
            }
        }
    }
#endif

}
