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
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Compression;
using DotPulsar.Internal.PulsarApi;

public sealed class Consumer<TMessage> : IConsumer<TMessage>
{
    private readonly TaskCompletionSource<IMessage<TMessage>> _emptyReceiveTaskCompletionSource;
    private readonly TaskCompletionSource<IMessage<TMessage>> _emptyPeekTaskCompletionSource;
    private readonly IConnectionPool _connectionPool;
    private readonly ProcessManager _processManager;
    private readonly StateManager<ConsumerState> _state;
    private readonly ConsumerOptions<TMessage> _consumerOptions;
    private readonly CancellationTokenSource _cts;
    private readonly IHandleException _exceptionHandler;
    private readonly IExecute _executor;
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly AsyncLock _lock;
    private SubConsumer<TMessage>[] _subConsumers;
    private bool _allSubConsumersAreReady;
    private int _isDisposed;
    private bool _isPartitionedTopic;
    private int _numberOfPartitions;
    private Task<IMessage<TMessage>>[] _receiveTasks;
    private Task<IMessage<TMessage>>[] _peekTasks;
    private int _subConsumerIndex;
    private Exception? _faultException;

    public Uri ServiceUrl { get; }
    public string SubscriptionName { get; }
    public SubscriptionType SubscriptionType { get; }
    public string Topic { get; }
    public IState<ConsumerState> State => _state;

    public Consumer(
        Uri serviceUrl,
        ConsumerOptions<TMessage> consumerOptions,
        IConnectionPool connectionPool,
        IHandleException exceptionHandler)
    {
        _lock = new AsyncLock();
        _state = CreateStateManager();
        ServiceUrl = serviceUrl;
        SubscriptionName = consumerOptions.SubscriptionName;
        SubscriptionType = consumerOptions.SubscriptionType;
        Topic = consumerOptions.Topic;
        _receiveTasks = Array.Empty<Task<IMessage<TMessage>>>();
        _peekTasks = Array.Empty<Task<IMessage<TMessage>>>();
        _cts = new CancellationTokenSource();
        _exceptionHandler = exceptionHandler;
        _semaphoreSlim = new SemaphoreSlim(1);
        _processManager = new ProcessManager();
        _executor = new Executor(Guid.Empty, _processManager, _exceptionHandler);
        _consumerOptions = consumerOptions;
        _connectionPool = connectionPool;
        _exceptionHandler = exceptionHandler;
        _isPartitionedTopic = false;
        _allSubConsumersAreReady = false;
        _isDisposed = 0;
        _subConsumers = Array.Empty<SubConsumer<TMessage>>();

        _emptyReceiveTaskCompletionSource = new TaskCompletionSource<IMessage<TMessage>>();
        _emptyPeekTaskCompletionSource = new TaskCompletionSource<IMessage<TMessage>>();

        _ = Setup();
    }

    private async Task Setup()
    {
        try
        {
            await _semaphoreSlim.WaitAsync(_cts.Token).ConfigureAwait(false);
            await _executor.Execute(Monitor, _cts.Token).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            if (_cts.IsCancellationRequested)
                return;

            _faultException = exception;
            _state.SetState(ConsumerState.Faulted);
            _semaphoreSlim.Release();
        }
    }

