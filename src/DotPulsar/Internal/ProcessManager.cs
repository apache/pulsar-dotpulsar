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

namespace DotPulsar.Internal
{
    using Abstractions;
    using Events;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class ProcessManager : IRegisterEvent, IAsyncDisposable
    {
        private readonly ConcurrentDictionary<Guid, IProcess> _processes;
        private readonly IConnectionPool _connectionPool;

        public ProcessManager(IConnectionPool connectionPool)
        {
            _processes = new ConcurrentDictionary<Guid, IProcess>();
            _connectionPool = connectionPool;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var proc in _processes.Values.ToArray())
                await proc.DisposeAsync().ConfigureAwait(false);

            await _connectionPool.DisposeAsync().ConfigureAwait(false);
        }

        public void Add(IProcess process)
            => _processes[process.CorrelationId] = process;

        private async void Remove(Guid correlationId)
        {
            if (_processes.TryRemove(correlationId, out var process))
                await process.DisposeAsync().ConfigureAwait(false);
        }

        public void Register(IEvent e)
        {
            switch (e)
            {
                case ConsumerCreated _:
                    DotPulsarEventSource.Log.ConsumerCreated();
                    break;
                case ConsumerDisposed consumerDisposed:
                    Remove(consumerDisposed.CorrelationId);
                    DotPulsarEventSource.Log.ConsumerDisposed();
                    break;
                case ProducerCreated _:
                    DotPulsarEventSource.Log.ProducerCreated();
                    break;
                case ProducerDisposed producerDisposed:
                    Remove(producerDisposed.CorrelationId);
                    DotPulsarEventSource.Log.ProducerDisposed();
                    break;
                case ReaderCreated _:
                    DotPulsarEventSource.Log.ReaderCreated();
                    break;
                case ReaderDisposed readerDisposed:
                    Remove(readerDisposed.CorrelationId);
                    DotPulsarEventSource.Log.ReaderDisposed();
                    break;
                default:
                    if (_processes.TryGetValue(e.CorrelationId, out var process))
                        process.Handle(e);
                    break;
            }
        }
    }
}
