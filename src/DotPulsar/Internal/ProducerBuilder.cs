using DotPulsar.Abstractions;

namespace DotPulsar.Internal
{
    public sealed class ProducerBuilder : IProducerBuilder
    {
        private readonly IPulsarClient _pulsarClient;
        private readonly ProducerOptions _options;

        public ProducerBuilder(IPulsarClient pulsarClient)
        {
            _pulsarClient = pulsarClient;
            _options = new ProducerOptions();
        }

        public IProducerBuilder ProducerName(string name)
        {
            _options.ProducerName = name;
            return this;
        }

        public IProducerBuilder InitialSequenceId(ulong initialSequenceId)
        {
            _options.InitialSequenceId = initialSequenceId;
            return this;
        }

        public IProducerBuilder Topic(string topic)
        {
            _options.Topic = topic;
            return this;
        }

        public IProducer Create() => _pulsarClient.CreateProducer(_options);
    }
}
