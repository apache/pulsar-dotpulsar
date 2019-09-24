using System.Buffers;
using System.Collections.Immutable;

namespace DotPulsar
{
    public sealed class Message
    {
        private readonly Internal.PulsarApi.MessageMetadata _messageMetadata;
        private ImmutableDictionary<string, string>? _properties;

        internal Message(MessageId messageId, Internal.PulsarApi.MessageMetadata messageMetadata, ReadOnlySequence<byte> data)
        {
            MessageId = messageId;
            _messageMetadata = messageMetadata;
            Data = data;
        }

        public MessageId MessageId { get; }
        public ReadOnlySequence<byte> Data { get; }
        public ulong EventTime => _messageMetadata.EventTime;
        public bool HasBase64EncodedKey => _messageMetadata.PartitionKeyB64Encoded;
        public bool HasKey => _messageMetadata.PartitionKey != null;
        public string Key => _messageMetadata.PartitionKey;
        public bool HasOrderingKey => _messageMetadata.OrderingKey != null;
        public byte[] OrderingKey => _messageMetadata.OrderingKey;
        public string ProducerName => _messageMetadata.ProducerName;
        public ulong PublishTime => _messageMetadata.PublishTime;
        public ulong SequenceId => _messageMetadata.SequenceId;

        public ImmutableDictionary<string, string> Properties
        {
            get
            {
                if (_properties is null)
                    _properties = _messageMetadata.Properties.ToImmutableDictionary(p => p.Key, p => p.Value);
                return _properties;
            }
        }
    }
}
