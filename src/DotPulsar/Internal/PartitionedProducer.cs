namespace DotPulsar.Internal
{
    using Abstractions;
    using DotPulsar.Abstractions;
    using Events;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class PartitionedProducer<TMessage> : IProducer<TMessage>
    {
        private readonly Guid _correlationId;
        private readonly IRegisterEvent _eventRegister;
        private readonly IStateChanged<ProducerState> _state;
        private readonly PulsarClient _pulsarClient;
        private readonly ProducerOptions<TMessage> _options;
        private readonly ConcurrentDictionary<int, IProducer<TMessage>> _producers;
        private readonly IMessageRouter _messageRouter;
        private readonly CancellationTokenSource _cts = new();
        private readonly int _producersCount;
        private int _isDisposed;
        public Uri ServiceUrl { get; }
        public string Topic { get; }

        public PartitionedProducer(
            Guid correlationId,
            Uri serviceUrl,
            string topic,
            IRegisterEvent registerEvent,
            IStateChanged<ProducerState> state,
            uint partitionsCount,
            ProducerOptions<TMessage> options,
            PulsarClient pulsarClient
        )
        {
            _correlationId = correlationId;
            ServiceUrl = serviceUrl;
            Topic = topic;
            _eventRegister = registerEvent;
            _state = state;
            _isDisposed = 0;
            _options = options;
            _pulsarClient = pulsarClient;
            _producersCount = (int) partitionsCount;
            _messageRouter = options.MessageRouter;

            _producers = new ConcurrentDictionary<int, IProducer<TMessage>>(Environment.ProcessorCount, _producersCount);
            CreateSubProducers(0, _producersCount);
        }

        private void CreateSubProducers(int startIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var producer = _pulsarClient.NewProducer(Topic, _options, (uint)(i+startIndex), _correlationId);
                _producers[i+startIndex] = producer;
            }
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

        public async ValueTask<MessageId> Send(TMessage message, CancellationToken cancellationToken = default)
            => await _producers[_messageRouter.ChoosePartition(null, _producersCount)].Send(message, cancellationToken);

        public async ValueTask<MessageId> Send(MessageMetadata metadata, TMessage message, CancellationToken cancellationToken = default)
            => await _producers[_messageRouter.ChoosePartition(metadata, _producersCount)].Send(message, cancellationToken);
    }
}
