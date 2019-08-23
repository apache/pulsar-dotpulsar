using DotPulsar.Exceptions;
using System;

namespace DotPulsar.Internal.Exceptions
{
    public sealed class UnexpectedResponseException : DotPulsarException
    {
        public UnexpectedResponseException(string message) : base(message) { }
        public UnexpectedResponseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
