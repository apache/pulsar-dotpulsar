namespace DotPulsar
{
    public sealed class ProducerOptions
    {
        public string ProducerName { get; set; }
        public ulong InitialSequenceId { get; set; }
        public string Topic { get; set; }
    }
}
