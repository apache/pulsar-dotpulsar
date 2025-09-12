#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandSubscribe : IMessage<CommandSubscribe>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandSubscribe> _parser = new pb::MessageParser<CommandSubscribe>(() => new CommandSubscribe());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandSubscribe> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[16]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandSubscribe() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandSubscribe(CommandSubscribe other) : this() {
        _hasBits0 = other._hasBits0;
        topic_ = other.topic_;
        subscription_ = other.subscription_;
        subType_ = other.subType_;
        consumerId_ = other.consumerId_;
        requestId_ = other.requestId_;
        consumerName_ = other.consumerName_;
        priorityLevel_ = other.priorityLevel_;
        durable_ = other.durable_;
        startMessageId_ = other.startMessageId_ != null ? other.startMessageId_.Clone() : null;
        metadata_ = other.metadata_.Clone();
        readCompacted_ = other.readCompacted_;
        schema_ = other.schema_ != null ? other.schema_.Clone() : null;
        initialPosition_ = other.initialPosition_;
        replicateSubscriptionState_ = other.replicateSubscriptionState_;
        forceTopicCreation_ = other.forceTopicCreation_;
        startMessageRollbackDurationSec_ = other.startMessageRollbackDurationSec_;
        keySharedMeta_ = other.keySharedMeta_ != null ? other.keySharedMeta_.Clone() : null;
        subscriptionProperties_ = other.subscriptionProperties_.Clone();
        consumerEpoch_ = other.consumerEpoch_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandSubscribe Clone() {
        return new CommandSubscribe(this);
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

    /// <summary>Field number for the "subscription" field.</summary>
    public const int SubscriptionFieldNumber = 2;
    private readonly static string SubscriptionDefaultValue = "";

    private string subscription_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Subscription {
        get { return subscription_ ?? SubscriptionDefaultValue; }
        set {
            subscription_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "subscription" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSubscription {
        get { return subscription_ != null; }
    }
    /// <summary>Clears the value of the "subscription" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSubscription() {
        subscription_ = null;
    }

    /// <summary>Field number for the "subType" field.</summary>
    public const int SubTypeFieldNumber = 3;
    private readonly static global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.SubType SubTypeDefaultValue = global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.SubType.Exclusive;

    private global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.SubType subType_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.SubType SubType {
        get { if ((_hasBits0 & 1) != 0) { return subType_; } else { return SubTypeDefaultValue; } }
        set {
            _hasBits0 |= 1;
            subType_ = value;
        }
    }
    /// <summary>Gets whether the "subType" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSubType {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "subType" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSubType() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "consumer_id" field.</summary>
    public const int ConsumerIdFieldNumber = 4;
    private readonly static ulong ConsumerIdDefaultValue = 0UL;

    private ulong consumerId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong ConsumerId {
        get { if ((_hasBits0 & 2) != 0) { return consumerId_; } else { return ConsumerIdDefaultValue; } }
        set {
            _hasBits0 |= 2;
            consumerId_ = value;
        }
    }
    /// <summary>Gets whether the "consumer_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasConsumerId {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "consumer_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearConsumerId() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 5;
    private readonly static ulong RequestIdDefaultValue = 0UL;

    private ulong requestId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong RequestId {
        get { if ((_hasBits0 & 4) != 0) { return requestId_; } else { return RequestIdDefaultValue; } }
        set {
            _hasBits0 |= 4;
            requestId_ = value;
        }
    }
    /// <summary>Gets whether the "request_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasRequestId {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "request_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearRequestId() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "consumer_name" field.</summary>
    public const int ConsumerNameFieldNumber = 6;
    private readonly static string ConsumerNameDefaultValue = "";

    private string consumerName_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ConsumerName {
        get { return consumerName_ ?? ConsumerNameDefaultValue; }
        set {
            consumerName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "consumer_name" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasConsumerName {
        get { return consumerName_ != null; }
    }
    /// <summary>Clears the value of the "consumer_name" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearConsumerName() {
        consumerName_ = null;
    }

    /// <summary>Field number for the "priority_level" field.</summary>
    public const int PriorityLevelFieldNumber = 7;
    private readonly static int PriorityLevelDefaultValue = 0;

    private int priorityLevel_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int PriorityLevel {
        get { if ((_hasBits0 & 8) != 0) { return priorityLevel_; } else { return PriorityLevelDefaultValue; } }
        set {
            _hasBits0 |= 8;
            priorityLevel_ = value;
        }
    }
    /// <summary>Gets whether the "priority_level" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasPriorityLevel {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "priority_level" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearPriorityLevel() {
        _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "durable" field.</summary>
    public const int DurableFieldNumber = 8;
    private readonly static bool DurableDefaultValue = true;

    private bool durable_;
    /// <summary>
    /// Signal wether the subscription should be backed by a
    /// durable cursor or not
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Durable {
        get { if ((_hasBits0 & 16) != 0) { return durable_; } else { return DurableDefaultValue; } }
        set {
            _hasBits0 |= 16;
            durable_ = value;
        }
    }
    /// <summary>Gets whether the "durable" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasDurable {
        get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "durable" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearDurable() {
        _hasBits0 &= ~16;
    }

    /// <summary>Field number for the "start_message_id" field.</summary>
    public const int StartMessageIdFieldNumber = 9;
    private global::DotPulsar.Internal.PulsarApi.MessageIdData startMessageId_;
    /// <summary>
    /// If specified, the subscription will position the cursor
    /// markd-delete position  on the particular message id and
    /// will send messages from that point
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.MessageIdData StartMessageId {
        get { return startMessageId_; }
        set {
            startMessageId_ = value;
        }
    }

    /// <summary>Field number for the "metadata" field.</summary>
    public const int MetadataFieldNumber = 10;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.KeyValue> _repeated_metadata_codec
        = pb::FieldCodec.ForMessage(82, global::DotPulsar.Internal.PulsarApi.KeyValue.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> metadata_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue>();
    /// <summary>
    ///&#x2F; Add optional metadata key=value to this consumer
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> Metadata {
        get { return metadata_; }
    }

    /// <summary>Field number for the "read_compacted" field.</summary>
    public const int ReadCompactedFieldNumber = 11;
    private readonly static bool ReadCompactedDefaultValue = false;

    private bool readCompacted_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool ReadCompacted {
        get { if ((_hasBits0 & 32) != 0) { return readCompacted_; } else { return ReadCompactedDefaultValue; } }
        set {
            _hasBits0 |= 32;
            readCompacted_ = value;
        }
    }
    /// <summary>Gets whether the "read_compacted" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasReadCompacted {
        get { return (_hasBits0 & 32) != 0; }
    }
    /// <summary>Clears the value of the "read_compacted" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearReadCompacted() {
        _hasBits0 &= ~32;
    }

    /// <summary>Field number for the "schema" field.</summary>
    public const int SchemaFieldNumber = 12;
    private global::DotPulsar.Internal.PulsarApi.Schema schema_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.Schema Schema {
        get { return schema_; }
        set {
            schema_ = value;
        }
    }

    /// <summary>Field number for the "initialPosition" field.</summary>
    public const int InitialPositionFieldNumber = 13;
    private readonly static global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.InitialPosition InitialPositionDefaultValue = global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.InitialPosition.Latest;

    private global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.InitialPosition initialPosition_;
    /// <summary>
    /// Signal whether the subscription will initialize on latest
    /// or not -- earliest
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.InitialPosition InitialPosition {
        get { if ((_hasBits0 & 64) != 0) { return initialPosition_; } else { return InitialPositionDefaultValue; } }
        set {
            _hasBits0 |= 64;
            initialPosition_ = value;
        }
    }
    /// <summary>Gets whether the "initialPosition" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasInitialPosition {
        get { return (_hasBits0 & 64) != 0; }
    }
    /// <summary>Clears the value of the "initialPosition" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearInitialPosition() {
        _hasBits0 &= ~64;
    }

    /// <summary>Field number for the "replicate_subscription_state" field.</summary>
    public const int ReplicateSubscriptionStateFieldNumber = 14;
    private readonly static bool ReplicateSubscriptionStateDefaultValue = false;

    private bool replicateSubscriptionState_;
    /// <summary>
    /// Mark the subscription as "replicated". Pulsar will make sure
    /// to periodically sync the state of replicated subscriptions
    /// across different clusters (when using geo-replication).
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool ReplicateSubscriptionState {
        get { if ((_hasBits0 & 128) != 0) { return replicateSubscriptionState_; } else { return ReplicateSubscriptionStateDefaultValue; } }
        set {
            _hasBits0 |= 128;
            replicateSubscriptionState_ = value;
        }
    }
    /// <summary>Gets whether the "replicate_subscription_state" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasReplicateSubscriptionState {
        get { return (_hasBits0 & 128) != 0; }
    }
    /// <summary>Clears the value of the "replicate_subscription_state" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearReplicateSubscriptionState() {
        _hasBits0 &= ~128;
    }

    /// <summary>Field number for the "force_topic_creation" field.</summary>
    public const int ForceTopicCreationFieldNumber = 15;
    private readonly static bool ForceTopicCreationDefaultValue = true;

    private bool forceTopicCreation_;
    /// <summary>
    /// If true, the subscribe operation will cause a topic to be
    /// created if it does not exist already (and if topic auto-creation
    /// is allowed by broker.
    /// If false, the subscribe operation will fail if the topic
    /// does not exist.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool ForceTopicCreation {
        get { if ((_hasBits0 & 256) != 0) { return forceTopicCreation_; } else { return ForceTopicCreationDefaultValue; } }
        set {
            _hasBits0 |= 256;
            forceTopicCreation_ = value;
        }
    }
    /// <summary>Gets whether the "force_topic_creation" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasForceTopicCreation {
        get { return (_hasBits0 & 256) != 0; }
    }
    /// <summary>Clears the value of the "force_topic_creation" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearForceTopicCreation() {
        _hasBits0 &= ~256;
    }

    /// <summary>Field number for the "start_message_rollback_duration_sec" field.</summary>
    public const int StartMessageRollbackDurationSecFieldNumber = 16;
    private readonly static ulong StartMessageRollbackDurationSecDefaultValue = 0UL;

    private ulong startMessageRollbackDurationSec_;
    /// <summary>
    /// If specified, the subscription will reset cursor's position back
    /// to specified seconds and  will send messages from that point
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong StartMessageRollbackDurationSec {
        get { if ((_hasBits0 & 512) != 0) { return startMessageRollbackDurationSec_; } else { return StartMessageRollbackDurationSecDefaultValue; } }
        set {
            _hasBits0 |= 512;
            startMessageRollbackDurationSec_ = value;
        }
    }
    /// <summary>Gets whether the "start_message_rollback_duration_sec" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasStartMessageRollbackDurationSec {
        get { return (_hasBits0 & 512) != 0; }
    }
    /// <summary>Clears the value of the "start_message_rollback_duration_sec" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearStartMessageRollbackDurationSec() {
        _hasBits0 &= ~512;
    }

    /// <summary>Field number for the "keySharedMeta" field.</summary>
    public const int KeySharedMetaFieldNumber = 17;
    private global::DotPulsar.Internal.PulsarApi.KeySharedMeta keySharedMeta_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.KeySharedMeta KeySharedMeta {
        get { return keySharedMeta_; }
        set {
            keySharedMeta_ = value;
        }
    }

    /// <summary>Field number for the "subscription_properties" field.</summary>
    public const int SubscriptionPropertiesFieldNumber = 18;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.KeyValue> _repeated_subscriptionProperties_codec
        = pb::FieldCodec.ForMessage(146, global::DotPulsar.Internal.PulsarApi.KeyValue.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> subscriptionProperties_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> SubscriptionProperties {
        get { return subscriptionProperties_; }
    }

    /// <summary>Field number for the "consumer_epoch" field.</summary>
    public const int ConsumerEpochFieldNumber = 19;
    private readonly static ulong ConsumerEpochDefaultValue = 0UL;

    private ulong consumerEpoch_;
    /// <summary>
    /// The consumer epoch, when exclusive and failover consumer redeliver unack message will increase the epoch
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong ConsumerEpoch {
        get { if ((_hasBits0 & 1024) != 0) { return consumerEpoch_; } else { return ConsumerEpochDefaultValue; } }
        set {
            _hasBits0 |= 1024;
            consumerEpoch_ = value;
        }
    }
    /// <summary>Gets whether the "consumer_epoch" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasConsumerEpoch {
        get { return (_hasBits0 & 1024) != 0; }
    }
    /// <summary>Clears the value of the "consumer_epoch" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearConsumerEpoch() {
        _hasBits0 &= ~1024;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandSubscribe);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandSubscribe other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (Topic != other.Topic) return false;
        if (Subscription != other.Subscription) return false;
        if (SubType != other.SubType) return false;
        if (ConsumerId != other.ConsumerId) return false;
        if (RequestId != other.RequestId) return false;
        if (ConsumerName != other.ConsumerName) return false;
        if (PriorityLevel != other.PriorityLevel) return false;
        if (Durable != other.Durable) return false;
        if (!object.Equals(StartMessageId, other.StartMessageId)) return false;
        if(!metadata_.Equals(other.metadata_)) return false;
        if (ReadCompacted != other.ReadCompacted) return false;
        if (!object.Equals(Schema, other.Schema)) return false;
        if (InitialPosition != other.InitialPosition) return false;
        if (ReplicateSubscriptionState != other.ReplicateSubscriptionState) return false;
        if (ForceTopicCreation != other.ForceTopicCreation) return false;
        if (StartMessageRollbackDurationSec != other.StartMessageRollbackDurationSec) return false;
        if (!object.Equals(KeySharedMeta, other.KeySharedMeta)) return false;
        if(!subscriptionProperties_.Equals(other.subscriptionProperties_)) return false;
        if (ConsumerEpoch != other.ConsumerEpoch) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasTopic) hash ^= Topic.GetHashCode();
        if (HasSubscription) hash ^= Subscription.GetHashCode();
        if (HasSubType) hash ^= SubType.GetHashCode();
        if (HasConsumerId) hash ^= ConsumerId.GetHashCode();
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasConsumerName) hash ^= ConsumerName.GetHashCode();
        if (HasPriorityLevel) hash ^= PriorityLevel.GetHashCode();
        if (HasDurable) hash ^= Durable.GetHashCode();
        if (startMessageId_ != null) hash ^= StartMessageId.GetHashCode();
        hash ^= metadata_.GetHashCode();
        if (HasReadCompacted) hash ^= ReadCompacted.GetHashCode();
        if (schema_ != null) hash ^= Schema.GetHashCode();
        if (HasInitialPosition) hash ^= InitialPosition.GetHashCode();
        if (HasReplicateSubscriptionState) hash ^= ReplicateSubscriptionState.GetHashCode();
        if (HasForceTopicCreation) hash ^= ForceTopicCreation.GetHashCode();
        if (HasStartMessageRollbackDurationSec) hash ^= StartMessageRollbackDurationSec.GetHashCode();
        if (keySharedMeta_ != null) hash ^= KeySharedMeta.GetHashCode();
        hash ^= subscriptionProperties_.GetHashCode();
        if (HasConsumerEpoch) hash ^= ConsumerEpoch.GetHashCode();
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
      if (HasSubscription) {
        output.WriteRawTag(18);
        output.WriteString(Subscription);
      }
      if (HasSubType) {
        output.WriteRawTag(24);
        output.WriteEnum((int) SubType);
      }
      if (HasConsumerId) {
        output.WriteRawTag(32);
        output.WriteUInt64(ConsumerId);
      }
      if (HasRequestId) {
        output.WriteRawTag(40);
        output.WriteUInt64(RequestId);
      }
      if (HasConsumerName) {
        output.WriteRawTag(50);
        output.WriteString(ConsumerName);
      }
      if (HasPriorityLevel) {
        output.WriteRawTag(56);
        output.WriteInt32(PriorityLevel);
      }
      if (HasDurable) {
        output.WriteRawTag(64);
        output.WriteBool(Durable);
      }
      if (startMessageId_ != null) {
        output.WriteRawTag(74);
        output.WriteMessage(StartMessageId);
      }
      metadata_.WriteTo(output, _repeated_metadata_codec);
      if (HasReadCompacted) {
        output.WriteRawTag(88);
        output.WriteBool(ReadCompacted);
      }
      if (schema_ != null) {
        output.WriteRawTag(98);
        output.WriteMessage(Schema);
      }
      if (HasInitialPosition) {
        output.WriteRawTag(104);
        output.WriteEnum((int) InitialPosition);
      }
      if (HasReplicateSubscriptionState) {
        output.WriteRawTag(112);
        output.WriteBool(ReplicateSubscriptionState);
      }
      if (HasForceTopicCreation) {
        output.WriteRawTag(120);
        output.WriteBool(ForceTopicCreation);
      }
      if (HasStartMessageRollbackDurationSec) {
        output.WriteRawTag(128, 1);
        output.WriteUInt64(StartMessageRollbackDurationSec);
      }
      if (keySharedMeta_ != null) {
        output.WriteRawTag(138, 1);
        output.WriteMessage(KeySharedMeta);
      }
      subscriptionProperties_.WriteTo(output, _repeated_subscriptionProperties_codec);
      if (HasConsumerEpoch) {
        output.WriteRawTag(152, 1);
        output.WriteUInt64(ConsumerEpoch);
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
        if (HasSubscription) {
            output.WriteRawTag(18);
            output.WriteString(Subscription);
        }
        if (HasSubType) {
            output.WriteRawTag(24);
            output.WriteEnum((int) SubType);
        }
        if (HasConsumerId) {
            output.WriteRawTag(32);
            output.WriteUInt64(ConsumerId);
        }
        if (HasRequestId) {
            output.WriteRawTag(40);
            output.WriteUInt64(RequestId);
        }
        if (HasConsumerName) {
            output.WriteRawTag(50);
            output.WriteString(ConsumerName);
        }
        if (HasPriorityLevel) {
            output.WriteRawTag(56);
            output.WriteInt32(PriorityLevel);
        }
        if (HasDurable) {
            output.WriteRawTag(64);
            output.WriteBool(Durable);
        }
        if (startMessageId_ != null) {
            output.WriteRawTag(74);
            output.WriteMessage(StartMessageId);
        }
        metadata_.WriteTo(ref output, _repeated_metadata_codec);
        if (HasReadCompacted) {
            output.WriteRawTag(88);
            output.WriteBool(ReadCompacted);
        }
        if (schema_ != null) {
            output.WriteRawTag(98);
            output.WriteMessage(Schema);
        }
        if (HasInitialPosition) {
            output.WriteRawTag(104);
            output.WriteEnum((int) InitialPosition);
        }
        if (HasReplicateSubscriptionState) {
            output.WriteRawTag(112);
            output.WriteBool(ReplicateSubscriptionState);
        }
        if (HasForceTopicCreation) {
            output.WriteRawTag(120);
            output.WriteBool(ForceTopicCreation);
        }
        if (HasStartMessageRollbackDurationSec) {
            output.WriteRawTag(128, 1);
            output.WriteUInt64(StartMessageRollbackDurationSec);
        }
        if (keySharedMeta_ != null) {
            output.WriteRawTag(138, 1);
            output.WriteMessage(KeySharedMeta);
        }
        subscriptionProperties_.WriteTo(ref output, _repeated_subscriptionProperties_codec);
        if (HasConsumerEpoch) {
            output.WriteRawTag(152, 1);
            output.WriteUInt64(ConsumerEpoch);
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
        if (HasSubscription) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Subscription);
        }
        if (HasSubType) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) SubType);
        }
        if (HasConsumerId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ConsumerId);
        }
        if (HasRequestId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(RequestId);
        }
        if (HasConsumerName) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ConsumerName);
        }
        if (HasPriorityLevel) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(PriorityLevel);
        }
        if (HasDurable) {
            size += 1 + 1;
        }
        if (startMessageId_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(StartMessageId);
        }
        size += metadata_.CalculateSize(_repeated_metadata_codec);
        if (HasReadCompacted) {
            size += 1 + 1;
        }
        if (schema_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Schema);
        }
        if (HasInitialPosition) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) InitialPosition);
        }
        if (HasReplicateSubscriptionState) {
            size += 1 + 1;
        }
        if (HasForceTopicCreation) {
            size += 1 + 1;
        }
        if (HasStartMessageRollbackDurationSec) {
            size += 2 + pb::CodedOutputStream.ComputeUInt64Size(StartMessageRollbackDurationSec);
        }
        if (keySharedMeta_ != null) {
            size += 2 + pb::CodedOutputStream.ComputeMessageSize(KeySharedMeta);
        }
        size += subscriptionProperties_.CalculateSize(_repeated_subscriptionProperties_codec);
        if (HasConsumerEpoch) {
            size += 2 + pb::CodedOutputStream.ComputeUInt64Size(ConsumerEpoch);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandSubscribe other) {
        if (other == null) {
            return;
        }
        if (other.HasTopic) {
            Topic = other.Topic;
        }
        if (other.HasSubscription) {
            Subscription = other.Subscription;
        }
        if (other.HasSubType) {
            SubType = other.SubType;
        }
        if (other.HasConsumerId) {
            ConsumerId = other.ConsumerId;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.HasConsumerName) {
            ConsumerName = other.ConsumerName;
        }
        if (other.HasPriorityLevel) {
            PriorityLevel = other.PriorityLevel;
        }
        if (other.HasDurable) {
            Durable = other.Durable;
        }
        if (other.startMessageId_ != null) {
            if (startMessageId_ == null) {
                StartMessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            StartMessageId.MergeFrom(other.StartMessageId);
        }
        metadata_.Add(other.metadata_);
        if (other.HasReadCompacted) {
            ReadCompacted = other.ReadCompacted;
        }
        if (other.schema_ != null) {
            if (schema_ == null) {
                Schema = new global::DotPulsar.Internal.PulsarApi.Schema();
            }
            Schema.MergeFrom(other.Schema);
        }
        if (other.HasInitialPosition) {
            InitialPosition = other.InitialPosition;
        }
        if (other.HasReplicateSubscriptionState) {
            ReplicateSubscriptionState = other.ReplicateSubscriptionState;
        }
        if (other.HasForceTopicCreation) {
            ForceTopicCreation = other.ForceTopicCreation;
        }
        if (other.HasStartMessageRollbackDurationSec) {
            StartMessageRollbackDurationSec = other.StartMessageRollbackDurationSec;
        }
        if (other.keySharedMeta_ != null) {
            if (keySharedMeta_ == null) {
                KeySharedMeta = new global::DotPulsar.Internal.PulsarApi.KeySharedMeta();
            }
            KeySharedMeta.MergeFrom(other.KeySharedMeta);
        }
        subscriptionProperties_.Add(other.subscriptionProperties_);
        if (other.HasConsumerEpoch) {
            ConsumerEpoch = other.ConsumerEpoch;
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
          case 18: {
            Subscription = input.ReadString();
            break;
          }
          case 24: {
            SubType = (global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.SubType) input.ReadEnum();
            break;
          }
          case 32: {
            ConsumerId = input.ReadUInt64();
            break;
          }
          case 40: {
            RequestId = input.ReadUInt64();
            break;
          }
          case 50: {
            ConsumerName = input.ReadString();
            break;
          }
          case 56: {
            PriorityLevel = input.ReadInt32();
            break;
          }
          case 64: {
            Durable = input.ReadBool();
            break;
          }
          case 74: {
            if (startMessageId_ == null) {
              StartMessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
            }
            input.ReadMessage(StartMessageId);
            break;
          }
          case 82: {
            metadata_.AddEntriesFrom(input, _repeated_metadata_codec);
            break;
          }
          case 88: {
            ReadCompacted = input.ReadBool();
            break;
          }
          case 98: {
            if (schema_ == null) {
              Schema = new global::DotPulsar.Internal.PulsarApi.Schema();
            }
            input.ReadMessage(Schema);
            break;
          }
          case 104: {
            InitialPosition = (global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.InitialPosition) input.ReadEnum();
            break;
          }
          case 112: {
            ReplicateSubscriptionState = input.ReadBool();
            break;
          }
          case 120: {
            ForceTopicCreation = input.ReadBool();
            break;
          }
          case 128: {
            StartMessageRollbackDurationSec = input.ReadUInt64();
            break;
          }
          case 138: {
            if (keySharedMeta_ == null) {
              KeySharedMeta = new global::DotPulsar.Internal.PulsarApi.KeySharedMeta();
            }
            input.ReadMessage(KeySharedMeta);
            break;
          }
          case 146: {
            subscriptionProperties_.AddEntriesFrom(input, _repeated_subscriptionProperties_codec);
            break;
          }
          case 152: {
            ConsumerEpoch = input.ReadUInt64();
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
                case 18: {
                    Subscription = input.ReadString();
                    break;
                }
                case 24: {
                    SubType = (global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.SubType) input.ReadEnum();
                    break;
                }
                case 32: {
                    ConsumerId = input.ReadUInt64();
                    break;
                }
                case 40: {
                    RequestId = input.ReadUInt64();
                    break;
                }
                case 50: {
                    ConsumerName = input.ReadString();
                    break;
                }
                case 56: {
                    PriorityLevel = input.ReadInt32();
                    break;
                }
                case 64: {
                    Durable = input.ReadBool();
                    break;
                }
                case 74: {
                    if (startMessageId_ == null) {
                        StartMessageId = new global::DotPulsar.Internal.PulsarApi.MessageIdData();
                    }
                    input.ReadMessage(StartMessageId);
                    break;
                }
                case 82: {
                    metadata_.AddEntriesFrom(ref input, _repeated_metadata_codec);
                    break;
                }
                case 88: {
                    ReadCompacted = input.ReadBool();
                    break;
                }
                case 98: {
                    if (schema_ == null) {
                        Schema = new global::DotPulsar.Internal.PulsarApi.Schema();
                    }
                    input.ReadMessage(Schema);
                    break;
                }
                case 104: {
                    InitialPosition = (global::DotPulsar.Internal.PulsarApi.CommandSubscribe.Types.InitialPosition) input.ReadEnum();
                    break;
                }
                case 112: {
                    ReplicateSubscriptionState = input.ReadBool();
                    break;
                }
                case 120: {
                    ForceTopicCreation = input.ReadBool();
                    break;
                }
                case 128: {
                    StartMessageRollbackDurationSec = input.ReadUInt64();
                    break;
                }
                case 138: {
                    if (keySharedMeta_ == null) {
                        KeySharedMeta = new global::DotPulsar.Internal.PulsarApi.KeySharedMeta();
                    }
                    input.ReadMessage(KeySharedMeta);
                    break;
                }
                case 146: {
                    subscriptionProperties_.AddEntriesFrom(ref input, _repeated_subscriptionProperties_codec);
                    break;
                }
                case 152: {
                    ConsumerEpoch = input.ReadUInt64();
                    break;
                }
            }
        }
    }
#endif

    #region Nested types
    /// <summary>Container for nested types declared in the CommandSubscribe message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Types {
        public enum SubType {
            [pbr::OriginalName("Exclusive")] Exclusive = 0,
            [pbr::OriginalName("Shared")] Shared = 1,
            [pbr::OriginalName("Failover")] Failover = 2,
            [pbr::OriginalName("Key_Shared")] KeyShared = 3,
        }

        public enum InitialPosition {
            [pbr::OriginalName("Latest")] Latest = 0,
            [pbr::OriginalName("Earliest")] Earliest = 1,
        }

    }
    #endregion

}
