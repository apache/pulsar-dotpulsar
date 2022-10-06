namespace DotPulsar.Internal.Exceptions;

using DotPulsar.Exceptions;
using System;

public sealed class ProducerSendReceiptOrderingException : DotPulsarException
{
    public ProducerSendReceiptOrderingException(string message) : base(message) {}

    public ProducerSendReceiptOrderingException(string message, Exception innerException) : base(message, innerException) { }
}
