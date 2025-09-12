#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandConsumerStatsResponse : IMessage<CommandConsumerStatsResponse>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandConsumerStatsResponse> _parser = new pb::MessageParser<CommandConsumerStatsResponse>(() => new CommandConsumerStatsResponse());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandConsumerStatsResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[43]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandConsumerStatsResponse() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandConsumerStatsResponse(CommandConsumerStatsResponse other) : this() {
        _hasBits0 = other._hasBits0;
        requestId_ = other.requestId_;
        errorCode_ = other.errorCode_;
        errorMessage_ = other.errorMessage_;
        msgRateOut_ = other.msgRateOut_;
        msgThroughputOut_ = other.msgThroughputOut_;
        msgRateRedeliver_ = other.msgRateRedeliver_;
        consumerName_ = other.consumerName_;
        availablePermits_ = other.availablePermits_;
        unackedMessages_ = other.unackedMessages_;
        blockedConsumerOnUnackedMsgs_ = other.blockedConsumerOnUnackedMsgs_;
        address_ = other.address_;
        connectedSince_ = other.connectedSince_;
        type_ = other.type_;
        msgRateExpired_ = other.msgRateExpired_;
        msgBacklog_ = other.msgBacklog_;
        messageAckRate_ = other.messageAckRate_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandConsumerStatsResponse Clone() {
        return new CommandConsumerStatsResponse(this);
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

    /// <summary>Field number for the "error_code" field.</summary>
    public const int ErrorCodeFieldNumber = 2;
    private readonly static global::DotPulsar.Internal.PulsarApi.ServerError ErrorCodeDefaultValue = global::DotPulsar.Internal.PulsarApi.ServerError.UnknownError;

    private global::DotPulsar.Internal.PulsarApi.ServerError errorCode_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.ServerError ErrorCode {
        get { if ((_hasBits0 & 2) != 0) { return errorCode_; } else { return ErrorCodeDefaultValue; } }
        set {
            _hasBits0 |= 2;
            errorCode_ = value;
        }
    }
    /// <summary>Gets whether the "error_code" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasErrorCode {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "error_code" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearErrorCode() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "error_message" field.</summary>
    public const int ErrorMessageFieldNumber = 3;
    private readonly static string ErrorMessageDefaultValue = "";

    private string errorMessage_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ErrorMessage {
        get { return errorMessage_ ?? ErrorMessageDefaultValue; }
        set {
            errorMessage_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "error_message" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasErrorMessage {
        get { return errorMessage_ != null; }
    }
    /// <summary>Clears the value of the "error_message" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearErrorMessage() {
        errorMessage_ = null;
    }

    /// <summary>Field number for the "msgRateOut" field.</summary>
    public const int MsgRateOutFieldNumber = 4;
    private readonly static double MsgRateOutDefaultValue = 0D;

    private double msgRateOut_;
    /// <summary>
    ///&#x2F; Total rate of messages delivered to the consumer. msg/s
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public double MsgRateOut {
        get { if ((_hasBits0 & 4) != 0) { return msgRateOut_; } else { return MsgRateOutDefaultValue; } }
        set {
            _hasBits0 |= 4;
            msgRateOut_ = value;
        }
    }
    /// <summary>Gets whether the "msgRateOut" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMsgRateOut {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "msgRateOut" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMsgRateOut() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "msgThroughputOut" field.</summary>
    public const int MsgThroughputOutFieldNumber = 5;
    private readonly static double MsgThroughputOutDefaultValue = 0D;

    private double msgThroughputOut_;
    /// <summary>
    ///&#x2F; Total throughput delivered to the consumer. bytes/s
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public double MsgThroughputOut {
        get { if ((_hasBits0 & 8) != 0) { return msgThroughputOut_; } else { return MsgThroughputOutDefaultValue; } }
        set {
            _hasBits0 |= 8;
            msgThroughputOut_ = value;
        }
    }
    /// <summary>Gets whether the "msgThroughputOut" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMsgThroughputOut {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "msgThroughputOut" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMsgThroughputOut() {
        _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "msgRateRedeliver" field.</summary>
    public const int MsgRateRedeliverFieldNumber = 6;
    private readonly static double MsgRateRedeliverDefaultValue = 0D;

    private double msgRateRedeliver_;
    /// <summary>
    ///&#x2F; Total rate of messages redelivered by this consumer. msg/s
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public double MsgRateRedeliver {
        get { if ((_hasBits0 & 16) != 0) { return msgRateRedeliver_; } else { return MsgRateRedeliverDefaultValue; } }
        set {
            _hasBits0 |= 16;
            msgRateRedeliver_ = value;
        }
    }
    /// <summary>Gets whether the "msgRateRedeliver" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMsgRateRedeliver {
        get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "msgRateRedeliver" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMsgRateRedeliver() {
        _hasBits0 &= ~16;
    }

    /// <summary>Field number for the "consumerName" field.</summary>
    public const int ConsumerNameFieldNumber = 7;
    private readonly static string ConsumerNameDefaultValue = "";

    private string consumerName_;
    /// <summary>
    ///&#x2F; Name of the consumer
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ConsumerName {
        get { return consumerName_ ?? ConsumerNameDefaultValue; }
        set {
            consumerName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "consumerName" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasConsumerName {
        get { return consumerName_ != null; }
    }
    /// <summary>Clears the value of the "consumerName" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearConsumerName() {
        consumerName_ = null;
    }

    /// <summary>Field number for the "availablePermits" field.</summary>
    public const int AvailablePermitsFieldNumber = 8;
    private readonly static ulong AvailablePermitsDefaultValue = 0UL;

    private ulong availablePermits_;
    /// <summary>
    ///&#x2F; Number of available message permits for the consumer
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong AvailablePermits {
        get { if ((_hasBits0 & 32) != 0) { return availablePermits_; } else { return AvailablePermitsDefaultValue; } }
        set {
            _hasBits0 |= 32;
            availablePermits_ = value;
        }
    }
    /// <summary>Gets whether the "availablePermits" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasAvailablePermits {
        get { return (_hasBits0 & 32) != 0; }
    }
    /// <summary>Clears the value of the "availablePermits" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearAvailablePermits() {
        _hasBits0 &= ~32;
    }

    /// <summary>Field number for the "unackedMessages" field.</summary>
    public const int UnackedMessagesFieldNumber = 9;
    private readonly static ulong UnackedMessagesDefaultValue = 0UL;

    private ulong unackedMessages_;
    /// <summary>
    ///&#x2F; Number of unacknowledged messages for the consumer
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong UnackedMessages {
        get { if ((_hasBits0 & 64) != 0) { return unackedMessages_; } else { return UnackedMessagesDefaultValue; } }
        set {
            _hasBits0 |= 64;
            unackedMessages_ = value;
        }
    }
    /// <summary>Gets whether the "unackedMessages" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasUnackedMessages {
        get { return (_hasBits0 & 64) != 0; }
    }
    /// <summary>Clears the value of the "unackedMessages" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearUnackedMessages() {
        _hasBits0 &= ~64;
    }

    /// <summary>Field number for the "blockedConsumerOnUnackedMsgs" field.</summary>
    public const int BlockedConsumerOnUnackedMsgsFieldNumber = 10;
    private readonly static bool BlockedConsumerOnUnackedMsgsDefaultValue = false;

    private bool blockedConsumerOnUnackedMsgs_;
    /// <summary>
    ///&#x2F; Flag to verify if consumer is blocked due to reaching threshold of unacked messages
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool BlockedConsumerOnUnackedMsgs {
        get { if ((_hasBits0 & 128) != 0) { return blockedConsumerOnUnackedMsgs_; } else { return BlockedConsumerOnUnackedMsgsDefaultValue; } }
        set {
            _hasBits0 |= 128;
            blockedConsumerOnUnackedMsgs_ = value;
        }
    }
    /// <summary>Gets whether the "blockedConsumerOnUnackedMsgs" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasBlockedConsumerOnUnackedMsgs {
        get { return (_hasBits0 & 128) != 0; }
    }
    /// <summary>Clears the value of the "blockedConsumerOnUnackedMsgs" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearBlockedConsumerOnUnackedMsgs() {
        _hasBits0 &= ~128;
    }

    /// <summary>Field number for the "address" field.</summary>
    public const int AddressFieldNumber = 11;
    private readonly static string AddressDefaultValue = "";

    private string address_;
    /// <summary>
    ///&#x2F; Address of this consumer
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Address {
        get { return address_ ?? AddressDefaultValue; }
        set {
            address_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "address" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasAddress {
        get { return address_ != null; }
    }
    /// <summary>Clears the value of the "address" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearAddress() {
        address_ = null;
    }

    /// <summary>Field number for the "connectedSince" field.</summary>
    public const int ConnectedSinceFieldNumber = 12;
    private readonly static string ConnectedSinceDefaultValue = "";

    private string connectedSince_;
    /// <summary>
    ///&#x2F; Timestamp of connection
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ConnectedSince {
        get { return connectedSince_ ?? ConnectedSinceDefaultValue; }
        set {
            connectedSince_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "connectedSince" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasConnectedSince {
        get { return connectedSince_ != null; }
    }
    /// <summary>Clears the value of the "connectedSince" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearConnectedSince() {
        connectedSince_ = null;
    }

    /// <summary>Field number for the "type" field.</summary>
    public const int TypeFieldNumber = 13;
    private readonly static string TypeDefaultValue = "";

    private string type_;
    /// <summary>
    ///&#x2F; Whether this subscription is Exclusive or Shared or Failover
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Type {
        get { return type_ ?? TypeDefaultValue; }
        set {
            type_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "type" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasType {
        get { return type_ != null; }
    }
    /// <summary>Clears the value of the "type" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearType() {
        type_ = null;
    }

    /// <summary>Field number for the "msgRateExpired" field.</summary>
    public const int MsgRateExpiredFieldNumber = 14;
    private readonly static double MsgRateExpiredDefaultValue = 0D;

    private double msgRateExpired_;
    /// <summary>
    ///&#x2F; Total rate of messages expired on this subscription. msg/s
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public double MsgRateExpired {
        get { if ((_hasBits0 & 256) != 0) { return msgRateExpired_; } else { return MsgRateExpiredDefaultValue; } }
        set {
            _hasBits0 |= 256;
            msgRateExpired_ = value;
        }
    }
    /// <summary>Gets whether the "msgRateExpired" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMsgRateExpired {
        get { return (_hasBits0 & 256) != 0; }
    }
    /// <summary>Clears the value of the "msgRateExpired" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMsgRateExpired() {
        _hasBits0 &= ~256;
    }

    /// <summary>Field number for the "msgBacklog" field.</summary>
    public const int MsgBacklogFieldNumber = 15;
    private readonly static ulong MsgBacklogDefaultValue = 0UL;

    private ulong msgBacklog_;
    /// <summary>
    ///&#x2F; Number of messages in the subscription backlog
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong MsgBacklog {
        get { if ((_hasBits0 & 512) != 0) { return msgBacklog_; } else { return MsgBacklogDefaultValue; } }
        set {
            _hasBits0 |= 512;
            msgBacklog_ = value;
        }
    }
    /// <summary>Gets whether the "msgBacklog" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMsgBacklog {
        get { return (_hasBits0 & 512) != 0; }
    }
    /// <summary>Clears the value of the "msgBacklog" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMsgBacklog() {
        _hasBits0 &= ~512;
    }

    /// <summary>Field number for the "messageAckRate" field.</summary>
    public const int MessageAckRateFieldNumber = 16;
    private readonly static double MessageAckRateDefaultValue = 0D;

    private double messageAckRate_;
    /// <summary>
    ///&#x2F; Total rate of messages ack. msg/s
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public double MessageAckRate {
        get { if ((_hasBits0 & 1024) != 0) { return messageAckRate_; } else { return MessageAckRateDefaultValue; } }
        set {
            _hasBits0 |= 1024;
            messageAckRate_ = value;
        }
    }
    /// <summary>Gets whether the "messageAckRate" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMessageAckRate {
        get { return (_hasBits0 & 1024) != 0; }
    }
    /// <summary>Clears the value of the "messageAckRate" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMessageAckRate() {
        _hasBits0 &= ~1024;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandConsumerStatsResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandConsumerStatsResponse other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (RequestId != other.RequestId) return false;
        if (ErrorCode != other.ErrorCode) return false;
        if (ErrorMessage != other.ErrorMessage) return false;
        if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(MsgRateOut, other.MsgRateOut)) return false;
        if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(MsgThroughputOut, other.MsgThroughputOut)) return false;
        if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(MsgRateRedeliver, other.MsgRateRedeliver)) return false;
        if (ConsumerName != other.ConsumerName) return false;
        if (AvailablePermits != other.AvailablePermits) return false;
        if (UnackedMessages != other.UnackedMessages) return false;
        if (BlockedConsumerOnUnackedMsgs != other.BlockedConsumerOnUnackedMsgs) return false;
        if (Address != other.Address) return false;
        if (ConnectedSince != other.ConnectedSince) return false;
        if (Type != other.Type) return false;
        if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(MsgRateExpired, other.MsgRateExpired)) return false;
        if (MsgBacklog != other.MsgBacklog) return false;
        if (!pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(MessageAckRate, other.MessageAckRate)) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasErrorCode) hash ^= ErrorCode.GetHashCode();
        if (HasErrorMessage) hash ^= ErrorMessage.GetHashCode();
        if (HasMsgRateOut) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(MsgRateOut);
        if (HasMsgThroughputOut) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(MsgThroughputOut);
        if (HasMsgRateRedeliver) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(MsgRateRedeliver);
        if (HasConsumerName) hash ^= ConsumerName.GetHashCode();
        if (HasAvailablePermits) hash ^= AvailablePermits.GetHashCode();
        if (HasUnackedMessages) hash ^= UnackedMessages.GetHashCode();
        if (HasBlockedConsumerOnUnackedMsgs) hash ^= BlockedConsumerOnUnackedMsgs.GetHashCode();
        if (HasAddress) hash ^= Address.GetHashCode();
        if (HasConnectedSince) hash ^= ConnectedSince.GetHashCode();
        if (HasType) hash ^= Type.GetHashCode();
        if (HasMsgRateExpired) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(MsgRateExpired);
        if (HasMsgBacklog) hash ^= MsgBacklog.GetHashCode();
        if (HasMessageAckRate) hash ^= pbc::ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(MessageAckRate);
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
      if (HasErrorCode) {
        output.WriteRawTag(16);
        output.WriteEnum((int) ErrorCode);
      }
      if (HasErrorMessage) {
        output.WriteRawTag(26);
        output.WriteString(ErrorMessage);
      }
      if (HasMsgRateOut) {
        output.WriteRawTag(33);
        output.WriteDouble(MsgRateOut);
      }
      if (HasMsgThroughputOut) {
        output.WriteRawTag(41);
        output.WriteDouble(MsgThroughputOut);
      }
      if (HasMsgRateRedeliver) {
        output.WriteRawTag(49);
        output.WriteDouble(MsgRateRedeliver);
      }
      if (HasConsumerName) {
        output.WriteRawTag(58);
        output.WriteString(ConsumerName);
      }
      if (HasAvailablePermits) {
        output.WriteRawTag(64);
        output.WriteUInt64(AvailablePermits);
      }
      if (HasUnackedMessages) {
        output.WriteRawTag(72);
        output.WriteUInt64(UnackedMessages);
      }
      if (HasBlockedConsumerOnUnackedMsgs) {
        output.WriteRawTag(80);
        output.WriteBool(BlockedConsumerOnUnackedMsgs);
      }
      if (HasAddress) {
        output.WriteRawTag(90);
        output.WriteString(Address);
      }
      if (HasConnectedSince) {
        output.WriteRawTag(98);
        output.WriteString(ConnectedSince);
      }
      if (HasType) {
        output.WriteRawTag(106);
        output.WriteString(Type);
      }
      if (HasMsgRateExpired) {
        output.WriteRawTag(113);
        output.WriteDouble(MsgRateExpired);
      }
      if (HasMsgBacklog) {
        output.WriteRawTag(120);
        output.WriteUInt64(MsgBacklog);
      }
      if (HasMessageAckRate) {
        output.WriteRawTag(129, 1);
        output.WriteDouble(MessageAckRate);
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
        if (HasErrorCode) {
            output.WriteRawTag(16);
            output.WriteEnum((int) ErrorCode);
        }
        if (HasErrorMessage) {
            output.WriteRawTag(26);
            output.WriteString(ErrorMessage);
        }
        if (HasMsgRateOut) {
            output.WriteRawTag(33);
            output.WriteDouble(MsgRateOut);
        }
        if (HasMsgThroughputOut) {
            output.WriteRawTag(41);
            output.WriteDouble(MsgThroughputOut);
        }
        if (HasMsgRateRedeliver) {
            output.WriteRawTag(49);
            output.WriteDouble(MsgRateRedeliver);
        }
        if (HasConsumerName) {
            output.WriteRawTag(58);
            output.WriteString(ConsumerName);
        }
        if (HasAvailablePermits) {
            output.WriteRawTag(64);
            output.WriteUInt64(AvailablePermits);
        }
        if (HasUnackedMessages) {
            output.WriteRawTag(72);
            output.WriteUInt64(UnackedMessages);
        }
        if (HasBlockedConsumerOnUnackedMsgs) {
            output.WriteRawTag(80);
            output.WriteBool(BlockedConsumerOnUnackedMsgs);
        }
        if (HasAddress) {
            output.WriteRawTag(90);
            output.WriteString(Address);
        }
        if (HasConnectedSince) {
            output.WriteRawTag(98);
            output.WriteString(ConnectedSince);
        }
        if (HasType) {
            output.WriteRawTag(106);
            output.WriteString(Type);
        }
        if (HasMsgRateExpired) {
            output.WriteRawTag(113);
            output.WriteDouble(MsgRateExpired);
        }
        if (HasMsgBacklog) {
            output.WriteRawTag(120);
            output.WriteUInt64(MsgBacklog);
        }
        if (HasMessageAckRate) {
            output.WriteRawTag(129, 1);
            output.WriteDouble(MessageAckRate);
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
        if (HasErrorCode) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ErrorCode);
        }
        if (HasErrorMessage) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ErrorMessage);
        }
        if (HasMsgRateOut) {
            size += 1 + 8;
        }
        if (HasMsgThroughputOut) {
            size += 1 + 8;
        }
        if (HasMsgRateRedeliver) {
            size += 1 + 8;
        }
        if (HasConsumerName) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ConsumerName);
        }
        if (HasAvailablePermits) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(AvailablePermits);
        }
        if (HasUnackedMessages) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(UnackedMessages);
        }
        if (HasBlockedConsumerOnUnackedMsgs) {
            size += 1 + 1;
        }
        if (HasAddress) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Address);
        }
        if (HasConnectedSince) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ConnectedSince);
        }
        if (HasType) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Type);
        }
        if (HasMsgRateExpired) {
            size += 1 + 8;
        }
        if (HasMsgBacklog) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(MsgBacklog);
        }
        if (HasMessageAckRate) {
            size += 2 + 8;
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandConsumerStatsResponse other) {
        if (other == null) {
            return;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.HasErrorCode) {
            ErrorCode = other.ErrorCode;
        }
        if (other.HasErrorMessage) {
            ErrorMessage = other.ErrorMessage;
        }
        if (other.HasMsgRateOut) {
            MsgRateOut = other.MsgRateOut;
        }
        if (other.HasMsgThroughputOut) {
            MsgThroughputOut = other.MsgThroughputOut;
        }
        if (other.HasMsgRateRedeliver) {
            MsgRateRedeliver = other.MsgRateRedeliver;
        }
        if (other.HasConsumerName) {
            ConsumerName = other.ConsumerName;
        }
        if (other.HasAvailablePermits) {
            AvailablePermits = other.AvailablePermits;
        }
        if (other.HasUnackedMessages) {
            UnackedMessages = other.UnackedMessages;
        }
        if (other.HasBlockedConsumerOnUnackedMsgs) {
            BlockedConsumerOnUnackedMsgs = other.BlockedConsumerOnUnackedMsgs;
        }
        if (other.HasAddress) {
            Address = other.Address;
        }
        if (other.HasConnectedSince) {
            ConnectedSince = other.ConnectedSince;
        }
        if (other.HasType) {
            Type = other.Type;
        }
        if (other.HasMsgRateExpired) {
            MsgRateExpired = other.MsgRateExpired;
        }
        if (other.HasMsgBacklog) {
            MsgBacklog = other.MsgBacklog;
        }
        if (other.HasMessageAckRate) {
            MessageAckRate = other.MessageAckRate;
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
            ErrorCode = (global::DotPulsar.Internal.PulsarApi.ServerError) input.ReadEnum();
            break;
          }
          case 26: {
            ErrorMessage = input.ReadString();
            break;
          }
          case 33: {
            MsgRateOut = input.ReadDouble();
            break;
          }
          case 41: {
            MsgThroughputOut = input.ReadDouble();
            break;
          }
          case 49: {
            MsgRateRedeliver = input.ReadDouble();
            break;
          }
          case 58: {
            ConsumerName = input.ReadString();
            break;
          }
          case 64: {
            AvailablePermits = input.ReadUInt64();
            break;
          }
          case 72: {
            UnackedMessages = input.ReadUInt64();
            break;
          }
          case 80: {
            BlockedConsumerOnUnackedMsgs = input.ReadBool();
            break;
          }
          case 90: {
            Address = input.ReadString();
            break;
          }
          case 98: {
            ConnectedSince = input.ReadString();
            break;
          }
          case 106: {
            Type = input.ReadString();
            break;
          }
          case 113: {
            MsgRateExpired = input.ReadDouble();
            break;
          }
          case 120: {
            MsgBacklog = input.ReadUInt64();
            break;
          }
          case 129: {
            MessageAckRate = input.ReadDouble();
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
                    ErrorCode = (global::DotPulsar.Internal.PulsarApi.ServerError) input.ReadEnum();
                    break;
                }
                case 26: {
                    ErrorMessage = input.ReadString();
                    break;
                }
                case 33: {
                    MsgRateOut = input.ReadDouble();
                    break;
                }
                case 41: {
                    MsgThroughputOut = input.ReadDouble();
                    break;
                }
                case 49: {
                    MsgRateRedeliver = input.ReadDouble();
                    break;
                }
                case 58: {
                    ConsumerName = input.ReadString();
                    break;
                }
                case 64: {
                    AvailablePermits = input.ReadUInt64();
                    break;
                }
                case 72: {
                    UnackedMessages = input.ReadUInt64();
                    break;
                }
                case 80: {
                    BlockedConsumerOnUnackedMsgs = input.ReadBool();
                    break;
                }
                case 90: {
                    Address = input.ReadString();
                    break;
                }
                case 98: {
                    ConnectedSince = input.ReadString();
                    break;
                }
                case 106: {
                    Type = input.ReadString();
                    break;
                }
                case 113: {
                    MsgRateExpired = input.ReadDouble();
                    break;
                }
                case 120: {
                    MsgBacklog = input.ReadUInt64();
                    break;
                }
                case 129: {
                    MessageAckRate = input.ReadDouble();
                    break;
                }
            }
        }
    }
#endif

}
