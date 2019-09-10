namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A reader building abstraction
    /// </summary>
    public interface IReaderBuilder
    {
        /// <summary>
        /// Set the reader name. This is optional.
        /// </summary>
        IReaderBuilder ReaderName(string name);

        /// <summary>
        /// Number of messages that will be prefetched. Default is 1000.
        /// </summary>
        IReaderBuilder MessagePrefetchCount(uint count);

        /// <summary>
        /// Whether to read from the compacted topic.
        /// </summary>
        IReaderBuilder ReadCompacted(bool readCompacted);

        /// <summary>
        /// The initial reader position is set to the specified message id. This is required.
        /// </summary>
        IReaderBuilder StartMessageId(MessageId messageId);

        /// <summary>
        /// Set the topic for this reader. This is required.
        /// </summary>
        IReaderBuilder Topic(string topic);

        /// <summary>
        /// Create the reader
        /// </summary>
        IReader Create();
    }
}
