using System;

namespace DotPulsar.Exceptions
{
    public abstract class DotPulsarException : Exception
    {
        public DotPulsarException(string message) : base(message) { }

        public DotPulsarException(string message, Exception innerException) : base(message, innerException) { }
    }
}
