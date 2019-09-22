using System;

namespace DotPulsar.Exceptions
{
    public sealed class AuthenticationException : DotPulsarException
    {
        public AuthenticationException(string message) : base(message) { }

        public AuthenticationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
