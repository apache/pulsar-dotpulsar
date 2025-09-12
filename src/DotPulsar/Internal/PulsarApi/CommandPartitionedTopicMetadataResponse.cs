#pragma warning disable 1591, 0612, 3021, 8981, 8625, 8604, 8618, 8601, 8767
namespace DotPulsar.Internal.PulsarApi;

using Google.Protobuf;
using pb = Google.Protobuf;
using pbr = Google.Protobuf.Reflection;

[global::System.Diagnostics.DebuggerDisplayAttribute("{ToString(),nq}")]
public sealed partial class CommandPartitionedTopicMetadataResponse : IMessage<CommandPartitionedTopicMetadataResponse>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , IBufferMessage
#endif
{
    private static readonly pb::MessageParser<CommandPartitionedTopicMetadataResponse> _parser = new pb::MessageParser<CommandPartitionedTopicMetadataResponse>(() => new CommandPartitionedTopicMetadataResponse());
    private pb::UnknownFieldSet _unknownFields;
    private int _hasBits0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<CommandPartitionedTopicMetadataResponse> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
        get { return global::DotPulsar.Internal.PulsarApi.PulsarApiReflection.Descriptor.MessageTypes[18]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandPartitionedTopicMetadataResponse() {
        OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandPartitionedTopicMetadataResponse(CommandPartitionedTopicMetadataResponse other) : this() {
        _hasBits0 = other._hasBits0;
        partitions_ = other.partitions_;
        requestId_ = other.requestId_;
        response_ = other.response_;
        error_ = other.error_;
        message_ = other.message_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public CommandPartitionedTopicMetadataResponse Clone() {
        return new CommandPartitionedTopicMetadataResponse(this);
    }

    /// <summary>Field number for the "partitions" field.</summary>
    public const int PartitionsFieldNumber = 1;
    private readonly static uint PartitionsDefaultValue = 0;

    private uint partitions_;
    /// <summary>
    /// Optional in case of error
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public uint Partitions {
        get { if ((_hasBits0 & 1) != 0) { return partitions_; } else { return PartitionsDefaultValue; } }
        set {
            _hasBits0 |= 1;
            partitions_ = value;
        }
    }
    /// <summary>Gets whether the "partitions" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasPartitions {
        get { return (_hasBits0 & 1) != 0; }
    }
    /// <summary>Clears the value of the "partitions" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearPartitions() {
        _hasBits0 &= ~1;
    }

    /// <summary>Field number for the "request_id" field.</summary>
    public const int RequestIdFieldNumber = 2;
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

    /// <summary>Field number for the "response" field.</summary>
    public const int ResponseFieldNumber = 3;
    private readonly static global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse.Types.LookupType ResponseDefaultValue = global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse.Types.LookupType.Success;

    private global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse.Types.LookupType response_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse.Types.LookupType Response {
        get { if ((_hasBits0 & 4) != 0) { return response_; } else { return ResponseDefaultValue; } }
        set {
            _hasBits0 |= 4;
            response_ = value;
        }
    }
    /// <summary>Gets whether the "response" field is set</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool HasResponse {
        get { return (_hasBits0 & 4) != 0; }
    }
    /// <summary>Clears the value of the "response" field</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearResponse() {
        _hasBits0 &= ~4;
    }

    /// <summary>Field number for the "error" field.</summary>
    public const int ErrorFieldNumber = 4;
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
    public const int MessageFieldNumber = 5;
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

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
        return Equals(other as CommandPartitionedTopicMetadataResponse);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(CommandPartitionedTopicMetadataResponse other) {
        if (ReferenceEquals(other, null)) {
            return false;
        }
        if (ReferenceEquals(other, this)) {
            return true;
        }
        if (Partitions != other.Partitions) return false;
        if (RequestId != other.RequestId) return false;
        if (Response != other.Response) return false;
        if (Error != other.Error) return false;
        if (Message != other.Message) return false;
        return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
        int hash = 1;
        if (HasPartitions) hash ^= Partitions.GetHashCode();
        if (HasRequestId) hash ^= RequestId.GetHashCode();
        if (HasResponse) hash ^= Response.GetHashCode();
        if (HasError) hash ^= Error.GetHashCode();
        if (HasMessage) hash ^= Message.GetHashCode();
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
      if (HasPartitions) {
        output.WriteRawTag(8);
        output.WriteUInt32(Partitions);
      }
      if (HasRequestId) {
        output.WriteRawTag(16);
        output.WriteUInt64(RequestId);
      }
      if (HasResponse) {
        output.WriteRawTag(24);
        output.WriteEnum((int) Response);
      }
      if (HasError) {
        output.WriteRawTag(32);
        output.WriteEnum((int) Error);
      }
      if (HasMessage) {
        output.WriteRawTag(42);
        output.WriteString(Message);
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
        if (HasPartitions) {
            output.WriteRawTag(8);
            output.WriteUInt32(Partitions);
        }
        if (HasRequestId) {
            output.WriteRawTag(16);
            output.WriteUInt64(RequestId);
        }
        if (HasResponse) {
            output.WriteRawTag(24);
            output.WriteEnum((int) Response);
        }
        if (HasError) {
            output.WriteRawTag(32);
            output.WriteEnum((int) Error);
        }
        if (HasMessage) {
            output.WriteRawTag(42);
            output.WriteString(Message);
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
        if (HasPartitions) {
            size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Partitions);
        }
        if (HasRequestId) {
            size += 1 + pb::CodedOutputStream.ComputeUInt64Size(RequestId);
        }
        if (HasResponse) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Response);
        }
        if (HasError) {
            size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Error);
        }
        if (HasMessage) {
            size += 1 + pb::CodedOutputStream.ComputeStringSize(Message);
        }
        if (_unknownFields != null) {
            size += _unknownFields.CalculateSize();
        }
        return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(CommandPartitionedTopicMetadataResponse other) {
        if (other == null) {
            return;
        }
        if (other.HasPartitions) {
            Partitions = other.Partitions;
        }
        if (other.HasRequestId) {
            RequestId = other.RequestId;
        }
        if (other.HasResponse) {
            Response = other.Response;
        }
        if (other.HasError) {
            Error = other.Error;
        }
        if (other.HasMessage) {
            Message = other.Message;
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
            Partitions = input.ReadUInt32();
            break;
          }
          case 16: {
            RequestId = input.ReadUInt64();
            break;
          }
          case 24: {
            Response = (global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse.Types.LookupType) input.ReadEnum();
            break;
          }
          case 32: {
            Error = (global::DotPulsar.Internal.PulsarApi.ServerError) input.ReadEnum();
            break;
          }
          case 42: {
            Message = input.ReadString();
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
                    Partitions = input.ReadUInt32();
                    break;
                }
                case 16: {
                    RequestId = input.ReadUInt64();
                    break;
                }
                case 24: {
                    Response = (global::DotPulsar.Internal.PulsarApi.CommandPartitionedTopicMetadataResponse.Types.LookupType) input.ReadEnum();
                    break;
                }
                case 32: {
                    Error = (global::DotPulsar.Internal.PulsarApi.ServerError) input.ReadEnum();
                    break;
                }
                case 42: {
                    Message = input.ReadString();
                    break;
                }
            }
        }
    }
#endif

    #region Nested types
    /// <summary>Container for nested types declared in the CommandPartitionedTopicMetadataResponse message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static partial class Types {
        public enum LookupType {
            [pbr::OriginalName("Success")] Success = 0,
            [pbr::OriginalName("Failed")] Failed = 1,
        }

    }
    #endregion

}
