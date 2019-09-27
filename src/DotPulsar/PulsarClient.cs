using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal;
using DotPulsar.Internal.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotPulsar
{
    public sealed class PulsarClient : IPulsarClient
    {
        private readonly object _lock;
        private readonly IFaultStrategy _faultStrategy;
        private readonly LinkedList<IAsyncDisposable> _disposabels;
        private readonly ConnectionPool _connectionPool;
        private bool _isClosed;

        internal PulsarClient(ConnectionPool connectionPool, IFaultStrategy faultStrategy)
        {
            _lock = new object();
            _faultStrategy = faultStrategy;
            _disposabels = new LinkedList<IAsyncDisposable>();
            _connectionPool = connectionPool;
            _isClosed = false;
        }

        public static IPulsarClientBuilder Builder() => new PulsarClientBuilder();

        public IProducer CreateProducer(ProducerOptions options)
        {
            lock (_lock)
            {
                ThrowIfClosed();
                var producer = new Producer(new ProducerStreamFactory(_connectionPool, options, _faultStrategy), _faultStrategy);
                _disposabels.AddFirst(producer);
                producer.StateChangedTo(ProducerState.Closed, default).AsTask().ContinueWith(t => Remove(producer));
                return producer;
            }
        }

        public IConsumer CreateConsumer(ConsumerOptions options)
        {
            lock (_lock)
            {
                ThrowIfClosed();
                var consumer = new Consumer(new ConsumerStreamFactory(_connectionPool, options, _faultStrategy), _faultStrategy, options.SubscriptionType != SubscriptionType.Failover);
                _disposabels.AddFirst(consumer);
                consumer.StateChangedTo(ConsumerState.Closed, default).AsTask().ContinueWith(t => Remove(consumer));
                return consumer;
            }
        }

        public IReader CreateReader(ReaderOptions options)
        {
            lock (_lock)
            {
                ThrowIfClosed();
                var reader = new Reader(new ConsumerStreamFactory(_connectionPool, options, _faultStrategy), _faultStrategy);
                _disposabels.AddFirst(reader);
                reader.StateChangedTo(ReaderState.Closed, default).AsTask().ContinueWith(t => Remove(reader));
                return reader;
            }
        }

        public async ValueTask DisposeAsync()
        {
            lock (_lock)
            {
                ThrowIfClosed();
                _isClosed = true;
            }

            foreach (var disposable in _disposabels)
            {
                await disposable.DisposeAsync();
            }

            await _connectionPool.DisposeAsync();
        }

        private void ThrowIfClosed()
        {
            if (_isClosed)
                throw new PulsarClientClosedException();
        }

        private void Remove(IAsyncDisposable disposable)
        {
            lock (_lock)
            {
                _disposabels.Remove(disposable);
            }
        }
    }
}
