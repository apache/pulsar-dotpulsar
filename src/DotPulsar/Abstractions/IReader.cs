using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A reader abstraction.
    /// </summary>
    public interface IReader : IStateChanged<ReaderState>, IAsyncDisposable
    {
        /// <summary>
        /// Receives a single message.
        /// </summary>
        Task<Message> Receive(CancellationToken cancellationToken = default);
    }
}
