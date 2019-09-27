using System;
using System.Collections.Generic;
using System.Threading;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A reader abstraction.
    /// </summary>
    public interface IReader : IStateChanged<ReaderState>, IAsyncDisposable
    {
        /// <summary>
        /// Get an IAsyncEnumerable for reading messages
        /// </summary>
        IAsyncEnumerable<Message> Messages(CancellationToken cancellationToken = default);
    }
}
