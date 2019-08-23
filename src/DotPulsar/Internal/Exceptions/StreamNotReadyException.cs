using DotPulsar.Exceptions;

namespace DotPulsar.Internal.Exceptions
{
    public sealed class StreamNotReadyException : DotPulsarException
    {
        public StreamNotReadyException() : base("The service is not ready yet") { }
    }
}
