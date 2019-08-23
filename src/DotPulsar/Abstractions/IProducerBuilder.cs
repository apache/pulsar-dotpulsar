namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A producer building abstraction
    /// </summary>
    public interface IProducerBuilder
    {
        /// <summary>
        /// Set the producer name. This is optional.
        /// </summary>
        IProducerBuilder ProducerName(string name);

        /// <summary>
        /// Set the initial sequence id. Default is 0.
        /// </summary>
        IProducerBuilder InitialSequenceId(ulong initialSequenceId);

        /// <summary>
        /// Set the topic for this producer. This is required.
        /// </summary>
        IProducerBuilder Topic(string topic);

        /// <summary>
        /// Create the producer
        /// </summary>
        IProducer Create();
    }
}
