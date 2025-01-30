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
using DotPulsar.Internal.PulsarApi;

public sealed class Consumer<TMessage> : IConsumer<TMessage>
{
    private readonly IConnectionPool _connectionPool;
    private readonly ProcessManager _processManager;
    private readonly StateManager<ConsumerState> _state;
    private readonly ConsumerOptions<TMessage> _consumerOptions;
    private readonly CancellationTokenSource _cts;
    private readonly IHandleException _exceptionHandler;
    private readonly IExecute _executor;
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly AsyncLock _lock;
    private readonly Dictionary<string, SubConsumer<TMessage>> _subConsumers;
    private readonly LinkedList<Task<IMessage<TMessage>>> _receiveTasks;
    private Dictionary<string, SubConsumer<TMessage>>.Enumerator _receiveEnumerator;
    private SubConsumer<TMessage>? _singleSubConsumer;
    private bool _allSubConsumersAreReady;
    private int _isDisposed;
    private int _numberOfSubConsumers;
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
        if (!string.IsNullOrEmpty(consumerOptions.Topic))
            Topic = consumerOptions.Topic;
        else
            Topic = string.Join(",", consumerOptions.Topics);
        _receiveTasks = [];
        _cts = new CancellationTokenSource();
        _exceptionHandler = exceptionHandler;
        _semaphoreSlim = new SemaphoreSlim(1);
        _processManager = new ProcessManager();
        _executor = new Executor(Guid.Empty, _processManager, _exceptionHandler);
        _consumerOptions = consumerOptions;
        _connectionPool = connectionPool;
        _exceptionHandler = exceptionHandler;
        _allSubConsumersAreReady = false;
        _isDisposed = 0;
        _subConsumers = [];
        _receiveEnumerator = _subConsumers.GetEnumerator();
        _singleSubConsumer = null;

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
        var userDefinedTopics = new List<string>(_consumerOptions.Topics);
        if (!string.IsNullOrEmpty(_consumerOptions.Topic))
            userDefinedTopics.Add(_consumerOptions.Topic);

        var topics = new List<string>();
        foreach (var topic in userDefinedTopics)
        {
            var numberOfPartitions = await _connectionPool.GetNumberOfPartitions(topic, _cts.Token).ConfigureAwait(false);
            if (numberOfPartitions == 0)
            {
                topics.Add(topic);
                continue;
            }
            
            for (var i = 0; i < numberOfPartitions; ++i)
            {
                topics.Add(GetPartitionedTopicName(topic, i));
            }
        }

        _numberOfSubConsumers = topics.Count;
        var monitoringTasks = new Task<ConsumerStateChanged>[_numberOfSubConsumers];
        var states = new ConsumerState[_numberOfSubConsumers];

        for (var i = 0; i < _numberOfSubConsumers; ++i)
        {
            var topic = topics[i];
            var subConsumer = CreateSubConsumer(topic);
            _subConsumers[topic] = subConsumer;
            monitoringTasks[i] = subConsumer.StateChangedFrom(ConsumerState.Disconnected, _cts.Token).AsTask();
        }

        if (_numberOfSubConsumers == 1)
            _singleSubConsumer = _subConsumers.First().Value;

        _receiveEnumerator = _subConsumers.GetEnumerator();
        _receiveEnumerator.MoveNext();_allSubConsumersAreReady = true;
        _semaphoreSlim.Release();

        while (true)
        {
            await Task.WhenAny(monitoringTasks).ConfigureAwait(false);

            for (var i = 0; i < _numberOfSubConsumers; ++i)
            {
                var task = monitoringTasks[i];
                if (!task.IsCompleted)
                    continue;

                var consumerStateChanged = task.Result;
                var state = consumerStateChanged.ConsumerState;
                states[i] = state;
                monitoringTasks[i] = consumerStateChanged.Consumer.StateChangedFrom(state, _cts.Token).AsTask();
            }

            if (_singleSubConsumer is not null)
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
            await subConsumer.Value.DisposeAsync().ConfigureAwait(false);
        }