    private async Task Monitor()
    {
        _numberOfPartitions = Convert.ToInt32(await _connectionPool.GetNumberOfPartitions(Topic, _cts.Token).ConfigureAwait(false));
        _isPartitionedTopic = _numberOfPartitions != 0;
        var numberOfSubConsumers = _isPartitionedTopic ? _numberOfPartitions : 1;
        _receiveTasks = new Task<IMessage<TMessage>>[numberOfSubConsumers];
        _peekTasks = new Task<IMessage<TMessage>>[numberOfSubConsumers];
        _subConsumers = new SubConsumer<TMessage>[numberOfSubConsumers];
        var monitoringTasks = new Task<ConsumerState>[numberOfSubConsumers];
        var states = new ConsumerState[numberOfSubConsumers];
        _subConsumerIndex = _isPartitionedTopic ? -1 : 0;

        for (var i = 0; i < numberOfSubConsumers; i++)
        {
            _receiveTasks[i] = _emptyReceiveTaskCompletionSource.Task;
            _peekTasks[i] = _emptyPeekTaskCompletionSource.Task;
            var topicName = _isPartitionedTopic ? GetPartitionedTopicName(i) : Topic;
            _subConsumers[i] = CreateSubConsumer(topicName);
            monitoringTasks[i] = _subConsumers[i].State.OnStateChangeFrom(ConsumerState.Disconnected, _cts.Token).AsTask();
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
                monitoringTasks[i] = _subConsumers[i].State.OnStateChangeFrom(state, _cts.Token).AsTask();
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

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _cts.Cancel();
        _cts.Dispose();

        await _processManager.DisposeAsync().ConfigureAwait(false);

        foreach (var subConsumer in _subConsumers)
        {
            if (subConsumer is not null)
                await subConsumer.DisposeAsync().ConfigureAwait(false);
        }

        await _lock.DisposeAsync().ConfigureAwait(false);
        _state.SetState(ConsumerState.Closed);
    }

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            return await _subConsumers[_subConsumerIndex].Receive(cancellationToken).ConfigureAwait(false);

        var iterations = 0;
        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            while (true)
            {
                iterations++;
                _subConsumerIndex++;
                if (_subConsumerIndex == _subConsumers.Length)
                    _subConsumerIndex = 0;

                var receiveTask = _receiveTasks[_subConsumerIndex];
                if (receiveTask == _emptyReceiveTaskCompletionSource.Task)
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
                        _receiveTasks[_subConsumerIndex] = _emptyReceiveTaskCompletionSource.Task;
                        return receiveTask.Result;
                    }
                }
                if (iterations == _subConsumers.Length)
                    await Task.WhenAny(_receiveTasks).ConfigureAwait(false);
            }
    }

    public async ValueTask<IMessage<TMessage>> Peek(CancellationToken cancellationToken = default)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            return await _subConsumers[_subConsumerIndex].Peek(cancellationToken).ConfigureAwait(false);

        var iterations = 0;
        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            while (true)
            {
                iterations++;
                _subConsumerIndex++;
                if (_subConsumerIndex == _subConsumers.Length)
                    _subConsumerIndex = 0;

                var peekTask = _peekTasks[_subConsumerIndex];
                if (peekTask == _emptyPeekTaskCompletionSource.Task)
                {
                    var peekTaskValueTask = _subConsumers[_subConsumerIndex].Peek(cancellationToken);
                    if (peekTaskValueTask.IsCompleted)
                        return peekTaskValueTask.Result;
                    _peekTasks[_subConsumerIndex] = peekTaskValueTask.AsTask();
                }
                else
                {
                    if (peekTask.IsCompleted)
                    {
                        _peekTasks[_subConsumerIndex] = _emptyPeekTaskCompletionSource.Task;
                        return peekTask.Result;
                    }
                }
                if (iterations == _subConsumers.Length)
                    await Task.WhenAny(_peekTasks).ConfigureAwait(false);
            }
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

    public async ValueTask Acknowledge(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken = default)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
        {
            await _subConsumers[_subConsumerIndex].Acknowledge(messageIds, cancellationToken).ConfigureAwait(false);
            return;
        }

        var groupedMessageIds = messageIds.GroupBy(messageIds => messageIds.Partition);
        var acknowledgeTasks = new List<Task>();
        foreach (var group in groupedMessageIds)
        {
            acknowledgeTasks.Add(_subConsumers[group.Key].Acknowledge(group, cancellationToken).AsTask());
        }

        await Task.WhenAll(acknowledgeTasks).ConfigureAwait(false);
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

        var groupedMessageIds = messageIds.GroupBy(messageIds => messageIds.Partition);
        var redeliverTasks = new List<Task>();
        foreach (var group in groupedMessageIds)
        {
            redeliverTasks.Add(_subConsumers[group.Key].RedeliverUnacknowledgedMessages(group, cancellationToken).AsTask());
        }
        await Task.WhenAll(redeliverTasks).ConfigureAwait(false);
    }

    public async ValueTask RedeliverUnacknowledgedMessages(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
        {
            await _subConsumers[_subConsumerIndex].RedeliverUnacknowledgedMessages(cancellationToken).ConfigureAwait(false);
            return;
        }

        var redeliverTasks = new List<Task>(_numberOfPartitions);
        foreach (var subConsumer in _subConsumers)
        {
            redeliverTasks.Add(subConsumer.RedeliverUnacknowledgedMessages(cancellationToken).AsTask());
        }
        await Task.WhenAll(redeliverTasks).ConfigureAwait(false);
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

    public async ValueTask<IEnumerable<MessageId>> GetLastMessageIds(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            return [await _subConsumers[_subConsumerIndex].GetLastMessageId(cancellationToken).ConfigureAwait(false)];

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
        var consumerChannelFactory = new ConsumerChannelFactory<TMessage>(correlationId, _processManager, _connectionPool, subscribe, messagePrefetchCount, batchHandler, messageFactory, decompressorFactories, topic);
        var stateManager = CreateStateManager();
        var initialChannel = new NotReadyChannel<TMessage>();
        var executor = new Executor(correlationId, _processManager, _exceptionHandler);
        var subConsumer = new SubConsumer<TMessage>(correlationId, ServiceUrl, _consumerOptions.SubscriptionName, _consumerOptions.SubscriptionType, topic, _processManager, initialChannel, executor, stateManager, consumerChannelFactory);
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

        if (_faultException is not null)
            throw new ConsumerFaultedException(_faultException);
    }
}
