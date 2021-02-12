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

namespace DotPulsar.Internal
{
    using Abstractions;
    using DotPulsar.Abstractions;
    using DotPulsar.Exceptions;
    using DotPulsar.Internal.Extensions;
    using Events;
    using Microsoft.Extensions.ObjectPool;
    using System;
    using System.Buffers;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class Producer : IProducer
    {
        private readonly ObjectPool<PulsarApi.MessageMetadata> _messageMetadataPool;
        private readonly Guid _correlationId;
        private readonly IRegisterEvent _eventRegister;
        private IProducerChannel _channel;
        private readonly IExecute _executor;
        private readonly IStateChanged<ProducerState> _state;
        private readonly SequenceId _sequenceId;
        private int _isDisposed;

        public string Topic { get; }

        public Producer(
            Guid correlationId,
            string topic,
            ulong initialSequenceId,
            IRegisterEvent registerEvent,
            IProducerChannel initialChannel,
            IExecute executor,
            IStateChanged<ProducerState> state)
        {
            var messageMetadataPolicy = new DefaultPooledObjectPolicy<PulsarApi.MessageMetadata>();
            _messageMetadataPool = new DefaultObjectPool<PulsarApi.MessageMetadata>(messageMetadataPolicy);
            _correlationId = correlationId;
            Topic = topic;
            _sequenceId = new SequenceId(initialSequenceId);
            _eventRegister = registerEvent;
            _channel = initialChannel;
            _executor = executor;
            _state = state;
            _isDisposed = 0;

            _eventRegister.Register(new ProducerCreated(_correlationId, this));
        }

        public async ValueTask<ProducerState> OnStateChangeTo(ProducerState state, CancellationToken cancellationToken)
            => await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);

        public async ValueTask<ProducerState> OnStateChangeFrom(ProducerState state, CancellationToken cancellationToken)
            => await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);

        public bool IsFinalState()
            => _state.IsFinalState();

        public bool IsFinalState(ProducerState state)
            => _state.IsFinalState(state);

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            _eventRegister.Register(new ProducerDisposed(_correlationId, this));
            await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
            await _channel.DisposeAsync().ConfigureAwait(false);
        }

        public async ValueTask<MessageId> Send(ReadOnlySequence<byte> data, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            var metadata = _messageMetadataPool.Get();
            try
            {
                metadata.SequenceId = _sequenceId.FetchNext();
                return await _executor.Execute(() => Send(metadata, data, cancellationToken), cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _messageMetadataPool.Return(metadata);
            }
        }

        public async ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlySequence<byte> data, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            var autoAssignSequenceId = metadata.SequenceId == 0;
            if (autoAssignSequenceId)
                metadata.SequenceId = _sequenceId.FetchNext();

            try
            {
                return await _executor.Execute(() => Send(metadata.Metadata, data, cancellationToken), cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (autoAssignSequenceId)
                    metadata.SequenceId = 0;
            }
        }

        private async ValueTask<MessageId> Send(PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> data, CancellationToken cancellationToken)
        {
            var response = await _channel.Send(metadata, data, cancellationToken).ConfigureAwait(false);
            return response.MessageId.ToMessageId();
        }

        internal async ValueTask SetChannel(IProducerChannel channel)
        {
            if (_isDisposed != 0)
            {
                await channel.DisposeAsync().ConfigureAwait(false);
                return;
            }

            var oldChannel = _channel;
            _channel = channel;

            if (oldChannel is not null)
                await oldChannel.DisposeAsync().ConfigureAwait(false);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new ProducerDisposedException();
        }
    }
}
