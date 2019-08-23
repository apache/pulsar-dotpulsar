namespace DotPulsar.Exceptions
{
    public sealed class ConsumerClosedException : DotPulsarException
    {
        public ConsumerClosedException() : base("Consumer has closed") { }
    }
}
