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

namespace DotPulsar.Internal;

#if NETSTANDARD2_0
public sealed class DotPulsarEventSource
{
    public static readonly DotPulsarEventSource Log = new();

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

#else
using System.Diagnostics.Tracing;
using System.Threading;

public sealed class DotPulsarEventSource : EventSource
{
#pragma warning disable IDE0052 // Remove unread private members
    private PollingCounter? _totalClientsCounter;
    private long _totalClients;

    private PollingCounter? _currentClientsCounter;
    private long _currentClients;

    private PollingCounter? _totalConnectionsCounter;
    private long _totalConnections;

    private PollingCounter? _currentConnectionsCounter;
    private long _currentConnections;

    private PollingCounter? _totalConsumersCounter;
    private long _totalConsumers;

    private PollingCounter? _currentConsumersCounter;
    private long _currentConsumers;

    private PollingCounter? _totalProducersCounter;
    private long _totalProducers;

    private PollingCounter? _currentProducersCounter;
    private long _currentProducers;

    private PollingCounter? _totalReadersCounter;
    private long _totalReaders;

    private PollingCounter? _currentReadersCounter;
    private long _currentReaders;
#pragma warning restore IDE0052 // Remove unread private members

    public static readonly DotPulsarEventSource Log = new();

    public DotPulsarEventSource() : base("DotPulsar") { }

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

    protected override void OnEventCommand(EventCommandEventArgs command)
    {
        if (command.Command != EventCommand.Enable)
            return;

        _totalClientsCounter ??= new PollingCounter("total-clients", this, () => Volatile.Read(ref _totalClients))
        {
            DisplayName = "Total number of clients"
        };

        _currentClientsCounter ??= new PollingCounter("current-clients", this, () => Volatile.Read(ref _currentClients))
        {
            DisplayName = "Current number of clients"
        };

        _totalConnectionsCounter ??= new PollingCounter("total-connections", this, () => Volatile.Read(ref _totalConnections))
        {
            DisplayName = "Total number of connections"
        };

        _currentConnectionsCounter ??= new PollingCounter("current-connections", this, () => Volatile.Read(ref _currentConnections))
        {
            DisplayName = "Current number of connections"
        };

        _totalConsumersCounter ??= new PollingCounter("total-consumers", this, () => Volatile.Read(ref _totalConsumers))
        {
            DisplayName = "Total number of consumers"
        };

        _currentConsumersCounter ??= new PollingCounter("current-consumers", this, () => Volatile.Read(ref _currentConsumers))
        {
            DisplayName = "Current number of consumers"
        };

        _totalProducersCounter ??= new PollingCounter("total-producers", this, () => Volatile.Read(ref _totalProducers))
        {
            DisplayName = "Total number of producers"
        };

        _currentProducersCounter ??= new PollingCounter("current-producers", this, () => Volatile.Read(ref _currentProducers))
        {
            DisplayName = "Current number of producers"
        };

        _totalReadersCounter ??= new PollingCounter("total-readers", this, () => Volatile.Read(ref _totalReaders))
        {
            DisplayName = "Total number of readers"
        };

        _currentReadersCounter ??= new PollingCounter("current-readers", this, () => Volatile.Read(ref _currentReaders))
        {
            DisplayName = "Current number of readers"
        };
    }
}
#endif
