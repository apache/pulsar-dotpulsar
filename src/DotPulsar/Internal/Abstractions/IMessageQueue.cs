namespace DotPulsar.Internal.Abstractions
{
    using System;

    public interface IMessageQueue : IDequeue<MessagePackage>, IDisposable
    {
        void Acknowledge(MessageId obj);
        void NegativeAcknowledge(MessageId obj);
    }
}