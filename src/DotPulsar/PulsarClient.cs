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

namespace DotPulsar
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Exceptions;
    using Internal;
    using Internal.Abstractions;

    public sealed class PulsarClient : IPulsarClient
    {
        private readonly IConnectionPool _connectionPool;
        private readonly ProcessManager _processManager;
        private readonly IHandleException _exceptionHandler;
        private int _isDisposed;

        internal PulsarClient(IConnectionPool connectionPool, ProcessManager processManager, IHandleException exceptionHandler)
        {
            _connectionPool = connectionPool;
            _processManager = processManager;
            _exceptionHandler = exceptionHandler;
            _isDisposed = 0;
            DotPulsarEventSource.Log.ClientCreated();
        }

        public static IPulsarClientBuilder Builder()
            => new PulsarClientBuilder();

        public IProducer CreateProducer(ProducerOptions options)
        {
            ThrowIfDisposed();
            var correlationId = Guid.NewGuid();
            var executor = new Executor(correlationId, _processManager, _exceptionHandler);
            var factory = new ProducerChannelFactory(correlationId, _processManager, _connectionPool, executor, options);
            var stateManager = new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);
            var producer = new Producer(correlationId, _processManager, new NotReadyChannel(), new AsyncLockExecutor(executor), stateManager);
            var process = new ProducerProcess(correlationId, stateManager, factory, producer);
            _processManager.Add(process);
            process.Start();
            return producer;
        }

        public IConsumer CreateConsumer(ConsumerOptions options)
        {
            ThrowIfDisposed();
            var correlationId = Guid.NewGuid();
            var executor = new Executor(correlationId, _processManager, _exceptionHandler);
            var factory = new ConsumerChannelFactory(correlationId, _processManager, _connectionPool, executor, options);

            var stateManager = new StateManager<ConsumerState>(ConsumerState.Disconnected, ConsumerState.Closed, ConsumerState.ReachedEndOfTopic,
                ConsumerState.Faulted);
            var consumer = new Consumer(correlationId, _processManager, new NotReadyChannel(), new AsyncLockExecutor(executor), stateManager);
            var process = new ConsumerProcess(correlationId, stateManager, factory, consumer, options.SubscriptionType == SubscriptionType.Failover);
            _processManager.Add(process);
            process.Start();
            return consumer;
        }

        public IReader CreateReader(ReaderOptions options)
        {
            ThrowIfDisposed();
            var correlationId = Guid.NewGuid();
            var executor = new Executor(correlationId, _processManager, _exceptionHandler);
            var factory = new ReaderChannelFactory(correlationId, _processManager, _connectionPool, executor, options);
            var stateManager = new StateManager<ReaderState>(ReaderState.Disconnected, ReaderState.Closed, ReaderState.ReachedEndOfTopic, ReaderState.Faulted);
            var reader = new Reader(correlationId, _processManager, new NotReadyChannel(), new AsyncLockExecutor(executor), stateManager);
            var process = new ReaderProcess(correlationId, stateManager, factory, reader);
            _processManager.Add(process);
            process.Start();
            return reader;
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            if (_processManager is IAsyncDisposable disposable)
                await disposable.DisposeAsync().ConfigureAwait(false);

            DotPulsarEventSource.Log.ClientDisposed();
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new PulsarClientDisposedException();
        }
    }
}
