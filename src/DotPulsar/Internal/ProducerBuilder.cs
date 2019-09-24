using DotPulsar.Abstractions;
using DotPulsar.Exceptions;

namespace DotPulsar.Internal
{
    public sealed class ProducerBuilder : IProducerBuilder
    {
        private readonly IPulsarClient _pulsarClient;
        private string? _producerName;
        private ulong _initialSequenceId;
        private string? _topic;

        public ProducerBuilder(IPulsarClient pulsarClient)
        {
            _pulsarClient = pulsarClient;
            _initialSequenceId = ProducerOptions.DefaultInitialSequenceId;
        }

        public IProducerBuilder ProducerName(string name)
        {
            _producerName = name;
            return this;
        }

        public IProducerBuilder InitialSequenceId(ulong initialSequenceId)
        {
            _initialSequenceId = initialSequenceId;
            return this;
        }

        public IProducerBuilder Topic(string topic)
        {
            _topic = topic;
            return this;
        }

        public IProducer Create()
        {
            if (string.IsNullOrEmpty(_topic))
                throw new ConfigurationException("ProducerOptions.Topic may not be null or empty");

            var options = new ProducerOptions(_topic!)
            {
                InitialSequenceId = _initialSequenceId,
                ProducerName = _producerName
            };

            return _pulsarClient.CreateProducer(options);
        }
    }
}
