using DotPulsar.Internal.Abstractions;

namespace DotPulsar.Internal
{
    public sealed class ProducerProxy : IProducerProxy
    {
        private readonly object _lock;
        private readonly StateManager<ProducerState> _stateManager;
        private bool _hasDisconnected;

        public ProducerProxy(StateManager<ProducerState> stateManager)
        {
            _lock = new object();
            _stateManager = stateManager;
            _hasDisconnected = false;
        }

        public void Connected()
        {
            lock (_lock)
            {
                if (!_hasDisconnected)
                    _stateManager.SetState(ProducerState.Connected);
            }
        }

        public void Disconnected()
        {
            lock (_lock)
            {
                if (_hasDisconnected)
                    return;

                _stateManager.SetState(ProducerState.Disconnected);
                _hasDisconnected = true;
            }
        }
    }
}
