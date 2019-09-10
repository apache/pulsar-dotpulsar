namespace DotPulsar
{
    public sealed class ReaderOptions
    {
        public string ReaderName { get; set; }
        public uint MessagePrefetchCount { get; set; }
        public bool ReadCompacted { get; set; }
        public MessageId StartMessageId { get; set; }
        public string Topic { get; set; }
    }
}
