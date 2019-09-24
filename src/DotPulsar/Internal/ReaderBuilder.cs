using DotPulsar.Abstractions;
using DotPulsar.Exceptions;

namespace DotPulsar.Internal
{
    public sealed class ReaderBuilder : IReaderBuilder
    {
        private readonly IPulsarClient _pulsarClient;
        private string? _readerName;
        private uint _messagePrefetchCount;
        private bool _readCompacted;
        private MessageId? _startMessageId;
        private string? _topic;

        public ReaderBuilder(IPulsarClient pulsarClient)
        {
            _pulsarClient = pulsarClient;
            _messagePrefetchCount = ReaderOptions.DefaultMessagePrefetchCount;
            _readCompacted = ReaderOptions.DefaultReadCompacted;
        }

        public IReaderBuilder ReaderName(string name)
        {
            _readerName = name;
            return this;
        }

        public IReaderBuilder MessagePrefetchCount(uint count)
        {
            _messagePrefetchCount = count;
            return this;
        }

        public IReaderBuilder ReadCompacted(bool readCompacted)
        {
            _readCompacted = readCompacted;
            return this;
        }

        public IReaderBuilder StartMessageId(MessageId messageId)
        {
            _startMessageId = messageId;
            return this;
        }

        public IReaderBuilder Topic(string topic)
        {
            _topic = topic;
            return this;
        }

        public IReader Create()
        {
            if (_startMessageId is null)
                throw new ConfigurationException("StartMessageId may not be null");

            if (string.IsNullOrEmpty(_topic))
                throw new ConfigurationException("Topic may not be null or empty");

            var options = new ReaderOptions(_startMessageId, _topic!)
            {
                MessagePrefetchCount = _messagePrefetchCount,
                ReadCompacted = _readCompacted,
                ReaderName = _readerName
            };

            return _pulsarClient.CreateReader(options);
        }
    }
}
