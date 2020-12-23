namespace DotPulsar.Internal.Abstractions
{
    using DotPulsar.Abstractions;
    using System.Threading.Tasks;
    using System;

    public interface IUnackedMessageTracker : IDisposable
    {
        void Add(MessageId messageId);

        void Ack(MessageId messageId);

        Task Start(IConsumer consumer);
    }
}
