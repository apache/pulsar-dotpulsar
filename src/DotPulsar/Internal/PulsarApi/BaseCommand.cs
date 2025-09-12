#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class BaseCommand : IMessage<BaseCommand>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<BaseCommand> _parser = new pb::MessageParser<BaseCommand>(() => new BaseCommand());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<BaseCommand> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[71]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public BaseCommand() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public BaseCommand(BaseCommand other) : this() {
        _hasBits0 = other._hasBits0;
        type_ = other.type_;
        connect_ = other.connect_ != null ? other.connect_.Clone() : null;
        connected_ = other.connected_ != null ? other.connected_.Clone() : null;
        subscribe_ = other.subscribe_ != null ? other.subscribe_.Clone() : null;
        producer_ = other.producer_ != null ? other.producer_.Clone() : null;
        send_ = other.send_ != null ? other.send_.Clone() : null;
        sendReceipt_ = other.sendReceipt_ != null ? other.sendReceipt_.Clone() : null;
        sendError_ = other.sendError_ != null ? other.sendError_.Clone() : null;
        message_ = other.message_ != null ? other.message_.Clone() : null;
        ack_ = other.ack_ != null ? other.ack_.Clone() : null;
        flow_ = other.flow_ != null ? other.flow_.Clone() : null;
        unsubscribe_ = other.unsubscribe_ != null ? other.unsubscribe_.Clone() : null;
        success_ = other.success_ != null ? other.success_.Clone() : null;
        error_ = other.error_ != null ? other.error_.Clone() : null;
        closeProducer_ = other.closeProducer_ != null ? other.closeProducer_.Clone() : null;
        closeConsumer_ = other.closeConsumer_ != null ? other.closeConsumer_.Clone() : null;
        producerSuccess_ = other.producerSuccess_ != null ? other.producerSuccess_.Clone() : null;
        ping_ = other.ping_ != null ? other.ping_.Clone() : null;
        pong_ = other.pong_ != null ? other.pong_.Clone() : null;
        redeliverUnacknowledgedMessages_ = other.redeliverUnacknowledgedMessages_ != null ? other.redeliverUnacknowledgedMessages_.Clone() : null;
        partitionMetadata_ = other.partitionMetadata_ != null ? other.partitionMetadata_.Clone() : null;
        partitionMetadataResponse_ = other.partitionMetadataResponse_ != null ? other.partitionMetadataResponse_.Clone() : null;
        lookupTopic_ = other.lookupTopic_ != null ? other.lookupTopic_.Clone() : null;
        lookupTopicResponse_ = other.lookupTopicResponse_ != null ? other.lookupTopicResponse_.Clone() : null;
        consumerStats_ = other.consumerStats_ != null ? other.consumerStats_.Clone() : null;
        consumerStatsResponse_ = other.consumerStatsResponse_ != null ? other.consumerStatsResponse_.Clone() : null;
        reachedEndOfTopic_ = other.reachedEndOfTopic_ != null ? other.reachedEndOfTopic_.Clone() : null;
        seek_ = other.seek_ != null ? other.seek_.Clone() : null;
        getLastMessageId_ = other.getLastMessageId_ != null ? other.getLastMessageId_.Clone() : null;
        getLastMessageIdResponse_ = other.getLastMessageIdResponse_ != null ? other.getLastMessageIdResponse_.Clone() : null;
        activeConsumerChange_ = other.activeConsumerChange_ != null ? other.activeConsumerChange_.Clone() : null;
        getTopicsOfNamespace_ = other.getTopicsOfNamespace_ != null ? other.getTopicsOfNamespace_.Clone() : null;
        getTopicsOfNamespaceResponse_ = other.getTopicsOfNamespaceResponse_ != null ? other.getTopicsOfNamespaceResponse_.Clone() : null;
        getSchema_ = other.getSchema_ != null ? other.getSchema_.Clone() : null;
        getSchemaResponse_ = other.getSchemaResponse_ != null ? other.getSchemaResponse_.Clone() : null;
        authChallenge_ = other.authChallenge_ != null ? other.authChallenge_.Clone() : null;
        authResponse_ = other.authResponse_ != null ? other.authResponse_.Clone() : null;
        ackResponse_ = other.ackResponse_ != null ? other.ackResponse_.Clone() : null;
        getOrCreateSchema_ = other.getOrCreateSchema_ != null ? other.getOrCreateSchema_.Clone() : null;
        getOrCreateSchemaResponse_ = other.getOrCreateSchemaResponse_ != null ? other.getOrCreateSchemaResponse_.Clone() : null;
        newTxn_ = other.newTxn_ != null ? other.newTxn_.Clone() : null;
        newTxnResponse_ = other.newTxnResponse_ != null ? other.newTxnResponse_.Clone() : null;
        addPartitionToTxn_ = other.addPartitionToTxn_ != null ? other.addPartitionToTxn_.Clone() : null;
        addPartitionToTxnResponse_ = other.addPartitionToTxnResponse_ != null ? other.addPartitionToTxnResponse_.Clone() : null;
        addSubscriptionToTxn_ = other.addSubscriptionToTxn_ != null ? other.addSubscriptionToTxn_.Clone() : null;
        addSubscriptionToTxnResponse_ = other.addSubscriptionToTxnResponse_ != null ? other.addSubscriptionToTxnResponse_.Clone() : null;
        endTxn_ = other.endTxn_ != null ? other.endTxn_.Clone() : null;
        endTxnResponse_ = other.endTxnResponse_ != null ? other.endTxnResponse_.Clone() : null;
        endTxnOnPartition_ = other.endTxnOnPartition_ != null ? other.endTxnOnPartition_.Clone() : null;
        endTxnOnPartitionResponse_ = other.endTxnOnPartitionResponse_ != null ? other.endTxnOnPartitionResponse_.Clone() : null;
        endTxnOnSubscription_ = other.endTxnOnSubscription_ != null ? other.endTxnOnSubscription_.Clone() : null;
        endTxnOnSubscriptionResponse_ = other.endTxnOnSubscriptionResponse_ != null ? other.endTxnOnSubscriptionResponse_.Clone() : null;
        tcClientConnectRequest_ = other.tcClientConnectRequest_ != null ? other.tcClientConnectRequest_.Clone() : null;
        tcClientConnectResponse_ = other.tcClientConnectResponse_ != null ? other.tcClientConnectResponse_.Clone() : null;
        watchTopicList_ = other.watchTopicList_ != null ? other.watchTopicList_.Clone() : null;
        watchTopicListSuccess_ = other.watchTopicListSuccess_ != null ? other.watchTopicListSuccess_.Clone() : null;
        watchTopicUpdate_ = other.watchTopicUpdate_ != null ? other.watchTopicUpdate_.Clone() : null;
        watchTopicListClose_ = other.watchTopicListClose_ != null ? other.watchTopicListClose_.Clone() : null;
        topicMigrated_ = other.topicMigrated_ != null ? other.topicMigrated_.Clone() : null;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public BaseCommand Clone() {
        return new BaseCommand(this);
    }

    /// <summary>Field number for the "type" field.</summary>
    public const int TypeFieldNumber = 1;
    private readonly static global::DotPulsar.Internal.PulsarApi.BaseCommand.Types.Type TypeDefaultValue = global::DotPulsar.Internal.PulsarApi.BaseCommand.Types.Type.Connect;

    private global::DotPulsar.Internal.PulsarApi.BaseCommand.Types.Type type_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.BaseCommand.Types.Type Type {
        get { if ((_hasBits0 & 1) != 0) { return type_; } else { return TypeDefaultValue; } }
        set {
            _hasBits0 |= 1;
            type_ = value;
        }
    }
    /// <summary>Gets whether the "type" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasType {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "type" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearType() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "connect" field.</summary>
    public const int ConnectFieldNumber = 2;
    private global::DotPulsar.Internal.PulsarApi.CommandConnect connect_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandConnect Connect {
        get { return connect_; }
        set {
            connect_ = value;
        }
    }

    /// <summary>Field number for the "connected" field.</summary>
    public const int ConnectedFieldNumber = 3;
    private global::DotPulsar.Internal.PulsarApi.CommandConnected connected_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandConnected Connected {
        get { return connected_; }
        set {
            connected_ = value;
        }
    }

    /// <summary>Field number for the "subscribe" field.</summary>
    public const int SubscribeFieldNumber = 4;
    private global::DotPulsar.Internal.PulsarApi.CommandSubscribe subscribe_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandSubscribe Subscribe {
        get { return subscribe_; }
        set {
            subscribe_ = value;
        }
    }

    /// <summary>Field number for the "producer" field.</summary>
    public const int ProducerFieldNumber = 5;
    private global::DotPulsar.Internal.PulsarApi.CommandProducer producer_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandProducer Producer {
        get { return producer_; }
        set {
            producer_ = value;
        }
    }

    /// <summary>Field number for the "send" field.</summary>
    public const int SendFieldNumber = 6;
    private global::DotPulsar.Internal.PulsarApi.CommandSend send_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandSend Send {
        get { return send_; }
        set {
            send_ = value;
        }
    }

    /// <summary>Field number for the "send_receipt" field.</summary>
    public const int SendReceiptFieldNumber = 7;
    private global::DotPulsar.Internal.PulsarApi.CommandSendReceipt sendReceipt_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandSendReceipt SendReceipt {
        get { return sendReceipt_; }
        set {
            sendReceipt_ = value;
        }
    }

    /// <summary>Field number for the "send_error" field.</summary>
    public const int SendErrorFieldNumber = 8;
    private global::DotPulsar.Internal.PulsarApi.CommandSendError sendError_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandSendError SendError {
        get { return sendError_; }
        set {
            sendError_ = value;
        }
    }

    /// <summary>Field number for the "message" field.</summary>
    public const int MessageFieldNumber = 9;
    private global::DotPulsar.Internal.PulsarApi.CommandMessage message_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandMessage Message {
        get { return message_; }
        set {
            message_ = value;
        }
    }

    /// <summary>Field number for the "ack" field.</summary>
    public const int AckFieldNumber = 10;
    private global::DotPulsar.Internal.PulsarApi.CommandAck ack_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandAck Ack {
        get { return ack_; }
        set {
            ack_ = value;
        }
    }

    /// <summary>Field number for the "flow" field.</summary>
    public const int FlowFieldNumber = 11;
    private global::DotPulsar.Internal.PulsarApi.CommandFlow flow_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandFlow Flow {
        get { return flow_; }
        set {
            flow_ = value;
        }
    }

    /// <summary>Field number for the "unsubscribe" field.</summary>
    public const int UnsubscribeFieldNumber = 12;
    private global::DotPulsar.Internal.PulsarApi.CommandUnsubscribe unsubscribe_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandUnsubscribe Unsubscribe {
        get { return unsubscribe_; }
        set {
            unsubscribe_ = value;
        }
    }

    /// <summary>Field number for the "success" field.</summary>
    public const int SuccessFieldNumber = 13;
    private global::DotPulsar.Internal.PulsarApi.CommandSuccess success_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandSuccess Success {
        get { return success_; }
        set {
            success_ = value;
        }
    }

    /// <summary>Field number for the "error" field.</summary>
    public const int ErrorFieldNumber = 14;
    private global::DotPulsar.Internal.PulsarApi.CommandError error_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandError Error {
        get { return error_; }
        set {
            error_ = value;
        }
    }

    /// <summary>Field number for the "close_producer" field.</summary>
    public const int CloseProducerFieldNumber = 15;
    private global::DotPulsar.Internal.PulsarApi.CommandCloseProducer closeProducer_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandCloseProducer CloseProducer {
        get { return closeProducer_; }
        set {
            closeProducer_ = value;
        }
    }

    /// <summary>Field number for the "close_consumer" field.</summary>
    public const int CloseConsumerFieldNumber = 16;
    private global::DotPulsar.Internal.PulsarApi.CommandCloseConsumer closeConsumer_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandCloseConsumer CloseConsumer {
        get { return closeConsumer_; }
        set {
            closeConsumer_ = value;
        }
    }

    /// <summary>Field number for the "producer_success" field.</summary>
    public const int ProducerSuccessFieldNumber = 17;
    private global::DotPulsar.Internal.PulsarApi.CommandProducerSuccess producerSuccess_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandProducerSuccess ProducerSuccess {
        get { return producerSuccess_; }
        set {
            producerSuccess_ = value;
        }
    }

    /// <summary>Field number for the "ping" field.</summary>
    public const int PingFieldNumber = 18;
    private global::DotPulsar.Internal.PulsarApi.CommandPing ping_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandPing Ping {
        get { return ping_; }
        set {
            ping_ = value;
        }
    }

    /// <summary>Field number for the "pong" field.</summary>
    public const int PongFieldNumber = 19;
    private global::DotPulsar.Internal.PulsarApi.CommandPong pong_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandPong Pong {
        get { return pong_; }
        set {
            pong_ = value;
        }
    }

    /// <summary>Field number for the "redeliverUnacknowledgedMessages" field.</summary>
    public const int RedeliverUnacknowledgedMessagesFieldNumber = 20;
    private global::DotPulsar.Internal.PulsarApi.CommandRedeliverUnacknowledgedMessages redeliverUnacknowledgedMessages_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandRedeliverUnacknowledgedMessages RedeliverUnacknowledgedMessages {
        get { return redeliverUnacknowledgedMessages_; }
        set {
            redeliverUnacknowledgedMessages_ = value;
        }
    }

    /// <summary>Field number for the "partitionMetadata" field.</summary>
    public const int PartitionMetadataFieldNumber = 21;
    private global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadata partitionMetadata_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadata PartitionMetadata {
        get { return partitionMetadata_; }
        set {
            partitionMetadata_ = value;
        }
    }

    /// <summary>Field number for the "partitionMetadataResponse" field.</summary>
    public const int PartitionMetadataResponseFieldNumber = 22;
    private global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse partitionMetadataResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse PartitionMetadataResponse {
        get { return partitionMetadataResponse_; }
        set {
            partitionMetadataResponse_ = value;
        }
    }

    /// <summary>Field number for the "lookupTopic" field.</summary>
    public const int LookupTopicFieldNumber = 23;
    private global::DotPulsar.Internal.PulsarApi.CommandLookupTopic lookupTopic_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandLookupTopic LookupTopic {
        get { return lookupTopic_; }
        set {
            lookupTopic_ = value;
        }
    }

    /// <summary>Field number for the "lookupTopicResponse" field.</summary>
    public const int LookupTopicResponseFieldNumber = 24;
    private global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse lookupTopicResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse LookupTopicResponse {
        get { return lookupTopicResponse_; }
        set {
            lookupTopicResponse_ = value;
        }
    }

    /// <summary>Field number for the "consumerStats" field.</summary>
    public const int ConsumerStatsFieldNumber = 25;
    private global::DotPulsar.Internal.PulsarApi.CommandConsumerStats consumerStats_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandConsumerStats ConsumerStats {
        get { return consumerStats_; }
        set {
            consumerStats_ = value;
        }
    }

    /// <summary>Field number for the "consumerStatsResponse" field.</summary>
    public const int ConsumerStatsResponseFieldNumber = 26;
    private global::DotPulsar.Internal.PulsarApi.CommandConsumerStatsResponse consumerStatsResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandConsumerStatsResponse ConsumerStatsResponse {
        get { return consumerStatsResponse_; }
        set {
            consumerStatsResponse_ = value;
        }
    }

    /// <summary>Field number for the "reachedEndOfTopic" field.</summary>
    public const int ReachedEndOfTopicFieldNumber = 27;
    private global::DotPulsar.Internal.PulsarApi.CommandReachedEndOfTopic reachedEndOfTopic_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandReachedEndOfTopic ReachedEndOfTopic {
        get { return reachedEndOfTopic_; }
        set {
            reachedEndOfTopic_ = value;
        }
    }

    /// <summary>Field number for the "seek" field.</summary>
    public const int SeekFieldNumber = 28;
    private global::DotPulsar.Internal.PulsarApi.CommandSeek seek_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandSeek Seek {
        get { return seek_; }
        set {
            seek_ = value;
        }
    }

    /// <summary>Field number for the "getLastMessageId" field.</summary>
    public const int GetLastMessageIdFieldNumber = 29;
    private global::DotPulsar.Internal.PulsarApi.CommandGetLastMessageId getLastMessageId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandGetLastMessageId GetLastMessageId {
        get { return getLastMessageId_; }
        set {
            getLastMessageId_ = value;
        }
    }

    /// <summary>Field number for the "getLastMessageIdResponse" field.</summary>
    public const int GetLastMessageIdResponseFieldNumber = 30;
    private global::DotPulsar.Internal.PulsarApi.CommandGetLastMessageIdResponse getLastMessageIdResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandGetLastMessageIdResponse GetLastMessageIdResponse {
        get { return getLastMessageIdResponse_; }
        set {
            getLastMessageIdResponse_ = value;
        }
    }

    /// <summary>Field number for the "active_consumer_change" field.</summary>
    public const int ActiveConsumerChangeFieldNumber = 31;
    private global::DotPulsar.Internal.PulsarApi.CommandActiveConsumerChange activeConsumerChange_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandActiveConsumerChange ActiveConsumerChange {
        get { return activeConsumerChange_; }
        set {
            activeConsumerChange_ = value;
        }
    }

    /// <summary>Field number for the "getTopicsOfNamespace" field.</summary>
    public const int GetTopicsOfNamespaceFieldNumber = 32;
    private global::DotPulsar.Internal.PulsarApi.CommandGetTopicsOfNamespace getTopicsOfNamespace_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandGetTopicsOfNamespace GetTopicsOfNamespace {
        get { return getTopicsOfNamespace_; }
        set {
            getTopicsOfNamespace_ = value;
        }
    }

    /// <summary>Field number for the "getTopicsOfNamespaceResponse" field.</summary>
    public const int GetTopicsOfNamespaceResponseFieldNumber = 33;
    private global::DotPulsar.Internal.PulsarApi.CommandGetTopicsOfNamespaceResponse getTopicsOfNamespaceResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandGetTopicsOfNamespaceResponse GetTopicsOfNamespaceResponse {
        get { return getTopicsOfNamespaceResponse_; }
        set {
            getTopicsOfNamespaceResponse_ = value;
        }
    }

    /// <summary>Field number for the "getSchema" field.</summary>
    public const int GetSchemaFieldNumber = 34;
    private global::DotPulsar.Internal.PulsarApi.CommandGetSchema getSchema_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandGetSchema GetSchema {
        get { return getSchema_; }
        set {
            getSchema_ = value;
        }
    }

    /// <summary>Field number for the "getSchemaResponse" field.</summary>
    public const int GetSchemaResponseFieldNumber = 35;
    private global::DotPulsar.Internal.PulsarApi.CommandGetSchemaResponse getSchemaResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandGetSchemaResponse GetSchemaResponse {
        get { return getSchemaResponse_; }
        set {
            getSchemaResponse_ = value;
        }
    }

    /// <summary>Field number for the "authChallenge" field.</summary>
    public const int AuthChallengeFieldNumber = 36;
    private global::DotPulsar.Internal.PulsarApi.CommandAuthChallenge authChallenge_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandAuthChallenge AuthChallenge {
        get { return authChallenge_; }
        set {
            authChallenge_ = value;
        }
    }

    /// <summary>Field number for the "authResponse" field.</summary>
    public const int AuthResponseFieldNumber = 37;
    private global::DotPulsar.Internal.PulsarApi.CommandAuthResponse authResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandAuthResponse AuthResponse {
        get { return authResponse_; }
        set {
            authResponse_ = value;
        }
    }

    /// <summary>Field number for the "ackResponse" field.</summary>
    public const int AckResponseFieldNumber = 38;
    private global::DotPulsar.Internal.PulsarApi.CommandAckResponse ackResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandAckResponse AckResponse {
        get { return ackResponse_; }
        set {
            ackResponse_ = value;
        }
    }

    /// <summary>Field number for the "getOrCreateSchema" field.</summary>
    public const int GetOrCreateSchemaFieldNumber = 39;
    private global::DotPulsar.Internal.PulsarApi.CommandGetOrCreateSchema getOrCreateSchema_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandGetOrCreateSchema GetOrCreateSchema {
        get { return getOrCreateSchema_; }
        set {
            getOrCreateSchema_ = value;
        }
    }

    /// <summary>Field number for the "getOrCreateSchemaResponse" field.</summary>
    public const int GetOrCreateSchemaResponseFieldNumber = 40;
    private global::DotPulsar.Internal.PulsarApi.CommandGetOrCreateSchemaResponse getOrCreateSchemaResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandGetOrCreateSchemaResponse GetOrCreateSchemaResponse {
        get { return getOrCreateSchemaResponse_; }
        set {
            getOrCreateSchemaResponse_ = value;
        }
    }

    /// <summary>Field number for the "newTxn" field.</summary>
    public const int NewTxnFieldNumber = 50;
    private global::DotPulsar.Internal.PulsarApi.CommandNewTxn newTxn_;
    /// <summary>
    /// transaction related
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandNewTxn NewTxn {
        get { return newTxn_; }
        set {
            newTxn_ = value;
        }
    }

    /// <summary>Field number for the "newTxnResponse" field.</summary>
    public const int NewTxnResponseFieldNumber = 51;
    private global::DotPulsar.Internal.PulsarApi.CommandNewTxnResponse newTxnResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandNewTxnResponse NewTxnResponse {
        get { return newTxnResponse_; }
        set {
            newTxnResponse_ = value;
        }
    }

    /// <summary>Field number for the "addPartitionToTxn" field.</summary>
    public const int AddPartitionToTxnFieldNumber = 52;
    private global::DotPulsar.Internal.PulsarApi.CommandAddPartitionToTxn addPartitionToTxn_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandAddPartitionToTxn AddPartitionToTxn {
        get { return addPartitionToTxn_; }
        set {
            addPartitionToTxn_ = value;
        }
    }

    /// <summary>Field number for the "addPartitionToTxnResponse" field.</summary>
    public const int AddPartitionToTxnResponseFieldNumber = 53;
    private global::DotPulsar.Internal.PulsarApi.CommandAddPartitionToTxnResponse addPartitionToTxnResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandAddPartitionToTxnResponse AddPartitionToTxnResponse {
        get { return addPartitionToTxnResponse_; }
        set {
            addPartitionToTxnResponse_ = value;
        }
    }

    /// <summary>Field number for the "addSubscriptionToTxn" field.</summary>
    public const int AddSubscriptionToTxnFieldNumber = 54;
    private global::DotPulsar.Internal.PulsarApi.CommandAddSubscriptionToTxn addSubscriptionToTxn_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandAddSubscriptionToTxn AddSubscriptionToTxn {
        get { return addSubscriptionToTxn_; }
        set {
            addSubscriptionToTxn_ = value;
        }
    }

    /// <summary>Field number for the "addSubscriptionToTxnResponse" field.</summary>
    public const int AddSubscriptionToTxnResponseFieldNumber = 55;
    private global::DotPulsar.Internal.PulsarApi.CommandAddSubscriptionToTxnResponse addSubscriptionToTxnResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandAddSubscriptionToTxnResponse AddSubscriptionToTxnResponse {
        get { return addSubscriptionToTxnResponse_; }
        set {
            addSubscriptionToTxnResponse_ = value;
        }
    }

    /// <summary>Field number for the "endTxn" field.</summary>
    public const int EndTxnFieldNumber = 56;
    private global::DotPulsar.Internal.PulsarApi.CommandEndTxn endTxn_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandEndTxn EndTxn {
        get { return endTxn_; }
        set {
            endTxn_ = value;
        }
    }

    /// <summary>Field number for the "endTxnResponse" field.</summary>
    public const int EndTxnResponseFieldNumber = 57;
    private global::DotPulsar.Internal.PulsarApi.CommandEndTxnResponse endTxnResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandEndTxnResponse EndTxnResponse {
        get { return endTxnResponse_; }
        set {
            endTxnResponse_ = value;
        }
    }

    /// <summary>Field number for the "endTxnOnPartition" field.</summary>
    public const int EndTxnOnPartitionFieldNumber = 58;
    private global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnPartition endTxnOnPartition_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnPartition EndTxnOnPartition {
        get { return endTxnOnPartition_; }
        set {
            endTxnOnPartition_ = value;
        }
    }

    /// <summary>Field number for the "endTxnOnPartitionResponse" field.</summary>
    public const int EndTxnOnPartitionResponseFieldNumber = 59;
    private global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnPartitionResponse endTxnOnPartitionResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnPartitionResponse EndTxnOnPartitionResponse {
        get { return endTxnOnPartitionResponse_; }
        set {
            endTxnOnPartitionResponse_ = value;
        }
    }

    /// <summary>Field number for the "endTxnOnSubscription" field.</summary>
    public const int EndTxnOnSubscriptionFieldNumber = 60;
    private global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnSubscription endTxnOnSubscription_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnSubscription EndTxnOnSubscription {
        get { return endTxnOnSubscription_; }
        set {
            endTxnOnSubscription_ = value;
        }
    }

    /// <summary>Field number for the "endTxnOnSubscriptionResponse" field.</summary>
    public const int EndTxnOnSubscriptionResponseFieldNumber = 61;
    private global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnSubscriptionResponse endTxnOnSubscriptionResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnSubscriptionResponse EndTxnOnSubscriptionResponse {
        get { return endTxnOnSubscriptionResponse_; }
        set {
            endTxnOnSubscriptionResponse_ = value;
        }
    }

    /// <summary>Field number for the "tcClientConnectRequest" field.</summary>
    public const int TcClientConnectRequestFieldNumber = 62;
    private global::DotPulsar.Internal.PulsarApi.CommandTcClientConnectRequest tcClientConnectRequest_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandTcClientConnectRequest TcClientConnectRequest {
        get { return tcClientConnectRequest_; }
        set {
            tcClientConnectRequest_ = value;
        }
    }

    /// <summary>Field number for the "tcClientConnectResponse" field.</summary>
    public const int TcClientConnectResponseFieldNumber = 63;
    private global::DotPulsar.Internal.PulsarApi.CommandTcClientConnectResponse tcClientConnectResponse_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandTcClientConnectResponse TcClientConnectResponse {
        get { return tcClientConnectResponse_; }
        set {
            tcClientConnectResponse_ = value;
        }
    }

    /// <summary>Field number for the "watchTopicList" field.</summary>
    public const int WatchTopicListFieldNumber = 64;
    private global::DotPulsar.Internal.PulsarApi.CommandWatchTopicList watchTopicList_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandWatchTopicList WatchTopicList {
        get { return watchTopicList_; }
        set {
            watchTopicList_ = value;
        }
    }

    /// <summary>Field number for the "watchTopicListSuccess" field.</summary>
    public const int WatchTopicListSuccessFieldNumber = 65;
    private global::DotPulsar.Internal.PulsarApi.CommandWatchTopicListSuccess watchTopicListSuccess_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandWatchTopicListSuccess WatchTopicListSuccess {
        get { return watchTopicListSuccess_; }
        set {
            watchTopicListSuccess_ = value;
        }
    }

    /// <summary>Field number for the "watchTopicUpdate" field.</summary>
    public const int WatchTopicUpdateFieldNumber = 66;
    private global::DotPulsar.Internal.PulsarApi.CommandWatchTopicUpdate watchTopicUpdate_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandWatchTopicUpdate WatchTopicUpdate {
        get { return watchTopicUpdate_; }
        set {
            watchTopicUpdate_ = value;
        }
    }

    /// <summary>Field number for the "watchTopicListClose" field.</summary>
    public const int WatchTopicListCloseFieldNumber = 67;
    private global::DotPulsar.Internal.PulsarApi.CommandWatchTopicListClose watchTopicListClose_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandWatchTopicListClose WatchTopicListClose {
        get { return watchTopicListClose_; }
        set {
            watchTopicListClose_ = value;
        }
    }

    /// <summary>Field number for the "topicMigrated" field.</summary>
    public const int TopicMigratedFieldNumber = 68;
    private global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated topicMigrated_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated TopicMigrated {
        get { return topicMigrated_; }
        set {
            topicMigrated_ = value;
        }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as BaseCommand);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(BaseCommand other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (Type != other.Type) return false;
        if (!object.Equals(Connect, other.Connect)) return false;
        if (!object.Equals(Connected, other.Connected)) return false;
        if (!object.Equals(Subscribe, other.Subscribe)) return false;
        if (!object.Equals(Producer, other.Producer)) return false;
        if (!object.Equals(Send, other.Send)) return false;
        if (!object.Equals(SendReceipt, other.SendReceipt)) return false;
        if (!object.Equals(SendError, other.SendError)) return false;
        if (!object.Equals(Message, other.Message)) return false;
        if (!object.Equals(Ack, other.Ack)) return false;
        if (!object.Equals(Flow, other.Flow)) return false;
        if (!object.Equals(Unsubscribe, other.Unsubscribe)) return false;
        if (!object.Equals(Success, other.Success)) return false;
        if (!object.Equals(Error, other.Error)) return false;
        if (!object.Equals(CloseProducer, other.CloseProducer)) return false;
        if (!object.Equals(CloseConsumer, other.CloseConsumer)) return false;
        if (!object.Equals(ProducerSuccess, other.ProducerSuccess)) return false;
        if (!object.Equals(Ping, other.Ping)) return false;
        if (!object.Equals(Pong, other.Pong)) return false;
        if (!object.Equals(RedeliverUnacknowledgedMessages, other.RedeliverUnacknowledgedMessages)) return false;
        if (!object.Equals(PartitionMetadata, other.PartitionMetadata)) return false;
        if (!object.Equals(PartitionMetadataResponse, other.PartitionMetadataResponse)) return false;
        if (!object.Equals(LookupTopic, other.LookupTopic)) return false;
        if (!object.Equals(LookupTopicResponse, other.LookupTopicResponse)) return false;
        if (!object.Equals(ConsumerStats, other.ConsumerStats)) return false;
        if (!object.Equals(ConsumerStatsResponse, other.ConsumerStatsResponse)) return false;
        if (!object.Equals(ReachedEndOfTopic, other.ReachedEndOfTopic)) return false;
        if (!object.Equals(Seek, other.Seek)) return false;
        if (!object.Equals(GetLastMessageId, other.GetLastMessageId)) return false;
        if (!object.Equals(GetLastMessageIdResponse, other.GetLastMessageIdResponse)) return false;
        if (!object.Equals(ActiveConsumerChange, other.ActiveConsumerChange)) return false;
        if (!object.Equals(GetTopicsOfNamespace, other.GetTopicsOfNamespace)) return false;
        if (!object.Equals(GetTopicsOfNamespaceResponse, other.GetTopicsOfNamespaceResponse)) return false;
        if (!object.Equals(GetSchema, other.GetSchema)) return false;
        if (!object.Equals(GetSchemaResponse, other.GetSchemaResponse)) return false;
        if (!object.Equals(AuthChallenge, other.AuthChallenge)) return false;
        if (!object.Equals(AuthResponse, other.AuthResponse)) return false;
        if (!object.Equals(AckResponse, other.AckResponse)) return false;
        if (!object.Equals(GetOrCreateSchema, other.GetOrCreateSchema)) return false;
        if (!object.Equals(GetOrCreateSchemaResponse, other.GetOrCreateSchemaResponse)) return false;
        if (!object.Equals(NewTxn, other.NewTxn)) return false;
        if (!object.Equals(NewTxnResponse, other.NewTxnResponse)) return false;
        if (!object.Equals(AddPartitionToTxn, other.AddPartitionToTxn)) return false;
        if (!object.Equals(AddPartitionToTxnResponse, other.AddPartitionToTxnResponse)) return false;
        if (!object.Equals(AddSubscriptionToTxn, other.AddSubscriptionToTxn)) return false;
        if (!object.Equals(AddSubscriptionToTxnResponse, other.AddSubscriptionToTxnResponse)) return false;
        if (!object.Equals(EndTxn, other.EndTxn)) return false;
        if (!object.Equals(EndTxnResponse, other.EndTxnResponse)) return false;
        if (!object.Equals(EndTxnOnPartition, other.EndTxnOnPartition)) return false;
        if (!object.Equals(EndTxnOnPartitionResponse, other.EndTxnOnPartitionResponse)) return false;
        if (!object.Equals(EndTxnOnSubscription, other.EndTxnOnSubscription)) return false;
        if (!object.Equals(EndTxnOnSubscriptionResponse, other.EndTxnOnSubscriptionResponse)) return false;
        if (!object.Equals(TcClientConnectRequest, other.TcClientConnectRequest)) return false;
        if (!object.Equals(TcClientConnectResponse, other.TcClientConnectResponse)) return false;
        if (!object.Equals(WatchTopicList, other.WatchTopicList)) return false;
        if (!object.Equals(WatchTopicListSuccess, other.WatchTopicListSuccess)) return false;
        if (!object.Equals(WatchTopicUpdate, other.WatchTopicUpdate)) return false;
        if (!object.Equals(WatchTopicListClose, other.WatchTopicListClose)) return false;
        if (!object.Equals(TopicMigrated, other.TopicMigrated)) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasType) hash ^= Type.GetHashCode();
        if (connect_ != null) hash ^= Connect.GetHashCode();
        if (connected_ != null) hash ^= Connected.GetHashCode();
        if (subscribe_ != null) hash ^= Subscribe.GetHashCode();
        if (producer_ != null) hash ^= Producer.GetHashCode();
        if (send_ != null) hash ^= Send.GetHashCode();
        if (sendReceipt_ != null) hash ^= SendReceipt.GetHashCode();
        if (sendError_ != null) hash ^= SendError.GetHashCode();
        if (message_ != null) hash ^= Message.GetHashCode();
        if (ack_ != null) hash ^= Ack.GetHashCode();
        if (flow_ != null) hash ^= Flow.GetHashCode();
        if (unsubscribe_ != null) hash ^= Unsubscribe.GetHashCode();
        if (success_ != null) hash ^= Success.GetHashCode();
        if (error_ != null) hash ^= Error.GetHashCode();
        if (closeProducer_ != null) hash ^= CloseProducer.GetHashCode();
        if (closeConsumer_ != null) hash ^= CloseConsumer.GetHashCode();
        if (producerSuccess_ != null) hash ^= ProducerSuccess.GetHashCode();
        if (ping_ != null) hash ^= Ping.GetHashCode();
        if (pong_ != null) hash ^= Pong.GetHashCode();
        if (redeliverUnacknowledgedMessages_ != null) hash ^= RedeliverUnacknowledgedMessages.GetHashCode();
        if (partitionMetadata_ != null) hash ^= PartitionMetadata.GetHashCode();
        if (partitionMetadataResponse_ != null) hash ^= PartitionMetadataResponse.GetHashCode();
        if (lookupTopic_ != null) hash ^= LookupTopic.GetHashCode();
        if (lookupTopicResponse_ != null) hash ^= LookupTopicResponse.GetHashCode();
        if (consumerStats_ != null) hash ^= ConsumerStats.GetHashCode();
        if (consumerStatsResponse_ != null) hash ^= ConsumerStatsResponse.GetHashCode();
        if (reachedEndOfTopic_ != null) hash ^= ReachedEndOfTopic.GetHashCode();
        if (seek_ != null) hash ^= Seek.GetHashCode();
        if (getLastMessageId_ != null) hash ^= GetLastMessageId.GetHashCode();
        if (getLastMessageIdResponse_ != null) hash ^= GetLastMessageIdResponse.GetHashCode();
        if (activeConsumerChange_ != null) hash ^= ActiveConsumerChange.GetHashCode();
        if (getTopicsOfNamespace_ != null) hash ^= GetTopicsOfNamespace.GetHashCode();
        if (getTopicsOfNamespaceResponse_ != null) hash ^= GetTopicsOfNamespaceResponse.GetHashCode();
        if (getSchema_ != null) hash ^= GetSchema.GetHashCode();
        if (getSchemaResponse_ != null) hash ^= GetSchemaResponse.GetHashCode();
        if (authChallenge_ != null) hash ^= AuthChallenge.GetHashCode();
        if (authResponse_ != null) hash ^= AuthResponse.GetHashCode();
        if (ackResponse_ != null) hash ^= AckResponse.GetHashCode();
        if (getOrCreateSchema_ != null) hash ^= GetOrCreateSchema.GetHashCode();
        if (getOrCreateSchemaResponse_ != null) hash ^= GetOrCreateSchemaResponse.GetHashCode();
        if (newTxn_ != null) hash ^= NewTxn.GetHashCode();
        if (newTxnResponse_ != null) hash ^= NewTxnResponse.GetHashCode();
        if (addPartitionToTxn_ != null) hash ^= AddPartitionToTxn.GetHashCode();
        if (addPartitionToTxnResponse_ != null) hash ^= AddPartitionToTxnResponse.GetHashCode();
        if (addSubscriptionToTxn_ != null) hash ^= AddSubscriptionToTxn.GetHashCode();
        if (addSubscriptionToTxnResponse_ != null) hash ^= AddSubscriptionToTxnResponse.GetHashCode();
        if (endTxn_ != null) hash ^= EndTxn.GetHashCode();
        if (endTxnResponse_ != null) hash ^= EndTxnResponse.GetHashCode();
        if (endTxnOnPartition_ != null) hash ^= EndTxnOnPartition.GetHashCode();
        if (endTxnOnPartitionResponse_ != null) hash ^= EndTxnOnPartitionResponse.GetHashCode();
        if (endTxnOnSubscription_ != null) hash ^= EndTxnOnSubscription.GetHashCode();
        if (endTxnOnSubscriptionResponse_ != null) hash ^= EndTxnOnSubscriptionResponse.GetHashCode();
        if (tcClientConnectRequest_ != null) hash ^= TcClientConnectRequest.GetHashCode();
        if (tcClientConnectResponse_ != null) hash ^= TcClientConnectResponse.GetHashCode();
        if (watchTopicList_ != null) hash ^= WatchTopicList.GetHashCode();
        if (watchTopicListSuccess_ != null) hash ^= WatchTopicListSuccess.GetHashCode();
        if (watchTopicUpdate_ != null) hash ^= WatchTopicUpdate.GetHashCode();
        if (watchTopicListClose_ != null) hash ^= WatchTopicListClose.GetHashCode();
        if (topicMigrated_ != null) hash ^= TopicMigrated.GetHashCode();
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
      if (HasType) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Type);
      }
      if (connect_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Connect);
      }
      if (connected_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(Connected);
      }
      if (subscribe_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Subscribe);
      }
      if (producer_ != null) {
        output.WriteRawTag(42);
        output.WriteMessage(Producer);
      }
      if (send_ != null) {
        output.WriteRawTag(50);
        output.WriteMessage(Send);
      }
      if (sendReceipt_ != null) {
        output.WriteRawTag(58);
        output.WriteMessage(SendReceipt);
      }
      if (sendError_ != null) {
        output.WriteRawTag(66);
        output.WriteMessage(SendError);
      }
      if (message_ != null) {
        output.WriteRawTag(74);
        output.WriteMessage(Message);
      }
      if (ack_ != null) {
        output.WriteRawTag(82);
        output.WriteMessage(Ack);
      }
      if (flow_ != null) {
        output.WriteRawTag(90);
        output.WriteMessage(Flow);
      }
      if (unsubscribe_ != null) {
        output.WriteRawTag(98);
        output.WriteMessage(Unsubscribe);
      }
      if (success_ != null) {
        output.WriteRawTag(106);
        output.WriteMessage(Success);
      }
      if (error_ != null) {
        output.WriteRawTag(114);
        output.WriteMessage(Error);
      }
      if (closeProducer_ != null) {
        output.WriteRawTag(122);
        output.WriteMessage(CloseProducer);
      }
      if (closeConsumer_ != null) {
        output.WriteRawTag(130, 1);
        output.WriteMessage(CloseConsumer);
      }
      if (producerSuccess_ != null) {
        output.WriteRawTag(138, 1);
        output.WriteMessage(ProducerSuccess);
      }
      if (ping_ != null) {
        output.WriteRawTag(146, 1);
        output.WriteMessage(Ping);
      }
      if (pong_ != null) {
        output.WriteRawTag(154, 1);
        output.WriteMessage(Pong);
      }
      if (redeliverUnacknowledgedMessages_ != null) {
        output.WriteRawTag(162, 1);
        output.WriteMessage(RedeliverUnacknowledgedMessages);
      }
      if (partitionMetadata_ != null) {
        output.WriteRawTag(170, 1);
        output.WriteMessage(PartitionMetadata);
      }
      if (partitionMetadataResponse_ != null) {
        output.WriteRawTag(178, 1);
        output.WriteMessage(PartitionMetadataResponse);
      }
      if (lookupTopic_ != null) {
        output.WriteRawTag(186, 1);
        output.WriteMessage(LookupTopic);
      }
      if (lookupTopicResponse_ != null) {
        output.WriteRawTag(194, 1);
        output.WriteMessage(LookupTopicResponse);
      }
      if (consumerStats_ != null) {
        output.WriteRawTag(202, 1);
        output.WriteMessage(ConsumerStats);
      }
      if (consumerStatsResponse_ != null) {
        output.WriteRawTag(210, 1);
        output.WriteMessage(ConsumerStatsResponse);
      }
      if (reachedEndOfTopic_ != null) {
        output.WriteRawTag(218, 1);
        output.WriteMessage(ReachedEndOfTopic);
      }
      if (seek_ != null) {
        output.WriteRawTag(226, 1);
        output.WriteMessage(Seek);
      }
      if (getLastMessageId_ != null) {
        output.WriteRawTag(234, 1);
        output.WriteMessage(GetLastMessageId);
      }
      if (getLastMessageIdResponse_ != null) {
        output.WriteRawTag(242, 1);
        output.WriteMessage(GetLastMessageIdResponse);
      }
      if (activeConsumerChange_ != null) {
        output.WriteRawTag(250, 1);
        output.WriteMessage(ActiveConsumerChange);
      }
      if (getTopicsOfNamespace_ != null) {
        output.WriteRawTag(130, 2);
        output.WriteMessage(GetTopicsOfNamespace);
      }
      if (getTopicsOfNamespaceResponse_ != null) {
        output.WriteRawTag(138, 2);
        output.WriteMessage(GetTopicsOfNamespaceResponse);
      }
      if (getSchema_ != null) {
        output.WriteRawTag(146, 2);
        output.WriteMessage(GetSchema);
      }
      if (getSchemaResponse_ != null) {
        output.WriteRawTag(154, 2);
        output.WriteMessage(GetSchemaResponse);
      }
      if (authChallenge_ != null) {
        output.WriteRawTag(162, 2);
        output.WriteMessage(AuthChallenge);
      }
      if (authResponse_ != null) {
        output.WriteRawTag(170, 2);
        output.WriteMessage(AuthResponse);
      }
      if (ackResponse_ != null) {
        output.WriteRawTag(178, 2);
        output.WriteMessage(AckResponse);
      }
      if (getOrCreateSchema_ != null) {
        output.WriteRawTag(186, 2);
        output.WriteMessage(GetOrCreateSchema);
      }
      if (getOrCreateSchemaResponse_ != null) {
        output.WriteRawTag(194, 2);
        output.WriteMessage(GetOrCreateSchemaResponse);
      }
      if (newTxn_ != null) {
        output.WriteRawTag(146, 3);
        output.WriteMessage(NewTxn);
      }
      if (newTxnResponse_ != null) {
        output.WriteRawTag(154, 3);
        output.WriteMessage(NewTxnResponse);
      }
      if (addPartitionToTxn_ != null) {
        output.WriteRawTag(162, 3);
        output.WriteMessage(AddPartitionToTxn);
      }
      if (addPartitionToTxnResponse_ != null) {
        output.WriteRawTag(170, 3);
        output.WriteMessage(AddPartitionToTxnResponse);
      }
      if (addSubscriptionToTxn_ != null) {
        output.WriteRawTag(178, 3);
        output.WriteMessage(AddSubscriptionToTxn);
      }
      if (addSubscriptionToTxnResponse_ != null) {
        output.WriteRawTag(186, 3);
        output.WriteMessage(AddSubscriptionToTxnResponse);
      }
      if (endTxn_ != null) {
        output.WriteRawTag(194, 3);
        output.WriteMessage(EndTxn);
      }
      if (endTxnResponse_ != null) {
        output.WriteRawTag(202, 3);
        output.WriteMessage(EndTxnResponse);
      }
      if (endTxnOnPartition_ != null) {
        output.WriteRawTag(210, 3);
        output.WriteMessage(EndTxnOnPartition);
      }
      if (endTxnOnPartitionResponse_ != null) {
        output.WriteRawTag(218, 3);
        output.WriteMessage(EndTxnOnPartitionResponse);
      }
      if (endTxnOnSubscription_ != null) {
        output.WriteRawTag(226, 3);
        output.WriteMessage(EndTxnOnSubscription);
      }
      if (endTxnOnSubscriptionResponse_ != null) {
        output.WriteRawTag(234, 3);
        output.WriteMessage(EndTxnOnSubscriptionResponse);
      }
      if (tcClientConnectRequest_ != null) {
        output.WriteRawTag(242, 3);
        output.WriteMessage(TcClientConnectRequest);
      }
      if (tcClientConnectResponse_ != null) {
        output.WriteRawTag(250, 3);
        output.WriteMessage(TcClientConnectResponse);
      }
      if (watchTopicList_ != null) {
        output.WriteRawTag(130, 4);
        output.WriteMessage(WatchTopicList);
      }
      if (watchTopicListSuccess_ != null) {
        output.WriteRawTag(138, 4);
        output.WriteMessage(WatchTopicListSuccess);
      }
      if (watchTopicUpdate_ != null) {
        output.WriteRawTag(146, 4);
        output.WriteMessage(WatchTopicUpdate);
      }
      if (watchTopicListClose_ != null) {
        output.WriteRawTag(154, 4);
        output.WriteMessage(WatchTopicListClose);
      }
      if (topicMigrated_ != null) {
        output.WriteRawTag(162, 4);
        output.WriteMessage(TopicMigrated);
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
        if (HasType) {
            output.WriteRawTag(8);
            output.WriteEnum((int) Type);
        }
        if (connect_ != null) {
            output.WriteRawTag(18);
            output.WriteMessage(Connect);
        }
        if (connected_ != null) {
            output.WriteRawTag(26);
            output.WriteMessage(Connected);
        }
        if (subscribe_ != null) {
            output.WriteRawTag(34);
            output.WriteMessage(Subscribe);
        }
        if (producer_ != null) {
            output.WriteRawTag(42);
            output.WriteMessage(Producer);
        }
        if (send_ != null) {
            output.WriteRawTag(50);
            output.WriteMessage(Send);
        }
        if (sendReceipt_ != null) {
            output.WriteRawTag(58);
            output.WriteMessage(SendReceipt);
        }
        if (sendError_ != null) {
            output.WriteRawTag(66);
            output.WriteMessage(SendError);
        }
        if (message_ != null) {
            output.WriteRawTag(74);
            output.WriteMessage(Message);
        }
        if (ack_ != null) {
            output.WriteRawTag(82);
            output.WriteMessage(Ack);
        }
        if (flow_ != null) {
            output.WriteRawTag(90);
            output.WriteMessage(Flow);
        }
        if (unsubscribe_ != null) {
            output.WriteRawTag(98);
            output.WriteMessage(Unsubscribe);
        }
        if (success_ != null) {
            output.WriteRawTag(106);
            output.WriteMessage(Success);
        }
        if (error_ != null) {
            output.WriteRawTag(114);
            output.WriteMessage(Error);
        }
        if (closeProducer_ != null) {
            output.WriteRawTag(122);
            output.WriteMessage(CloseProducer);
        }
        if (closeConsumer_ != null) {
            output.WriteRawTag(130, 1);
            output.WriteMessage(CloseConsumer);
        }
        if (producerSuccess_ != null) {
            output.WriteRawTag(138, 1);
            output.WriteMessage(ProducerSuccess);
        }
        if (ping_ != null) {
            output.WriteRawTag(146, 1);
            output.WriteMessage(Ping);
        }
        if (pong_ != null) {
            output.WriteRawTag(154, 1);
            output.WriteMessage(Pong);
        }
        if (redeliverUnacknowledgedMessages_ != null) {
            output.WriteRawTag(162, 1);
            output.WriteMessage(RedeliverUnacknowledgedMessages);
        }
        if (partitionMetadata_ != null) {
            output.WriteRawTag(170, 1);
            output.WriteMessage(PartitionMetadata);
        }
        if (partitionMetadataResponse_ != null) {
            output.WriteRawTag(178, 1);
            output.WriteMessage(PartitionMetadataResponse);
        }
        if (lookupTopic_ != null) {
            output.WriteRawTag(186, 1);
            output.WriteMessage(LookupTopic);
        }
        if (lookupTopicResponse_ != null) {
            output.WriteRawTag(194, 1);
            output.WriteMessage(LookupTopicResponse);
        }
        if (consumerStats_ != null) {
            output.WriteRawTag(202, 1);
            output.WriteMessage(ConsumerStats);
        }
        if (consumerStatsResponse_ != null) {
            output.WriteRawTag(210, 1);
            output.WriteMessage(ConsumerStatsResponse);
        }
        if (reachedEndOfTopic_ != null) {
            output.WriteRawTag(218, 1);
            output.WriteMessage(ReachedEndOfTopic);
        }
        if (seek_ != null) {
            output.WriteRawTag(226, 1);
            output.WriteMessage(Seek);
        }
        if (getLastMessageId_ != null) {
            output.WriteRawTag(234, 1);
            output.WriteMessage(GetLastMessageId);
        }
        if (getLastMessageIdResponse_ != null) {
            output.WriteRawTag(242, 1);
            output.WriteMessage(GetLastMessageIdResponse);
        }
        if (activeConsumerChange_ != null) {
            output.WriteRawTag(250, 1);
            output.WriteMessage(ActiveConsumerChange);
        }
        if (getTopicsOfNamespace_ != null) {
            output.WriteRawTag(130, 2);
            output.WriteMessage(GetTopicsOfNamespace);
        }
        if (getTopicsOfNamespaceResponse_ != null) {
            output.WriteRawTag(138, 2);
            output.WriteMessage(GetTopicsOfNamespaceResponse);
        }
        if (getSchema_ != null) {
            output.WriteRawTag(146, 2);
            output.WriteMessage(GetSchema);
        }
        if (getSchemaResponse_ != null) {
            output.WriteRawTag(154, 2);
            output.WriteMessage(GetSchemaResponse);
        }
        if (authChallenge_ != null) {
            output.WriteRawTag(162, 2);
            output.WriteMessage(AuthChallenge);
        }
        if (authResponse_ != null) {
            output.WriteRawTag(170, 2);
            output.WriteMessage(AuthResponse);
        }
        if (ackResponse_ != null) {
            output.WriteRawTag(178, 2);
            output.WriteMessage(AckResponse);
        }
        if (getOrCreateSchema_ != null) {
            output.WriteRawTag(186, 2);
            output.WriteMessage(GetOrCreateSchema);
        }
        if (getOrCreateSchemaResponse_ != null) {
            output.WriteRawTag(194, 2);
            output.WriteMessage(GetOrCreateSchemaResponse);
        }
        if (newTxn_ != null) {
            output.WriteRawTag(146, 3);
            output.WriteMessage(NewTxn);
        }
        if (newTxnResponse_ != null) {
            output.WriteRawTag(154, 3);
            output.WriteMessage(NewTxnResponse);
        }
        if (addPartitionToTxn_ != null) {
            output.WriteRawTag(162, 3);
            output.WriteMessage(AddPartitionToTxn);
        }
        if (addPartitionToTxnResponse_ != null) {
            output.WriteRawTag(170, 3);
            output.WriteMessage(AddPartitionToTxnResponse);
        }
        if (addSubscriptionToTxn_ != null) {
            output.WriteRawTag(178, 3);
            output.WriteMessage(AddSubscriptionToTxn);
        }
        if (addSubscriptionToTxnResponse_ != null) {
            output.WriteRawTag(186, 3);
            output.WriteMessage(AddSubscriptionToTxnResponse);
        }
        if (endTxn_ != null) {
            output.WriteRawTag(194, 3);
            output.WriteMessage(EndTxn);
        }
        if (endTxnResponse_ != null) {
            output.WriteRawTag(202, 3);
            output.WriteMessage(EndTxnResponse);
        }
        if (endTxnOnPartition_ != null) {
            output.WriteRawTag(210, 3);
            output.WriteMessage(EndTxnOnPartition);
        }
        if (endTxnOnPartitionResponse_ != null) {
            output.WriteRawTag(218, 3);
            output.WriteMessage(EndTxnOnPartitionResponse);
        }
        if (endTxnOnSubscription_ != null) {
            output.WriteRawTag(226, 3);
            output.WriteMessage(EndTxnOnSubscription);
        }
        if (endTxnOnSubscriptionResponse_ != null) {
            output.WriteRawTag(234, 3);
            output.WriteMessage(EndTxnOnSubscriptionResponse);
        }
        if (tcClientConnectRequest_ != null) {
            output.WriteRawTag(242, 3);
            output.WriteMessage(TcClientConnectRequest);
        }
        if (tcClientConnectResponse_ != null) {
            output.WriteRawTag(250, 3);
            output.WriteMessage(TcClientConnectResponse);
        }
        if (watchTopicList_ != null) {
            output.WriteRawTag(130, 4);
            output.WriteMessage(WatchTopicList);
        }
        if (watchTopicListSuccess_ != null) {
            output.WriteRawTag(138, 4);
            output.WriteMessage(WatchTopicListSuccess);
        }
        if (watchTopicUpdate_ != null) {
            output.WriteRawTag(146, 4);
            output.WriteMessage(WatchTopicUpdate);
        }
        if (watchTopicListClose_ != null) {
            output.WriteRawTag(154, 4);
            output.WriteMessage(WatchTopicListClose);
        }
        if (topicMigrated_ != null) {
            output.WriteRawTag(162, 4);
            output.WriteMessage(TopicMigrated);
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
        if (HasType) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Type);
        }
        if (connect_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Connect);
        }
        if (connected_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Connected);
        }
        if (subscribe_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Subscribe);
        }
        if (producer_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Producer);
        }
        if (send_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Send);
        }
        if (sendReceipt_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(SendReceipt);
        }
        if (sendError_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(SendError);
        }
        if (message_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Message);
        }
        if (ack_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Ack);
        }
        if (flow_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Flow);
        }
        if (unsubscribe_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Unsubscribe);
        }
        if (success_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Success);
        }
        if (error_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Error);
        }
        if (closeProducer_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(CloseProducer);
        }
        if (closeConsumer_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(CloseConsumer);
        }
        if (producerSuccess_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(ProducerSuccess);
        }
        if (ping_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(Ping);
        }
        if (pong_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(Pong);
        }
        if (redeliverUnacknowledgedMessages_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(RedeliverUnacknowledgedMessages);
        }
        if (partitionMetadata_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(PartitionMetadata);
        }
        if (partitionMetadataResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(PartitionMetadataResponse);
        }
        if (lookupTopic_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(LookupTopic);
        }
        if (lookupTopicResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(LookupTopicResponse);
        }
        if (consumerStats_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(ConsumerStats);
        }
        if (consumerStatsResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(ConsumerStatsResponse);
        }
        if (reachedEndOfTopic_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(ReachedEndOfTopic);
        }
        if (seek_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(Seek);
        }
        if (getLastMessageId_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(GetLastMessageId);
        }
        if (getLastMessageIdResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(GetLastMessageIdResponse);
        }
        if (activeConsumerChange_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(ActiveConsumerChange);
        }
        if (getTopicsOfNamespace_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(GetTopicsOfNamespace);
        }
        if (getTopicsOfNamespaceResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(GetTopicsOfNamespaceResponse);
        }
        if (getSchema_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(GetSchema);
        }
        if (getSchemaResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(GetSchemaResponse);
        }
        if (authChallenge_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(AuthChallenge);
        }
        if (authResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(AuthResponse);
        }
        if (ackResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(AckResponse);
        }
        if (getOrCreateSchema_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(GetOrCreateSchema);
        }
        if (getOrCreateSchemaResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(GetOrCreateSchemaResponse);
        }
        if (newTxn_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(NewTxn);
        }
        if (newTxnResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(NewTxnResponse);
        }
        if (addPartitionToTxn_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(AddPartitionToTxn);
        }
        if (addPartitionToTxnResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(AddPartitionToTxnResponse);
        }
        if (addSubscriptionToTxn_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(AddSubscriptionToTxn);
        }
        if (addSubscriptionToTxnResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(AddSubscriptionToTxnResponse);
        }
        if (endTxn_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(EndTxn);
        }
        if (endTxnResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(EndTxnResponse);
        }
        if (endTxnOnPartition_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(EndTxnOnPartition);
        }
        if (endTxnOnPartitionResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(EndTxnOnPartitionResponse);
        }
        if (endTxnOnSubscription_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(EndTxnOnSubscription);
        }
        if (endTxnOnSubscriptionResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(EndTxnOnSubscriptionResponse);
        }
        if (tcClientConnectRequest_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(TcClientConnectRequest);
        }
        if (tcClientConnectResponse_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(TcClientConnectResponse);
        }
        if (watchTopicList_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(WatchTopicList);
        }
        if (watchTopicListSuccess_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(WatchTopicListSuccess);
        }
        if (watchTopicUpdate_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(WatchTopicUpdate);
        }
        if (watchTopicListClose_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(WatchTopicListClose);
        }
        if (topicMigrated_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(TopicMigrated);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(BaseCommand other) {
        if (other == null) {
            return;
        }
        if (other.HasType) {
            Type = other.Type;
        }
        if (other.connect_ != null) {
            if (connect_ == null) {
                Connect = new global::DotPulsar.Internal.PulsarApi.CommandConnect();
            }
            Connect.MergeFrom(other.Connect);
        }
        if (other.connected_ != null) {
            if (connected_ == null) {
                Connected = new global::DotPulsar.Internal.PulsarApi.CommandConnected();
            }
            Connected.MergeFrom(other.Connected);
        }
        if (other.subscribe_ != null) {
            if (subscribe_ == null) {
                Subscribe = new global::DotPulsar.Internal.PulsarApi.CommandSubscribe();
            }
            Subscribe.MergeFrom(other.Subscribe);
        }
        if (other.producer_ != null) {
            if (producer_ == null) {
                Producer = new global::DotPulsar.Internal.PulsarApi.CommandProducer();
            }
            Producer.MergeFrom(other.Producer);
        }
        if (other.send_ != null) {
            if (send_ == null) {
                Send = new global::DotPulsar.Internal.PulsarApi.CommandSend();
            }
            Send.MergeFrom(other.Send);
        }
        if (other.sendReceipt_ != null) {
            if (sendReceipt_ == null) {
                SendReceipt = new global::DotPulsar.Internal.PulsarApi.CommandSendReceipt();
            }
            SendReceipt.MergeFrom(other.SendReceipt);
        }
        if (other.sendError_ != null) {
            if (sendError_ == null) {
                SendError = new global::DotPulsar.Internal.PulsarApi.CommandSendError();
            }
            SendError.MergeFrom(other.SendError);
        }
        if (other.message_ != null) {
            if (message_ == null) {
                Message = new global::DotPulsar.Internal.PulsarApi.CommandMessage();
            }
            Message.MergeFrom(other.Message);
        }
        if (other.ack_ != null) {
            if (ack_ == null) {
                Ack = new global::DotPulsar.Internal.PulsarApi.CommandAck();
            }
            Ack.MergeFrom(other.Ack);
        }
        if (other.flow_ != null) {
            if (flow_ == null) {
                Flow = new global::DotPulsar.Internal.PulsarApi.CommandFlow();
            }
            Flow.MergeFrom(other.Flow);
        }
        if (other.unsubscribe_ != null) {
            if (unsubscribe_ == null) {
                Unsubscribe = new global::DotPulsar.Internal.PulsarApi.CommandUnsubscribe();
            }
            Unsubscribe.MergeFrom(other.Unsubscribe);
        }
        if (other.success_ != null) {
            if (success_ == null) {
                Success = new global::DotPulsar.Internal.PulsarApi.CommandSuccess();
            }
            Success.MergeFrom(other.Success);
        }
        if (other.error_ != null) {
            if (error_ == null) {
                Error = new global::DotPulsar.Internal.PulsarApi.CommandError();
            }
            Error.MergeFrom(other.Error);
        }
        if (other.closeProducer_ != null) {
            if (closeProducer_ == null) {
                CloseProducer = new global::DotPulsar.Internal.PulsarApi.CommandCloseProducer();
            }
            CloseProducer.MergeFrom(other.CloseProducer);
        }
        if (other.closeConsumer_ != null) {
            if (closeConsumer_ == null) {
                CloseConsumer = new global::DotPulsar.Internal.PulsarApi.CommandCloseConsumer();
            }
            CloseConsumer.MergeFrom(other.CloseConsumer);
        }
        if (other.producerSuccess_ != null) {
            if (producerSuccess_ == null) {
                ProducerSuccess = new global::DotPulsar.Internal.PulsarApi.CommandProducerSuccess();
            }
            ProducerSuccess.MergeFrom(other.ProducerSuccess);
        }
        if (other.ping_ != null) {
            if (ping_ == null) {
                Ping = new global::DotPulsar.Internal.PulsarApi.CommandPing();
            }
            Ping.MergeFrom(other.Ping);
        }
        if (other.pong_ != null) {
            if (pong_ == null) {
                Pong = new global::DotPulsar.Internal.PulsarApi.CommandPong();
            }
            Pong.MergeFrom(other.Pong);
        }
        if (other.redeliverUnacknowledgedMessages_ != null) {
            if (redeliverUnacknowledgedMessages_ == null) {
                RedeliverUnacknowledgedMessages = new global::DotPulsar.Internal.PulsarApi.CommandRedeliverUnacknowledgedMessages();
            }
            RedeliverUnacknowledgedMessages.MergeFrom(other.RedeliverUnacknowledgedMessages);
        }
        if (other.partitionMetadata_ != null) {
            if (partitionMetadata_ == null) {
                PartitionMetadata = new global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadata();
            }
            PartitionMetadata.MergeFrom(other.PartitionMetadata);
        }
        if (other.partitionMetadataResponse_ != null) {
            if (partitionMetadataResponse_ == null) {
                PartitionMetadataResponse = new global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse();
            }
            PartitionMetadataResponse.MergeFrom(other.PartitionMetadataResponse);
        }
        if (other.lookupTopic_ != null) {
            if (lookupTopic_ == null) {
                LookupTopic = new global::DotPulsar.Internal.PulsarApi.CommandLookupTopic();
            }
            LookupTopic.MergeFrom(other.LookupTopic);
        }
        if (other.lookupTopicResponse_ != null) {
            if (lookupTopicResponse_ == null) {
                LookupTopicResponse = new global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse();
            }
            LookupTopicResponse.MergeFrom(other.LookupTopicResponse);
        }
        if (other.consumerStats_ != null) {
            if (consumerStats_ == null) {
                ConsumerStats = new global::DotPulsar.Internal.PulsarApi.CommandConsumerStats();
            }
            ConsumerStats.MergeFrom(other.ConsumerStats);
        }
        if (other.consumerStatsResponse_ != null) {
            if (consumerStatsResponse_ == null) {
                ConsumerStatsResponse = new global::DotPulsar.Internal.PulsarApi.CommandConsumerStatsResponse();
            }
            ConsumerStatsResponse.MergeFrom(other.ConsumerStatsResponse);
        }
        if (other.reachedEndOfTopic_ != null) {
            if (reachedEndOfTopic_ == null) {
                ReachedEndOfTopic = new global::DotPulsar.Internal.PulsarApi.CommandReachedEndOfTopic();
            }
            ReachedEndOfTopic.MergeFrom(other.ReachedEndOfTopic);
        }
        if (other.seek_ != null) {
            if (seek_ == null) {
                Seek = new global::DotPulsar.Internal.PulsarApi.CommandSeek();
            }
            Seek.MergeFrom(other.Seek);
        }
        if (other.getLastMessageId_ != null) {
            if (getLastMessageId_ == null) {
                GetLastMessageId = new global::DotPulsar.Internal.PulsarApi.CommandGetLastMessageId();
            }
            GetLastMessageId.MergeFrom(other.GetLastMessageId);
        }
        if (other.getLastMessageIdResponse_ != null) {
            if (getLastMessageIdResponse_ == null) {
                GetLastMessageIdResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetLastMessageIdResponse();
            }
            GetLastMessageIdResponse.MergeFrom(other.GetLastMessageIdResponse);
        }
        if (other.activeConsumerChange_ != null) {
            if (activeConsumerChange_ == null) {
                ActiveConsumerChange = new global::DotPulsar.Internal.PulsarApi.CommandActiveConsumerChange();
            }
            ActiveConsumerChange.MergeFrom(other.ActiveConsumerChange);
        }
        if (other.getTopicsOfNamespace_ != null) {
            if (getTopicsOfNamespace_ == null) {
                GetTopicsOfNamespace = new global::DotPulsar.Internal.PulsarApi.CommandGetTopicsOfNamespace();
            }
            GetTopicsOfNamespace.MergeFrom(other.GetTopicsOfNamespace);
        }
        if (other.getTopicsOfNamespaceResponse_ != null) {
            if (getTopicsOfNamespaceResponse_ == null) {
                GetTopicsOfNamespaceResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetTopicsOfNamespaceResponse();
            }
            GetTopicsOfNamespaceResponse.MergeFrom(other.GetTopicsOfNamespaceResponse);
        }
        if (other.getSchema_ != null) {
            if (getSchema_ == null) {
                GetSchema = new global::DotPulsar.Internal.PulsarApi.CommandGetSchema();
            }
            GetSchema.MergeFrom(other.GetSchema);
        }
        if (other.getSchemaResponse_ != null) {
            if (getSchemaResponse_ == null) {
                GetSchemaResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetSchemaResponse();
            }
            GetSchemaResponse.MergeFrom(other.GetSchemaResponse);
        }
        if (other.authChallenge_ != null) {
            if (authChallenge_ == null) {
                AuthChallenge = new global::DotPulsar.Internal.PulsarApi.CommandAuthChallenge();
            }
            AuthChallenge.MergeFrom(other.AuthChallenge);
        }
        if (other.authResponse_ != null) {
            if (authResponse_ == null) {
                AuthResponse = new global::DotPulsar.Internal.PulsarApi.CommandAuthResponse();
            }
            AuthResponse.MergeFrom(other.AuthResponse);
        }
        if (other.ackResponse_ != null) {
            if (ackResponse_ == null) {
                AckResponse = new global::DotPulsar.Internal.PulsarApi.CommandAckResponse();
            }
            AckResponse.MergeFrom(other.AckResponse);
        }
        if (other.getOrCreateSchema_ != null) {
            if (getOrCreateSchema_ == null) {
                GetOrCreateSchema = new global::DotPulsar.Internal.PulsarApi.CommandGetOrCreateSchema();
            }
            GetOrCreateSchema.MergeFrom(other.GetOrCreateSchema);
        }
        if (other.getOrCreateSchemaResponse_ != null) {
            if (getOrCreateSchemaResponse_ == null) {
                GetOrCreateSchemaResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetOrCreateSchemaResponse();
            }
            GetOrCreateSchemaResponse.MergeFrom(other.GetOrCreateSchemaResponse);
        }
        if (other.newTxn_ != null) {
            if (newTxn_ == null) {
                NewTxn = new global::DotPulsar.Internal.PulsarApi.CommandNewTxn();
            }
            NewTxn.MergeFrom(other.NewTxn);
        }
        if (other.newTxnResponse_ != null) {
            if (newTxnResponse_ == null) {
                NewTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandNewTxnResponse();
            }
            NewTxnResponse.MergeFrom(other.NewTxnResponse);
        }
        if (other.addPartitionToTxn_ != null) {
            if (addPartitionToTxn_ == null) {
                AddPartitionToTxn = new global::DotPulsar.Internal.PulsarApi.CommandAddPartitionToTxn();
            }
            AddPartitionToTxn.MergeFrom(other.AddPartitionToTxn);
        }
        if (other.addPartitionToTxnResponse_ != null) {
            if (addPartitionToTxnResponse_ == null) {
                AddPartitionToTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandAddPartitionToTxnResponse();
            }
            AddPartitionToTxnResponse.MergeFrom(other.AddPartitionToTxnResponse);
        }
        if (other.addSubscriptionToTxn_ != null) {
            if (addSubscriptionToTxn_ == null) {
                AddSubscriptionToTxn = new global::DotPulsar.Internal.PulsarApi.CommandAddSubscriptionToTxn();
            }
            AddSubscriptionToTxn.MergeFrom(other.AddSubscriptionToTxn);
        }
        if (other.addSubscriptionToTxnResponse_ != null) {
            if (addSubscriptionToTxnResponse_ == null) {
                AddSubscriptionToTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandAddSubscriptionToTxnResponse();
            }
            AddSubscriptionToTxnResponse.MergeFrom(other.AddSubscriptionToTxnResponse);
        }
        if (other.endTxn_ != null) {
            if (endTxn_ == null) {
                EndTxn = new global::DotPulsar.Internal.PulsarApi.CommandEndTxn();
            }
            EndTxn.MergeFrom(other.EndTxn);
        }
        if (other.endTxnResponse_ != null) {
            if (endTxnResponse_ == null) {
                EndTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnResponse();
            }
            EndTxnResponse.MergeFrom(other.EndTxnResponse);
        }
        if (other.endTxnOnPartition_ != null) {
            if (endTxnOnPartition_ == null) {
                EndTxnOnPartition = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnPartition();
            }
            EndTxnOnPartition.MergeFrom(other.EndTxnOnPartition);
        }
        if (other.endTxnOnPartitionResponse_ != null) {
            if (endTxnOnPartitionResponse_ == null) {
                EndTxnOnPartitionResponse = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnPartitionResponse();
            }
            EndTxnOnPartitionResponse.MergeFrom(other.EndTxnOnPartitionResponse);
        }
        if (other.endTxnOnSubscription_ != null) {
            if (endTxnOnSubscription_ == null) {
                EndTxnOnSubscription = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnSubscription();
            }
            EndTxnOnSubscription.MergeFrom(other.EndTxnOnSubscription);
        }
        if (other.endTxnOnSubscriptionResponse_ != null) {
            if (endTxnOnSubscriptionResponse_ == null) {
                EndTxnOnSubscriptionResponse = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnSubscriptionResponse();
            }
            EndTxnOnSubscriptionResponse.MergeFrom(other.EndTxnOnSubscriptionResponse);
        }
        if (other.tcClientConnectRequest_ != null) {
            if (tcClientConnectRequest_ == null) {
                TcClientConnectRequest = new global::DotPulsar.Internal.PulsarApi.CommandTcClientConnectRequest();
            }
            TcClientConnectRequest.MergeFrom(other.TcClientConnectRequest);
        }
        if (other.tcClientConnectResponse_ != null) {
            if (tcClientConnectResponse_ == null) {
                TcClientConnectResponse = new global::DotPulsar.Internal.PulsarApi.CommandTcClientConnectResponse();
            }
            TcClientConnectResponse.MergeFrom(other.TcClientConnectResponse);
        }
        if (other.watchTopicList_ != null) {
            if (watchTopicList_ == null) {
                WatchTopicList = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicList();
            }
            WatchTopicList.MergeFrom(other.WatchTopicList);
        }
        if (other.watchTopicListSuccess_ != null) {
            if (watchTopicListSuccess_ == null) {
                WatchTopicListSuccess = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicListSuccess();
            }
            WatchTopicListSuccess.MergeFrom(other.WatchTopicListSuccess);
        }
        if (other.watchTopicUpdate_ != null) {
            if (watchTopicUpdate_ == null) {
                WatchTopicUpdate = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicUpdate();
            }
            WatchTopicUpdate.MergeFrom(other.WatchTopicUpdate);
        }
        if (other.watchTopicListClose_ != null) {
            if (watchTopicListClose_ == null) {
                WatchTopicListClose = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicListClose();
            }
            WatchTopicListClose.MergeFrom(other.WatchTopicListClose);
        }
        if (other.topicMigrated_ != null) {
            if (topicMigrated_ == null) {
                TopicMigrated = new global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated();
            }
            TopicMigrated.MergeFrom(other.TopicMigrated);
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
            Type = (global::DotPulsar.Internal.PulsarApi.BaseCommand.Types.Type) input.ReadEnum();
            break;
          }
          case 18: {
            if (connect_ == null) {
              Connect = new global::DotPulsar.Internal.PulsarApi.CommandConnect();
            }
            input.ReadMessage(Connect);
            break;
          }
          case 26: {
            if (connected_ == null) {
              Connected = new global::DotPulsar.Internal.PulsarApi.CommandConnected();
            }
            input.ReadMessage(Connected);
            break;
          }
          case 34: {
            if (subscribe_ == null) {
              Subscribe = new global::DotPulsar.Internal.PulsarApi.CommandSubscribe();
            }
            input.ReadMessage(Subscribe);
            break;
          }
          case 42: {
            if (producer_ == null) {
              Producer = new global::DotPulsar.Internal.PulsarApi.CommandProducer();
            }
            input.ReadMessage(Producer);
            break;
          }
          case 50: {
            if (send_ == null) {
              Send = new global::DotPulsar.Internal.PulsarApi.CommandSend();
            }
            input.ReadMessage(Send);
            break;
          }
          case 58: {
            if (sendReceipt_ == null) {
              SendReceipt = new global::DotPulsar.Internal.PulsarApi.CommandSendReceipt();
            }
            input.ReadMessage(SendReceipt);
            break;
          }
          case 66: {
            if (sendError_ == null) {
              SendError = new global::DotPulsar.Internal.PulsarApi.CommandSendError();
            }
            input.ReadMessage(SendError);
            break;
          }
          case 74: {
            if (message_ == null) {
              Message = new global::DotPulsar.Internal.PulsarApi.CommandMessage();
            }
            input.ReadMessage(Message);
            break;
          }
          case 82: {
            if (ack_ == null) {
              Ack = new global::DotPulsar.Internal.PulsarApi.CommandAck();
            }
            input.ReadMessage(Ack);
            break;
          }
          case 90: {
            if (flow_ == null) {
              Flow = new global::DotPulsar.Internal.PulsarApi.CommandFlow();
            }
            input.ReadMessage(Flow);
            break;
          }
          case 98: {
            if (unsubscribe_ == null) {
              Unsubscribe = new global::DotPulsar.Internal.PulsarApi.CommandUnsubscribe();
            }
            input.ReadMessage(Unsubscribe);
            break;
          }
          case 106: {
            if (success_ == null) {
              Success = new global::DotPulsar.Internal.PulsarApi.CommandSuccess();
            }
            input.ReadMessage(Success);
            break;
          }
          case 114: {
            if (error_ == null) {
              Error = new global::DotPulsar.Internal.PulsarApi.CommandError();
            }
            input.ReadMessage(Error);
            break;
          }
          case 122: {
            if (closeProducer_ == null) {
              CloseProducer = new global::DotPulsar.Internal.PulsarApi.CommandCloseProducer();
            }
            input.ReadMessage(CloseProducer);
            break;
          }
          case 130: {
            if (closeConsumer_ == null) {
              CloseConsumer = new global::DotPulsar.Internal.PulsarApi.CommandCloseConsumer();
            }
            input.ReadMessage(CloseConsumer);
            break;
          }
          case 138: {
            if (producerSuccess_ == null) {
              ProducerSuccess = new global::DotPulsar.Internal.PulsarApi.CommandProducerSuccess();
            }
            input.ReadMessage(ProducerSuccess);
            break;
          }
          case 146: {
            if (ping_ == null) {
              Ping = new global::DotPulsar.Internal.PulsarApi.CommandPing();
            }
            input.ReadMessage(Ping);
            break;
          }
          case 154: {
            if (pong_ == null) {
              Pong = new global::DotPulsar.Internal.PulsarApi.CommandPong();
            }
            input.ReadMessage(Pong);
            break;
          }
          case 162: {
            if (redeliverUnacknowledgedMessages_ == null) {
              RedeliverUnacknowledgedMessages = new global::DotPulsar.Internal.PulsarApi.CommandRedeliverUnacknowledgedMessages();
            }
            input.ReadMessage(RedeliverUnacknowledgedMessages);
            break;
          }
          case 170: {
            if (partitionMetadata_ == null) {
              PartitionMetadata = new global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadata();
            }
            input.ReadMessage(PartitionMetadata);
            break;
          }
          case 178: {
            if (partitionMetadataResponse_ == null) {
              PartitionMetadataResponse = new global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse();
            }
            input.ReadMessage(PartitionMetadataResponse);
            break;
          }
          case 186: {
            if (lookupTopic_ == null) {
              LookupTopic = new global::DotPulsar.Internal.PulsarApi.CommandLookupTopic();
            }
            input.ReadMessage(LookupTopic);
            break;
          }
          case 194: {
            if (lookupTopicResponse_ == null) {
              LookupTopicResponse = new global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse();
            }
            input.ReadMessage(LookupTopicResponse);
            break;
          }
          case 202: {
            if (consumerStats_ == null) {
              ConsumerStats = new global::DotPulsar.Internal.PulsarApi.CommandConsumerStats();
            }
            input.ReadMessage(ConsumerStats);
            break;
          }
          case 210: {
            if (consumerStatsResponse_ == null) {
              ConsumerStatsResponse = new global::DotPulsar.Internal.PulsarApi.CommandConsumerStatsResponse();
            }
            input.ReadMessage(ConsumerStatsResponse);
            break;
          }
          case 218: {
            if (reachedEndOfTopic_ == null) {
              ReachedEndOfTopic = new global::DotPulsar.Internal.PulsarApi.CommandReachedEndOfTopic();
            }
            input.ReadMessage(ReachedEndOfTopic);
            break;
          }
          case 226: {
            if (seek_ == null) {
              Seek = new global::DotPulsar.Internal.PulsarApi.CommandSeek();
            }
            input.ReadMessage(Seek);
            break;
          }
          case 234: {
            if (getLastMessageId_ == null) {
              GetLastMessageId = new global::DotPulsar.Internal.PulsarApi.CommandGetLastMessageId();
            }
            input.ReadMessage(GetLastMessageId);
            break;
          }
          case 242: {
            if (getLastMessageIdResponse_ == null) {
              GetLastMessageIdResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetLastMessageIdResponse();
            }
            input.ReadMessage(GetLastMessageIdResponse);
            break;
          }
          case 250: {
            if (activeConsumerChange_ == null) {
              ActiveConsumerChange = new global::DotPulsar.Internal.PulsarApi.CommandActiveConsumerChange();
            }
            input.ReadMessage(ActiveConsumerChange);
            break;
          }
          case 258: {
            if (getTopicsOfNamespace_ == null) {
              GetTopicsOfNamespace = new global::DotPulsar.Internal.PulsarApi.CommandGetTopicsOfNamespace();
            }
            input.ReadMessage(GetTopicsOfNamespace);
            break;
          }
          case 266: {
            if (getTopicsOfNamespaceResponse_ == null) {
              GetTopicsOfNamespaceResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetTopicsOfNamespaceResponse();
            }
            input.ReadMessage(GetTopicsOfNamespaceResponse);
            break;
          }
          case 274: {
            if (getSchema_ == null) {
              GetSchema = new global::DotPulsar.Internal.PulsarApi.CommandGetSchema();
            }
            input.ReadMessage(GetSchema);
            break;
          }
          case 282: {
            if (getSchemaResponse_ == null) {
              GetSchemaResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetSchemaResponse();
            }
            input.ReadMessage(GetSchemaResponse);
            break;
          }
          case 290: {
            if (authChallenge_ == null) {
              AuthChallenge = new global::DotPulsar.Internal.PulsarApi.CommandAuthChallenge();
            }
            input.ReadMessage(AuthChallenge);
            break;
          }
          case 298: {
            if (authResponse_ == null) {
              AuthResponse = new global::DotPulsar.Internal.PulsarApi.CommandAuthResponse();
            }
            input.ReadMessage(AuthResponse);
            break;
          }
          case 306: {
            if (ackResponse_ == null) {
              AckResponse = new global::DotPulsar.Internal.PulsarApi.CommandAckResponse();
            }
            input.ReadMessage(AckResponse);
            break;
          }
          case 314: {
            if (getOrCreateSchema_ == null) {
              GetOrCreateSchema = new global::DotPulsar.Internal.PulsarApi.CommandGetOrCreateSchema();
            }
            input.ReadMessage(GetOrCreateSchema);
            break;
          }
          case 322: {
            if (getOrCreateSchemaResponse_ == null) {
              GetOrCreateSchemaResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetOrCreateSchemaResponse();
            }
            input.ReadMessage(GetOrCreateSchemaResponse);
            break;
          }
          case 402: {
            if (newTxn_ == null) {
              NewTxn = new global::DotPulsar.Internal.PulsarApi.CommandNewTxn();
            }
            input.ReadMessage(NewTxn);
            break;
          }
          case 410: {
            if (newTxnResponse_ == null) {
              NewTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandNewTxnResponse();
            }
            input.ReadMessage(NewTxnResponse);
            break;
          }
          case 418: {
            if (addPartitionToTxn_ == null) {
              AddPartitionToTxn = new global::DotPulsar.Internal.PulsarApi.CommandAddPartitionToTxn();
            }
            input.ReadMessage(AddPartitionToTxn);
            break;
          }
          case 426: {
            if (addPartitionToTxnResponse_ == null) {
              AddPartitionToTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandAddPartitionToTxnResponse();
            }
            input.ReadMessage(AddPartitionToTxnResponse);
            break;
          }
          case 434: {
            if (addSubscriptionToTxn_ == null) {
              AddSubscriptionToTxn = new global::DotPulsar.Internal.PulsarApi.CommandAddSubscriptionToTxn();
            }
            input.ReadMessage(AddSubscriptionToTxn);
            break;
          }
          case 442: {
            if (addSubscriptionToTxnResponse_ == null) {
              AddSubscriptionToTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandAddSubscriptionToTxnResponse();
            }
            input.ReadMessage(AddSubscriptionToTxnResponse);
            break;
          }
          case 450: {
            if (endTxn_ == null) {
              EndTxn = new global::DotPulsar.Internal.PulsarApi.CommandEndTxn();
            }
            input.ReadMessage(EndTxn);
            break;
          }
          case 458: {
            if (endTxnResponse_ == null) {
              EndTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnResponse();
            }
            input.ReadMessage(EndTxnResponse);
            break;
          }
          case 466: {
            if (endTxnOnPartition_ == null) {
              EndTxnOnPartition = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnPartition();
            }
            input.ReadMessage(EndTxnOnPartition);
            break;
          }
          case 474: {
            if (endTxnOnPartitionResponse_ == null) {
              EndTxnOnPartitionResponse = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnPartitionResponse();
            }
            input.ReadMessage(EndTxnOnPartitionResponse);
            break;
          }
          case 482: {
            if (endTxnOnSubscription_ == null) {
              EndTxnOnSubscription = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnSubscription();
            }
            input.ReadMessage(EndTxnOnSubscription);
            break;
          }
          case 490: {
            if (endTxnOnSubscriptionResponse_ == null) {
              EndTxnOnSubscriptionResponse = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnSubscriptionResponse();
            }
            input.ReadMessage(EndTxnOnSubscriptionResponse);
            break;
          }
          case 498: {
            if (tcClientConnectRequest_ == null) {
              TcClientConnectRequest = new global::DotPulsar.Internal.PulsarApi.CommandTcClientConnectRequest();
            }
            input.ReadMessage(TcClientConnectRequest);
            break;
          }
          case 506: {
            if (tcClientConnectResponse_ == null) {
              TcClientConnectResponse = new global::DotPulsar.Internal.PulsarApi.CommandTcClientConnectResponse();
            }
            input.ReadMessage(TcClientConnectResponse);
            break;
          }
          case 514: {
            if (watchTopicList_ == null) {
              WatchTopicList = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicList();
            }
            input.ReadMessage(WatchTopicList);
            break;
          }
          case 522: {
            if (watchTopicListSuccess_ == null) {
              WatchTopicListSuccess = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicListSuccess();
            }
            input.ReadMessage(WatchTopicListSuccess);
            break;
          }
          case 530: {
            if (watchTopicUpdate_ == null) {
              WatchTopicUpdate = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicUpdate();
            }
            input.ReadMessage(WatchTopicUpdate);
            break;
          }
          case 538: {
            if (watchTopicListClose_ == null) {
              WatchTopicListClose = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicListClose();
            }
            input.ReadMessage(WatchTopicListClose);
            break;
          }
          case 546: {
            if (topicMigrated_ == null) {
              TopicMigrated = new global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated();
            }
            input.ReadMessage(TopicMigrated);
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
                    Type = (global::DotPulsar.Internal.PulsarApi.BaseCommand.Types.Type) input.ReadEnum();
                    break;
                }
                case 18: {
                    if (connect_ == null) {
                        Connect = new global::DotPulsar.Internal.PulsarApi.CommandConnect();
                    }
                    input.ReadMessage(Connect);
                    break;
                }
                case 26: {
                    if (connected_ == null) {
                        Connected = new global::DotPulsar.Internal.PulsarApi.CommandConnected();
                    }
                    input.ReadMessage(Connected);
                    break;
                }
                case 34: {
                    if (subscribe_ == null) {
                        Subscribe = new global::DotPulsar.Internal.PulsarApi.CommandSubscribe();
                    }
                    input.ReadMessage(Subscribe);
                    break;
                }
                case 42: {
                    if (producer_ == null) {
                        Producer = new global::DotPulsar.Internal.PulsarApi.CommandProducer();
                    }
                    input.ReadMessage(Producer);
                    break;
                }
                case 50: {
                    if (send_ == null) {
                        Send = new global::DotPulsar.Internal.PulsarApi.CommandSend();
                    }
                    input.ReadMessage(Send);
                    break;
                }
                case 58: {
                    if (sendReceipt_ == null) {
                        SendReceipt = new global::DotPulsar.Internal.PulsarApi.CommandSendReceipt();
                    }
                    input.ReadMessage(SendReceipt);
                    break;
                }
                case 66: {
                    if (sendError_ == null) {
                        SendError = new global::DotPulsar.Internal.PulsarApi.CommandSendError();
                    }
                    input.ReadMessage(SendError);
                    break;
                }
                case 74: {
                    if (message_ == null) {
                        Message = new global::DotPulsar.Internal.PulsarApi.CommandMessage();
                    }
                    input.ReadMessage(Message);
                    break;
                }
                case 82: {
                    if (ack_ == null) {
                        Ack = new global::DotPulsar.Internal.PulsarApi.CommandAck();
                    }
                    input.ReadMessage(Ack);
                    break;
                }
                case 90: {
                    if (flow_ == null) {
                        Flow = new global::DotPulsar.Internal.PulsarApi.CommandFlow();
                    }
                    input.ReadMessage(Flow);
                    break;
                }
                case 98: {
                    if (unsubscribe_ == null) {
                        Unsubscribe = new global::DotPulsar.Internal.PulsarApi.CommandUnsubscribe();
                    }
                    input.ReadMessage(Unsubscribe);
                    break;
                }
                case 106: {
                    if (success_ == null) {
                        Success = new global::DotPulsar.Internal.PulsarApi.CommandSuccess();
                    }
                    input.ReadMessage(Success);
                    break;
                }
                case 114: {
                    if (error_ == null) {
                        Error = new global::DotPulsar.Internal.PulsarApi.CommandError();
                    }
                    input.ReadMessage(Error);
                    break;
                }
                case 122: {
                    if (closeProducer_ == null) {
                        CloseProducer = new global::DotPulsar.Internal.PulsarApi.CommandCloseProducer();
                    }
                    input.ReadMessage(CloseProducer);
                    break;
                }
                case 130: {
                    if (closeConsumer_ == null) {
                        CloseConsumer = new global::DotPulsar.Internal.PulsarApi.CommandCloseConsumer();
                    }
                    input.ReadMessage(CloseConsumer);
                    break;
                }
                case 138: {
                    if (producerSuccess_ == null) {
                        ProducerSuccess = new global::DotPulsar.Internal.PulsarApi.CommandProducerSuccess();
                    }
                    input.ReadMessage(ProducerSuccess);
                    break;
                }
                case 146: {
                    if (ping_ == null) {
                        Ping = new global::DotPulsar.Internal.PulsarApi.CommandPing();
                    }
                    input.ReadMessage(Ping);
                    break;
                }
                case 154: {
                    if (pong_ == null) {
                        Pong = new global::DotPulsar.Internal.PulsarApi.CommandPong();
                    }
                    input.ReadMessage(Pong);
                    break;
                }
                case 162: {
                    if (redeliverUnacknowledgedMessages_ == null) {
                        RedeliverUnacknowledgedMessages = new global::DotPulsar.Internal.PulsarApi.CommandRedeliverUnacknowledgedMessages();
                    }
                    input.ReadMessage(RedeliverUnacknowledgedMessages);
                    break;
                }
                case 170: {
                    if (partitionMetadata_ == null) {
                        PartitionMetadata = new global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadata();
                    }
                    input.ReadMessage(PartitionMetadata);
                    break;
                }
                case 178: {
                    if (partitionMetadataResponse_ == null) {
                        PartitionMetadataResponse = new global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse();
                    }
                    input.ReadMessage(PartitionMetadataResponse);
                    break;
                }
                case 186: {
                    if (lookupTopic_ == null) {
                        LookupTopic = new global::DotPulsar.Internal.PulsarApi.CommandLookupTopic();
                    }
                    input.ReadMessage(LookupTopic);
                    break;
                }
                case 194: {
                    if (lookupTopicResponse_ == null) {
                        LookupTopicResponse = new global::DotPulsar.Internal.PulsarApi.CommandLookupTopicResponse();
                    }
                    input.ReadMessage(LookupTopicResponse);
                    break;
                }
                case 202: {
                    if (consumerStats_ == null) {
                        ConsumerStats = new global::DotPulsar.Internal.PulsarApi.CommandConsumerStats();
                    }
                    input.ReadMessage(ConsumerStats);
                    break;
                }
                case 210: {
                    if (consumerStatsResponse_ == null) {
                        ConsumerStatsResponse = new global::DotPulsar.Internal.PulsarApi.CommandConsumerStatsResponse();
                    }
                    input.ReadMessage(ConsumerStatsResponse);
                    break;
                }
                case 218: {
                    if (reachedEndOfTopic_ == null) {
                        ReachedEndOfTopic = new global::DotPulsar.Internal.PulsarApi.CommandReachedEndOfTopic();
                    }
                    input.ReadMessage(ReachedEndOfTopic);
                    break;
                }
                case 226: {
                    if (seek_ == null) {
                        Seek = new global::DotPulsar.Internal.PulsarApi.CommandSeek();
                    }
                    input.ReadMessage(Seek);
                    break;
                }
                case 234: {
                    if (getLastMessageId_ == null) {
                        GetLastMessageId = new global::DotPulsar.Internal.PulsarApi.CommandGetLastMessageId();
                    }
                    input.ReadMessage(GetLastMessageId);
                    break;
                }
                case 242: {
                    if (getLastMessageIdResponse_ == null) {
                        GetLastMessageIdResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetLastMessageIdResponse();
                    }
                    input.ReadMessage(GetLastMessageIdResponse);
                    break;
                }
                case 250: {
                    if (activeConsumerChange_ == null) {
                        ActiveConsumerChange = new global::DotPulsar.Internal.PulsarApi.CommandActiveConsumerChange();
                    }
                    input.ReadMessage(ActiveConsumerChange);
                    break;
                }
                case 258: {
                    if (getTopicsOfNamespace_ == null) {
                        GetTopicsOfNamespace = new global::DotPulsar.Internal.PulsarApi.CommandGetTopicsOfNamespace();
                    }
                    input.ReadMessage(GetTopicsOfNamespace);
                    break;
                }
                case 266: {
                    if (getTopicsOfNamespaceResponse_ == null) {
                        GetTopicsOfNamespaceResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetTopicsOfNamespaceResponse();
                    }
                    input.ReadMessage(GetTopicsOfNamespaceResponse);
                    break;
                }
                case 274: {
                    if (getSchema_ == null) {
                        GetSchema = new global::DotPulsar.Internal.PulsarApi.CommandGetSchema();
                    }
                    input.ReadMessage(GetSchema);
                    break;
                }
                case 282: {
                    if (getSchemaResponse_ == null) {
                        GetSchemaResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetSchemaResponse();
                    }
                    input.ReadMessage(GetSchemaResponse);
                    break;
                }
                case 290: {
                    if (authChallenge_ == null) {
                        AuthChallenge = new global::DotPulsar.Internal.PulsarApi.CommandAuthChallenge();
                    }
                    input.ReadMessage(AuthChallenge);
                    break;
                }
                case 298: {
                    if (authResponse_ == null) {
                        AuthResponse = new global::DotPulsar.Internal.PulsarApi.CommandAuthResponse();
                    }
                    input.ReadMessage(AuthResponse);
                    break;
                }
                case 306: {
                    if (ackResponse_ == null) {
                        AckResponse = new global::DotPulsar.Internal.PulsarApi.CommandAckResponse();
                    }
                    input.ReadMessage(AckResponse);
                    break;
                }
                case 314: {
                    if (getOrCreateSchema_ == null) {
                        GetOrCreateSchema = new global::DotPulsar.Internal.PulsarApi.CommandGetOrCreateSchema();
                    }
                    input.ReadMessage(GetOrCreateSchema);
                    break;
                }
                case 322: {
                    if (getOrCreateSchemaResponse_ == null) {
                        GetOrCreateSchemaResponse = new global::DotPulsar.Internal.PulsarApi.CommandGetOrCreateSchemaResponse();
                    }
                    input.ReadMessage(GetOrCreateSchemaResponse);
                    break;
                }
                case 402: {
                    if (newTxn_ == null) {
                        NewTxn = new global::DotPulsar.Internal.PulsarApi.CommandNewTxn();
                    }
                    input.ReadMessage(NewTxn);
                    break;
                }
                case 410: {
                    if (newTxnResponse_ == null) {
                        NewTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandNewTxnResponse();
                    }
                    input.ReadMessage(NewTxnResponse);
                    break;
                }
                case 418: {
                    if (addPartitionToTxn_ == null) {
                        AddPartitionToTxn = new global::DotPulsar.Internal.PulsarApi.CommandAddPartitionToTxn();
                    }
                    input.ReadMessage(AddPartitionToTxn);
                    break;
                }
                case 426: {
                    if (addPartitionToTxnResponse_ == null) {
                        AddPartitionToTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandAddPartitionToTxnResponse();
                    }
                    input.ReadMessage(AddPartitionToTxnResponse);
                    break;
                }
                case 434: {
                    if (addSubscriptionToTxn_ == null) {
                        AddSubscriptionToTxn = new global::DotPulsar.Internal.PulsarApi.CommandAddSubscriptionToTxn();
                    }
                    input.ReadMessage(AddSubscriptionToTxn);
                    break;
                }
                case 442: {
                    if (addSubscriptionToTxnResponse_ == null) {
                        AddSubscriptionToTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandAddSubscriptionToTxnResponse();
                    }
                    input.ReadMessage(AddSubscriptionToTxnResponse);
                    break;
                }
                case 450: {
                    if (endTxn_ == null) {
                        EndTxn = new global::DotPulsar.Internal.PulsarApi.CommandEndTxn();
                    }
                    input.ReadMessage(EndTxn);
                    break;
                }
                case 458: {
                    if (endTxnResponse_ == null) {
                        EndTxnResponse = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnResponse();
                    }
                    input.ReadMessage(EndTxnResponse);
                    break;
                }
                case 466: {
                    if (endTxnOnPartition_ == null) {
                        EndTxnOnPartition = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnPartition();
                    }
                    input.ReadMessage(EndTxnOnPartition);
                    break;
                }
                case 474: {
                    if (endTxnOnPartitionResponse_ == null) {
                        EndTxnOnPartitionResponse = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnPartitionResponse();
                    }
                    input.ReadMessage(EndTxnOnPartitionResponse);
                    break;
                }
                case 482: {
                    if (endTxnOnSubscription_ == null) {
                        EndTxnOnSubscription = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnSubscription();
                    }
                    input.ReadMessage(EndTxnOnSubscription);
                    break;
                }
                case 490: {
                    if (endTxnOnSubscriptionResponse_ == null) {
                        EndTxnOnSubscriptionResponse = new global::DotPulsar.Internal.PulsarApi.CommandEndTxnOnSubscriptionResponse();
                    }
                    input.ReadMessage(EndTxnOnSubscriptionResponse);
                    break;
                }
                case 498: {
                    if (tcClientConnectRequest_ == null) {
                        TcClientConnectRequest = new global::DotPulsar.Internal.PulsarApi.CommandTcClientConnectRequest();
                    }
                    input.ReadMessage(TcClientConnectRequest);
                    break;
                }
                case 506: {
                    if (tcClientConnectResponse_ == null) {
                        TcClientConnectResponse = new global::DotPulsar.Internal.PulsarApi.CommandTcClientConnectResponse();
                    }
                    input.ReadMessage(TcClientConnectResponse);
                    break;
                }
                case 514: {
                    if (watchTopicList_ == null) {
                        WatchTopicList = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicList();
                    }
                    input.ReadMessage(WatchTopicList);
                    break;
                }
                case 522: {
                    if (watchTopicListSuccess_ == null) {
                        WatchTopicListSuccess = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicListSuccess();
                    }
                    input.ReadMessage(WatchTopicListSuccess);
                    break;
                }
                case 530: {
                    if (watchTopicUpdate_ == null) {
                        WatchTopicUpdate = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicUpdate();
                    }
                    input.ReadMessage(WatchTopicUpdate);
                    break;
                }
                case 538: {
                    if (watchTopicListClose_ == null) {
                        WatchTopicListClose = new global::DotPulsar.Internal.PulsarApi.CommandWatchTopicListClose();
                    }
                    input.ReadMessage(WatchTopicListClose);
                    break;
                }
                case 546: {
                    if (topicMigrated_ == null) {
                        TopicMigrated = new global::DotPulsar.Internal.PulsarApi.CommandTopicMigrated();
                    }
                    input.ReadMessage(TopicMigrated);
                    break;
                }
            }
        }
    }
