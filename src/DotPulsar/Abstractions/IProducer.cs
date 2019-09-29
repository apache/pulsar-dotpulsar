using System;
using System.Buffers;
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
        ValueTask<MessageId> Send(byte[] data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message.
        /// </summary>
        ValueTask<MessageId> Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message.
        /// </summary>
        ValueTask<MessageId> Send(ReadOnlySequence<byte> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message with metadata.
        /// </summary>
        ValueTask<MessageId> Send(MessageMetadata metadata, byte[] data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message with metadata.
        /// </summary>
        ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message with metadata.
        /// </summary>
        ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlySequence<byte> data, CancellationToken cancellationToken = default);
    }
}
