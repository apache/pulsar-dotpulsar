namespace DotPulsar
{
    public sealed class ReaderOptions
    {
        internal const uint DefaultMessagePrefetchCount = 1000;
        internal const bool DefaultReadCompacted = false;

        public ReaderOptions(MessageId startMessageId, string topic)
        {
            MessagePrefetchCount = DefaultMessagePrefetchCount;
            ReadCompacted = DefaultReadCompacted;
            StartMessageId = startMessageId;
            Topic = topic;
        }

        public string? ReaderName { get; set; }
        public uint MessagePrefetchCount { get; set; }
        public bool ReadCompacted { get; set; }
        public MessageId StartMessageId { get; set; }
        public string Topic { get; set; }
    }
}
