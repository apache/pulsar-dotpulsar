using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal;
using DotPulsar.Internal.Abstractions;
using System;
using System.Collections.Generic;

namespace DotPulsar
{
    public sealed class PulsarClient : IPulsarClient
    {
        private readonly object _lock;
        private readonly IFaultStrategy _faultStrategy;
        private readonly LinkedList<IDisposable> _disposabels;
        private readonly ConnectionPool _connectionPool;
        private bool _isClosed;

        internal PulsarClient(ConnectionPool connectionPool, IFaultStrategy faultStrategy)
        {
            _lock = new object();
            _faultStrategy = faultStrategy;
            _disposabels = new LinkedList<IDisposable>();
            _connectionPool = connectionPool;
            _isClosed = false;
        }

        public static IPulsarClientBuilder Builder() => new PulsarClientBuilder();

        public IProducer CreateProducer(ProducerOptions options)
        {
            Validate(options);

            lock (_lock)
            {
                ThrowIfClosed();
                var producer = new Producer(new ProducerStreamFactory(_connectionPool, options, _faultStrategy), _faultStrategy);
                _disposabels.AddFirst(producer);
                producer.StateChangedTo(ProducerState.Closed, default).ContinueWith(t => Remove(producer));
                return producer;
            }
        }

        public IConsumer CreateConsumer(ConsumerOptions options)
        {
            Validate(options);

            lock (_lock)
            {
                ThrowIfClosed();
                var consumer = new Consumer(new ConsumerStreamFactory(_connectionPool, options, _faultStrategy), _faultStrategy, options.SubscriptionType != SubscriptionType.Failover);
                _disposabels.AddFirst(consumer);
                consumer.StateChangedTo(ConsumerState.Closed, default).ContinueWith(t => Remove(consumer));
                return consumer;
            }
        }

        public IReader CreateReader(ReaderOptions options)
        {
            Validate(options);

            lock (_lock)
            {
                ThrowIfClosed();
                var reader = new Reader(new ConsumerStreamFactory(_connectionPool, options, _faultStrategy), _faultStrategy);
                _disposabels.AddFirst(reader);
                reader.StateChangedTo(ReaderState.Closed, default).ContinueWith(t => Remove(reader));
                return reader;
            }
        }

        public void Dispose() //While we wait for IAsyncDisposable
        {
            lock (_lock)
            {
                ThrowIfClosed();
                _isClosed = true;
                foreach (var disposable in _disposabels)
                {
                    disposable.Dispose();
                }
            }

            _connectionPool.Dispose();
        }

        private void ThrowIfClosed()
        {
            if (_isClosed)
                throw new PulsarClientClosedException();
        }

        private void Remove(IDisposable disposable)
        {
            lock (_lock)
            {
                _disposabels.Remove(disposable);
            }
        }

        private void Validate(ProducerOptions options)
        {
            if (string.IsNullOrEmpty(options.Topic))
                throw new ConfigurationException("ProducerOptions.Topic may not be null or empty");
        }

        private void Validate(ConsumerOptions options)
        {
            if (string.IsNullOrEmpty(options.SubscriptionName))
                throw new ConfigurationException("ConsumerOptions.SubscriptionName may not be null or empty");

            if (string.IsNullOrEmpty(options.Topic))
                throw new ConfigurationException("ConsumerOptions.Topic may not be null or empty");
        }

        private void Validate(ReaderOptions options)
        {
            if (options.StartMessageId is null)
                throw new ConfigurationException("ReaderOptions.StartMessageId may not be null");

            if (string.IsNullOrEmpty(options.Topic))
                throw new ConfigurationException("ReaderOptions.Topic may not be null or empty");
        }
    }
}
