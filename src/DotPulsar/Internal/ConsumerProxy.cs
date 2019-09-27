using DotPulsar.Internal.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class ConsumerProxy : IConsumerProxy, IDisposable
    {
        private readonly object _lock;
        private readonly StateManager<ConsumerState> _stateManager;
        private readonly AsyncQueue<MessagePackage> _queue;
        private bool _hasDisconnected;

        public ConsumerProxy(StateManager<ConsumerState> stateManager, AsyncQueue<MessagePackage> queue)
        {
            _lock = new object();
            _stateManager = stateManager;
            _queue = queue;
            _hasDisconnected = false;
        }

        public void Active() => SetState(ConsumerState.Active);
        public void Inactive() => SetState(ConsumerState.Inactive);
        public void ReachedEndOfTopic() => SetState(ConsumerState.ReachedEndOfTopic);

        public void Disconnected()
        {
            lock (_lock)
            {
                if (_hasDisconnected)
                    return;

                _stateManager.SetState(ConsumerState.Disconnected);
                _hasDisconnected = true;
            }
        }


        public void Enqueue(MessagePackage package) => _queue.Enqueue(package);
        public async ValueTask<MessagePackage> Dequeue(CancellationToken cancellationToken) => await _queue.Dequeue(cancellationToken);

        private void SetState(ConsumerState state)
        {
            lock (_lock)
            {
                if (!_hasDisconnected)
                    _stateManager.SetState(state);
            }
        }

        public void Dispose() => _queue.Dispose();
    }
}