#endif

    #region Nested types
    /// <summary>Container for nested types declared in the BaseCommand message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Types {
        public enum Type {
            [pbr::OriginalName("CONNECT")] Connect = 2,
            [pbr::OriginalName("CONNECTED")] Connected = 3,
            [pbr::OriginalName("SUBSCRIBE")] Subscribe = 4,
            [pbr::OriginalName("PRODUCER")] Producer = 5,
            [pbr::OriginalName("SEND")] Send = 6,
            [pbr::OriginalName("SEND_RECEIPT")] SendReceipt = 7,
            [pbr::OriginalName("SEND_ERROR")] SendError = 8,
            [pbr::OriginalName("MESSAGE")] Message = 9,
            [pbr::OriginalName("ACK")] Ack = 10,
            [pbr::OriginalName("FLOW")] Flow = 11,
            [pbr::OriginalName("UNSUBSCRIBE")] Unsubscribe = 12,
            [pbr::OriginalName("SUCCESS")] Success = 13,
            [pbr::OriginalName("ERROR")] Error = 14,
            [pbr::OriginalName("CLOSE_PRODUCER")] CloseProducer = 15,
            [pbr::OriginalName("CLOSE_CONSUMER")] CloseConsumer = 16,
            [pbr::OriginalName("PRODUCER_SUCCESS")] ProducerSuccess = 17,
            [pbr::OriginalName("PING")] Ping = 18,
            [pbr::OriginalName("PONG")] Pong = 19,
            [pbr::OriginalName("REDELIVER_UNACKNOWLEDGED_MESSAGES")] RedeliverUnacknowledgedMessages = 20,
            [pbr::OriginalName("PARTITIONED_METADATA")] PartitionedMetadata = 21,
            [pbr::OriginalName("PARTITIONED_METADATA_RESPONSE")] PartitionedMetadataResponse = 22,
            [pbr::OriginalName("LOOKUP")] Lookup = 23,
            [pbr::OriginalName("LOOKUP_RESPONSE")] LookupResponse = 24,
            [pbr::OriginalName("CONSUMER_STATS")] ConsumerStats = 25,
            [pbr::OriginalName("CONSUMER_STATS_RESPONSE")] ConsumerStatsResponse = 26,
            [pbr::OriginalName("REACHED_END_OF_TOPIC")] ReachedEndOfTopic = 27,
            [pbr::OriginalName("SEEK")] Seek = 28,
            [pbr::OriginalName("GET_LAST_MESSAGE_ID")] GetLastMessageId = 29,
            [pbr::OriginalName("GET_LAST_MESSAGE_ID_RESPONSE")] GetLastMessageIdResponse = 30,
            [pbr::OriginalName("ACTIVE_CONSUMER_CHANGE")] ActiveConsumerChange = 31,
            [pbr::OriginalName("GET_TOPICS_OF_NAMESPACE")] GetTopicsOfNamespace = 32,
            [pbr::OriginalName("GET_TOPICS_OF_NAMESPACE_RESPONSE")] GetTopicsOfNamespaceResponse = 33,
            [pbr::OriginalName("GET_SCHEMA")] GetSchema = 34,
            [pbr::OriginalName("GET_SCHEMA_RESPONSE")] GetSchemaResponse = 35,
            [pbr::OriginalName("AUTH_CHALLENGE")] AuthChallenge = 36,
            [pbr::OriginalName("AUTH_RESPONSE")] AuthResponse = 37,
            [pbr::OriginalName("ACK_RESPONSE")] AckResponse = 38,
            [pbr::OriginalName("GET_OR_CREATE_SCHEMA")] GetOrCreateSchema = 39,
            [pbr::OriginalName("GET_OR_CREATE_SCHEMA_RESPONSE")] GetOrCreateSchemaResponse = 40,
            /// <summary>
            /// transaction related
            /// </summary>
            [pbr::OriginalName("NEW_TXN")] NewTxn = 50,
            [pbr::OriginalName("NEW_TXN_RESPONSE")] NewTxnResponse = 51,
            [pbr::OriginalName("ADD_PARTITION_TO_TXN")] AddPartitionToTxn = 52,
            [pbr::OriginalName("ADD_PARTITION_TO_TXN_RESPONSE")] AddPartitionToTxnResponse = 53,
            [pbr::OriginalName("ADD_SUBSCRIPTION_TO_TXN")] AddSubscriptionToTxn = 54,
            [pbr::OriginalName("ADD_SUBSCRIPTION_TO_TXN_RESPONSE")] AddSubscriptionToTxnResponse = 55,
            [pbr::OriginalName("END_TXN")] EndTxn = 56,
            [pbr::OriginalName("END_TXN_RESPONSE")] EndTxnResponse = 57,
            [pbr::OriginalName("END_TXN_ON_PARTITION")] EndTxnOnPartition = 58,
            [pbr::OriginalName("END_TXN_ON_PARTITION_RESPONSE")] EndTxnOnPartitionResponse = 59,
            [pbr::OriginalName("END_TXN_ON_SUBSCRIPTION")] EndTxnOnSubscription = 60,
            [pbr::OriginalName("END_TXN_ON_SUBSCRIPTION_RESPONSE")] EndTxnOnSubscriptionResponse = 61,
            [pbr::OriginalName("TC_CLIENT_CONNECT_REQUEST")] TcClientConnectRequest = 62,
            [pbr::OriginalName("TC_CLIENT_CONNECT_RESPONSE")] TcClientConnectResponse = 63,
            [pbr::OriginalName("WATCH_TOPIC_LIST")] WatchTopicList = 64,
            [pbr::OriginalName("WATCH_TOPIC_LIST_SUCCESS")] WatchTopicListSuccess = 65,
            [pbr::OriginalName("WATCH_TOPIC_UPDATE")] WatchTopicUpdate = 66,
            [pbr::OriginalName("WATCH_TOPIC_LIST_CLOSE")] WatchTopicListClose = 67,
            [pbr::OriginalName("TOPIC_MIGRATED")] TopicMigrated = 68,
        }

    }
    #endregion

}
