namespace DotPulsar.Internal.Abstractions
{
    using DotPulsar.Abstractions;
    using System.Threading.Tasks;
    using System.Threading;
    using System;

    public interface IUnackedMessageTracker : IDisposable
    {
        void Add(MessageId messageId);

        void Ack(MessageId messageId);

        Task Start(IConsumer consumer, CancellationToken cancellationToken = default);
    }
}
