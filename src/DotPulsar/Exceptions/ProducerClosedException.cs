namespace DotPulsar.Exceptions
{
    public sealed class ProducerClosedException : DotPulsarException
    {
        public ProducerClosedException() : base("Producer has closed") { }
    }
}
