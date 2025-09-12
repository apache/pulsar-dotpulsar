#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbc = Google.Protobuf.Collections;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class MessageMetadata : IMessage<MessageMetadata>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<MessageMetadata> _parser = new pb::MessageParser<MessageMetadata>(() => new MessageMetadata());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<MessageMetadata> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[6]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public MessageMetadata() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public MessageMetadata(MessageMetadata other) : this() {
        _hasBits0 = other._hasBits0;
        producerName_ = other.producerName_;
        sequenceId_ = other.sequenceId_;
        publishTime_ = other.publishTime_;
        properties_ = other.properties_.Clone();
        replicatedFrom_ = other.replicatedFrom_;
        partitionKey_ = other.partitionKey_;
        replicateTo_ = other.replicateTo_.Clone();
        compression_ = other.compression_;
        uncompressedSize_ = other.uncompressedSize_;
        numMessagesInBatch_ = other.numMessagesInBatch_;
        eventTime_ = other.eventTime_;
        encryptionKeys_ = other.encryptionKeys_.Clone();
        encryptionAlgo_ = other.encryptionAlgo_;
        encryptionParam_ = other.encryptionParam_;
        schemaVersion_ = other.schemaVersion_;
        partitionKeyB64Encoded_ = other.partitionKeyB64Encoded_;
        orderingKey_ = other.orderingKey_;
        deliverAtTime_ = other.deliverAtTime_;
        markerType_ = other.markerType_;
        txnidLeastBits_ = other.txnidLeastBits_;
        txnidMostBits_ = other.txnidMostBits_;
        highestSequenceId_ = other.highestSequenceId_;
        nullValue_ = other.nullValue_;
        uuid_ = other.uuid_;
        numChunksFromMsg_ = other.numChunksFromMsg_;
        totalChunkMsgSize_ = other.totalChunkMsgSize_;
        chunkId_ = other.chunkId_;
        nullPartitionKey_ = other.nullPartitionKey_;
        compactedBatchIndexes_ = other.compactedBatchIndexes_.Clone();
        schemaId_ = other.schemaId_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public MessageMetadata Clone() {
        return new MessageMetadata(this);
    }

    /// <summary>Field number for the "producer_name" field.</summary>
    public const int ProducerNameFieldNumber = 1;
    private readonly static string ProducerNameDefaultValue = "";

    private string producerName_;
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

    /// <summary>Field number for the "sequence_id" field.</summary>
    public const int SequenceIdFieldNumber = 2;
    private readonly static ulong SequenceIdDefaultValue = 0UL;

    private ulong sequenceId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong SequenceId {
        get { if ((_hasBits0 & 1) != 0) { return sequenceId_; } else { return SequenceIdDefaultValue; } }
        set {
            _hasBits0 |= 1;
            sequenceId_ = value;
        }
    }
    /// <summary>Gets whether the "sequence_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSequenceId {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "sequence_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSequenceId() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "publish_time" field.</summary>
    public const int PublishTimeFieldNumber = 3;
    private readonly static ulong PublishTimeDefaultValue = 0UL;

    private ulong publishTime_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong PublishTime {
        get { if ((_hasBits0 & 2) != 0) { return publishTime_; } else { return PublishTimeDefaultValue; } }
        set {
            _hasBits0 |= 2;
            publishTime_ = value;
        }
    }
    /// <summary>Gets whether the "publish_time" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasPublishTime {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "publish_time" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearPublishTime() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "properties" field.</summary>
    public const int PropertiesFieldNumber = 4;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.KeyValue> _repeated_properties_codec
        = pb::FieldCodec.ForMessage(34, global::DotPulsar.Internal.PulsarApi.KeyValue.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> properties_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.KeyValue> Properties {
        get { return properties_; }
    }

    /// <summary>Field number for the "replicated_from" field.</summary>
    public const int ReplicatedFromFieldNumber = 5;
    private readonly static string ReplicatedFromDefaultValue = "";

    private string replicatedFrom_;
    /// <summary>
    /// Property set on replicated message,
    /// includes the source cluster name
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string ReplicatedFrom {
        get { return replicatedFrom_ ?? ReplicatedFromDefaultValue; }
        set {
            replicatedFrom_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "replicated_from" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasReplicatedFrom {
        get { return replicatedFrom_ != null; }
    }
    /// <summary>Clears the value of the "replicated_from" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearReplicatedFrom() {
        replicatedFrom_ = null;
    }

    /// <summary>Field number for the "partition_key" field.</summary>
    public const int PartitionKeyFieldNumber = 6;
    private readonly static string PartitionKeyDefaultValue = "";

    private string partitionKey_;
    /// <summary>
    ///key to decide partition for the msg
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string PartitionKey {
        get { return partitionKey_ ?? PartitionKeyDefaultValue; }
        set {
            partitionKey_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "partition_key" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasPartitionKey {
        get { return partitionKey_ != null; }
    }
    /// <summary>Clears the value of the "partition_key" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearPartitionKey() {
        partitionKey_ = null;
    }

    /// <summary>Field number for the "replicate_to" field.</summary>
    public const int ReplicateToFieldNumber = 7;
    private static readonly pb::FieldCodec<string> _repeated_replicateTo_codec
        = pb::FieldCodec.ForString(58);
    private readonly pbc::RepeatedField<string> replicateTo_ = new pbc::RepeatedField<string>();
    /// <summary>
    /// Override namespace's replication
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<string> ReplicateTo {
        get { return replicateTo_; }
    }

    /// <summary>Field number for the "compression" field.</summary>
    public const int CompressionFieldNumber = 8;
    private readonly static global::DotPulsar.Internal.PulsarApi.CompressionType CompressionDefaultValue = global::DotPulsar.Internal.PulsarApi.CompressionType.None;

    private global::DotPulsar.Internal.PulsarApi.CompressionType compression_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CompressionType Compression {
        get { if ((_hasBits0 & 4) != 0) { return compression_; } else { return CompressionDefaultValue; } }
        set {
            _hasBits0 |= 4;
            compression_ = value;
        }
    }
    /// <summary>Gets whether the "compression" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasCompression {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "compression" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearCompression() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "uncompressed_size" field.</summary>
    public const int UncompressedSizeFieldNumber = 9;
    private readonly static uint UncompressedSizeDefaultValue = 0;

    private uint uncompressedSize_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint UncompressedSize {
        get { if ((_hasBits0 & 8) != 0) { return uncompressedSize_; } else { return UncompressedSizeDefaultValue; } }
        set {
            _hasBits0 |= 8;
            uncompressedSize_ = value;
        }
    }
    /// <summary>Gets whether the "uncompressed_size" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasUncompressedSize {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "uncompressed_size" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearUncompressedSize() {
        _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "num_messages_in_batch" field.</summary>
    public const int NumMessagesInBatchFieldNumber = 11;
    private readonly static int NumMessagesInBatchDefaultValue = 1;

    private int numMessagesInBatch_;
    /// <summary>
    /// Removed below checksum field from Metadata as
    /// it should be part of send-command which keeps checksum of header + payload
    ///optional sfixed64 checksum = 10;
    /// differentiate single and batch message metadata
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int NumMessagesInBatch {
        get { if ((_hasBits0 & 16) != 0) { return numMessagesInBatch_; } else { return NumMessagesInBatchDefaultValue; } }
        set {
            _hasBits0 |= 16;
            numMessagesInBatch_ = value;
        }
    }
    /// <summary>Gets whether the "num_messages_in_batch" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNumMessagesInBatch {
        get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "num_messages_in_batch" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNumMessagesInBatch() {
        _hasBits0 &= ~16;
    }

    /// <summary>Field number for the "event_time" field.</summary>
    public const int EventTimeFieldNumber = 12;
    private readonly static ulong EventTimeDefaultValue = 0UL;

    private ulong eventTime_;
    /// <summary>
    /// the timestamp that this event occurs. it is typically set by applications.
    /// if this field is omitted, `publish_time` can be used for the purpose of `event_time`.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong EventTime {
        get { if ((_hasBits0 & 32) != 0) { return eventTime_; } else { return EventTimeDefaultValue; } }
        set {
            _hasBits0 |= 32;
            eventTime_ = value;
        }
    }
    /// <summary>Gets whether the "event_time" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasEventTime {
        get { return (_hasBits0 & 32) != 0; }
    }
    /// <summary>Clears the value of the "event_time" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearEventTime() {
        _hasBits0 &= ~32;
    }

    /// <summary>Field number for the "encryption_keys" field.</summary>
    public const int EncryptionKeysFieldNumber = 13;
    private static readonly pb::FieldCodec<global::DotPulsar.Internal.PulsarApi.EncryptionKeys> _repeated_encryptionKeys_codec
        = pb::FieldCodec.ForMessage(106, global::DotPulsar.Internal.PulsarApi.EncryptionKeys.Parser);
    private readonly pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.EncryptionKeys> encryptionKeys_ = new pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.EncryptionKeys>();
    /// <summary>
    /// Contains encryption key name, encrypted key and metadata to describe the key
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::DotPulsar.Internal.PulsarApi.EncryptionKeys> EncryptionKeys {
        get { return encryptionKeys_; }
    }

    /// <summary>Field number for the "encryption_algo" field.</summary>
    public const int EncryptionAlgoFieldNumber = 14;
    private readonly static string EncryptionAlgoDefaultValue = "";

    private string encryptionAlgo_;
    /// <summary>
    /// Algorithm used to encrypt data key
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string EncryptionAlgo {
        get { return encryptionAlgo_ ?? EncryptionAlgoDefaultValue; }
        set {
            encryptionAlgo_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "encryption_algo" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasEncryptionAlgo {
        get { return encryptionAlgo_ != null; }
    }
    /// <summary>Clears the value of the "encryption_algo" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearEncryptionAlgo() {
        encryptionAlgo_ = null;
    }

    /// <summary>Field number for the "encryption_param" field.</summary>
    public const int EncryptionParamFieldNumber = 15;
    private readonly static pb::ByteString EncryptionParamDefaultValue = pb::ByteString.Empty;

    private pb::ByteString encryptionParam_;
    /// <summary>
    /// Additional parameters required by encryption
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString EncryptionParam {
        get { return encryptionParam_ ?? EncryptionParamDefaultValue; }
        set {
            encryptionParam_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "encryption_param" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasEncryptionParam {
        get { return encryptionParam_ != null; }
    }
    /// <summary>Clears the value of the "encryption_param" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearEncryptionParam() {
        encryptionParam_ = null;
    }

    /// <summary>Field number for the "schema_version" field.</summary>
    public const int SchemaVersionFieldNumber = 16;
    private readonly static pb::ByteString SchemaVersionDefaultValue = pb::ByteString.Empty;

    private pb::ByteString schemaVersion_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString SchemaVersion {
        get { return schemaVersion_ ?? SchemaVersionDefaultValue; }
        set {
            schemaVersion_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "schema_version" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSchemaVersion {
        get { return schemaVersion_ != null; }
    }
    /// <summary>Clears the value of the "schema_version" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSchemaVersion() {
        schemaVersion_ = null;
    }

    /// <summary>Field number for the "partition_key_b64_encoded" field.</summary>
    public const int PartitionKeyB64EncodedFieldNumber = 17;
    private readonly static bool PartitionKeyB64EncodedDefaultValue = false;

    private bool partitionKeyB64Encoded_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool PartitionKeyB64Encoded {
        get { if ((_hasBits0 & 64) != 0) { return partitionKeyB64Encoded_; } else { return PartitionKeyB64EncodedDefaultValue; } }
        set {
            _hasBits0 |= 64;
            partitionKeyB64Encoded_ = value;
        }
    }
    /// <summary>Gets whether the "partition_key_b64_encoded" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasPartitionKeyB64Encoded {
        get { return (_hasBits0 & 64) != 0; }
    }
    /// <summary>Clears the value of the "partition_key_b64_encoded" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearPartitionKeyB64Encoded() {
        _hasBits0 &= ~64;
    }

    /// <summary>Field number for the "ordering_key" field.</summary>
    public const int OrderingKeyFieldNumber = 18;
    private readonly static pb::ByteString OrderingKeyDefaultValue = pb::ByteString.Empty;

    private pb::ByteString orderingKey_;
    /// <summary>
    /// Specific a key to overwrite the message key which used for ordering dispatch in Key_Shared mode.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString OrderingKey {
        get { return orderingKey_ ?? OrderingKeyDefaultValue; }
        set {
            orderingKey_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "ordering_key" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasOrderingKey {
        get { return orderingKey_ != null; }
    }
    /// <summary>Clears the value of the "ordering_key" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearOrderingKey() {
        orderingKey_ = null;
    }

    /// <summary>Field number for the "deliver_at_time" field.</summary>
    public const int DeliverAtTimeFieldNumber = 19;
    private readonly static long DeliverAtTimeDefaultValue = 0L;

    private long deliverAtTime_;
    /// <summary>
    /// Mark the message to be delivered at or after the specified timestamp
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public long DeliverAtTime {
        get { if ((_hasBits0 & 128) != 0) { return deliverAtTime_; } else { return DeliverAtTimeDefaultValue; } }
        set {
            _hasBits0 |= 128;
            deliverAtTime_ = value;
        }
    }
    /// <summary>Gets whether the "deliver_at_time" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasDeliverAtTime {
        get { return (_hasBits0 & 128) != 0; }
    }
    /// <summary>Clears the value of the "deliver_at_time" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearDeliverAtTime() {
        _hasBits0 &= ~128;
    }

    /// <summary>Field number for the "marker_type" field.</summary>
    public const int MarkerTypeFieldNumber = 20;
    private readonly static int MarkerTypeDefaultValue = 0;

    private int markerType_;
    /// <summary>
    /// Identify whether a message is a "marker" message used for
    /// internal metadata instead of application published data.
    /// Markers will generally not be propagated back to clients
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int MarkerType {
        get { if ((_hasBits0 & 256) != 0) { return markerType_; } else { return MarkerTypeDefaultValue; } }
        set {
            _hasBits0 |= 256;
            markerType_ = value;
        }
    }
    /// <summary>Gets whether the "marker_type" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasMarkerType {
        get { return (_hasBits0 & 256) != 0; }
    }
    /// <summary>Clears the value of the "marker_type" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearMarkerType() {
        _hasBits0 &= ~256;
    }

    /// <summary>Field number for the "txnid_least_bits" field.</summary>
    public const int TxnidLeastBitsFieldNumber = 22;
    private readonly static ulong TxnidLeastBitsDefaultValue = 0UL;

    private ulong txnidLeastBits_;
    /// <summary>
    /// transaction related message info
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong TxnidLeastBits {
        get { if ((_hasBits0 & 512) != 0) { return txnidLeastBits_; } else { return TxnidLeastBitsDefaultValue; } }
        set {
            _hasBits0 |= 512;
            txnidLeastBits_ = value;
        }
    }
    /// <summary>Gets whether the "txnid_least_bits" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTxnidLeastBits {
        get { return (_hasBits0 & 512) != 0; }
    }
    /// <summary>Clears the value of the "txnid_least_bits" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTxnidLeastBits() {
        _hasBits0 &= ~512;
    }

    /// <summary>Field number for the "txnid_most_bits" field.</summary>
    public const int TxnidMostBitsFieldNumber = 23;
    private readonly static ulong TxnidMostBitsDefaultValue = 0UL;

    private ulong txnidMostBits_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong TxnidMostBits {
        get { if ((_hasBits0 & 1024) != 0) { return txnidMostBits_; } else { return TxnidMostBitsDefaultValue; } }
        set {
            _hasBits0 |= 1024;
            txnidMostBits_ = value;
        }
    }
    /// <summary>Gets whether the "txnid_most_bits" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTxnidMostBits {
        get { return (_hasBits0 & 1024) != 0; }
    }
    /// <summary>Clears the value of the "txnid_most_bits" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTxnidMostBits() {
        _hasBits0 &= ~1024;
    }

    /// <summary>Field number for the "highest_sequence_id" field.</summary>
    public const int HighestSequenceIdFieldNumber = 24;
    private readonly static ulong HighestSequenceIdDefaultValue = 0UL;

    private ulong highestSequenceId_;
    /// <summary>
    ///&#x2F; Add highest sequence id to support batch message with external sequence id
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ulong HighestSequenceId {
        get { if ((_hasBits0 & 2048) != 0) { return highestSequenceId_; } else { return HighestSequenceIdDefaultValue; } }
        set {
            _hasBits0 |= 2048;
            highestSequenceId_ = value;
        }
    }
    /// <summary>Gets whether the "highest_sequence_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasHighestSequenceId {
        get { return (_hasBits0 & 2048) != 0; }
    }
    /// <summary>Clears the value of the "highest_sequence_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearHighestSequenceId() {
        _hasBits0 &= ~2048;
    }

    /// <summary>Field number for the "null_value" field.</summary>
    public const int NullValueFieldNumber = 25;
    private readonly static bool NullValueDefaultValue = false;

    private bool nullValue_;
    /// <summary>
    /// Indicate if the message payload value is set
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool NullValue {
        get { if ((_hasBits0 & 4096) != 0) { return nullValue_; } else { return NullValueDefaultValue; } }
        set {
            _hasBits0 |= 4096;
            nullValue_ = value;
        }
    }
    /// <summary>Gets whether the "null_value" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNullValue {
        get { return (_hasBits0 & 4096) != 0; }
    }
    /// <summary>Clears the value of the "null_value" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNullValue() {
        _hasBits0 &= ~4096;
    }

    /// <summary>Field number for the "uuid" field.</summary>
    public const int UuidFieldNumber = 26;
    private readonly static string UuidDefaultValue = "";

    private string uuid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Uuid {
        get { return uuid_ ?? UuidDefaultValue; }
        set {
            uuid_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "uuid" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasUuid {
        get { return uuid_ != null; }
    }
    /// <summary>Clears the value of the "uuid" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearUuid() {
        uuid_ = null;
    }

    /// <summary>Field number for the "num_chunks_from_msg" field.</summary>
    public const int NumChunksFromMsgFieldNumber = 27;
    private readonly static int NumChunksFromMsgDefaultValue = 0;

    private int numChunksFromMsg_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int NumChunksFromMsg {
        get { if ((_hasBits0 & 8192) != 0) { return numChunksFromMsg_; } else { return NumChunksFromMsgDefaultValue; } }
        set {
            _hasBits0 |= 8192;
            numChunksFromMsg_ = value;
        }
    }
    /// <summary>Gets whether the "num_chunks_from_msg" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNumChunksFromMsg {
        get { return (_hasBits0 & 8192) != 0; }
    }
    /// <summary>Clears the value of the "num_chunks_from_msg" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNumChunksFromMsg() {
        _hasBits0 &= ~8192;
    }

    /// <summary>Field number for the "total_chunk_msg_size" field.</summary>
    public const int TotalChunkMsgSizeFieldNumber = 28;
    private readonly static int TotalChunkMsgSizeDefaultValue = 0;

    private int totalChunkMsgSize_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int TotalChunkMsgSize {
        get { if ((_hasBits0 & 16384) != 0) { return totalChunkMsgSize_; } else { return TotalChunkMsgSizeDefaultValue; } }
        set {
            _hasBits0 |= 16384;
            totalChunkMsgSize_ = value;
        }
    }
    /// <summary>Gets whether the "total_chunk_msg_size" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasTotalChunkMsgSize {
        get { return (_hasBits0 & 16384) != 0; }
    }
    /// <summary>Clears the value of the "total_chunk_msg_size" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearTotalChunkMsgSize() {
        _hasBits0 &= ~16384;
    }

    /// <summary>Field number for the "chunk_id" field.</summary>
    public const int ChunkIdFieldNumber = 29;
    private readonly static int ChunkIdDefaultValue = 0;

    private int chunkId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int ChunkId {
        get { if ((_hasBits0 & 32768) != 0) { return chunkId_; } else { return ChunkIdDefaultValue; } }
        set {
            _hasBits0 |= 32768;
            chunkId_ = value;
        }
    }
    /// <summary>Gets whether the "chunk_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasChunkId {
        get { return (_hasBits0 & 32768) != 0; }
    }
    /// <summary>Clears the value of the "chunk_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearChunkId() {
        _hasBits0 &= ~32768;
    }

    /// <summary>Field number for the "null_partition_key" field.</summary>
    public const int NullPartitionKeyFieldNumber = 30;
    private readonly static bool NullPartitionKeyDefaultValue = false;

    private bool nullPartitionKey_;
    /// <summary>
    /// Indicate if the message partition key is set
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool NullPartitionKey {
        get { if ((_hasBits0 & 65536) != 0) { return nullPartitionKey_; } else { return NullPartitionKeyDefaultValue; } }
        set {
            _hasBits0 |= 65536;
            nullPartitionKey_ = value;
        }
    }
    /// <summary>Gets whether the "null_partition_key" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasNullPartitionKey {
        get { return (_hasBits0 & 65536) != 0; }
    }
    /// <summary>Clears the value of the "null_partition_key" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearNullPartitionKey() {
        _hasBits0 &= ~65536;
    }

    /// <summary>Field number for the "compacted_batch_indexes" field.</summary>
    public const int CompactedBatchIndexesFieldNumber = 31;
    private static readonly pb::FieldCodec<int> _repeated_compactedBatchIndexes_codec
        = pb::FieldCodec.ForInt32(248);
    private readonly pbc::RepeatedField<int> compactedBatchIndexes_ = new pbc::RepeatedField<int>();
    /// <summary>
    /// Indicates the indexes of messages retained in the batch after compaction. When a batch is compacted,
    /// some messages may be removed (compacted out). For example, if the original batch contains:
    /// `k0 => v0, k1 => v1, k2 => v2, k1 => null`, the compacted batch will retain only `k0 => v0` and `k2 => v2`.
    /// In this case, this field will be set to `[0, 2]`, and the payload buffer will only include the retained messages.
    ///
    /// Note: Batches compacted by older versions of the compaction service do not include this field. For such batches,
    /// the `compacted_out` field in `SingleMessageMetadata` must be checked to identify and filter out compacted
    /// messages (e.g., `k1 => v1` and `k1 => null` in the example above).
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<int> CompactedBatchIndexes {
        get { return compactedBatchIndexes_; }
    }

    /// <summary>Field number for the "schema_id" field.</summary>
    public const int SchemaIdFieldNumber = 32;
    private readonly static pb::ByteString SchemaIdDefaultValue = pb::ByteString.Empty;

    private pb::ByteString schemaId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pb::ByteString SchemaId {
        get { return schemaId_ ?? SchemaIdDefaultValue; }
        set {
            schemaId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
    }
    /// <summary>Gets whether the "schema_id" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSchemaId {
        get { return schemaId_ != null; }
    }
    /// <summary>Clears the value of the "schema_id" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSchemaId() {
        schemaId_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as MessageMetadata);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(MessageMetadata other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (ProducerName != other.ProducerName) return false;
        if (SequenceId != other.SequenceId) return false;
        if (PublishTime != other.PublishTime) return false;
        if(!properties_.Equals(other.properties_)) return false;
        if (ReplicatedFrom != other.ReplicatedFrom) return false;
        if (PartitionKey != other.PartitionKey) return false;
        if(!replicateTo_.Equals(other.replicateTo_)) return false;
        if (Compression != other.Compression) return false;
        if (UncompressedSize != other.UncompressedSize) return false;
        if (NumMessagesInBatch != other.NumMessagesInBatch) return false;
        if (EventTime != other.EventTime) return false;
        if(!encryptionKeys_.Equals(other.encryptionKeys_)) return false;
        if (EncryptionAlgo != other.EncryptionAlgo) return false;
        if (EncryptionParam != other.EncryptionParam) return false;
        if (SchemaVersion != other.SchemaVersion) return false;
        if (PartitionKeyB64Encoded != other.PartitionKeyB64Encoded) return false;
        if (OrderingKey != other.OrderingKey) return false;
        if (DeliverAtTime != other.DeliverAtTime) return false;
        if (MarkerType != other.MarkerType) return false;
        if (TxnidLeastBits != other.TxnidLeastBits) return false;
        if (TxnidMostBits != other.TxnidMostBits) return false;
        if (HighestSequenceId != other.HighestSequenceId) return false;
        if (NullValue != other.NullValue) return false;
        if (Uuid != other.Uuid) return false;
        if (NumChunksFromMsg != other.NumChunksFromMsg) return false;
        if (TotalChunkMsgSize != other.TotalChunkMsgSize) return false;
        if (ChunkId != other.ChunkId) return false;
        if (NullPartitionKey != other.NullPartitionKey) return false;
        if(!compactedBatchIndexes_.Equals(other.compactedBatchIndexes_)) return false;
        if (SchemaId != other.SchemaId) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasProducerName) hash ^= ProducerName.GetHashCode();
        if (HasSequenceId) hash ^= SequenceId.GetHashCode();
        if (HasPublishTime) hash ^= PublishTime.GetHashCode();
        hash ^= properties_.GetHashCode();
        if (HasReplicatedFrom) hash ^= ReplicatedFrom.GetHashCode();
        if (HasPartitionKey) hash ^= PartitionKey.GetHashCode();
        hash ^= replicateTo_.GetHashCode();
        if (HasCompression) hash ^= Compression.GetHashCode();
        if (HasUncompressedSize) hash ^= UncompressedSize.GetHashCode();
        if (HasNumMessagesInBatch) hash ^= NumMessagesInBatch.GetHashCode();
        if (HasEventTime) hash ^= EventTime.GetHashCode();
        hash ^= encryptionKeys_.GetHashCode();
        if (HasEncryptionAlgo) hash ^= EncryptionAlgo.GetHashCode();
        if (HasEncryptionParam) hash ^= EncryptionParam.GetHashCode();
        if (HasSchemaVersion) hash ^= SchemaVersion.GetHashCode();
        if (HasPartitionKeyB64Encoded) hash ^= PartitionKeyB64Encoded.GetHashCode();
        if (HasOrderingKey) hash ^= OrderingKey.GetHashCode();
        if (HasDeliverAtTime) hash ^= DeliverAtTime.GetHashCode();
        if (HasMarkerType) hash ^= MarkerType.GetHashCode();
        if (HasTxnidLeastBits) hash ^= TxnidLeastBits.GetHashCode();
        if (HasTxnidMostBits) hash ^= TxnidMostBits.GetHashCode();
        if (HasHighestSequenceId) hash ^= HighestSequenceId.GetHashCode();
        if (HasNullValue) hash ^= NullValue.GetHashCode();
        if (HasUuid) hash ^= Uuid.GetHashCode();
        if (HasNumChunksFromMsg) hash ^= NumChunksFromMsg.GetHashCode();
        if (HasTotalChunkMsgSize) hash ^= TotalChunkMsgSize.GetHashCode();
        if (HasChunkId) hash ^= ChunkId.GetHashCode();
        if (HasNullPartitionKey) hash ^= NullPartitionKey.GetHashCode();
        hash ^= compactedBatchIndexes_.GetHashCode();
        if (HasSchemaId) hash ^= SchemaId.GetHashCode();
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
      if (HasProducerName) {
        output.WriteRawTag(10);
        output.WriteString(ProducerName);
      }
      if (HasSequenceId) {
        output.WriteRawTag(16);
        output.WriteUInt64(SequenceId);
      }
      if (HasPublishTime) {
        output.WriteRawTag(24);
        output.WriteUInt64(PublishTime);
      }
      properties_.WriteTo(output, _repeated_properties_codec);
      if (HasReplicatedFrom) {
        output.WriteRawTag(42);
        output.WriteString(ReplicatedFrom);
      }
      if (HasPartitionKey) {
        output.WriteRawTag(50);
        output.WriteString(PartitionKey);
      }
      replicateTo_.WriteTo(output, _repeated_replicateTo_codec);
      if (HasCompression) {
        output.WriteRawTag(64);
        output.WriteEnum((int) Compression);
      }
      if (HasUncompressedSize) {
        output.WriteRawTag(72);
        output.WriteUInt32(UncompressedSize);
      }
      if (HasNumMessagesInBatch) {
        output.WriteRawTag(88);
        output.WriteInt32(NumMessagesInBatch);
      }
      if (HasEventTime) {
        output.WriteRawTag(96);
        output.WriteUInt64(EventTime);
      }
      encryptionKeys_.WriteTo(output, _repeated_encryptionKeys_codec);
      if (HasEncryptionAlgo) {
        output.WriteRawTag(114);
        output.WriteString(EncryptionAlgo);
      }
      if (HasEncryptionParam) {
        output.WriteRawTag(122);
        output.WriteBytes(EncryptionParam);
      }
      if (HasSchemaVersion) {
        output.WriteRawTag(130, 1);
        output.WriteBytes(SchemaVersion);
      }
      if (HasPartitionKeyB64Encoded) {
        output.WriteRawTag(136, 1);
        output.WriteBool(PartitionKeyB64Encoded);
      }
      if (HasOrderingKey) {
        output.WriteRawTag(146, 1);
        output.WriteBytes(OrderingKey);
      }
      if (HasDeliverAtTime) {
        output.WriteRawTag(152, 1);
        output.WriteInt64(DeliverAtTime);
      }
      if (HasMarkerType) {
        output.WriteRawTag(160, 1);
        output.WriteInt32(MarkerType);
      }
      if (HasTxnidLeastBits) {
        output.WriteRawTag(176, 1);
        output.WriteUInt64(TxnidLeastBits);
      }
      if (HasTxnidMostBits) {
        output.WriteRawTag(184, 1);
        output.WriteUInt64(TxnidMostBits);
      }
      if (HasHighestSequenceId) {
        output.WriteRawTag(192, 1);
        output.WriteUInt64(HighestSequenceId);
      }
      if (HasNullValue) {
        output.WriteRawTag(200, 1);
        output.WriteBool(NullValue);
      }
      if (HasUuid) {
        output.WriteRawTag(210, 1);
        output.WriteString(Uuid);
      }
      if (HasNumChunksFromMsg) {
        output.WriteRawTag(216, 1);
        output.WriteInt32(NumChunksFromMsg);
      }
      if (HasTotalChunkMsgSize) {
        output.WriteRawTag(224, 1);
        output.WriteInt32(TotalChunkMsgSize);
      }
      if (HasChunkId) {
        output.WriteRawTag(232, 1);
        output.WriteInt32(ChunkId);
      }
      if (HasNullPartitionKey) {
        output.WriteRawTag(240, 1);
        output.WriteBool(NullPartitionKey);
      }
      compactedBatchIndexes_.WriteTo(output, _repeated_compactedBatchIndexes_codec);
      if (HasSchemaId) {
        output.WriteRawTag(130, 2);
        output.WriteBytes(SchemaId);
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
        if (HasProducerName) {
            output.WriteRawTag(10);
            output.WriteString(ProducerName);
        }
        if (HasSequenceId) {
            output.WriteRawTag(16);
            output.WriteUInt64(SequenceId);
        }
        if (HasPublishTime) {
            output.WriteRawTag(24);
            output.WriteUInt64(PublishTime);
        }
        properties_.WriteTo(ref output, _repeated_properties_codec);
        if (HasReplicatedFrom) {
            output.WriteRawTag(42);
            output.WriteString(ReplicatedFrom);
        }
        if (HasPartitionKey) {
            output.WriteRawTag(50);
            output.WriteString(PartitionKey);
        }
        replicateTo_.WriteTo(ref output, _repeated_replicateTo_codec);
        if (HasCompression) {
            output.WriteRawTag(64);
            output.WriteEnum((int) Compression);
        }
        if (HasUncompressedSize) {
            output.WriteRawTag(72);
            output.WriteUInt32(UncompressedSize);
        }
        if (HasNumMessagesInBatch) {
            output.WriteRawTag(88);
            output.WriteInt32(NumMessagesInBatch);
        }
        if (HasEventTime) {
            output.WriteRawTag(96);
            output.WriteUInt64(EventTime);
        }
        encryptionKeys_.WriteTo(ref output, _repeated_encryptionKeys_codec);
        if (HasEncryptionAlgo) {
            output.WriteRawTag(114);
            output.WriteString(EncryptionAlgo);
        }
        if (HasEncryptionParam) {
            output.WriteRawTag(122);
            output.WriteBytes(EncryptionParam);
        }
        if (HasSchemaVersion) {
            output.WriteRawTag(130, 1);
            output.WriteBytes(SchemaVersion);
        }
        if (HasPartitionKeyB64Encoded) {
            output.WriteRawTag(136, 1);
            output.WriteBool(PartitionKeyB64Encoded);
        }
        if (HasOrderingKey) {
            output.WriteRawTag(146, 1);
            output.WriteBytes(OrderingKey);
        }
        if (HasDeliverAtTime) {
            output.WriteRawTag(152, 1);
            output.WriteInt64(DeliverAtTime);
        }
        if (HasMarkerType) {
            output.WriteRawTag(160, 1);
            output.WriteInt32(MarkerType);
        }
        if (HasTxnidLeastBits) {
            output.WriteRawTag(176, 1);
            output.WriteUInt64(TxnidLeastBits);
        }
        if (HasTxnidMostBits) {
            output.WriteRawTag(184, 1);
            output.WriteUInt64(TxnidMostBits);
        }
        if (HasHighestSequenceId) {
            output.WriteRawTag(192, 1);
            output.WriteUInt64(HighestSequenceId);
        }
        if (HasNullValue) {
            output.WriteRawTag(200, 1);
            output.WriteBool(NullValue);
        }
        if (HasUuid) {
            output.WriteRawTag(210, 1);
            output.WriteString(Uuid);
        }
        if (HasNumChunksFromMsg) {
            output.WriteRawTag(216, 1);
            output.WriteInt32(NumChunksFromMsg);
        }
        if (HasTotalChunkMsgSize) {
            output.WriteRawTag(224, 1);
            output.WriteInt32(TotalChunkMsgSize);
        }
        if (HasChunkId) {
            output.WriteRawTag(232, 1);
            output.WriteInt32(ChunkId);
        }
        if (HasNullPartitionKey) {
            output.WriteRawTag(240, 1);
            output.WriteBool(NullPartitionKey);
        }
        compactedBatchIndexes_.WriteTo(ref output, _repeated_compactedBatchIndexes_codec);
        if (HasSchemaId) {
            output.WriteRawTag(130, 2);
            output.WriteBytes(SchemaId);
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
        if (HasProducerName) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ProducerName);
        }
        if (HasSequenceId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(SequenceId);
        }
        if (HasPublishTime) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(PublishTime);
        }
        size += properties_.CalculateSize(_repeated_properties_codec);
        if (HasReplicatedFrom) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(ReplicatedFrom);
        }
        if (HasPartitionKey) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(PartitionKey);
        }
        size += replicateTo_.CalculateSize(_repeated_replicateTo_codec);
        if (HasCompression) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Compression);
        }
        if (HasUncompressedSize) {
            size += 1 + pb::CodedOutputStream.ComputeUInt32Size(UncompressedSize);
        }
        if (HasNumMessagesInBatch) {
            size += 1 + pb::CodedOutputStream.ComputeInt32Size(NumMessagesInBatch);
        }
        if (HasEventTime) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(EventTime);
        }
        size += encryptionKeys_.CalculateSize(_repeated_encryptionKeys_codec);
        if (HasEncryptionAlgo) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(EncryptionAlgo);
        }
        if (HasEncryptionParam) {
            size += 1 + pb::CodedOutputStream.ComputeBytesSize(EncryptionParam);
        }
        if (HasSchemaVersion) {
            size += 2 + pb::CodedOutputStream.ComputeBytesSize(SchemaVersion);
        }
        if (HasPartitionKeyB64Encoded) {
            size += 2 + 1;
        }
        if (HasOrderingKey) {
            size += 2 + pb::CodedOutputStream.ComputeBytesSize(OrderingKey);
        }
        if (HasDeliverAtTime) {
            size += 2 + pb::CodedOutputStream.ComputeInt64Size(DeliverAtTime);
        }
        if (HasMarkerType) {
            size += 2 + pb::CodedOutputStream.ComputeInt32Size(MarkerType);
        }
        if (HasTxnidLeastBits) {
            size += 2 + pb::CodedOutputStream.ComputeUInt64Size(TxnidLeastBits);
        }
        if (HasTxnidMostBits) {
            size += 2 + pb::CodedOutputStream.ComputeUInt64Size(TxnidMostBits);
        }
        if (HasHighestSequenceId) {
            size += 2 + pb::CodedOutputStream.ComputeUInt64Size(HighestSequenceId);
        }
        if (HasNullValue) {
            size += 2 + 1;
        }
        if (HasUuid) {
            size += 2 + pb::CodedOutputStream.ComputeStringSize(Uuid);
        }
        if (HasNumChunksFromMsg) {
            size += 2 + pb::CodedOutputStream.ComputeInt32Size(NumChunksFromMsg);
        }
        if (HasTotalChunkMsgSize) {
            size += 2 + pb::CodedOutputStream.ComputeInt32Size(TotalChunkMsgSize);
        }
        if (HasChunkId) {
            size += 2 + pb::CodedOutputStream.ComputeInt32Size(ChunkId);
        }
        if (HasNullPartitionKey) {
            size += 2 + 1;
        }
        size += compactedBatchIndexes_.CalculateSize(_repeated_compactedBatchIndexes_codec);
        if (HasSchemaId) {
            size += 2 + pb::CodedOutputStream.ComputeBytesSize(SchemaId);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(MessageMetadata other) {
        if (other == null) {
            return;
        }
        if (other.HasProducerName) {
            ProducerName = other.ProducerName;
        }
        if (other.HasSequenceId) {
            SequenceId = other.SequenceId;
        }
        if (other.HasPublishTime) {
            PublishTime = other.PublishTime;
        }
        properties_.Add(other.properties_);
        if (other.HasReplicatedFrom) {
            ReplicatedFrom = other.ReplicatedFrom;
        }
        if (other.HasPartitionKey) {
            PartitionKey = other.PartitionKey;
        }
        replicateTo_.Add(other.replicateTo_);
        if (other.HasCompression) {
            Compression = other.Compression;
        }
        if (other.HasUncompressedSize) {
            UncompressedSize = other.UncompressedSize;
        }
        if (other.HasNumMessagesInBatch) {
            NumMessagesInBatch = other.NumMessagesInBatch;
        }
        if (other.HasEventTime) {
            EventTime = other.EventTime;
        }
        encryptionKeys_.Add(other.encryptionKeys_);
        if (other.HasEncryptionAlgo) {
            EncryptionAlgo = other.EncryptionAlgo;
        }
        if (other.HasEncryptionParam) {
            EncryptionParam = other.EncryptionParam;
        }
        if (other.HasSchemaVersion) {
            SchemaVersion = other.SchemaVersion;
        }
        if (other.HasPartitionKeyB64Encoded) {
            PartitionKeyB64Encoded = other.PartitionKeyB64Encoded;
        }
        if (other.HasOrderingKey) {
            OrderingKey = other.OrderingKey;
        }
        if (other.HasDeliverAtTime) {
            DeliverAtTime = other.DeliverAtTime;
        }
        if (other.HasMarkerType) {
            MarkerType = other.MarkerType;
        }
        if (other.HasTxnidLeastBits) {
            TxnidLeastBits = other.TxnidLeastBits;
        }
        if (other.HasTxnidMostBits) {
            TxnidMostBits = other.TxnidMostBits;
        }
        if (other.HasHighestSequenceId) {
            HighestSequenceId = other.HighestSequenceId;
        }
        if (other.HasNullValue) {
            NullValue = other.NullValue;
        }
        if (other.HasUuid) {
            Uuid = other.Uuid;
        }
        if (other.HasNumChunksFromMsg) {
            NumChunksFromMsg = other.NumChunksFromMsg;
        }
        if (other.HasTotalChunkMsgSize) {
            TotalChunkMsgSize = other.TotalChunkMsgSize;
        }
        if (other.HasChunkId) {
            ChunkId = other.ChunkId;
        }
        if (other.HasNullPartitionKey) {
            NullPartitionKey = other.NullPartitionKey;
        }
        compactedBatchIndexes_.Add(other.compactedBatchIndexes_);
        if (other.HasSchemaId) {
            SchemaId = other.SchemaId;
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
            ProducerName = input.ReadString();
            break;
          }
          case 16: {
            SequenceId = input.ReadUInt64();
            break;
          }
          case 24: {
            PublishTime = input.ReadUInt64();
            break;
          }
          case 34: {
            properties_.AddEntriesFrom(input, _repeated_properties_codec);
            break;
          }
          case 42: {
            ReplicatedFrom = input.ReadString();
            break;
          }
          case 50: {
            PartitionKey = input.ReadString();
            break;
          }
          case 58: {
            replicateTo_.AddEntriesFrom(input, _repeated_replicateTo_codec);
            break;
          }
          case 64: {
            Compression = (global::DotPulsar.Internal.PulsarApi.CompressionType) input.ReadEnum();
            break;
          }
          case 72: {
            UncompressedSize = input.ReadUInt32();
            break;
          }
          case 88: {
            NumMessagesInBatch = input.ReadInt32();
            break;
          }
          case 96: {
            EventTime = input.ReadUInt64();
            break;
          }
          case 106: {
            encryptionKeys_.AddEntriesFrom(input, _repeated_encryptionKeys_codec);
            break;
          }
          case 114: {
            EncryptionAlgo = input.ReadString();
            break;
          }
          case 122: {
            EncryptionParam = input.ReadBytes();
            break;
          }
          case 130: {
            SchemaVersion = input.ReadBytes();
            break;
          }
          case 136: {
            PartitionKeyB64Encoded = input.ReadBool();
            break;
          }
          case 146: {
            OrderingKey = input.ReadBytes();
            break;
          }
          case 152: {
            DeliverAtTime = input.ReadInt64();
            break;
          }
          case 160: {
            MarkerType = input.ReadInt32();
            break;
          }
          case 176: {
            TxnidLeastBits = input.ReadUInt64();
            break;
          }
          case 184: {
            TxnidMostBits = input.ReadUInt64();
            break;
          }
          case 192: {
            HighestSequenceId = input.ReadUInt64();
            break;
          }
          case 200: {
            NullValue = input.ReadBool();
            break;
          }
          case 210: {
            Uuid = input.ReadString();
            break;
          }
          case 216: {
            NumChunksFromMsg = input.ReadInt32();
            break;
          }
          case 224: {
            TotalChunkMsgSize = input.ReadInt32();
            break;
          }
          case 232: {
            ChunkId = input.ReadInt32();
            break;
          }
          case 240: {
            NullPartitionKey = input.ReadBool();
            break;
          }
          case 250:
          case 248: {
            compactedBatchIndexes_.AddEntriesFrom(input, _repeated_compactedBatchIndexes_codec);
            break;
          }
          case 258: {
            SchemaId = input.ReadBytes();
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
                    ProducerName = input.ReadString();
                    break;
                }
                case 16: {
                    SequenceId = input.ReadUInt64();
                    break;
                }
                case 24: {
                    PublishTime = input.ReadUInt64();
                    break;
                }
                case 34: {
                    properties_.AddEntriesFrom(ref input, _repeated_properties_codec);
                    break;
                }
                case 42: {
                    ReplicatedFrom = input.ReadString();
                    break;
                }
                case 50: {
                    PartitionKey = input.ReadString();
                    break;
                }
                case 58: {
                    replicateTo_.AddEntriesFrom(ref input, _repeated_replicateTo_codec);
                    break;
                }
                case 64: {
                    Compression = (global::DotPulsar.Internal.PulsarApi.CompressionType) input.ReadEnum();
                    break;
                }
                case 72: {
                    UncompressedSize = input.ReadUInt32();
                    break;
                }
                case 88: {
                    NumMessagesInBatch = input.ReadInt32();
                    break;
                }
                case 96: {
                    EventTime = input.ReadUInt64();
                    break;
                }
                case 106: {
                    encryptionKeys_.AddEntriesFrom(ref input, _repeated_encryptionKeys_codec);
                    break;
                }
                case 114: {
                    EncryptionAlgo = input.ReadString();
                    break;
                }
                case 122: {
                    EncryptionParam = input.ReadBytes();
                    break;
                }
                case 130: {
                    SchemaVersion = input.ReadBytes();
                    break;
                }
                case 136: {
                    PartitionKeyB64Encoded = input.ReadBool();
                    break;
                }
                case 146: {
                    OrderingKey = input.ReadBytes();
                    break;
                }
                case 152: {
                    DeliverAtTime = input.ReadInt64();
                    break;
                }
                case 160: {
                    MarkerType = input.ReadInt32();
                    break;
                }
                case 176: {
                    TxnidLeastBits = input.ReadUInt64();
                    break;
                }
                case 184: {
                    TxnidMostBits = input.ReadUInt64();
                    break;
                }
                case 192: {
                    HighestSequenceId = input.ReadUInt64();
                    break;
                }
                case 200: {
                    NullValue = input.ReadBool();
                    break;
                }
                case 210: {
                    Uuid = input.ReadString();
                    break;
                }
                case 216: {
                    NumChunksFromMsg = input.ReadInt32();
                    break;
                }
                case 224: {
                    TotalChunkMsgSize = input.ReadInt32();
                    break;
                }
                case 232: {
                    ChunkId = input.ReadInt32();
                    break;
                }
                case 240: {
                    NullPartitionKey = input.ReadBool();
                    break;
                }
                case 250:
                case 248: {
                    compactedBatchIndexes_.AddEntriesFrom(ref input, _repeated_compactedBatchIndexes_codec);
                    break;
                }
                case 258: {
                    SchemaId = input.ReadBytes();
                    break;
                }
            }
        }
    }
#endif

}
