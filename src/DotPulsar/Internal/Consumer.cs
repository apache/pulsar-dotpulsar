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

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Extensions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Compression;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

public sealed class Consumer<TMessage> : IConsumer<TMessage>, IRegisterEvent
{
    private readonly IExecute _executor;
    private readonly StateManager<ConsumerState> _state;
    private readonly ConsumerOptions<TMessage> _options;
    private readonly ProcessManager _processManager;
    private readonly IHandleException _exceptionHandler;
    private readonly IConnectionPool _connectionPool;
    private readonly CancellationTokenSource _cts;
    private readonly ConcurrentDictionary<string, IConsumer<TMessage>> _consumers;
    private readonly AsyncReaderWriterLock _lock;
    private ConcurrentQueue<IMessage<TMessage>> _messagesQueue;
    private int _isDisposed;
    private int _consumerCount;
    private uint _numberOfPartitions;
    private bool _isPartitioned;
    private Exception? _faultException;

    public Uri ServiceUrl { get; }
    public string SubscriptionName { get; }
    public string Topic { get; }

    public Consumer(
        Uri serviceUrl,
        ConsumerOptions<TMessage> options,
        ProcessManager processManager,
        IHandleException exceptionHandler,
        IConnectionPool connectionPool)
    {
        Topic = options.Topic;
        ServiceUrl = serviceUrl;
        SubscriptionName = options.SubscriptionName;

        _options = options;
        _processManager = processManager;
        _exceptionHandler = exceptionHandler;
        _connectionPool = connectionPool;

        _state = new StateManager<ConsumerState>(ConsumerState.Disconnected, ConsumerState.Closed, ConsumerState.ReachedEndOfTopic, ConsumerState.Faulted);
        _cts = new CancellationTokenSource();
        _executor = new Executor(Guid.Empty, this, _exceptionHandler);
        _consumers = new ConcurrentDictionary<string, IConsumer<TMessage>>();
        _messagesQueue = new ConcurrentQueue<IMessage<TMessage>>();
        _isDisposed = 0;

        _lock = new AsyncReaderWriterLock();

        _ = Setup();
    }

