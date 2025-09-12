#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandConnect : IMessage<CommandConnect>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandConnect> _parser = new pb::MessageParser<CommandConnect>(() => new CommandConnect());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandConnect> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[9]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandConnect() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandConnect(CommandConnect other) : this() {
        _hasBits0 = other._hasBits0;
        clientVersion_ = other.clientVersion_;
        authMethod_ = other.authMethod_;
        authMethodName_ = other.authMethodName_;
        authData_ = other.authData_;
        protocolVersion_ = other.protocolVersion_;
        proxyToBrokerUrl_ = other.proxyToBrokerUrl_;
        originalPrincipal_ = other.originalPrincipal_;
        originalAuthData_ = other.originalAuthData_;
        originalAuthMethod_ = other.originalAuthMethod_;
        featureFlags_ = other.featureFlags_ != null ? other.featureFlags_.Clone() : null;
        proxyVersion_ = other.proxyVersion_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandConnect Clone() {
        return new CommandConnect(this);
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

    /// <summary>Field number for the "auth_method" field.</summary>
    public const int AuthMethodFieldNumber = 2;
    private readonly static global::DotPulsar.Internal.PulsarApi.AuthMethod AuthMethodDefaultValue = global::DotPulsar.Internal.PulsarApi.AuthMethod.None;

    private global::DotPulsar.Internal.PulsarApi.AuthMethod authMethod_;
    /// <summary>
    /// Deprecated. Use "auth_method_name" instead.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.AuthMethod AuthMethod {
        get { if ((_hasBits0 & 1) != 0) { return authMethod_; } else { return AuthMethodDefaultValue; } }
        set {
            _hasBits0 |= 1;
            authMethod_ = value;
        }
    }
    /// <summary>Gets whether the "auth_method" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasAuthMethod {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "auth_method" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearAuthMethod() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "auth_method_name" field.</summary>
    public const int AuthMethodNameFieldNumber = 5;
    private readonly static string AuthMethodNameDefaultValue = "";

    private string authMethodName_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string AuthMethodName {
        get { return authMethodName_ ?? AuthMethodNameDefaultValue; }
        set {
            authMethodName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "auth_method_name" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasAuthMethodName {
        get { return authMethodName_ != null; }
    }
    /// <summary>Clears the value of the "auth_method_name" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearAuthMethodName() {
        authMethodName_ = null;
    }

    /// <summary>Field number for the "auth_data" field.</summary>
    public const int AuthDataFieldNumber = 3;
    private readonly static pb::ByteString AuthDataDefaultValue = pb::ByteString.Empty;

    private pb::ByteString authData_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString AuthData {
        get { return authData_ ?? AuthDataDefaultValue; }
        set {
            authData_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "auth_data" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasAuthData {
        get { return authData_ != null; }
    }
    /// <summary>Clears the value of the "auth_data" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearAuthData() {
        authData_ = null;
    }

    /// <summary>Field number for the "protocol_version" field.</summary>
    public const int ProtocolVersionFieldNumber = 4;
    private readonly static int ProtocolVersionDefaultValue = 0;

    private int protocolVersion_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int ProtocolVersion {
        get { if ((_hasBits0 & 2) != 0) { return protocolVersion_; } else { return ProtocolVersionDefaultValue; } }
        set {
            _hasBits0 |= 2;
            protocolVersion_ = value;
        }
    }
    /// <summary>Gets whether the "protocol_version" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProtocolVersion {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "protocol_version" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProtocolVersion() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "proxy_to_broker_url" field.</summary>
    public const int ProxyToBrokerUrlFieldNumber = 6;
    private readonly static string ProxyToBrokerUrlDefaultValue = "";

    private string proxyToBrokerUrl_;
    /// <summary>
    /// Client can ask to be proxyied to a specific broker
    /// This is only honored by a Pulsar proxy
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ProxyToBrokerUrl {
        get { return proxyToBrokerUrl_ ?? ProxyToBrokerUrlDefaultValue; }
        set {
            proxyToBrokerUrl_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "proxy_to_broker_url" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProxyToBrokerUrl {
        get { return proxyToBrokerUrl_ != null; }
    }
    /// <summary>Clears the value of the "proxy_to_broker_url" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProxyToBrokerUrl() {
        proxyToBrokerUrl_ = null;
    }

    /// <summary>Field number for the "original_principal" field.</summary>
    public const int OriginalPrincipalFieldNumber = 7;
    private readonly static string OriginalPrincipalDefaultValue = "";

    private string originalPrincipal_;
    /// <summary>
    /// Original principal that was verified by
    /// a Pulsar proxy. In this case the auth info above
    /// will be the auth of the proxy itself
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
    public const int OriginalAuthDataFieldNumber = 8;
    private readonly static string OriginalAuthDataDefaultValue = "";

    private string originalAuthData_;
    /// <summary>
    /// Original auth role and auth Method that was passed
    /// to the proxy. In this case the auth info above
    /// will be the auth of the proxy itself
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
    public const int OriginalAuthMethodFieldNumber = 9;
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

    /// <summary>Field number for the "feature_flags" field.</summary>
    public const int FeatureFlagsFieldNumber = 10;
    private global::DotPulsar.Internal.PulsarApi.FeatureFlags featureFlags_;
    /// <summary>
    /// Feature flags
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.FeatureFlags FeatureFlags {
        get { return featureFlags_; }
        set {
            featureFlags_ = value;
        }
    }

    /// <summary>Field number for the "proxy_version" field.</summary>
    public const int ProxyVersionFieldNumber = 11;
    private readonly static string ProxyVersionDefaultValue = "";

    private string proxyVersion_;
    /// <summary>
    /// Version of the proxy. Should only be forwarded by a proxy.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ProxyVersion {
        get { return proxyVersion_ ?? ProxyVersionDefaultValue; }
        set {
            proxyVersion_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "proxy_version" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProxyVersion {
        get { return proxyVersion_ != null; }
    }
    /// <summary>Clears the value of the "proxy_version" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProxyVersion() {
        proxyVersion_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandConnect);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandConnect other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ClientVersion != other.ClientVersion) return false;
        if (AuthMethod != other.AuthMethod) return false;
        if (AuthMethodName != other.AuthMethodName) return false;
        if (AuthData != other.AuthData) return false;
        if (ProtocolVersion != other.ProtocolVersion) return false;
        if (ProxyToBrokerUrl != other.ProxyToBrokerUrl) return false;
        if (OriginalPrincipal != other.OriginalPrincipal) return false;
        if (OriginalAuthData != other.OriginalAuthData) return false;
        if (OriginalAuthMethod != other.OriginalAuthMethod) return false;
        if (!object.Equals(FeatureFlags, other.FeatureFlags)) return false;
        if (ProxyVersion != other.ProxyVersion) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasClientVersion) hash ^= ClientVersion.GetHashCode();
        if (HasAuthMethod) hash ^= AuthMethod.GetHashCode();
        if (HasAuthMethodName) hash ^= AuthMethodName.GetHashCode();
        if (HasAuthData) hash ^= AuthData.GetHashCode();
        if (HasProtocolVersion) hash ^= ProtocolVersion.GetHashCode();
        if (HasProxyToBrokerUrl) hash ^= ProxyToBrokerUrl.GetHashCode();
        if (HasOriginalPrincipal) hash ^= OriginalPrincipal.GetHashCode();
        if (HasOriginalAuthData) hash ^= OriginalAuthData.GetHashCode();
        if (HasOriginalAuthMethod) hash ^= OriginalAuthMethod.GetHashCode();
        if (featureFlags_ != null) hash ^= FeatureFlags.GetHashCode();
        if (HasProxyVersion) hash ^= ProxyVersion.GetHashCode();
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
      if (HasAuthMethod) {
        output.WriteRawTag(16);
        output.WriteEnum((int) AuthMethod);
      }
      if (HasAuthData) {
        output.WriteRawTag(26);
        output.WriteBytes(AuthData);
      }
      if (HasProtocolVersion) {
        output.WriteRawTag(32);
        output.WriteInt32(ProtocolVersion);
      }
      if (HasAuthMethodName) {
        output.WriteRawTag(42);
        output.WriteString(AuthMethodName);
      }
      if (HasProxyToBrokerUrl) {
        output.WriteRawTag(50);
        output.WriteString(ProxyToBrokerUrl);
      }
      if (HasOriginalPrincipal) {
        output.WriteRawTag(58);
        output.WriteString(OriginalPrincipal);
      }
      if (HasOriginalAuthData) {
        output.WriteRawTag(66);
        output.WriteString(OriginalAuthData);
      }
      if (HasOriginalAuthMethod) {
        output.WriteRawTag(74);
        output.WriteString(OriginalAuthMethod);
      }
      if (featureFlags_ != null) {
        output.WriteRawTag(82);
        output.WriteMessage(FeatureFlags);
      }
      if (HasProxyVersion) {
        output.WriteRawTag(90);
        output.WriteString(ProxyVersion);
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
        if (HasAuthMethod) {
            output.WriteRawTag(16);
            output.WriteEnum((int) AuthMethod);
        }
        if (HasAuthData) {
            output.WriteRawTag(26);
            output.WriteBytes(AuthData);
        }
        if (HasProtocolVersion) {
            output.WriteRawTag(32);
            output.WriteInt32(ProtocolVersion);
        }
        if (HasAuthMethodName) {
            output.WriteRawTag(42);
            output.WriteString(AuthMethodName);
        }
        if (HasProxyToBrokerUrl) {
            output.WriteRawTag(50);
            output.WriteString(ProxyToBrokerUrl);
        }
        if (HasOriginalPrincipal) {
            output.WriteRawTag(58);
            output.WriteString(OriginalPrincipal);
        }
        if (HasOriginalAuthData) {
            output.WriteRawTag(66);
            output.WriteString(OriginalAuthData);
        }
        if (HasOriginalAuthMethod) {
            output.WriteRawTag(74);
            output.WriteString(OriginalAuthMethod);
        }
        if (featureFlags_ != null) {
            output.WriteRawTag(82);
            output.WriteMessage(FeatureFlags);
        }
        if (HasProxyVersion) {
            output.WriteRawTag(90);
            output.WriteString(ProxyVersion);
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
        if (HasAuthMethod) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) AuthMethod);
        }
        if (HasAuthMethodName) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(AuthMethodName);
        }
        if (HasAuthData) {
            size += 1 + pb::CodedOutputStream.ComputeBytesSize(AuthData);
        }
        if (HasProtocolVersion) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(ProtocolVersion);
        }
        if (HasProxyToBrokerUrl) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ProxyToBrokerUrl);
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
        if (featureFlags_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(FeatureFlags);
        }
        if (HasProxyVersion) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ProxyVersion);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandConnect other) {
        if (other == null) {
            return;
        }
        if (other.HasClientVersion) {
            ClientVersion = other.ClientVersion;
        }
        if (other.HasAuthMethod) {
            AuthMethod = other.AuthMethod;
        }
        if (other.HasAuthMethodName) {
            AuthMethodName = other.AuthMethodName;
        }
        if (other.HasAuthData) {
            AuthData = other.AuthData;
        }
        if (other.HasProtocolVersion) {
            ProtocolVersion = other.ProtocolVersion;
        }
        if (other.HasProxyToBrokerUrl) {
            ProxyToBrokerUrl = other.ProxyToBrokerUrl;
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
        if (other.featureFlags_ != null) {
            if (featureFlags_ == null) {
                FeatureFlags = new global::DotPulsar.Internal.PulsarApi.FeatureFlags();
            }
            FeatureFlags.MergeFrom(other.FeatureFlags);
        }
        if (other.HasProxyVersion) {
            ProxyVersion = other.ProxyVersion;
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
          case 16: {
            AuthMethod = (global::DotPulsar.Internal.PulsarApi.AuthMethod) input.ReadEnum();
            break;
          }
          case 26: {
            AuthData = input.ReadBytes();
            break;
          }
          case 32: {
            ProtocolVersion = input.ReadInt32();
            break;
          }
          case 42: {
            AuthMethodName = input.ReadString();
            break;
          }
          case 50: {
            ProxyToBrokerUrl = input.ReadString();
            break;
          }
          case 58: {
            OriginalPrincipal = input.ReadString();
            break;
          }
          case 66: {
            OriginalAuthData = input.ReadString();
            break;
          }
          case 74: {
            OriginalAuthMethod = input.ReadString();
            break;
          }
          case 82: {
            if (featureFlags_ == null) {
              FeatureFlags = new global::DotPulsar.Internal.PulsarApi.FeatureFlags();
            }
            input.ReadMessage(FeatureFlags);
            break;
          }
          case 90: {
            ProxyVersion = input.ReadString();
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
                case 16: {
                    AuthMethod = (global::DotPulsar.Internal.PulsarApi.AuthMethod) input.ReadEnum();
                    break;
                }
                case 26: {
                    AuthData = input.ReadBytes();
                    break;
                }
                case 32: {
                    ProtocolVersion = input.ReadInt32();
                    break;
                }
                case 42: {
                    AuthMethodName = input.ReadString();
                    break;
                }
                case 50: {
                    ProxyToBrokerUrl = input.ReadString();
                    break;
                }
                case 58: {
                    OriginalPrincipal = input.ReadString();
                    break;
                }
                case 66: {
                    OriginalAuthData = input.ReadString();
                    break;
                }
                case 74: {
                    OriginalAuthMethod = input.ReadString();
                    break;
                }
                case 82: {
                    if (featureFlags_ == null) {
                        FeatureFlags = new global::DotPulsar.Internal.PulsarApi.FeatureFlags();
                    }
                    input.ReadMessage(FeatureFlags);
                    break;
                }
                case 90: {
                    ProxyVersion = input.ReadString();
                    break;
                }
            }
        }
    }
#endif

}
