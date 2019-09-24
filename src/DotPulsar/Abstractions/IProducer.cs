using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A producer abstraction.
    /// </summary>
    public interface IProducer : IStateChanged<ProducerState>, IAsyncDisposable
    {
        /// <summary>
        /// Sends a message.
        /// </summary>
        Task<MessageId> Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message and metadata.
        /// </summary>
        Task<MessageId> Send(MessageMetadata metadata, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
    }
}
