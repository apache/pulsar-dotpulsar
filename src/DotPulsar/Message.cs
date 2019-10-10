using DotPulsar.Internal.Extensions;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace DotPulsar
{
    public sealed class Message
    {
        private readonly Internal.PulsarApi.MessageMetadata _messageMetadata;
        private IReadOnlyDictionary<string, string>? _properties;

        internal Message(MessageId messageId, Internal.PulsarApi.MessageMetadata messageMetadata, ReadOnlySequence<byte> data)
        {
            MessageId = messageId;
            _messageMetadata = messageMetadata;
            Data = data;
        }

        public MessageId MessageId { get; }
        public ReadOnlySequence<byte> Data { get; }
        public string ProducerName => _messageMetadata.ProducerName;
        public ulong SequenceId => _messageMetadata.SequenceId;

        public bool HasEventTime => _messageMetadata.EventTime != 0;
        public ulong EventTime => _messageMetadata.EventTime;
        public DateTimeOffset EventTimeAsDateTimeOffset => _messageMetadata.GetEventTimeAsDateTimeOffset();

        public bool HasBase64EncodedKey => _messageMetadata.PartitionKeyB64Encoded;
        public bool HasKey => _messageMetadata.PartitionKey != null;
        public string? Key => _messageMetadata.PartitionKey;
        public byte[]? KeyBytes => _messageMetadata.GetKeyAsBytes();

        public bool HasOrderingKey => _messageMetadata.OrderingKey != null;
        public byte[]? OrderingKey => _messageMetadata.OrderingKey;


        public ulong PublishTime => _messageMetadata.PublishTime;
        public DateTimeOffset PublishTimeAsDateTimeOffset => _messageMetadata.GetPublishTimeAsDateTimeOffset();


        public IReadOnlyDictionary<string, string> Properties
        {
            get => _properties ??= _messageMetadata.Properties.ToDictionary(p => p.Key, p => p.Value);
        }
    }
}
