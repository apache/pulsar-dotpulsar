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
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Compression;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed class Consumer<TMessage> : IConsumer<TMessage>
{
    private readonly TaskCompletionSource<IMessage<TMessage>> _emptyTaskCompletionSource;
    private readonly IConnectionPool _connectionPool;
    private readonly ProcessManager _processManager;
    private readonly StateManager<ConsumerState> _state;
    private readonly ConsumerOptions<TMessage> _consumerOptions;
    private readonly CancellationTokenSource _cts;
    private readonly IHandleException _exceptionHandler;
    private readonly IExecute _executor;
    private readonly SemaphoreSlim _semaphoreSlim;
    private SubConsumer<TMessage>[] _subConsumers;
    private bool _allSubConsumersAreReady;
    private int _isDisposed;
    private bool _isPartitionedTopic;
    private int _numberOfPartitions;
    private Task<IMessage<TMessage>>[] _receiveTasks;
    private int _subConsumerIndex;
    private Exception? _faultException;

    public Uri ServiceUrl { get; }
    public string SubscriptionName { get; }
    public string Topic { get; }

    public Consumer(
        Uri serviceUrl,
        ProcessManager processManager,
        ConsumerOptions<TMessage> consumerOptions,
        IConnectionPool connectionPool,
        IHandleException exceptionHandler)
    {
        _state = CreateStateManager();
        ServiceUrl = serviceUrl;
        SubscriptionName = consumerOptions.SubscriptionName;
        Topic = consumerOptions.Topic;
        _receiveTasks = Array.Empty<Task<IMessage<TMessage>>>();
        _cts = new CancellationTokenSource();
        _exceptionHandler = exceptionHandler;
        _semaphoreSlim = new SemaphoreSlim(1);
        _processManager = processManager;
        _executor = new Executor(Guid.Empty, _processManager, _exceptionHandler);
        _consumerOptions = consumerOptions;
        _connectionPool = connectionPool;
        _exceptionHandler = exceptionHandler;
        _isPartitionedTopic = false;
        _allSubConsumersAreReady = false;
        _isDisposed = 0;
        _subConsumers = null!;

        _emptyTaskCompletionSource = new TaskCompletionSource<IMessage<TMessage>>();

        _ = Setup();
    }

    private async Task Setup()
    {
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
        await _semaphoreSlim.WaitAsync(_cts.Token).ConfigureAwait(false);

        _numberOfPartitions = Convert.ToInt32(await _connectionPool.GetNumberOfPartitions(Topic, _cts.Token).ConfigureAwait(false));
        _isPartitionedTopic = _numberOfPartitions != 0;
        var numberOfSubConsumers = _isPartitionedTopic ? _numberOfPartitions : 1;
        _receiveTasks = new Task<IMessage<TMessage>>[numberOfSubConsumers];
        _subConsumers = new SubConsumer<TMessage>[numberOfSubConsumers];
        var monitoringTasks = new Task<ConsumerState>[numberOfSubConsumers];
        var states = new ConsumerState[numberOfSubConsumers];
        _subConsumerIndex = _isPartitionedTopic ? -1 : 0;

        for (var i = 0; i < numberOfSubConsumers; i++)
        {
            _receiveTasks[i] = _emptyTaskCompletionSource.Task;
            var topicName = _isPartitionedTopic ? GetPartitionedTopicName(i) : Topic;
            _subConsumers[i] = CreateSubConsumer(topicName);
            monitoringTasks[i] = _subConsumers[i].OnStateChangeFrom(ConsumerState.Disconnected, _cts.Token).AsTask();
        }

        _allSubConsumersAreReady = true;
        _semaphoreSlim.Release();

        while (true)
        {
            await Task.WhenAny(monitoringTasks).ConfigureAwait(false);

            for (var i = 0; i < numberOfSubConsumers; ++i)
            {
                var task = monitoringTasks[i];
                if (!task.IsCompleted)
                    continue;

                var state = task.Result;
                states[i] = state;
                monitoringTasks[i] = _subConsumers[i].OnStateChangeFrom(state, _cts.Token).AsTask();
            }

            if (!_isPartitionedTopic)
                _state.SetState(states[0]);
            else if (states.Any(x => x == ConsumerState.Faulted))
                _state.SetState(ConsumerState.Faulted);
            else if (states.All(x => x == ConsumerState.Active))
                _state.SetState(ConsumerState.Active);
            else if (states.All(x => x == ConsumerState.Inactive))
                _state.SetState(ConsumerState.Inactive);
            else if (states.All(x => x == ConsumerState.ReachedEndOfTopic))
                _state.SetState(ConsumerState.ReachedEndOfTopic);
            else if (states.All(x => x == ConsumerState.Disconnected))
                _state.SetState(ConsumerState.Disconnected);
            else if (states.Any(x => x == ConsumerState.Disconnected))
                _state.SetState(ConsumerState.PartiallyConnected);
            else
                _state.SetState(ConsumerState.Inactive);
        }
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

        if (_subConsumers is null)
            return;

        foreach (var subConsumer in _subConsumers)
        {
            await subConsumer.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            return await _subConsumers[_subConsumerIndex].Receive(cancellationToken).ConfigureAwait(false);

        var iterations = 0;
        while (true)
        {
            iterations++;
            _subConsumerIndex++;
            if (_subConsumerIndex == _subConsumers.Length)
                _subConsumerIndex = 0;

            var receiveTask = _receiveTasks[_subConsumerIndex];
            if (receiveTask == _emptyTaskCompletionSource.Task)
            {
                var receiveTaskValueTask = _subConsumers[_subConsumerIndex].Receive(cancellationToken);
                if (receiveTaskValueTask.IsCompleted)
                    return receiveTaskValueTask.Result;
                _receiveTasks[_subConsumerIndex] = receiveTaskValueTask.AsTask();
            }
            else
            {
                if (receiveTask.IsCompleted)
                {
                    _receiveTasks[_subConsumerIndex] = _emptyTaskCompletionSource.Task;
                    return receiveTask.Result;
                }
            }
            if (iterations == _subConsumers.Length)
                await Task.WhenAny(_receiveTasks).ConfigureAwait(false);
        }
    }

    public async ValueTask Acknowledge(MessageId messageId, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            await _subConsumers[_subConsumerIndex].Acknowledge(messageId, cancellationToken).ConfigureAwait(false);
        else
            await _subConsumers[messageId.Partition].Acknowledge(messageId, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask AcknowledgeCumulative(MessageId messageId, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            await _subConsumers[_subConsumerIndex].AcknowledgeCumulative(messageId, cancellationToken).ConfigureAwait(false);
        else
            await _subConsumers[messageId.Partition].AcknowledgeCumulative(messageId, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RedeliverUnacknowledgedMessages(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
        {
            await _subConsumers[_subConsumerIndex].RedeliverUnacknowledgedMessages(messageIds, cancellationToken).ConfigureAwait(false);
            return;
        }

        var messageIdSortedIntoTopics = new Dictionary<int, LinkedList<MessageId>>(_numberOfPartitions);
        //sort messageIds into topics
        foreach (var messageId in messageIds)
        {
            if (messageIdSortedIntoTopics.ContainsKey(messageId.Partition))
            {
                messageIdSortedIntoTopics[messageId.Partition].AddLast(messageId);
            }
            else
            {
                var linkedList = new LinkedList<MessageId>();
                linkedList.AddLast(messageId);
                messageIdSortedIntoTopics.Add(messageId.Partition, linkedList);
            }
        }
        var redeliverUnacknowledgedMessagesTasks = new Task[messageIdSortedIntoTopics.Count];
        var iterations = -1;
        //Collect tasks from _subConsumers RedeliverUnacknowledgedMessages without waiting
        foreach (var messageIdSortedByPartition in messageIdSortedIntoTopics)
        {
            iterations++;
            var task = _subConsumers[messageIdSortedByPartition.Key].RedeliverUnacknowledgedMessages(messageIdSortedByPartition.Value, cancellationToken).AsTask();
            redeliverUnacknowledgedMessagesTasks[iterations] = task;
        }
        //await all of the tasks.
        await Task.WhenAll(redeliverUnacknowledgedMessagesTasks).ConfigureAwait(false);
    }

    public async ValueTask RedeliverUnacknowledgedMessages(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
        {
            await _subConsumers[_subConsumerIndex].RedeliverUnacknowledgedMessages(cancellationToken).ConfigureAwait(false);
            return;
        }

        foreach (var subConsumer in _subConsumers)
        {
            await subConsumer.RedeliverUnacknowledgedMessages(cancellationToken).ConfigureAwait(false);
        }
    }

    public async ValueTask Unsubscribe(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
        {
            await _subConsumers[_subConsumerIndex].Unsubscribe(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var unsubscribeTasks = new List<Task>(_numberOfPartitions);
            foreach (var subConsumer in _subConsumers)
            {
                var getLastMessageIdTask = subConsumer.Unsubscribe(cancellationToken);
                unsubscribeTasks.Add(getLastMessageIdTask.AsTask());
            }

            await Task.WhenAll(unsubscribeTasks).ConfigureAwait(false);
        }

        _state.SetState(ConsumerState.Unsubscribed);
    }

    public async ValueTask Seek(MessageId messageId, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
        {
            await _subConsumers[_subConsumerIndex].Seek(messageId, cancellationToken).ConfigureAwait(false);
            return;
        }

        var seekTasks = new List<Task>(_numberOfPartitions);
        foreach (var subConsumer in _subConsumers)
        {
            var getLastMessageIdTask = subConsumer.Seek(messageId, cancellationToken);
            seekTasks.Add(getLastMessageIdTask.AsTask());
        }
        await Task.WhenAll(seekTasks).ConfigureAwait(false);
    }

    public async ValueTask Seek(ulong publishTime, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
        {
            await _subConsumers[_subConsumerIndex].Seek(publishTime, cancellationToken).ConfigureAwait(false);
            return;
        }

        var seekTasks = new List<Task>(_numberOfPartitions);
        foreach (var subConsumer in _subConsumers)
        {
            var getLastMessageIdTask = subConsumer.Seek(publishTime, cancellationToken);
            seekTasks.Add(getLastMessageIdTask.AsTask());
        }
        await Task.WhenAll(seekTasks).ConfigureAwait(false);
    }

    [Obsolete("GetLastMessageId is obsolete. Please use GetLastMessageIds instead.")]
    public async ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            return await _subConsumers[_subConsumerIndex].GetLastMessageId(cancellationToken).ConfigureAwait(false);

        throw new NotSupportedException("GetLastMessageId can't be used on partitioned topics. Please use GetLastMessageIds");
    }

    public async ValueTask<IEnumerable<MessageId>> GetLastMessageIds(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            return new[] { await _subConsumers[_subConsumerIndex].GetLastMessageId(cancellationToken).ConfigureAwait(false) };

        var getLastMessageIdsTasks = new List<Task<MessageId>>(_numberOfPartitions);

        foreach (var subConsumer in _subConsumers)
        {
            var getLastMessageIdTask = subConsumer.GetLastMessageId(cancellationToken);
            getLastMessageIdsTasks.Add(getLastMessageIdTask.AsTask());
        }

        //await all of the tasks.
        await Task.WhenAll(getLastMessageIdsTasks).ConfigureAwait(false);

        //collect MessageIds
        var messageIds = new List<MessageId>();
        for (var i = 0; i < _subConsumers.Length; i++)
        {
            messageIds.Add(getLastMessageIdsTasks[i].Result);
        }
        return messageIds;
    }

    private SubConsumer<TMessage> CreateSubConsumer(string topic)
    {
        var correlationId = Guid.NewGuid();
        var consumerName = _consumerOptions.ConsumerName ?? $"Consumer-{correlationId:N}";

        var subscribe = new CommandSubscribe
        {
            ConsumerName = consumerName,
            InitialPosition = (CommandSubscribe.InitialPositionType) _consumerOptions.InitialPosition,
            PriorityLevel = _consumerOptions.PriorityLevel,
            ReadCompacted = _consumerOptions.ReadCompacted,
            ReplicateSubscriptionState = _consumerOptions.ReplicateSubscriptionState,
            Subscription = _consumerOptions.SubscriptionName,
            Topic = topic,
            Type = (CommandSubscribe.SubType) _consumerOptions.SubscriptionType
        };

        foreach (var property in _consumerOptions.SubscriptionProperties)
        {
            var keyValue = new KeyValue { Key = property.Key, Value = property.Value };
            subscribe.SubscriptionProperties.Add(keyValue);
        }

        var messagePrefetchCount = _consumerOptions.MessagePrefetchCount;
        var messageFactory = new MessageFactory<TMessage>(_consumerOptions.Schema);
        var batchHandler = new BatchHandler<TMessage>(true, messageFactory);
        var decompressorFactories = CompressionFactories.DecompressorFactories();
        var consumerChannelFactory = new ConsumerChannelFactory<TMessage>(correlationId, _processManager, _connectionPool, subscribe,
            messagePrefetchCount, batchHandler, messageFactory, decompressorFactories, topic);
        var stateManager = CreateStateManager();
        var initialChannel = new NotReadyChannel<TMessage>();
        var executor = new Executor(correlationId, _processManager, _exceptionHandler);
        var subConsumer = new SubConsumer<TMessage>(correlationId, ServiceUrl, _consumerOptions.SubscriptionName, topic,
            _processManager, initialChannel, executor, stateManager, consumerChannelFactory);
        var process = new ConsumerProcess(correlationId, stateManager, subConsumer, _consumerOptions.SubscriptionType == SubscriptionType.Failover);
        _processManager.Add(process);
        process.Start();
        return subConsumer;
    }

    private string GetPartitionedTopicName(int partitionNumber) => $"{Topic}-partition-{partitionNumber}";

    private static StateManager<ConsumerState> CreateStateManager()
        => new(ConsumerState.Disconnected, ConsumerState.Closed, ConsumerState.ReachedEndOfTopic, ConsumerState.Faulted);

    private async Task Guard(CancellationToken cancellationToken)
    {
        if (!_allSubConsumersAreReady)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            _semaphoreSlim.Release();
        }
    }
}
