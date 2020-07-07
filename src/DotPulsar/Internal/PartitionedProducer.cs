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
    using DotPulsar.Abstractions;
    using DotPulsar.Internal.Abstractions;
    using DotPulsar.Internal.Events;
    using System;
    using System.Buffers;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class PartitionedProducer : IProducer
    {
        private readonly ConcurrentDictionary<int, IProducer> _producers;
        private readonly Guid _correlationId;
        private readonly IRegisterEvent _eventRegister;
        private readonly IExecute _executor;
        private readonly IStateChanged<ProducerState> _state;
        private readonly IPulsarClient _pulsarClient;
        private readonly PartitionedTopicMetadata _partitionedTopicMetadata;
        private int _isDisposed;

        public string Topic { get; }

        public PartitionedProducer(
            Guid correlationId,
            string topic,
            IRegisterEvent registerEvent,
            IProducerChannel initialChannel,
            IExecute executor,
            IStateChanged<ProducerState> state,
            IPulsarClient pulsarClient,
            PartitionedTopicMetadata partitionedTopicMetadata)
        {
            _correlationId = correlationId;
            Topic = topic;
            _eventRegister = registerEvent;
            _executor = executor;
            _state = state;
            _pulsarClient = pulsarClient;
            _partitionedTopicMetadata = partitionedTopicMetadata;
            _isDisposed = 0;

            _eventRegister.Register(new PartitionedProducerCreated(_correlationId, this));

            _producers = new ConcurrentDictionary<int, IProducer>();

            Start();
        }

        private void Start()
        {
            for(int )
        }

        private IProducer GetProducer()
        {
            if (_producers.Count > 0) return _producers[0];
            return null;
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            _eventRegister.Register(new PartitionedProducerDisposed(_correlationId, this))
        }

        public bool IsFinalState()
            => _state.IsFinalState();

        public bool IsFinalState(ProducerState state)
            => _state.IsFinalState(state);

        public ValueTask<MessageId> Send(byte[] data, CancellationToken cancellationToken)
            => GetProducer().Send(new ReadOnlySequence<byte>(data), cancellationToken);

        public ValueTask<MessageId> Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            => GetProducer().Send(new ReadOnlySequence<byte>(data), cancellationToken);

        public ValueTask<MessageId> Send(ReadOnlySequence<byte> data, CancellationToken cancellationToken)
            => GetProducer().Send(data, cancellationToken);

        public ValueTask<MessageId> Send(MessageMetadata metadata, byte[] data, CancellationToken cancellationToken)
            => GetProducer().Send(metadata, new ReadOnlySequence<byte>(data), cancellationToken);

        public ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            => GetProducer().Send(metadata, new ReadOnlySequence<byte>(data), cancellationToken);

        public ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlySequence<byte> data, CancellationToken cancellationToken)
            => GetProducer().Send(metadata, data, cancellationToken);

        public async ValueTask<ProducerStateChanged> StateChangedFrom(ProducerState state, CancellationToken cancellationToken = default)
        {
            var newState = await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);
            return new ProducerStateChanged(this, newState);
        }

        public async ValueTask<ProducerStateChanged> StateChangedTo(ProducerState state, CancellationToken cancellationToken = default)
        {
            var newState = await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);
            return new ProducerStateChanged(this, newState);
        }
    }
}
