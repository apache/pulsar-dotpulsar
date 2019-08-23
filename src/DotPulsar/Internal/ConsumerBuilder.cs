using DotPulsar.Abstractions;

namespace DotPulsar.Internal
{
    public sealed class ConsumerBuilder : IConsumerBuilder
    {
        private readonly IPulsarClient _pulsarClient;
        private readonly ConsumerOptions _options;

        public ConsumerBuilder(IPulsarClient pulsarClient)
        {
            _pulsarClient = pulsarClient;
            _options = new ConsumerOptions();
        }

        public IConsumerBuilder ConsumerName(string name)
        {
            _options.ConsumerName = name;
            return this;
        }

        public IConsumerBuilder InitialPosition(SubscriptionInitialPosition initialPosition)
        {
            _options.InitialPosition = initialPosition;
            return this;
        }

        public IConsumerBuilder PriorityLevel(int priorityLevel)
        {
            _options.PriorityLevel = priorityLevel;
            return this;
        }

        public IConsumerBuilder MessagePrefetchCount(uint count)
        {
            _options.MessagePrefetchCount = count;
            return this;
        }

        public IConsumerBuilder SubscriptionName(string name)
        {
            _options.SubscriptionName = name;
            return this;
        }

        public IConsumerBuilder SubscriptionType(SubscriptionType type)
        {
            _options.SubscriptionType = type;
            return this;
        }

        public IConsumerBuilder Topic(string topic)
        {
            _options.Topic = topic;
            return this;
        }

        public IConsumer Create() => _pulsarClient.CreateConsumer(_options);
    }
}
