using System;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A pulsar client abstraction.
    /// </summary>
    public interface IPulsarClient : IDisposable
    {
        /// <summary>
        /// Create a producer.
        /// </summary>
        IProducer CreateProducer(ProducerOptions options);

        /// <summary>
        /// Create a consumer.
        /// </summary>
        IConsumer CreateConsumer(ConsumerOptions options);

        /// <summary>
        /// Create a reader.
        /// </summary>
        IReader CreateReader(ReaderOptions options);
    }
}
