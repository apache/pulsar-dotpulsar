using DotPulsar.Abstractions;

namespace DotPulsar.Internal
{
    public sealed class ReaderBuilder : IReaderBuilder
    {
        private readonly IPulsarClient _pulsarClient;
        private readonly ReaderOptions _options;

        public ReaderBuilder(IPulsarClient pulsarClient)
        {
            _pulsarClient = pulsarClient;
            _options = new ReaderOptions();
        }

        public IReaderBuilder ReaderName(string name)
        {
            _options.ReaderName = name;
            return this;
        }

        public IReaderBuilder MessagePrefetchCount(uint count)
        {
            _options.MessagePrefetchCount = count;
            return this;
        }

        public IReaderBuilder ReadCompacted(bool readCompacted)
        {
            _options.ReadCompacted = readCompacted;
            return this;
        }

        public IReaderBuilder StartMessageId(MessageId messageId)
        {
            _options.StartMessageId = messageId;
            return this;
        }

        public IReaderBuilder Topic(string topic)
        {
            _options.Topic = topic;
            return this;
        }

        public IReader Create() => _pulsarClient.CreateReader(_options);
    }
}
