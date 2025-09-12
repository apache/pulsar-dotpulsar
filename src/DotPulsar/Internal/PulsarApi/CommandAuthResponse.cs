#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandAuthResponse : IMessage<CommandAuthResponse>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandAuthResponse> _parser = new pb::MessageParser<CommandAuthResponse>(() => new CommandAuthResponse());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandAuthResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[12]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAuthResponse() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAuthResponse(CommandAuthResponse other) : this() {
        _hasBits0 = other._hasBits0;
        clientVersion_ = other.clientVersion_;
        response_ = other.response_ != null ? other.response_.Clone() : null;
        protocolVersion_ = other.protocolVersion_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandAuthResponse Clone() {
        return new CommandAuthResponse(this);
    }

    /// <summary>Field number for the "client_version" field.</summary>
    public const int ClientVersionFieldNumber = 1;
    private readonly static string ClientVersionDefaultValue = "";

    private string clientVersion_;
    /// <summary>
    /// The version of the client. Proxy should forward client's client_version.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ClientVersion {
        get { return clientVersion_ ?? ClientVersionDefaultValue; }
        set {
            clientVersion_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "client_version" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasClientVersion {
        get { return clientVersion_ != null; }
    }
    /// <summary>Clears the value of the "client_version" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearClientVersion() {
        clientVersion_ = null;
    }

    /// <summary>Field number for the "response" field.</summary>
    public const int ResponseFieldNumber = 2;
    private global::DotPulsar.Internal.PulsarApi.AuthData response_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.AuthData Response {
        get { return response_; }
        set {
            response_ = value;
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
        return Equals(other as CommandAuthResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandAuthResponse other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ClientVersion != other.ClientVersion) return false;
        if (!object.Equals(Response, other.Response)) return false;
        if (ProtocolVersion != other.ProtocolVersion) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasClientVersion) hash ^= ClientVersion.GetHashCode();
        if (response_ != null) hash ^= Response.GetHashCode();
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
      if (HasClientVersion) {
        output.WriteRawTag(10);
        output.WriteString(ClientVersion);
      }
      if (response_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Response);
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
        if (HasClientVersion) {
            output.WriteRawTag(10);
            output.WriteString(ClientVersion);
        }
        if (response_ != null) {
            output.WriteRawTag(18);
            output.WriteMessage(Response);
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
        if (HasClientVersion) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ClientVersion);
        }
        if (response_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Response);
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
    public void MergeFrom(CommandAuthResponse other) {
        if (other == null) {
            return;
        }
        if (other.HasClientVersion) {
            ClientVersion = other.ClientVersion;
        }
        if (other.response_ != null) {
            if (response_ == null) {
                Response = new global::DotPulsar.Internal.PulsarApi.AuthData();
            }
            Response.MergeFrom(other.Response);
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
            ClientVersion = input.ReadString();
            break;
          }
          case 18: {
            if (response_ == null) {
              Response = new global::DotPulsar.Internal.PulsarApi.AuthData();
            }
            input.ReadMessage(Response);
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
                    ClientVersion = input.ReadString();
                    break;
                }
                case 18: {
                    if (response_ == null) {
                        Response = new global::DotPulsar.Internal.PulsarApi.AuthData();
                    }
                    input.ReadMessage(Response);
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
