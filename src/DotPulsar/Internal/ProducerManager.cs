using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System;

namespace DotPulsar.Internal
{
    public sealed class ProducerManager : IDisposable
    {
        private readonly IdLookup<IProducerProxy> _proxies;

        public ProducerManager() => _proxies = new IdLookup<IProducerProxy>();

        public bool HasProducers => !_proxies.IsEmpty();

        public void Outgoing(CommandProducer producer, IProducerProxy proxy) => producer.ProducerId = _proxies.Add(proxy);

        public void Remove(ulong producerId) => _proxies.Remove(producerId);

        public void Incoming(CommandCloseProducer command) => RemoveProducer(command.ProducerId);

        public void Dispose()
        {
            foreach (var id in _proxies.AllIds())
            {
                RemoveProducer(id);
            }
        }

        private void RemoveProducer(ulong producerId)
        {
            var stateManager = _proxies[producerId];
            if (stateManager == null) return;
            stateManager.Disconnected();
            _proxies.Remove(producerId);
        }
    }
}
