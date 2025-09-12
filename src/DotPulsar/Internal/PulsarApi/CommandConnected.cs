#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandConnected : IMessage<CommandConnected>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandConnected> _parser = new pb::MessageParser<CommandConnected>(() => new CommandConnected());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandConnected> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[11]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandConnected() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandConnected(CommandConnected other) : this() {
        _hasBits0 = other._hasBits0;
        serverVersion_ = other.serverVersion_;
        protocolVersion_ = other.protocolVersion_;
        maxMessageSize_ = other.maxMessageSize_;
        featureFlags_ = other.featureFlags_ != null ? other.featureFlags_.Clone() : null;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandConnected Clone() {
        return new CommandConnected(this);
    }

    /// <summary>Field number for the "server_version" field.</summary>
    public const int ServerVersionFieldNumber = 1;
    private readonly static string ServerVersionDefaultValue = "";

    private string serverVersion_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ServerVersion {
        get { return serverVersion_ ?? ServerVersionDefaultValue; }
        set {
            serverVersion_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "server_version" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasServerVersion {
        get { return serverVersion_ != null; }
    }
    /// <summary>Clears the value of the "server_version" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearServerVersion() {
        serverVersion_ = null;
    }

    /// <summary>Field number for the "protocol_version" field.</summary>
    public const int ProtocolVersionFieldNumber = 2;
    private readonly static int ProtocolVersionDefaultValue = 0;

    private int protocolVersion_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int ProtocolVersion {
        get { if ((_hasBits0 & 1) != 0) { return protocolVersion_; } else { return ProtocolVersionDefaultValue; } }
        set {
            _hasBits0 |= 1;
            protocolVersion_ = value;
        }
    }
    /// <summary>Gets whether the "protocol_version" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProtocolVersion {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "protocol_version" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProtocolVersion() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "max_message_size" field.</summary>
    public const int MaxMessageSizeFieldNumber = 3;
    private readonly static int MaxMessageSizeDefaultValue = 0;

    private int maxMessageSize_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int MaxMessageSize {
        get { if ((_hasBits0 & 2) != 0) { return maxMessageSize_; } else { return MaxMessageSizeDefaultValue; } }
        set {
            _hasBits0 |= 2;
            maxMessageSize_ = value;
        }
    }
    /// <summary>Gets whether the "max_message_size" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMaxMessageSize {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "max_message_size" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMaxMessageSize() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "feature_flags" field.</summary>
    public const int FeatureFlagsFieldNumber = 4;
    private global::DotPulsar.Internal.PulsarApi.FeatureFlags featureFlags_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.FeatureFlags FeatureFlags {
        get { return featureFlags_; }
        set {
            featureFlags_ = value;
        }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandConnected);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandConnected other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ServerVersion != other.ServerVersion) return false;
        if (ProtocolVersion != other.ProtocolVersion) return false;
        if (MaxMessageSize != other.MaxMessageSize) return false;
        if (!object.Equals(FeatureFlags, other.FeatureFlags)) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasServerVersion) hash ^= ServerVersion.GetHashCode();
        if (HasProtocolVersion) hash ^= ProtocolVersion.GetHashCode();
        if (HasMaxMessageSize) hash ^= MaxMessageSize.GetHashCode();
        if (featureFlags_ != null) hash ^= FeatureFlags.GetHashCode();
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
      if (HasServerVersion) {
        output.WriteRawTag(10);
        output.WriteString(ServerVersion);
      }
      if (HasProtocolVersion) {
        output.WriteRawTag(16);
        output.WriteInt32(ProtocolVersion);
      }
      if (HasMaxMessageSize) {
        output.WriteRawTag(24);
        output.WriteInt32(MaxMessageSize);
      }
      if (featureFlags_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(FeatureFlags);
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
        if (HasServerVersion) {
            output.WriteRawTag(10);
            output.WriteString(ServerVersion);
        }
        if (HasProtocolVersion) {
            output.WriteRawTag(16);
            output.WriteInt32(ProtocolVersion);
        }
        if (HasMaxMessageSize) {
            output.WriteRawTag(24);
            output.WriteInt32(MaxMessageSize);
        }
        if (featureFlags_ != null) {
            output.WriteRawTag(34);
            output.WriteMessage(FeatureFlags);
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
        if (HasServerVersion) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ServerVersion);
        }
        if (HasProtocolVersion) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(ProtocolVersion);
        }
        if (HasMaxMessageSize) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(MaxMessageSize);
        }
        if (featureFlags_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(FeatureFlags);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandConnected other) {
        if (other == null) {
            return;
        }
        if (other.HasServerVersion) {
            ServerVersion = other.ServerVersion;
        }
        if (other.HasProtocolVersion) {
            ProtocolVersion = other.ProtocolVersion;
        }
        if (other.HasMaxMessageSize) {
            MaxMessageSize = other.MaxMessageSize;
        }
        if (other.featureFlags_ != null) {
            if (featureFlags_ == null) {
                FeatureFlags = new global::DotPulsar.Internal.PulsarApi.FeatureFlags();
            }
            FeatureFlags.MergeFrom(other.FeatureFlags);
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
            ServerVersion = input.ReadString();
            break;
          }
          case 16: {
            ProtocolVersion = input.ReadInt32();
            break;
          }
          case 24: {
            MaxMessageSize = input.ReadInt32();
            break;
          }
          case 34: {
            if (featureFlags_ == null) {
              FeatureFlags = new global::DotPulsar.Internal.PulsarApi.FeatureFlags();
            }
            input.ReadMessage(FeatureFlags);
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
                    ServerVersion = input.ReadString();
                    break;
                }
                case 16: {
                    ProtocolVersion = input.ReadInt32();
                    break;
                }
                case 24: {
                    MaxMessageSize = input.ReadInt32();
                    break;
                }
                case 34: {
                    if (featureFlags_ == null) {
                        FeatureFlags = new global::DotPulsar.Internal.PulsarApi.FeatureFlags();
                    }
                    input.ReadMessage(FeatureFlags);
                    break;
                }
            }
        }
    }
#endif

}
