#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandLookupTopicResponse : IMessage<CommandLookupTopicResponse>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandLookupTopicResponse> _parser = new pb::MessageParser<CommandLookupTopicResponse>(() => new CommandLookupTopicResponse());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandLookupTopicResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[20]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandLookupTopicResponse() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandLookupTopicResponse(CommandLookupTopicResponse other) : this() {
        _hasBits0 = other._hasBits0;
        brokerServiceUrl_ = other.brokerServiceUrl_;
        brokerServiceUrlTls_ = other.brokerServiceUrlTls_;
        response_ = other.response_;
        requestId_ = other.requestId_;
        authoritative_ = other.authoritative_;
        error_ = other.error_;
        message_ = other.message_;
        proxyThroughServiceUrl_ = other.proxyThroughServiceUrl_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandLookupTopicResponse Clone() {
        return new CommandLookupTopicResponse(this);
    }

    /// <summary>Field number for the "brokerServiceUrl" field.</summary>
    public const int BrokerServiceUrlFieldNumber = 1;
    private readonly static string BrokerServiceUrlDefaultValue = "";

    private string brokerServiceUrl_;
    /// <summary>
    /// Optional in case of error
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string BrokerServiceUrl {
        get { return brokerServiceUrl_ ?? BrokerServiceUrlDefaultValue; }
        set {
            brokerServiceUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "brokerServiceUrl" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasBrokerServiceUrl {
        get { return brokerServiceUrl_ != null; }
    }
    /// <summary>Clears the value of the "brokerServiceUrl" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearBrokerServiceUrl() {
        brokerServiceUrl_ = null;
    }

    /// <summary>Field number for the "brokerServiceUrlTls" field.</summary>
    public const int BrokerServiceUrlTlsFieldNumber = 2;
    private readonly static string BrokerServiceUrlTlsDefaultValue = "";

    private string brokerServiceUrlTls_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string BrokerServiceUrlTls {
        get { return brokerServiceUrlTls_ ?? BrokerServiceUrlTlsDefaultValue; }
        set {
            brokerServiceUrlTls_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "brokerServiceUrlTls" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasBrokerServiceUrlTls {
        get { return brokerServiceUrlTls_ != null; }
    }
    /// <summary>Clears the value of the "brokerServiceUrlTls" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearBrokerServiceUrlTls() {
        brokerServiceUrlTls_ = null;
    }

    /// <summary>Field number for the "response" field.</summary>
    public const int ResponseFieldNumber = 3;
    private readonly static global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse.Types.LookupType ResponseDefaultValue = global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse.Types.LookupType.Redirect;

    private global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse.Types.LookupType response_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse.Types.LookupType Response {
        get { if ((_hasBits0 & 1) != 0) { return response_; } else { return ResponseDefaultValue; } }
        set {
            _hasBits0 |= 1;
            response_ = value;
        }
    }
    /// <summary>Gets whether the "response" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasResponse {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "response" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearResponse() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 4;
    private readonly static ulong RequestIdDefaultValue = 0UL;

    private ulong requestId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong RequestId {
        get { if ((_hasBits0 & 2) != 0) { return requestId_; } else { return RequestIdDefaultValue; } }
        set {
            _hasBits0 |= 2;
            requestId_ = value;
        }
    }
    /// <summary>Gets whether the "request_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasRequestId {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "request_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearRequestId() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "authoritative" field.</summary>
    public const int AuthoritativeFieldNumber = 5;
    private readonly static bool AuthoritativeDefaultValue = false;

    private bool authoritative_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Authoritative {
        get { if ((_hasBits0 & 4) != 0) { return authoritative_; } else { return AuthoritativeDefaultValue; } }
        set {
            _hasBits0 |= 4;
            authoritative_ = value;
        }
    }
    /// <summary>Gets whether the "authoritative" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasAuthoritative {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "authoritative" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearAuthoritative() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "error" field.</summary>
    public const int ErrorFieldNumber = 6;
    private readonly static global::DotPulsar.Internal.PulsarApi.ServerError ErrorDefaultValue = global::DotPulsar.Internal.PulsarApi.ServerError.UnknownError;

    private global::DotPulsar.Internal.PulsarApi.ServerError error_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.ServerError Error {
        get { if ((_hasBits0 & 8) != 0) { return error_; } else { return ErrorDefaultValue; } }
        set {
            _hasBits0 |= 8;
            error_ = value;
        }
    }
    /// <summary>Gets whether the "error" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasError {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "error" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearError() {
        _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "message" field.</summary>
    public const int MessageFieldNumber = 7;
    private readonly static string MessageDefaultValue = "";

    private string message_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Message {
        get { return message_ ?? MessageDefaultValue; }
        set {
            message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "message" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMessage {
        get { return message_ != null; }
    }
    /// <summary>Clears the value of the "message" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMessage() {
        message_ = null;
    }

    /// <summary>Field number for the "proxy_through_service_url" field.</summary>
    public const int ProxyThroughServiceUrlFieldNumber = 8;
    private readonly static bool ProxyThroughServiceUrlDefaultValue = false;

    private bool proxyThroughServiceUrl_;
    /// <summary>
    /// If it's true, indicates to the client that it must
    /// always connect through the service url after the
    /// lookup has been completed.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool ProxyThroughServiceUrl {
        get { if ((_hasBits0 & 16) != 0) { return proxyThroughServiceUrl_; } else { return ProxyThroughServiceUrlDefaultValue; } }
        set {
            _hasBits0 |= 16;
            proxyThroughServiceUrl_ = value;
        }
    }
    /// <summary>Gets whether the "proxy_through_service_url" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProxyThroughServiceUrl {
        get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "proxy_through_service_url" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProxyThroughServiceUrl() {
        _hasBits0 &= ~16;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandLookupTopicResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandLookupTopicResponse other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (BrokerServiceUrl != other.BrokerServiceUrl) return false;
        if (BrokerServiceUrlTls != other.BrokerServiceUrlTls) return false;
        if (Response != other.Response) return false;
        if (RequestId != other.RequestId) return false;
        if (Authoritative != other.Authoritative) return false;
        if (Error != other.Error) return false;
        if (Message != other.Message) return false;
        if (ProxyThroughServiceUrl != other.ProxyThroughServiceUrl) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasBrokerServiceUrl) hash ^= BrokerServiceUrl.GetHashCode();
        if (HasBrokerServiceUrlTls) hash ^= BrokerServiceUrlTls.GetHashCode();
        if (HasResponse) hash ^= Response.GetHashCode();
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasAuthoritative) hash ^= Authoritative.GetHashCode();
        if (HasError) hash ^= Error.GetHashCode();
        if (HasMessage) hash ^= Message.GetHashCode();
        if (HasProxyThroughServiceUrl) hash ^= ProxyThroughServiceUrl.GetHashCode();
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
      if (HasBrokerServiceUrl) {
        output.WriteRawTag(10);
        output.WriteString(BrokerServiceUrl);
      }
      if (HasBrokerServiceUrlTls) {
        output.WriteRawTag(18);
        output.WriteString(BrokerServiceUrlTls);
      }
      if (HasResponse) {
        output.WriteRawTag(24);
        output.WriteEnum((int) Response);
      }
      if (HasRequestId) {
        output.WriteRawTag(32);
        output.WriteUInt64(RequestId);
      }
      if (HasAuthoritative) {
        output.WriteRawTag(40);
        output.WriteBool(Authoritative);
      }
      if (HasError) {
        output.WriteRawTag(48);
        output.WriteEnum((int) Error);
      }
      if (HasMessage) {
        output.WriteRawTag(58);
        output.WriteString(Message);
      }
      if (HasProxyThroughServiceUrl) {
        output.WriteRawTag(64);
        output.WriteBool(ProxyThroughServiceUrl);
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
        if (HasBrokerServiceUrl) {
            output.WriteRawTag(10);
            output.WriteString(BrokerServiceUrl);
        }
        if (HasBrokerServiceUrlTls) {
            output.WriteRawTag(18);
            output.WriteString(BrokerServiceUrlTls);
        }
        if (HasResponse) {
            output.WriteRawTag(24);
            output.WriteEnum((int) Response);
        }
        if (HasRequestId) {
            output.WriteRawTag(32);
            output.WriteUInt64(RequestId);
        }
        if (HasAuthoritative) {
            output.WriteRawTag(40);
            output.WriteBool(Authoritative);
        }
        if (HasError) {
            output.WriteRawTag(48);
            output.WriteEnum((int) Error);
        }
        if (HasMessage) {
            output.WriteRawTag(58);
            output.WriteString(Message);
        }
        if (HasProxyThroughServiceUrl) {
            output.WriteRawTag(64);
            output.WriteBool(ProxyThroughServiceUrl);
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
        if (HasBrokerServiceUrl) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(BrokerServiceUrl);
        }
        if (HasBrokerServiceUrlTls) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(BrokerServiceUrlTls);
        }
        if (HasResponse) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Response);
        }
        if (HasRequestId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(RequestId);
        }
        if (HasAuthoritative) {
            size += 1 + 1;
        }
        if (HasError) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Error);
        }
        if (HasMessage) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Message);
        }
        if (HasProxyThroughServiceUrl) {
            size += 1 + 1;
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandLookupTopicResponse other) {
        if (other == null) {
            return;
        }
        if (other.HasBrokerServiceUrl) {
            BrokerServiceUrl = other.BrokerServiceUrl;
        }
        if (other.HasBrokerServiceUrlTls) {
            BrokerServiceUrlTls = other.BrokerServiceUrlTls;
        }
        if (other.HasResponse) {
            Response = other.Response;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.HasAuthoritative) {
            Authoritative = other.Authoritative;
        }
        if (other.HasError) {
            Error = other.Error;
        }
        if (other.HasMessage) {
            Message = other.Message;
        }
        if (other.HasProxyThroughServiceUrl) {
            ProxyThroughServiceUrl = other.ProxyThroughServiceUrl;
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
            BrokerServiceUrl = input.ReadString();
            break;
          }
          case 18: {
            BrokerServiceUrlTls = input.ReadString();
            break;
          }
          case 24: {
            Response = (global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse.Types.LookupType) input.ReadEnum();
            break;
          }
          case 32: {
            RequestId = input.ReadUInt64();
            break;
          }
          case 40: {
            Authoritative = input.ReadBool();
            break;
          }
          case 48: {
            Error = (global::DotPulsar.Internal.PulsarApi.ServerError) input.ReadEnum();
            break;
          }
          case 58: {
            Message = input.ReadString();
            break;
          }
          case 64: {
            ProxyThroughServiceUrl = input.ReadBool();
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
                    BrokerServiceUrl = input.ReadString();
                    break;
                }
                case 18: {
                    BrokerServiceUrlTls = input.ReadString();
                    break;
                }
                case 24: {
                    Response = (global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse.Types.LookupType) input.ReadEnum();
                    break;
                }
                case 32: {
                    RequestId = input.ReadUInt64();
                    break;
                }
                case 40: {
                    Authoritative = input.ReadBool();
                    break;
                }
                case 48: {
                    Error = (global::DotPulsar.Internal.PulsarApi.ServerError) input.ReadEnum();
                    break;
                }
                case 58: {
                    Message = input.ReadString();
                    break;
                }
                case 64: {
                    ProxyThroughServiceUrl = input.ReadBool();
                    break;
                }
            }
        }
    }
#endif

    #region Nested types
    /// <summary>Container for nested types declared in the CommandLookupTopicResponse message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Types {
        public enum LookupType {
            [pbr::OriginalName("Redirect")] Redirect = 0,
            [pbr::OriginalName("Connect")] Connect = 1,
            [pbr::OriginalName("Failed")] Failed = 2,
        }

    }
    #endregion

}