        await _lock.DisposeAsync().ConfigureAwait(false);
        _state.SetState(ConsumerState.Closed);
    }

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (_singleSubConsumer is not null)
            return await _singleSubConsumer.Receive(cancellationToken).ConfigureAwait(false);

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            var startTopic = string.Empty;

            while (true)
            {
                var receiveTaskNode = _receiveTasks.First;
                while (receiveTaskNode is not null)
                {
                    if (receiveTaskNode.Value.IsCompleted)
                    {
                        _receiveTasks.Remove(receiveTaskNode);
                        return receiveTaskNode.Value.Result;
                    }
                    receiveTaskNode = receiveTaskNode.Next;
                }

                if (_receiveEnumerator.Current.Key is not null)
                    startTopic = _receiveEnumerator.Current.Key;

                if (!_receiveEnumerator.MoveNext())
                {
                    _receiveEnumerator = _subConsumers.GetEnumerator();
                    _receiveEnumerator.MoveNext();
                }

                var subConsumer = _receiveEnumerator.Current;

                var receiveTask = subConsumer.Value.Receive(_cts.Token);
                if (receiveTask.IsCompleted)
                    return receiveTask.Result;

                _receiveTasks.AddLast(receiveTask.AsTask());

                if (startTopic == subConsumer.Key)
                {
                    var tcs = new TaskCompletionSource<IMessage<TMessage>>();
                    using var registration = cancellationToken.Register(() => tcs.TrySetCanceled());
                    _receiveTasks.AddLast(tcs.Task);
                    await Task.WhenAny(_receiveTasks).ConfigureAwait(false);
                    _receiveTasks.RemoveLast();
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
    }

    public async ValueTask Acknowledge(MessageId messageId, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (_singleSubConsumer is not null)
            await _singleSubConsumer.Acknowledge(messageId, cancellationToken).ConfigureAwait(false);
        else
            await _subConsumers[messageId.Topic].Acknowledge(messageId, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask Acknowledge(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken = default)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (_singleSubConsumer is not null)
        {
            await _singleSubConsumer.Acknowledge(messageIds, cancellationToken).ConfigureAwait(false);
            return;
        }

        var groupedMessageIds = messageIds.GroupBy(messageIds => messageIds.Topic);
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

        if (_singleSubConsumer is not null)
            await _singleSubConsumer.AcknowledgeCumulative(messageId, cancellationToken).ConfigureAwait(false);
        else
            await _subConsumers[messageId.Topic].AcknowledgeCumulative(messageId, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RedeliverUnacknowledgedMessages(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (_singleSubConsumer is not null)
        {
            await _singleSubConsumer.RedeliverUnacknowledgedMessages(messageIds, cancellationToken).ConfigureAwait(false);
            return;
        }

        var groupedMessageIds = messageIds.GroupBy(messageIds => messageIds.Topic);
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

        if (_singleSubConsumer is not null)
        {
            await _singleSubConsumer.RedeliverUnacknowledgedMessages(cancellationToken).ConfigureAwait(false);
            return;
        }

        var redeliverTasks = new List<Task>(_numberOfSubConsumers);
        foreach (var subConsumer in _subConsumers)
        {
            redeliverTasks.Add(subConsumer.Value.RedeliverUnacknowledgedMessages(cancellationToken).AsTask());
        }
        await Task.WhenAll(redeliverTasks).ConfigureAwait(false);
    }

    public async ValueTask Unsubscribe(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (_singleSubConsumer is not null)
        {
            await _singleSubConsumer.Unsubscribe(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var unsubscribeTasks = new List<Task>(_numberOfSubConsumers);
            foreach (var subConsumer in _subConsumers)
            {
                var getLastMessageIdTask = subConsumer.Value.Unsubscribe(cancellationToken);
                unsubscribeTasks.Add(getLastMessageIdTask.AsTask());
            }

            await Task.WhenAll(unsubscribeTasks).ConfigureAwait(false);
        }

        _state.SetState(ConsumerState.Unsubscribed);
    }

    public async ValueTask Seek(MessageId messageId, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (_singleSubConsumer is not null)
        {
            await _singleSubConsumer.Seek(messageId, cancellationToken).ConfigureAwait(false);
            return;
        }

        var seekTasks = new List<Task>(_numberOfSubConsumers);
        foreach (var subConsumer in _subConsumers)
        {
            var getLastMessageIdTask = subConsumer.Value.Seek(messageId, cancellationToken);
            seekTasks.Add(getLastMessageIdTask.AsTask());
        }
        await Task.WhenAll(seekTasks).ConfigureAwait(false);
    }

    public async ValueTask Seek(ulong publishTime, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (_singleSubConsumer is not null)
        {
            await _singleSubConsumer.Seek(publishTime, cancellationToken).ConfigureAwait(false);
            return;
        }

        var seekTasks = new List<Task>(_numberOfSubConsumers);
        foreach (var subConsumer in _subConsumers)
        {
            var getLastMessageIdTask = subConsumer.Value.Seek(publishTime, cancellationToken);
            seekTasks.Add(getLastMessageIdTask.AsTask());
        }
        await Task.WhenAll(seekTasks).ConfigureAwait(false);
    }

    public async ValueTask<IEnumerable<MessageId>> GetLastMessageIds(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (_singleSubConsumer is not null)
            return [await _singleSubConsumer.GetLastMessageId(cancellationToken).ConfigureAwait(false)];

        var getLastMessageIdsTasks = new List<Task<MessageId>>(_numberOfSubConsumers);

        foreach (var subConsumer in _subConsumers)
        {
            var getLastMessageIdTask = subConsumer.Value.GetLastMessageId(cancellationToken);
            getLastMessageIdsTasks.Add(getLastMessageIdTask.AsTask());
        }

        //await all of the tasks.
        await Task.WhenAll(getLastMessageIdsTasks).ConfigureAwait(false);

        //collect MessageIds
        var messageIds = new List<MessageId>();
        for (var i = 0; i < _subConsumers.Count; i++)
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

    private string GetPartitionedTopicName(string topic, int partitionNumber) => $"{topic}-partition-{partitionNumber}";

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
