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
    using Events;
    using Exceptions;
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class Producer<TMessage> : IEstablishNewChannel, IProducer<TMessage>
    {
        private readonly Guid _correlationId;
        private readonly IRegisterEvent _eventRegister;
        private readonly IExecute _executor;
        private readonly IStateChanged<ProducerState> _state;
        private readonly PulsarClient _pulsarClient;
        private readonly ProducerOptions<TMessage> _options;
        private readonly ConcurrentDictionary<int, IProducer<TMessage>> _producers;
        private readonly IMessageRouter _messageRouter;
        private readonly CancellationTokenSource _cts = new();
        private int _producersCount;
        private int _isDisposed;
        public Uri ServiceUrl { get; }
        public string Topic { get; }

        public Producer(
            Guid correlationId,
            Uri serviceUrl,
            string topic,
            IRegisterEvent registerEvent,
            IExecute executor,
            IStateChanged<ProducerState> state,
            ProducerOptions<TMessage> options,
            PulsarClient pulsarClient
        )
        {
            _correlationId = correlationId;
            ServiceUrl = serviceUrl;
            Topic = topic;
            _eventRegister = registerEvent;
            _executor = executor;
            _state = state;
            _isDisposed = 0;
            _options = options;
            _pulsarClient = pulsarClient;
            _messageRouter = options.MessageRouter;

            _producers = new ConcurrentDictionary<int, IProducer<TMessage>>(1, 31);
        }

        private void CreateSubProducers(int startIndex, int count)
        {
            if (count == 0)
            {
                var producer = _pulsarClient.NewSubProducer(Topic, _options, _executor, _correlationId);
                _producers[0] = producer;
                return;
            }

            for (var i = startIndex; i < count; ++i)
            {
                var producer = _pulsarClient.NewSubProducer(Topic, _options, _executor, _correlationId, (uint) i);
                _producers[i] = producer;
            }
        }

        private async void UpdatePartitions(CancellationToken cancellationToken)
        {
            var partitionsCount = (int) await _pulsarClient.GetNumberOfPartitions(Topic, cancellationToken).ConfigureAwait(false);
            _eventRegister.Register(new UpdatePartitions(_correlationId, (uint)partitionsCount));
            CreateSubProducers(_producers.Count, partitionsCount);
            _producersCount = partitionsCount;
        }

        public bool IsFinalState()
            => _state.IsFinalState();

        public bool IsFinalState(ProducerState state)
            => _state.IsFinalState(state);

        public async ValueTask<ProducerState> OnStateChangeTo(ProducerState state, CancellationToken cancellationToken = default)
            => await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);

        public async ValueTask<ProducerState> OnStateChangeFrom(ProducerState state, CancellationToken cancellationToken = default)
            => await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            _cts.Cancel();
            _cts.Dispose();

            foreach (var producer in _producers.Values)
            {
                await producer.DisposeAsync().ConfigureAwait(false);
            }

            _eventRegister.Register(new ProducerDisposed(_correlationId));
        }

        public async Task EstablishNewChannel(CancellationToken cancellationToken)
        {
            await _executor.Execute(() => UpdatePartitions(cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private int ChoosePartitions(MessageMetadata? metadata)
        {
            if (_producers.IsEmpty)
            {
                throw new LookupNotReadyException();
            }
            return _producersCount == 0 ? 0 : _messageRouter.ChoosePartition(metadata, _producersCount);
        }

        public async ValueTask<MessageId> Send(TMessage message, CancellationToken cancellationToken = default)
            => await _executor.Execute(() => _producers[ChoosePartitions(null)].Send(message, cancellationToken), cancellationToken).ConfigureAwait(false);

        public async ValueTask<MessageId> Send(MessageMetadata metadata, TMessage message, CancellationToken cancellationToken = default)
            => await _executor.Execute(() => _producers[ChoosePartitions(metadata)].Send(message, cancellationToken), cancellationToken).ConfigureAwait(false);
    }
}
