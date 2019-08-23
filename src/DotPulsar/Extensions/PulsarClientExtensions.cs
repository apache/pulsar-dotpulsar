using DotPulsar.Abstractions;
using DotPulsar.Internal;

namespace DotPulsar.Extensions
{
    public static class PulsarClientExtensions
    {
        public static IProducerBuilder NewProducer(this IPulsarClient pulsarClient) => new ProducerBuilder(pulsarClient);
        public static IConsumerBuilder NewConsumer(this IPulsarClient pulsarClient) => new ConsumerBuilder(pulsarClient);
        public static IReaderBuilder NewReader(this IPulsarClient pulsarClient) => new ReaderBuilder(pulsarClient);
    }
}
