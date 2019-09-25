using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System;

namespace DotPulsar.Internal
{
    public sealed class ConsumerManager : IDisposable
    {
        private readonly IdLookup<IConsumerProxy> _proxies;

        public ConsumerManager() => _proxies = new IdLookup<IConsumerProxy>();

        public bool HasConsumers => !_proxies.IsEmpty();

        public void Outgoing(CommandSubscribe subscribe, IConsumerProxy proxy) => subscribe.ConsumerId = _proxies.Add(proxy);

        public void Dispose()
        {
            foreach (var id in _proxies.AllIds())
            {
                RemoveConsumer(id);
            }
        }

        public void Incoming(MessagePackage package)
        {
            var consumerId = package.Command.ConsumerId;
            var proxy = _proxies[consumerId];
            proxy?.Enqueue(package);
        }

        public void Incoming(CommandCloseConsumer command) => RemoveConsumer(command.ConsumerId);

        public void Incoming(CommandActiveConsumerChange command)
        {
            var proxy = _proxies[command.ConsumerId];
            if (proxy is null) return;

            if (command.IsActive)
                proxy.Active();
            else
                proxy.Inactive();
        }

        public void Incoming(CommandReachedEndOfTopic command)
        {
            var proxy = _proxies[command.ConsumerId];
            proxy?.ReachedEndOfTopic();
        }

        public void Remove(ulong consumerId) => _proxies.Remove(consumerId);

        private void RemoveConsumer(ulong consumerId)
        {
            var proxy = _proxies[consumerId];
            if (proxy is null) return;
            proxy.Disconnected();
            _proxies.Remove(consumerId);
        }
    }
}
