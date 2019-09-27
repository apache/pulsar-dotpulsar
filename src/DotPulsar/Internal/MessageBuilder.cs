using DotPulsar.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class MessageBuilder : IMessageBuilder
    {
        private readonly IProducer _producer;
        private readonly MessageMetadata _metadata;

        public MessageBuilder(IProducer producer)
        {
            _producer = producer;
            _metadata = new MessageMetadata();
        }

        public IMessageBuilder DeliverAt(long timestamp)
        {
            _metadata.DeliverAtTime = timestamp;
            return this;
        }

        public IMessageBuilder EventTime(ulong eventTime)
        {
            _metadata.EventTime = eventTime;
            return this;
        }

        public IMessageBuilder Key(string key)
        {
            _metadata.Key = key;
            return this;
        }

        public IMessageBuilder KeyBytes(byte[] key)
        {
            _metadata.KeyBytes = key;
            return this;
        }

        public IMessageBuilder OrderingKey(byte[] key)
        {
            _metadata.OrderingKey = key;
            return this;
        }

        public IMessageBuilder Property(string key, string value)
        {
            _metadata[key] = value;
            return this;
        }

        public IMessageBuilder SequenceId(ulong sequenceId)
        {
            _metadata.SequenceId = sequenceId;
            return this;
        }

        public async ValueTask<MessageId> Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken) => await _producer.Send(_metadata, data, cancellationToken);
    }
}
