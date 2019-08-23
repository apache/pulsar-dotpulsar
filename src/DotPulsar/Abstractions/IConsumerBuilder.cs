namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A consumer building abstraction
    /// </summary>
    public interface IConsumerBuilder
    {
        /// <summary>
        /// Set the consumer name. This is optional.
        /// </summary>
        IConsumerBuilder ConsumerName(string name);

        /// <summary>
        /// Set initial position for the subscription. Default is 'Latest'.
        /// </summary>
        IConsumerBuilder InitialPosition(SubscriptionInitialPosition initialPosition);

        /// <summary>
        /// Set the priority level for the shared subscription consumer. Default is 0.
        /// </summary>
        IConsumerBuilder PriorityLevel(int priorityLevel);

        /// <summary>
        /// Number of messages that will be prefetched. Default is 1000.
        /// </summary>
        IConsumerBuilder MessagePrefetchCount(uint count);

        /// <summary>
        /// Set the subscription name for this consumer. This is required.
        /// </summary>
        IConsumerBuilder SubscriptionName(string name);

        /// <summary>
        /// Set the subscription type for this consumer. Default is 'Exclusive'.
        /// </summary>
        IConsumerBuilder SubscriptionType(SubscriptionType type);

        /// <summary>
        /// Set the topic for this consumer. This is required.
        /// </summary>
        IConsumerBuilder Topic(string topic);

        /// <summary>
        /// Create the consumer
        /// </summary>
        IConsumer Create();
    }
}