    public async ValueTask<ConsumerState> OnStateChangeTo(ConsumerState state, CancellationToken cancellationToken)
        => await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);

    public async ValueTask<ConsumerState> OnStateChangeFrom(ConsumerState state, CancellationToken cancellationToken)
        => await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);

    public bool IsFinalState()
        => _state.IsFinalState();

    public bool IsFinalState(ConsumerState state)
        => _state.IsFinalState(state);

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _cts.Cancel();
        _cts.Dispose();

        _state.SetState(ConsumerState.Closed);

        using (_lock.ReaderLock())
        {
            foreach (var consumer in _consumers.Values)
            {
                await consumer.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task Setup()
    {
        await Task.Yield();

        try
        {
            await _executor.Execute(Monitor, _cts.Token).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            if (_cts.IsCancellationRequested)
                return;

            _faultException = exception;
            _state.SetState(ConsumerState.Faulted);
        }
    }
    private async Task Monitor()
    {
        _numberOfPartitions = await GetNumberOfPartitions(Topic, _cts.Token).ConfigureAwait(false);
        _isPartitioned = _numberOfPartitions != 0;
        var monitoringTasks = new List<Task<ConsumerStateChanged>>();

        using (_lock.ReaderLock())
        {
            if (_isPartitioned)
            {

                for (var partition = 0; partition < _numberOfPartitions; ++partition)
                {
                    var partitionedTopicName = getPartitonedTopicName(partition);

                    var consumer = CreateSubConsumer(partitionedTopicName);
                    _ = _consumers.TryAdd(partitionedTopicName, consumer);
                    monitoringTasks.Add(consumer.StateChangedFrom(ConsumerState.Disconnected, _cts.Token).AsTask());
                }
            }

            else
            {
                var consumer = CreateSubConsumer(Topic);
                _ = _consumers.TryAdd(Topic, consumer);
                monitoringTasks.Add(consumer.StateChangedFrom(ConsumerState.Disconnected, _cts.Token).AsTask());
            }

            Interlocked.Exchange(ref _consumerCount, monitoringTasks.Count);
        }
        var activeConsumers = 0;
        while (true)
        {
            await Task.WhenAny(monitoringTasks).ConfigureAwait(false);

            for (var i = 0; i < monitoringTasks.Count; ++i)
            {
                var task = monitoringTasks[i];
                if (!task.IsCompleted)
                    continue;
                var state = task.Result.ConsumerState;
                switch (state)
                {
                    case ConsumerState.Active:
                        ++activeConsumers;
                        break;
                    case ConsumerState.Disconnected:
                        --activeConsumers;
                        break;
                    case ConsumerState.ReachedEndOfTopic:
                        _state.SetState(ConsumerState.ReachedEndOfTopic);
                        return;
                    case ConsumerState.Faulted:
                        _state.SetState(ConsumerState.Faulted);
                        return;
                    case ConsumerState.Unsubscribed:
                        _state.SetState(ConsumerState.Unsubscribed);
                        return;
                }

                monitoringTasks[i] = task.Result.Consumer.StateChangedFrom(state, _cts.Token).AsTask();
            }

            if (activeConsumers == 0)
                _state.SetState(ConsumerState.Disconnected);
            else if (activeConsumers == monitoringTasks.Count)
                _state.SetState(ConsumerState.Active);
            else
                _state.SetState(ConsumerState.PartiallyActive);
        }
    }

    private SubConsumer<TMessage> CreateSubConsumer(string topic)
    {
        var correlationId = Guid.NewGuid();
        var consumerName = _options.ConsumerName ?? $"Consumer-{correlationId:N}";

        var subscribe = new CommandSubscribe
        {
            ConsumerName = consumerName,
            InitialPosition = (CommandSubscribe.InitialPositionType) _options.InitialPosition,
            PriorityLevel = _options.PriorityLevel,
            ReadCompacted = _options.ReadCompacted,
            ReplicateSubscriptionState = _options.ReplicateSubscriptionState,
            Subscription = _options.SubscriptionName,
            Topic = topic,
            Type = (CommandSubscribe.SubType) _options.SubscriptionType
        };

        foreach (var property in _options.SubscriptionProperties)
        {
            var keyValue = new KeyValue { Key = property.Key, Value = property.Value };
            subscribe.SubscriptionProperties.Add(keyValue);
        }

        var messagePrefetchCount = _options.MessagePrefetchCount;
        var messageFactory = new MessageFactory<TMessage>(_options.Schema);
        var batchHandler = new BatchHandler<TMessage>(true, messageFactory);
        var decompressorFactories = CompressionFactories.DecompressorFactories();

        var factory = new ConsumerChannelFactory<TMessage>(correlationId, _processManager, _connectionPool, subscribe, messagePrefetchCount, batchHandler, messageFactory, decompressorFactories);
        var stateManager = new StateManager<ConsumerState>(ConsumerState.Disconnected, ConsumerState.Closed, ConsumerState.ReachedEndOfTopic, ConsumerState.Faulted);
        var initialChannel = new NotReadyChannel<TMessage>();
        var executor = new Executor(correlationId, _processManager, _exceptionHandler);
        var consumer = new SubConsumer<TMessage>(correlationId, ServiceUrl, _options.SubscriptionName, topic, _processManager, initialChannel, executor, stateManager, factory);
        var process = new ConsumerProcess(correlationId, stateManager, consumer, _options.SubscriptionType == SubscriptionType.Failover);
        _processManager.Add(process);
        process.Start();
        return consumer;
    }

    private async Task<uint> GetNumberOfPartitions(string topic, CancellationToken cancellationToken)
    {
        var connection = await _connectionPool.FindConnectionForTopic(topic, cancellationToken).ConfigureAwait(false);
        var commandPartitionedMetadata = new PulsarApi.CommandPartitionedTopicMetadata { Topic = topic };
        var response = await connection.Send(commandPartitionedMetadata, cancellationToken).ConfigureAwait(false);

        response.Expect(PulsarApi.BaseCommand.Type.PartitionedMetadataResponse);

        if (response.PartitionMetadataResponse.Response == PulsarApi.CommandPartitionedTopicMetadataResponse.LookupType.Failed)
            response.PartitionMetadataResponse.Throw();

        return response.PartitionMetadataResponse.Partitions;
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
            throw new ConsumerDisposedException(GetType().FullName!);
    }
    public async ValueTask Acknowledge(MessageId messageId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var sourceTopic = Topic;
        if (_isPartitioned)
        {
            sourceTopic = getPartitonedTopicName(messageId.Partition);
        }
        await _executor.Execute(() =>
                {
                    ThrowIfNotActive();

                    using (_lock.ReaderLock())
                    {
                        return _consumers[sourceTopic].Acknowledge(messageId, cancellationToken);
                    }
                }, cancellationToken)
                .ConfigureAwait(false);
    }

    public async ValueTask AcknowledgeCumulative(MessageId messageId, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        await _executor.Execute(() =>
            {
                ThrowIfNotActive();

                using (_lock.ReaderLock())
                {
                    var sourceTopic = Topic;
                    if (_isPartitioned)
                    {
                        sourceTopic = getPartitonedTopicName(messageId.Partition);
                    }
                    return _consumers[sourceTopic].AcknowledgeCumulative(messageId, cancellationToken);
                }
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var getLastMessageId = new CommandGetLastMessageId();
        return await _executor.Execute(() => GetLastMessageId(getLastMessageId, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<MessageId> GetLastMessageId(CommandGetLastMessageId command, CancellationToken cancellationToken)
    {
        ThrowIfNotActive();

        if (_isPartitioned)
        {
            throw new NotImplementedException("GetLastMessageId is not implemented for partitioned topics");
        }
        using (_lock.ReaderLock())
        {
            return await _consumers.First().Value.GetLastMessageId(cancellationToken).ConfigureAwait(false);
        }
    }

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        return await _executor.Execute(() => ReceiveMessage(cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<IMessage<TMessage>> ReceiveMessage(CancellationToken cancellationToken)
    {
        ThrowIfNotActive();

        using (_lock.ReaderLock())
        {
            if (_messagesQueue.TryDequeue(out var message))
            {
                return message;
            }
            var cts = new CancellationTokenSource();
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
            while (!cancellationToken.IsCancellationRequested)
            {
                var done = false;

                Task<IMessage<TMessage>>[] receiveTasks = _consumers.Values.Select(consumer => consumer.Receive(linkedCts.Token).AsTask()).ToArray();
                await Task.WhenAny(receiveTasks).ConfigureAwait(false);

                try
                {
                    receiveTasks.Where(t => t.IsCompleted).ToList().ForEach(t =>
                        {
                            if (t.Result == null)
                            {
                                return;
                            }

                            done = true;
                            _messagesQueue.Enqueue(t.Result);
                        }
                    );
                }
                catch (Exception exception)
                {
                    if (linkedCts.IsCancellationRequested)
                    {
                        cts.Cancel();
                    }
                    else
                    {
                        throw exception;
                    }
                }
                if (done)
                {
                    break;
                }
            }
            cts.Cancel();
            cts.Dispose();
            _messagesQueue.TryDequeue(out var result);
            return result;
        }
    }

    public async ValueTask RedeliverUnacknowledgedMessages(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _executor.Execute<ValueTask>(async () =>
        {
            ThrowIfNotActive();

            using (_lock.ReaderLock())
            {
                var tasks = messageIds.Select(n =>
                {
                    var sourceTopic = Topic;
                    if (_isPartitioned)
                    {
                        sourceTopic = getPartitonedTopicName(n.Partition);
                    }
                    return _consumers[getPartitonedTopicName(n.Partition)].RedeliverUnacknowledgedMessages(cancellationToken).AsTask();
                });
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RedeliverUnacknowledgedMessages(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _executor.Execute<ValueTask>(async () =>
        {
            ThrowIfNotActive();

            using (_lock.ReaderLock())
            {
                var tasks = _consumers.Values.Select(consumer =>
                    consumer.RedeliverUnacknowledgedMessages(cancellationToken).AsTask()
                );
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask Seek(MessageId messageId, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _executor.Execute<ValueTask>(async () =>
        {
            ThrowIfNotActive();

            using (_lock.ReaderLock())
            {
                if (messageId.Equals(null))
                {
                    throw new ArgumentException("Illegal messageId cannot be null");
                }

                var tasks = _consumers.Values.Select(consumer =>
                    consumer.Seek(messageId, cancellationToken).AsTask()
                );
                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
                _messagesQueue = new ConcurrentQueue<IMessage<TMessage>>();
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask Seek(ulong publishTime, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _executor.Execute<ValueTask>(async () =>
        {
            ThrowIfNotActive();

            using (_lock.ReaderLock())
            {
                var tasks = _consumers.Values.Select(consumer =>
                    consumer.Seek(publishTime, cancellationToken).AsTask()
                );
                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
                _messagesQueue = new ConcurrentQueue<IMessage<TMessage>>();
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask Unsubscribe(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        await _executor.Execute<ValueTask>(async () =>
        {
            ThrowIfNotActive();

            using (_lock.ReaderLock())
            {
                var tasks = _consumers.Values.Select(consumer =>
                    consumer.Unsubscribe(cancellationToken).AsTask()
                );
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }, cancellationToken).ConfigureAwait(false);
    }

    private string getPartitonedTopicName(int partitionNumber)
    {
        return $"{Topic}-partition-{partitionNumber}";
    }

    private void ThrowIfNotActive()
    {
        if (_state.CurrentState != ConsumerState.Active)
            throw new ConsumerNotActiveException("The consumer is not yet activated.");
    }

    public void Register(IEvent @event) { }
}
