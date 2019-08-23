using DotPulsar.Exceptions;

namespace DotPulsar.Internal.Exceptions
{
    public sealed class TooManyRequestsException : DotPulsarException
    {
        public TooManyRequestsException(string message) : base(message) { }
    }
}
