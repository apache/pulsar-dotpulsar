#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

/// <summary>
///&#x2F; Create a new Producer on a topic, assigning the given producer_id,
///&#x2F; all messages sent with this producer_id will be persisted on the topic
/// </summary>
[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandProducer : IMessage<CommandProducer>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandProducer> _parser = new pb::MessageParser<CommandProducer>(() => new CommandProducer());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandProducer> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[21]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandProducer() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandProducer(CommandProducer other) : this() {
        _hasBits0 = other._hasBits0;
        topic_ = other.topic_;
        producerId_ = other.producerId_;
        requestId_ = other.requestId_;
        producerName_ = other.producerName_;
        encrypted_ = other.encrypted_;
        metadata_ = other.metadata_.Clone();
        schema_ = other.schema_ != null ? other.schema_.Clone() : null;
        epoch_ = other.epoch_;
        userProvidedProducerName_ = other.userProvidedProducerName_;
        producerAccessMode_ = other.producerAccessMode_;
        topicEpoch_ = other.topicEpoch_;
        txnEnabled_ = other.txnEnabled_;
        initialSubscriptionName_ = other.initialSubscriptionName_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandProducer Clone() {
        return new CommandProducer(this);
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

    /// <summary>Field number for the "producer_id" field.</summary>
    public const int ProducerIdFieldNumber = 2;
    private readonly static ulong ProducerIdDefaultValue = 0UL;

    private ulong producerId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong ProducerId {
        get { if ((_hasBits0 & 1) != 0) { return producerId_; } else { return ProducerIdDefaultValue; } }
        set {
            _hasBits0 |= 1;
            producerId_ = value;
        }
    }
    /// <summary>Gets whether the "producer_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProducerId {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "producer_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProducerId() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 3;
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

    /// <summary>Field number for the "producer_name" field.</summary>
    public const int ProducerNameFieldNumber = 4;
    private readonly static string ProducerNameDefaultValue = "";

    private string producerName_;
    /// <summary>
    ///&#x2F; If a producer name is specified, the name will be used,
    ///&#x2F; otherwise the broker will generate a unique name
    /// </summary>
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

    /// <summary>Field number for the "encrypted" field.</summary>
    public const int EncryptedFieldNumber = 5;
    private readonly static bool EncryptedDefaultValue = false;

    private bool encrypted_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Encrypted {
        get { if ((_hasBits0 & 4) != 0) { return encrypted_; } else { return EncryptedDefaultValue; } }
        set {
            _hasBits0 |= 4;
            encrypted_ = value;
        }
    }
    /// <summary>Gets whether the "encrypted" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasEncrypted {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "encrypted" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearEncrypted() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "metadata" field.</summary>
    public const int MetadataFieldNumber = 6;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.KeyValue> _repeated_metadata_codec
        = pb::FieldCodec.ForMessage(50, global::DotPulsar.Internal.PulsarApi.KeyValue.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> metadata_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue>();
    /// <summary>
    ///&#x2F; Add optional metadata key=value to this producer
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> Metadata {
        get { return metadata_; }
    }

    /// <summary>Field number for the "schema" field.</summary>
    public const int SchemaFieldNumber = 7;
    private global::DotPulsar.Internal.PulsarApi.Schema schema_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.Schema Schema {
        get { return schema_; }
        set {
            schema_ = value;
        }
    }

    /// <summary>Field number for the "epoch" field.</summary>
    public const int EpochFieldNumber = 8;
    private readonly static ulong EpochDefaultValue = 0UL;

    private ulong epoch_;
    /// <summary>
    /// If producer reconnect to broker, the epoch of this producer will +1
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong Epoch {
        get { if ((_hasBits0 & 8) != 0) { return epoch_; } else { return EpochDefaultValue; } }
        set {
            _hasBits0 |= 8;
            epoch_ = value;
        }
    }
    /// <summary>Gets whether the "epoch" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasEpoch {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "epoch" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearEpoch() {
        _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "user_provided_producer_name" field.</summary>
    public const int UserProvidedProducerNameFieldNumber = 9;
    private readonly static bool UserProvidedProducerNameDefaultValue = true;

    private bool userProvidedProducerName_;
    /// <summary>
    /// Indicate the name of the producer is generated or user provided
    /// Use default true here is in order to be forward compatible with the client
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool UserProvidedProducerName {
        get { if ((_hasBits0 & 16) != 0) { return userProvidedProducerName_; } else { return UserProvidedProducerNameDefaultValue; } }
        set {
            _hasBits0 |= 16;
            userProvidedProducerName_ = value;
        }
    }
    /// <summary>Gets whether the "user_provided_producer_name" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasUserProvidedProducerName {
        get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "user_provided_producer_name" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearUserProvidedProducerName() {
        _hasBits0 &= ~16;
    }

    /// <summary>Field number for the "producer_access_mode" field.</summary>
    public const int ProducerAccessModeFieldNumber = 10;
    private readonly static global::DotPulsar.Internal.PulsarApi.ProducerAccessMode ProducerAccessModeDefaultValue = global::DotPulsar.Internal.PulsarApi.ProducerAccessMode.Shared;

    private global::DotPulsar.Internal.PulsarApi.ProducerAccessMode producerAccessMode_;
    /// <summary>
    /// Require that this producers will be the only producer allowed on the topic
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.ProducerAccessMode ProducerAccessMode {
        get { if ((_hasBits0 & 32) != 0) { return producerAccessMode_; } else { return ProducerAccessModeDefaultValue; } }
        set {
            _hasBits0 |= 32;
            producerAccessMode_ = value;
        }
    }
    /// <summary>Gets whether the "producer_access_mode" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasProducerAccessMode {
        get { return (_hasBits0 & 32) != 0; }
    }
    /// <summary>Clears the value of the "producer_access_mode" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearProducerAccessMode() {
        _hasBits0 &= ~32;
    }

    /// <summary>Field number for the "topic_epoch" field.</summary>
    public const int TopicEpochFieldNumber = 11;
    private readonly static ulong TopicEpochDefaultValue = 0UL;

    private ulong topicEpoch_;
    /// <summary>
    /// Topic epoch is used to fence off producers that reconnects after a new
    /// exclusive producer has already taken over. This id is assigned by the
    /// broker on the CommandProducerSuccess. The first time, the client will
    /// leave it empty and then it will always carry the same epoch number on
    /// the subsequent reconnections.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong TopicEpoch {
        get { if ((_hasBits0 & 64) != 0) { return topicEpoch_; } else { return TopicEpochDefaultValue; } }
        set {
            _hasBits0 |= 64;
            topicEpoch_ = value;
        }
    }
    /// <summary>Gets whether the "topic_epoch" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTopicEpoch {
        get { return (_hasBits0 & 64) != 0; }
    }
    /// <summary>Clears the value of the "topic_epoch" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTopicEpoch() {
        _hasBits0 &= ~64;
    }

    /// <summary>Field number for the "txn_enabled" field.</summary>
    public const int TxnEnabledFieldNumber = 12;
    private readonly static bool TxnEnabledDefaultValue = false;

    private bool txnEnabled_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool TxnEnabled {
        get { if ((_hasBits0 & 128) != 0) { return txnEnabled_; } else { return TxnEnabledDefaultValue; } }
        set {
            _hasBits0 |= 128;
            txnEnabled_ = value;
        }
    }
    /// <summary>Gets whether the "txn_enabled" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTxnEnabled {
        get { return (_hasBits0 & 128) != 0; }
    }
    /// <summary>Clears the value of the "txn_enabled" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTxnEnabled() {
        _hasBits0 &= ~128;
    }

    /// <summary>Field number for the "initial_subscription_name" field.</summary>
    public const int InitialSubscriptionNameFieldNumber = 13;
    private readonly static string InitialSubscriptionNameDefaultValue = "";

    private string initialSubscriptionName_;
    /// <summary>
    /// Name of the initial subscription of the topic.
    /// If this field is not set, the initial subscription will not be created.
    /// If this field is set but the broker's `allowAutoSubscriptionCreation`
    /// is disabled, the producer will fail to be created.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string InitialSubscriptionName {
        get { return initialSubscriptionName_ ?? InitialSubscriptionNameDefaultValue; }
        set {
            initialSubscriptionName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "initial_subscription_name" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasInitialSubscriptionName {
        get { return initialSubscriptionName_ != null; }
    }
    /// <summary>Clears the value of the "initial_subscription_name" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearInitialSubscriptionName() {
        initialSubscriptionName_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandProducer);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandProducer other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (Topic != other.Topic) return false;
        if (ProducerId != other.ProducerId) return false;
        if (RequestId != other.RequestId) return false;
        if (ProducerName != other.ProducerName) return false;
        if (Encrypted != other.Encrypted) return false;
        if(!metadata_.Equals(other.metadata_)) return false;
        if (!object.Equals(Schema, other.Schema)) return false;
        if (Epoch != other.Epoch) return false;
        if (UserProvidedProducerName != other.UserProvidedProducerName) return false;
        if (ProducerAccessMode != other.ProducerAccessMode) return false;
        if (TopicEpoch != other.TopicEpoch) return false;
        if (TxnEnabled != other.TxnEnabled) return false;
        if (InitialSubscriptionName != other.InitialSubscriptionName) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasTopic) hash ^= Topic.GetHashCode();
        if (HasProducerId) hash ^= ProducerId.GetHashCode();
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasProducerName) hash ^= ProducerName.GetHashCode();
        if (HasEncrypted) hash ^= Encrypted.GetHashCode();
        hash ^= metadata_.GetHashCode();
        if (schema_ != null) hash ^= Schema.GetHashCode();
        if (HasEpoch) hash ^= Epoch.GetHashCode();
        if (HasUserProvidedProducerName) hash ^= UserProvidedProducerName.GetHashCode();
        if (HasProducerAccessMode) hash ^= ProducerAccessMode.GetHashCode();
        if (HasTopicEpoch) hash ^= TopicEpoch.GetHashCode();
        if (HasTxnEnabled) hash ^= TxnEnabled.GetHashCode();
        if (HasInitialSubscriptionName) hash ^= InitialSubscriptionName.GetHashCode();
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
      if (HasProducerId) {
        output.WriteRawTag(16);
        output.WriteUInt64(ProducerId);
      }
      if (HasRequestId) {
        output.WriteRawTag(24);
        output.WriteUInt64(RequestId);
      }
      if (HasProducerName) {
        output.WriteRawTag(34);
        output.WriteString(ProducerName);
      }
      if (HasEncrypted) {
        output.WriteRawTag(40);
        output.WriteBool(Encrypted);
      }
      metadata_.WriteTo(output, _repeated_metadata_codec);
      if (schema_ != null) {
        output.WriteRawTag(58);
        output.WriteMessage(Schema);
      }
      if (HasEpoch) {
        output.WriteRawTag(64);
        output.WriteUInt64(Epoch);
      }
      if (HasUserProvidedProducerName) {
        output.WriteRawTag(72);
        output.WriteBool(UserProvidedProducerName);
      }
      if (HasProducerAccessMode) {
        output.WriteRawTag(80);
        output.WriteEnum((int) ProducerAccessMode);
      }
      if (HasTopicEpoch) {
        output.WriteRawTag(88);
        output.WriteUInt64(TopicEpoch);
      }
      if (HasTxnEnabled) {
        output.WriteRawTag(96);
        output.WriteBool(TxnEnabled);
      }
      if (HasInitialSubscriptionName) {
        output.WriteRawTag(106);
        output.WriteString(InitialSubscriptionName);
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
        if (HasProducerId) {
            output.WriteRawTag(16);
            output.WriteUInt64(ProducerId);
        }
        if (HasRequestId) {
            output.WriteRawTag(24);
            output.WriteUInt64(RequestId);
        }
        if (HasProducerName) {
            output.WriteRawTag(34);
            output.WriteString(ProducerName);
        }
        if (HasEncrypted) {
            output.WriteRawTag(40);
            output.WriteBool(Encrypted);
        }
        metadata_.WriteTo(ref output, _repeated_metadata_codec);
        if (schema_ != null) {
            output.WriteRawTag(58);
            output.WriteMessage(Schema);
        }
        if (HasEpoch) {
            output.WriteRawTag(64);
            output.WriteUInt64(Epoch);
        }
        if (HasUserProvidedProducerName) {
            output.WriteRawTag(72);
            output.WriteBool(UserProvidedProducerName);
        }
        if (HasProducerAccessMode) {
            output.WriteRawTag(80);
            output.WriteEnum((int) ProducerAccessMode);
        }
        if (HasTopicEpoch) {
            output.WriteRawTag(88);
            output.WriteUInt64(TopicEpoch);
        }
        if (HasTxnEnabled) {
            output.WriteRawTag(96);
            output.WriteBool(TxnEnabled);
        }
        if (HasInitialSubscriptionName) {
            output.WriteRawTag(106);
            output.WriteString(InitialSubscriptionName);
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
        if (HasProducerId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ProducerId);
        }
        if (HasRequestId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(RequestId);
        }
        if (HasProducerName) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ProducerName);
        }
        if (HasEncrypted) {
            size += 1 + 1;
        }
        size += metadata_.CalculateSize(_repeated_metadata_codec);
        if (schema_ != null) {
            size += 1 + pb::CodedOutputStream.ComputeMessageSize(Schema);
        }
        if (HasEpoch) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Epoch);
        }
        if (HasUserProvidedProducerName) {
            size += 1 + 1;
        }
        if (HasProducerAccessMode) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ProducerAccessMode);
        }
        if (HasTopicEpoch) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(TopicEpoch);
        }
        if (HasTxnEnabled) {
            size += 1 + 1;
        }
        if (HasInitialSubscriptionName) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(InitialSubscriptionName);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandProducer other) {
        if (other == null) {
            return;
        }
        if (other.HasTopic) {
            Topic = other.Topic;
        }
        if (other.HasProducerId) {
            ProducerId = other.ProducerId;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.HasProducerName) {
            ProducerName = other.ProducerName;
        }
        if (other.HasEncrypted) {
            Encrypted = other.Encrypted;
        }
        metadata_.Add(other.metadata_);
        if (other.schema_ != null) {
            if (schema_ == null) {
                Schema = new global::DotPulsar.Internal.PulsarApi.Schema();
            }
            Schema.MergeFrom(other.Schema);
        }
        if (other.HasEpoch) {
            Epoch = other.Epoch;
        }
        if (other.HasUserProvidedProducerName) {
            UserProvidedProducerName = other.UserProvidedProducerName;
        }
        if (other.HasProducerAccessMode) {
            ProducerAccessMode = other.ProducerAccessMode;
        }
        if (other.HasTopicEpoch) {
            TopicEpoch = other.TopicEpoch;
        }
        if (other.HasTxnEnabled) {
            TxnEnabled = other.TxnEnabled;
        }
        if (other.HasInitialSubscriptionName) {
            InitialSubscriptionName = other.InitialSubscriptionName;
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
            ProducerId = input.ReadUInt64();
            break;
          }
          case 24: {
            RequestId = input.ReadUInt64();
            break;
          }
          case 34: {
            ProducerName = input.ReadString();
            break;
          }
          case 40: {
            Encrypted = input.ReadBool();
            break;
          }
          case 50: {
            metadata_.AddEntriesFrom(input, _repeated_metadata_codec);
            break;
          }
          case 58: {
            if (schema_ == null) {
              Schema = new global::DotPulsar.Internal.PulsarApi.Schema();
            }
            input.ReadMessage(Schema);
            break;
          }
          case 64: {
            Epoch = input.ReadUInt64();
            break;
          }
          case 72: {
            UserProvidedProducerName = input.ReadBool();
            break;
          }
          case 80: {
            ProducerAccessMode = (global::DotPulsar.Internal.PulsarApi.ProducerAccessMode) input.ReadEnum();
            break;
          }
          case 88: {
            TopicEpoch = input.ReadUInt64();
            break;
          }
          case 96: {
            TxnEnabled = input.ReadBool();
            break;
          }
          case 106: {
            InitialSubscriptionName = input.ReadString();
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
                    ProducerId = input.ReadUInt64();
                    break;
                }
                case 24: {
                    RequestId = input.ReadUInt64();
                    break;
                }
                case 34: {
                    ProducerName = input.ReadString();
                    break;
                }
                case 40: {
                    Encrypted = input.ReadBool();
                    break;
                }
                case 50: {
                    metadata_.AddEntriesFrom(ref input, _repeated_metadata_codec);
                    break;
                }
                case 58: {
                    if (schema_ == null) {
                        Schema = new global::DotPulsar.Internal.PulsarApi.Schema();
                    }
                    input.ReadMessage(Schema);
                    break;
                }
                case 64: {
                    Epoch = input.ReadUInt64();
                    break;
                }
                case 72: {
                    UserProvidedProducerName = input.ReadBool();
                    break;
                }
                case 80: {
                    ProducerAccessMode = (global::DotPulsar.Internal.PulsarApi.ProducerAccessMode) input.ReadEnum();
                    break;
                }
                case 88: {
                    TopicEpoch = input.ReadUInt64();
                    break;
                }
                case 96: {
                    TxnEnabled = input.ReadBool();
                    break;
                }
                case 106: {
                    InitialSubscriptionName = input.ReadString();
                    break;
                }
            }
        }
    }
#endif

}
