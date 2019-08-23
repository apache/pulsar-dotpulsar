using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class Producer : IProducer
    {
        private readonly Executor _executor;
        private readonly IProducerStreamFactory _streamFactory;
        private readonly IFaultStrategy _faultStrategy;
        private readonly StateManager<ProducerState> _stateManager;
        private readonly CancellationTokenSource _connectTokenSource;
        private readonly Task _connectTask;
        private Action _throwIfClosedOrFaulted;
        private IProducerStream Stream { get; set; }

        public Producer(IProducerStreamFactory streamFactory, IFaultStrategy faultStrategy)
        {
            _executor = new Executor(ExecutorOnException);
            _streamFactory = streamFactory;
            _faultStrategy = faultStrategy;
            _stateManager = new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);
            _connectTokenSource = new CancellationTokenSource();
            Stream = new NotReadyStream();
            _connectTask = Connect(_connectTokenSource.Token);
            _throwIfClosedOrFaulted = () => { };
        }

        public async Task<ProducerState> StateChangedTo(ProducerState state, CancellationToken cancellationToken) => await _stateManager.StateChangedTo(state, cancellationToken);
        public async Task<ProducerState> StateChangedFrom(ProducerState state, CancellationToken cancellationToken) => await _stateManager.StateChangedFrom(state, cancellationToken);
        public bool IsFinalState() => _stateManager.IsFinalState();
        public bool IsFinalState(ProducerState state) => _stateManager.IsFinalState(state);

        public void Dispose()
        {
            _executor.Dispose();
            _connectTokenSource.Cancel();
            _connectTask.Wait();
        }

        public async Task<MessageId> Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            => await Send(new MessageMetadata(), data, cancellationToken);

        public async Task<MessageId> Send(MessageMetadata metadata, ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            => await Send(metadata.Metadata, data, cancellationToken);

        private async Task<MessageId> Send(PulsarApi.MessageMetadata metadata, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken)
        {
            var response = await _executor.Execute(() => Stream.Send(metadata, payload), cancellationToken);
            return new MessageId(response.MessageId);
        }

        private async Task ExecutorOnException(Exception exception, CancellationToken cancellationToken)
        {
            _throwIfClosedOrFaulted();

            switch (_faultStrategy.DetermineFaultAction(exception))
            {
                case FaultAction.Retry:
                    await Task.Delay(_faultStrategy.TimeToWait, cancellationToken);
                    break;
                case FaultAction.Relookup:
                    await _stateManager.StateChangedTo(ProducerState.Connected, cancellationToken);
                    break;
                case FaultAction.Fault:
                    HasFaulted(exception);
                    break;
            }

            _throwIfClosedOrFaulted();
        }

        private void HasFaulted(Exception exception)
        {
            _throwIfClosedOrFaulted = () => throw exception;
            _stateManager.SetState(ProducerState.Faulted);
        }

        private void HasClosed()
        {
            _throwIfClosedOrFaulted = () => throw new ProducerClosedException();
            _stateManager.SetState(ProducerState.Closed);
        }

        private async Task Connect(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    var proxy = new ProducerProxy(_stateManager);

                    using (Stream = await _streamFactory.CreateStream(proxy, cancellationToken))
                    {
                        proxy.Connected();
                        await _stateManager.StateChangedFrom(ProducerState.Connected, cancellationToken);
                        if (_stateManager.IsFinalState())
                            return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                HasClosed();
            }
            catch (Exception exception)
            {
                HasFaulted(exception);
            }
        }
    }
}
