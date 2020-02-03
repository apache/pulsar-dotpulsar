/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

﻿using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal;
using DotPulsar.Internal.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            IAsyncDisposable[] disposables;

            lock (_lock)
            {
                if (_isClosed)
                    return;

                _isClosed = true;
                disposables = _disposabels.ToArray();
            }

            
            foreach (var disposable in disposables)
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
