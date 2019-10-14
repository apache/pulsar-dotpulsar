using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace DotPulsar
{
    public sealed class Message
    {
        private readonly List<Internal.PulsarApi.KeyValue> _keyVaues;
        private IReadOnlyDictionary<string, string>? _properties;

        internal Message(
            MessageId messageId,
            Internal.PulsarApi.MessageMetadata metadata,
            Internal.PulsarApi.SingleMessageMetadata? singleMetadata,
            ReadOnlySequence<byte> data)
        {
            MessageId = messageId;
            ProducerName = metadata.ProducerName;
            PublishTime = metadata.PublishTime;
            Data = data;

            if (singleMetadata is null)
            {
                EventTime = metadata.EventTime;
                HasBase64EncodedKey = metadata.PartitionKeyB64Encoded;
                Key = metadata.PartitionKey;
                SequenceId = metadata.SequenceId;
                OrderingKey = metadata.OrderingKey;
                _keyVaues = metadata.Properties;
            }
            else
            {
                EventTime = singleMetadata.EventTime;
                HasBase64EncodedKey = singleMetadata.PartitionKeyB64Encoded;
                Key = singleMetadata.PartitionKey;
                OrderingKey = singleMetadata.OrderingKey;
                SequenceId = singleMetadata.SequenceId;
                _keyVaues = singleMetadata.Properties;
            }
        }

        public MessageId MessageId { get; }
        public ReadOnlySequence<byte> Data { get; }
        public string ProducerName { get; }
        public ulong SequenceId { get; }

        public bool HasEventTime => EventTime != 0;
        public ulong EventTime { get; }
        public DateTimeOffset EventTimeAsDateTimeOffset => DateTimeOffset.FromUnixTimeMilliseconds((long)EventTime);

        public bool HasBase64EncodedKey { get; }
        public bool HasKey => Key != null;
        public string? Key { get; }
        public byte[]? KeyBytes => HasBase64EncodedKey ? Convert.FromBase64String(Key) : null;

        public bool HasOrderingKey => OrderingKey != null;
        public byte[]? OrderingKey { get; }


        public ulong PublishTime { get; }
        public DateTimeOffset PublishTimeAsDateTimeOffset => DateTimeOffset.FromUnixTimeMilliseconds((long)PublishTime);

        public IReadOnlyDictionary<string, string> Properties => _properties ??= _keyVaues.ToDictionary(p => p.Key, p => p.Value);
    }
}
