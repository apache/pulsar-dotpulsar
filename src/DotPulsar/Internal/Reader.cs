/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using DotPulsar.Abstractions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Events;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class Reader : IReader
    {
        private readonly Guid _correlationId;
        private readonly IRegisterEvent _eventRegister;
        private IReaderChannel _channel;
        private readonly IExecute _executor;
        private readonly IStateChanged<ReaderState> _state;
        private int _isDisposed;

        public Reader(
            Guid correlationId,
            IRegisterEvent eventRegister,
            IReaderChannel initialChannel,
            IExecute executor,
            IStateChanged<ReaderState> state)
        {
            _correlationId = correlationId;
            _eventRegister = eventRegister;
            _channel = initialChannel;
            _executor = executor;
            _state = state;
            _isDisposed = 0;

            _eventRegister.Register(new ReaderCreated(_correlationId, this));
        }

        public async ValueTask<ReaderState> StateChangedTo(ReaderState state, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return await _state.StateChangedTo(state, cancellationToken);
        }

        public async ValueTask<ReaderState> StateChangedFrom(ReaderState state, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return await _state.StateChangedFrom(state, cancellationToken);
        }

        public bool IsFinalState()
        {
            ThrowIfDisposed();
            return _state.IsFinalState();
        }

        public bool IsFinalState(ReaderState state)
        {
            ThrowIfDisposed();
            return _state.IsFinalState(state);
        }

        public async IAsyncEnumerable<Message> Messages([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            while (!cancellationToken.IsCancellationRequested)
            {
                yield return await _executor.Execute(() => _channel.Receive(cancellationToken), cancellationToken);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;
            
            _eventRegister.Register(new ReaderDisposed(_correlationId, this));
            await _channel.DisposeAsync();
        }

        internal void SetChannel(IReaderChannel channel)
        {
            ThrowIfDisposed();
            _channel = channel;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new ObjectDisposedException(nameof(Reader));
        }
    }
}
