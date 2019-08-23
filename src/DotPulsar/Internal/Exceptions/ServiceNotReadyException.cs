using DotPulsar.Exceptions;

namespace DotPulsar.Internal.Exceptions
{
    public sealed class ServiceNotReadyException : DotPulsarException
    {
        public ServiceNotReadyException(string message) : base(message) { }
    }
}
