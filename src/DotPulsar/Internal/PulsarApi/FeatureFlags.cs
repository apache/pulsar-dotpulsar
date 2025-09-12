#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

/// <summary>
/// Please also add a new enum for the class "PulsarClientException.FailedFeatureCheck" when adding a new feature flag.
/// </summary>
[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class FeatureFlags : IMessage<FeatureFlags>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<FeatureFlags> _parser = new pb::MessageParser<FeatureFlags>(() => new FeatureFlags());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<FeatureFlags> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[10]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public FeatureFlags() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public FeatureFlags(FeatureFlags other) : this() {
        _hasBits0 = other._hasBits0;
        supportsAuthRefresh_ = other.supportsAuthRefresh_;
        supportsBrokerEntryMetadata_ = other.supportsBrokerEntryMetadata_;
        supportsPartialProducer_ = other.supportsPartialProducer_;
        supportsTopicWatchers_ = other.supportsTopicWatchers_;
        supportsGetPartitionedMetadataWithoutAutoCreation_ = other.supportsGetPartitionedMetadataWithoutAutoCreation_;
        supportsReplDedupByLidAndEid_ = other.supportsReplDedupByLidAndEid_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public FeatureFlags Clone() {
        return new FeatureFlags(this);
    }

    /// <summary>Field number for the "supports_auth_refresh" field.</summary>
    public const int SupportsAuthRefreshFieldNumber = 1;
    private readonly static bool SupportsAuthRefreshDefaultValue = false;

    private bool supportsAuthRefresh_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool SupportsAuthRefresh {
        get { if ((_hasBits0 & 1) != 0) { return supportsAuthRefresh_; } else { return SupportsAuthRefreshDefaultValue; } }
        set {
            _hasBits0 |= 1;
            supportsAuthRefresh_ = value;
        }
    }
    /// <summary>Gets whether the "supports_auth_refresh" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSupportsAuthRefresh {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "supports_auth_refresh" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSupportsAuthRefresh() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "supports_broker_entry_metadata" field.</summary>
    public const int SupportsBrokerEntryMetadataFieldNumber = 2;
    private readonly static bool SupportsBrokerEntryMetadataDefaultValue = false;

    private bool supportsBrokerEntryMetadata_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool SupportsBrokerEntryMetadata {
        get { if ((_hasBits0 & 2) != 0) { return supportsBrokerEntryMetadata_; } else { return SupportsBrokerEntryMetadataDefaultValue; } }
        set {
            _hasBits0 |= 2;
            supportsBrokerEntryMetadata_ = value;
        }
    }
    /// <summary>Gets whether the "supports_broker_entry_metadata" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSupportsBrokerEntryMetadata {
        get { return (_hasBits0 & 2) != 0; }
    }
    /// <summary>Clears the value of the "supports_broker_entry_metadata" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSupportsBrokerEntryMetadata() {
        _hasBits0 &= ~2;
    }

    /// <summary>Field number for the "supports_partial_producer" field.</summary>
    public const int SupportsPartialProducerFieldNumber = 3;
    private readonly static bool SupportsPartialProducerDefaultValue = false;

    private bool supportsPartialProducer_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool SupportsPartialProducer {
        get { if ((_hasBits0 & 4) != 0) { return supportsPartialProducer_; } else { return SupportsPartialProducerDefaultValue; } }
        set {
            _hasBits0 |= 4;
            supportsPartialProducer_ = value;
        }
    }
    /// <summary>Gets whether the "supports_partial_producer" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSupportsPartialProducer {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "supports_partial_producer" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSupportsPartialProducer() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "supports_topic_watchers" field.</summary>
    public const int SupportsTopicWatchersFieldNumber = 4;
    private readonly static bool SupportsTopicWatchersDefaultValue = false;

    private bool supportsTopicWatchers_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool SupportsTopicWatchers {
        get { if ((_hasBits0 & 8) != 0) { return supportsTopicWatchers_; } else { return SupportsTopicWatchersDefaultValue; } }
        set {
            _hasBits0 |= 8;
            supportsTopicWatchers_ = value;
        }
    }
    /// <summary>Gets whether the "supports_topic_watchers" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSupportsTopicWatchers {
        get { return (_hasBits0 & 8) != 0; }
    }
    /// <summary>Clears the value of the "supports_topic_watchers" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSupportsTopicWatchers() {
        _hasBits0 &= ~8;
    }

    /// <summary>Field number for the "supports_get_partitioned_metadata_without_auto_creation" field.</summary>
    public const int SupportsGetPartitionedMetadataWithoutAutoCreationFieldNumber = 5;
    private readonly static bool SupportsGetPartitionedMetadataWithoutAutoCreationDefaultValue = false;

    private bool supportsGetPartitionedMetadataWithoutAutoCreation_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool SupportsGetPartitionedMetadataWithoutAutoCreation {
        get { if ((_hasBits0 & 16) != 0) { return supportsGetPartitionedMetadataWithoutAutoCreation_; } else { return SupportsGetPartitionedMetadataWithoutAutoCreationDefaultValue; } }
        set {
            _hasBits0 |= 16;
            supportsGetPartitionedMetadataWithoutAutoCreation_ = value;
        }
    }
    /// <summary>Gets whether the "supports_get_partitioned_metadata_without_auto_creation" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSupportsGetPartitionedMetadataWithoutAutoCreation {
        get { return (_hasBits0 & 16) != 0; }
    }
    /// <summary>Clears the value of the "supports_get_partitioned_metadata_without_auto_creation" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSupportsGetPartitionedMetadataWithoutAutoCreation() {
        _hasBits0 &= ~16;
    }

    /// <summary>Field number for the "supports_repl_dedup_by_lid_and_eid" field.</summary>
    public const int SupportsReplDedupByLidAndEidFieldNumber = 6;
    private readonly static bool SupportsReplDedupByLidAndEidDefaultValue = false;

    private bool supportsReplDedupByLidAndEid_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool SupportsReplDedupByLidAndEid {
        get { if ((_hasBits0 & 32) != 0) { return supportsReplDedupByLidAndEid_; } else { return SupportsReplDedupByLidAndEidDefaultValue; } }
        set {
            _hasBits0 |= 32;
            supportsReplDedupByLidAndEid_ = value;
        }
    }
    /// <summary>Gets whether the "supports_repl_dedup_by_lid_and_eid" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasSupportsReplDedupByLidAndEid {
        get { return (_hasBits0 & 32) != 0; }
    }
    /// <summary>Clears the value of the "supports_repl_dedup_by_lid_and_eid" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearSupportsReplDedupByLidAndEid() {
        _hasBits0 &= ~32;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as FeatureFlags);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(FeatureFlags other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (SupportsAuthRefresh != other.SupportsAuthRefresh) return false;
        if (SupportsBrokerEntryMetadata != other.SupportsBrokerEntryMetadata) return false;
        if (SupportsPartialProducer != other.SupportsPartialProducer) return false;
        if (SupportsTopicWatchers != other.SupportsTopicWatchers) return false;
        if (SupportsGetPartitionedMetadataWithoutAutoCreation != other.SupportsGetPartitionedMetadataWithoutAutoCreation) return false;
        if (SupportsReplDedupByLidAndEid != other.SupportsReplDedupByLidAndEid) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasSupportsAuthRefresh) hash ^= SupportsAuthRefresh.GetHashCode();
        if (HasSupportsBrokerEntryMetadata) hash ^= SupportsBrokerEntryMetadata.GetHashCode();
        if (HasSupportsPartialProducer) hash ^= SupportsPartialProducer.GetHashCode();
        if (HasSupportsTopicWatchers) hash ^= SupportsTopicWatchers.GetHashCode();
        if (HasSupportsGetPartitionedMetadataWithoutAutoCreation) hash ^= SupportsGetPartitionedMetadataWithoutAutoCreation.GetHashCode();
        if (HasSupportsReplDedupByLidAndEid) hash ^= SupportsReplDedupByLidAndEid.GetHashCode();
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
      if (HasSupportsAuthRefresh) {
        output.WriteRawTag(8);
        output.WriteBool(SupportsAuthRefresh);
      }
      if (HasSupportsBrokerEntryMetadata) {
        output.WriteRawTag(16);
        output.WriteBool(SupportsBrokerEntryMetadata);
      }
      if (HasSupportsPartialProducer) {
        output.WriteRawTag(24);
        output.WriteBool(SupportsPartialProducer);
      }
      if (HasSupportsTopicWatchers) {
        output.WriteRawTag(32);
        output.WriteBool(SupportsTopicWatchers);
      }
      if (HasSupportsGetPartitionedMetadataWithoutAutoCreation) {
        output.WriteRawTag(40);
        output.WriteBool(SupportsGetPartitionedMetadataWithoutAutoCreation);
      }
      if (HasSupportsReplDedupByLidAndEid) {
        output.WriteRawTag(48);
        output.WriteBool(SupportsReplDedupByLidAndEid);
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
        if (HasSupportsAuthRefresh) {
            output.WriteRawTag(8);
            output.WriteBool(SupportsAuthRefresh);
        }
        if (HasSupportsBrokerEntryMetadata) {
            output.WriteRawTag(16);
            output.WriteBool(SupportsBrokerEntryMetadata);
        }
        if (HasSupportsPartialProducer) {
            output.WriteRawTag(24);
            output.WriteBool(SupportsPartialProducer);
        }
        if (HasSupportsTopicWatchers) {
            output.WriteRawTag(32);
            output.WriteBool(SupportsTopicWatchers);
        }
        if (HasSupportsGetPartitionedMetadataWithoutAutoCreation) {
            output.WriteRawTag(40);
            output.WriteBool(SupportsGetPartitionedMetadataWithoutAutoCreation);
        }
        if (HasSupportsReplDedupByLidAndEid) {
            output.WriteRawTag(48);
            output.WriteBool(SupportsReplDedupByLidAndEid);
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
        if (HasSupportsAuthRefresh) {
            size += 1 + 1;
        }
        if (HasSupportsBrokerEntryMetadata) {
            size += 1 + 1;
        }
        if (HasSupportsPartialProducer) {
            size += 1 + 1;
        }
        if (HasSupportsTopicWatchers) {
            size += 1 + 1;
        }
        if (HasSupportsGetPartitionedMetadataWithoutAutoCreation) {
            size += 1 + 1;
        }
        if (HasSupportsReplDedupByLidAndEid) {
            size += 1 + 1;
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(FeatureFlags other) {
        if (other == null) {
            return;
        }
        if (other.HasSupportsAuthRefresh) {
            SupportsAuthRefresh = other.SupportsAuthRefresh;
        }
        if (other.HasSupportsBrokerEntryMetadata) {
            SupportsBrokerEntryMetadata = other.SupportsBrokerEntryMetadata;
        }
        if (other.HasSupportsPartialProducer) {
            SupportsPartialProducer = other.SupportsPartialProducer;
        }
        if (other.HasSupportsTopicWatchers) {
            SupportsTopicWatchers = other.SupportsTopicWatchers;
        }
        if (other.HasSupportsGetPartitionedMetadataWithoutAutoCreation) {
            SupportsGetPartitionedMetadataWithoutAutoCreation = other.SupportsGetPartitionedMetadataWithoutAutoCreation;
        }
        if (other.HasSupportsReplDedupByLidAndEid) {
            SupportsReplDedupByLidAndEid = other.SupportsReplDedupByLidAndEid;
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
            SupportsAuthRefresh = input.ReadBool();
            break;
          }
          case 16: {
            SupportsBrokerEntryMetadata = input.ReadBool();
            break;
          }
          case 24: {
            SupportsPartialProducer = input.ReadBool();
            break;
          }
          case 32: {
            SupportsTopicWatchers = input.ReadBool();
            break;
          }
          case 40: {
            SupportsGetPartitionedMetadataWithoutAutoCreation = input.ReadBool();
            break;
          }
          case 48: {
            SupportsReplDedupByLidAndEid = input.ReadBool();
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
                    SupportsAuthRefresh = input.ReadBool();
                    break;
                }
                case 16: {
                    SupportsBrokerEntryMetadata = input.ReadBool();
                    break;
                }
                case 24: {
                    SupportsPartialProducer = input.ReadBool();
                    break;
                }
                case 32: {
                    SupportsTopicWatchers = input.ReadBool();
                    break;
                }
                case 40: {
                    SupportsGetPartitionedMetadataWithoutAutoCreation = input.ReadBool();
                    break;
                }
                case 48: {
                    SupportsReplDedupByLidAndEid = input.ReadBool();
                    break;
                }
            }
        }
    }
#endif

}
