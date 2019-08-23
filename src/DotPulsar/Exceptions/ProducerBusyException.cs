namespace DotPulsar.Exceptions
{
    public sealed class ProducerBusyException : DotPulsarException
    {
        public ProducerBusyException(string message) : base(message) { }
    }
}