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
#if NETSTANDARD2_1
    using System.Diagnostics.Tracing;
    using System.Threading;

    public sealed class DotPulsarEventSource : EventSource
    {
        private readonly PollingCounter _totalClientsCounter;
        private long _totalClients;

        private readonly PollingCounter _currentClientsCounter;
        private long _currentClients;

        private readonly PollingCounter _totalConnectionsCounter;
        private long _totalConnections;

        private readonly PollingCounter _currentConnectionsCounter;
        private long _currentConnections;

        private readonly PollingCounter _totalConsumersCounter;
        private long _totalConsumers;

        private readonly PollingCounter _currentConsumersCounter;
        private long _currentConsumers;

        private readonly PollingCounter _totalProducersCounter;
        private long _totalProducers;

        private readonly PollingCounter _currentProducersCounter;
        private long _currentProducers;

        private readonly PollingCounter _totalReadersCounter;
        private long _totalReaders;

        private readonly PollingCounter _currentReadersCounter;
        private long _currentReaders;

        public static readonly DotPulsarEventSource Log = new DotPulsarEventSource();

        public DotPulsarEventSource() : base("DotPulsar")
        {
            _totalClientsCounter = new PollingCounter("total-clients", this, () => Interlocked.Read(ref _totalClients))
            {
                DisplayName = "Total number of clients"
            };

            _currentClientsCounter = new PollingCounter("current-clients", this, () => Interlocked.Read(ref _currentClients))
            {
                DisplayName = "Current number of clients"
            };

            _totalConnectionsCounter = new PollingCounter("total-connections", this, () => Interlocked.Read(ref _totalConnections))
            {
                DisplayName = "Total number of connections"
            };

            _currentConnectionsCounter = new PollingCounter("current-connections", this, () => Interlocked.Read(ref _currentConnections))
            {
                DisplayName = "Current number of connections"
            };

            _totalConsumersCounter = new PollingCounter("total-consumers", this, () => Interlocked.Read(ref _totalConsumers))
            {
                DisplayName = "Total number of consumers"
            };

            _currentConsumersCounter = new PollingCounter("current-consumers", this, () => Interlocked.Read(ref _currentConsumers))
            {
                DisplayName = "Current number of consumers"
            };

            _totalProducersCounter = new PollingCounter("total-producers", this, () => Interlocked.Read(ref _totalProducers))
            {
                DisplayName = "Total number of producers"
            };

            _currentProducersCounter = new PollingCounter("current-producers", this, () => Interlocked.Read(ref _currentProducers))
            {
                DisplayName = "Current number of producers"
            };

            _totalReadersCounter = new PollingCounter("total-readers", this, () => Interlocked.Read(ref _totalReaders))
            {
                DisplayName = "Total number of readers"
            };

            _currentReadersCounter = new PollingCounter("current-readers", this, () => Interlocked.Read(ref _currentReaders))
            {
                DisplayName = "Current number of readers"
            };
        }

        public void ClientCreated()
        {
            Interlocked.Increment(ref _totalClients);
            Interlocked.Increment(ref _currentClients);
        }

        public void ClientDisposed()
        {
            Interlocked.Decrement(ref _currentClients);
        }

        public void ConnectionCreated()
        {
            Interlocked.Increment(ref _totalConnections);
            Interlocked.Increment(ref _currentConnections);
        }

        public void ConnectionDisposed()
        {
            Interlocked.Decrement(ref _currentConnections);
        }

        public void ConsumerCreated()
        {
            Interlocked.Increment(ref _totalConsumers);
            Interlocked.Increment(ref _currentConsumers);
        }

        public void ConsumerDisposed()
        {
            Interlocked.Decrement(ref _currentConsumers);
        }

        public void ProducerCreated()
        {
            Interlocked.Increment(ref _totalProducers);
            Interlocked.Increment(ref _currentProducers);
        }

        public void ProducerDisposed()
        {
            Interlocked.Decrement(ref _currentProducers);
        }

        public void ReaderCreated()
        {
            Interlocked.Increment(ref _totalReaders);
            Interlocked.Increment(ref _currentReaders);
        }

        public void ReaderDisposed()
        {
            Interlocked.Decrement(ref _currentReaders);
        }
    }
#else
    public sealed class DotPulsarEventSource
    {
        public static readonly DotPulsarEventSource Log = new DotPulsarEventSource();

        public void ClientCreated() { }

        public void ClientDisposed() { }

        public void ConnectionCreated() { }

        public void ConnectionDisposed() { }

        public void ConsumerCreated() { }

        public void ConsumerDisposed() { }

        public void ProducerCreated() { }

        public void ProducerDisposed() { }

        public void ReaderCreated() { }

        public void ReaderDisposed() { }
    }
#endif
}
