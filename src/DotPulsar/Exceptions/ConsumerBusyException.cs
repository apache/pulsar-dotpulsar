namespace DotPulsar.Exceptions
{
    public sealed class ConsumerBusyException : DotPulsarException
    {
        public ConsumerBusyException(string message) : base(message) { }
    }
}