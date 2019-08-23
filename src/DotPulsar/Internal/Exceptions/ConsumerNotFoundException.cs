using DotPulsar.Exceptions;

namespace DotPulsar.Internal.Exceptions
{
    public sealed class ConsumerNotFoundException : DotPulsarException
    {
        public ConsumerNotFoundException(string message) : base(message) { }
    }
}
