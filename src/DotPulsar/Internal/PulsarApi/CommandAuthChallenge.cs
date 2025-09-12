#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandAuthChallenge : IMessage<CommandAuthChallenge>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandAuthChallenge> _parser = new pb::MessageParser<CommandAuthChallenge>(() => new CommandAuthChallenge());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandAuthChallenge> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[13]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAuthChallenge() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAuthChallenge(CommandAuthChallenge other) : this() {
        _hasBits0 = other._hasBits0;
        serverVersion_ = other.serverVersion_;
        challenge_ = other.challenge_ != null ? other.challenge_.Clone() : null;
        protocolVersion_ = other.protocolVersion_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAuthChallenge Clone() {
        return new CommandAuthChallenge(this);
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

    /// <summary>Field number for the "challenge" field.</summary>
    public const int ChallengeFieldNumber = 2;
    private global::DotPulsar.Internal.PulsarApi.AuthData challenge_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.AuthData Challenge {
        get { return challenge_; }
        set {
            challenge_ = value;
        }
    }

    /// <summary>Field number for the "protocol_version" field.</summary>
    public const int ProtocolVersionFieldNumber = 3;
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

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandAuthChallenge);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandAuthChallenge other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ServerVersion != other.ServerVersion) return false;
        if (!object.Equals(Challenge, other.Challenge)) return false;
        if (ProtocolVersion != other.ProtocolVersion) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasServerVersion) hash ^= ServerVersion.GetHashCode();
        if (challenge_ != null) hash ^= Challenge.GetHashCode();
        if (HasProtocolVersion) hash ^= ProtocolVersion.GetHashCode();
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
      if (challenge_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Challenge);
      }
      if (HasProtocolVersion) {
        output.WriteRawTag(24);
        output.WriteInt32(ProtocolVersion);
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
        if (challenge_ != null) {
            output.WriteRawTag(18);
            output.WriteMessage(Challenge);
        }
        if (HasProtocolVersion) {
            output.WriteRawTag(24);
            output.WriteInt32(ProtocolVersion);
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
        if (challenge_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Challenge);
        }
        if (HasProtocolVersion) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(ProtocolVersion);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandAuthChallenge other) {
        if (other == null) {
            return;
        }
        if (other.HasServerVersion) {
            ServerVersion = other.ServerVersion;
        }
        if (other.challenge_ != null) {
            if (challenge_ == null) {
                Challenge = new global::DotPulsar.Internal.PulsarApi.AuthData();
            }
            Challenge.MergeFrom(other.Challenge);
        }
        if (other.HasProtocolVersion) {
            ProtocolVersion = other.ProtocolVersion;
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
          case 18: {
            if (challenge_ == null) {
              Challenge = new global::DotPulsar.Internal.PulsarApi.AuthData();
            }
            input.ReadMessage(Challenge);
            break;
          }
          case 24: {
            ProtocolVersion = input.ReadInt32();
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
                case 18: {
                    if (challenge_ == null) {
                        Challenge = new global::DotPulsar.Internal.PulsarApi.AuthData();
                    }
                    input.ReadMessage(Challenge);
                    break;
                }
                case 24: {
                    ProtocolVersion = input.ReadInt32();
                    break;
                }
            }
        }
    }
#endif

}
