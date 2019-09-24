namespace DotPulsar
{
    public sealed class ProducerOptions
    {
        internal const ulong DefaultInitialSequenceId = 0;

        public ProducerOptions(string topic)
        {
            InitialSequenceId = DefaultInitialSequenceId;
            Topic = topic;
        }

        public string? ProducerName { get; set; }
        public ulong InitialSequenceId { get; set; }
        public string Topic { get; set; }
    }
}
