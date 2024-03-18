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

public sealed class Reader<TMessage> : IReader<TMessage>
{
    private readonly TaskCompletionSource<IMessage<TMessage>> _emptyTaskCompletionSource;
    private readonly ReaderOptions<TMessage> _readerOptions;
    private readonly IHandleException _exceptionHandler;
    private readonly IConnectionPool _connectionPool;
    private readonly ProcessManager _processManager;
    private readonly CancellationTokenSource _cts;
    private readonly IExecute _executor;
    private readonly StateManager<ReaderState> _state;
    private readonly SemaphoreSlim _semaphoreSlim;
    private SubReader<TMessage>[] _subReaders;
    private bool _allSubReadersAreReady;
    private Task<IMessage<TMessage>>[] _receiveTasks;
    private int _subReaderIndex;
    private bool _isPartitionedTopic;
    private int _numberOfPartitions;
    private int _isDisposed;
    private Exception? _faultException;

    public Uri ServiceUrl { get; }
    public string Topic { get; }

    public Reader(
        Uri serviceUrl,
        ReaderOptions<TMessage> readerOptions,
        IHandleException exceptionHandler,
        IConnectionPool connectionPool)
    {
        ServiceUrl = serviceUrl;
        Topic = readerOptions.Topic;
        _readerOptions = readerOptions;
        _connectionPool = connectionPool;
        _processManager = new ProcessManager();
        _exceptionHandler = exceptionHandler;
        _semaphoreSlim = new SemaphoreSlim(1);
        _state = CreateStateManager();
        _receiveTasks = Array.Empty<Task<IMessage<TMessage>>>();
        _cts = new CancellationTokenSource();
        _executor = new Executor(Guid.Empty, _processManager, _exceptionHandler);
        _isDisposed = 0;
        _subReaders = Array.Empty<SubReader<TMessage>>();

        _emptyTaskCompletionSource = new TaskCompletionSource<IMessage<TMessage>>();

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
            _state.SetState(ReaderState.Faulted);
            _semaphoreSlim.Release();
        }
    }

    private async Task Monitor()
    {
        _numberOfPartitions = Convert.ToInt32(await _connectionPool.GetNumberOfPartitions(Topic, _cts.Token).ConfigureAwait(false));
        _isPartitionedTopic = _numberOfPartitions != 0;
        var numberOfSubReaders = _isPartitionedTopic ? _numberOfPartitions : 1;
        _receiveTasks = new Task<IMessage<TMessage>>[numberOfSubReaders];
        _subReaders = new SubReader<TMessage>[numberOfSubReaders];
        var monitoringTasks = new Task<ReaderState>[numberOfSubReaders];
        var states = new ReaderState[numberOfSubReaders];
        _subReaderIndex = _isPartitionedTopic ? -1 : 0;

        for (var i = 0; i < numberOfSubReaders; i++)
        {
            _receiveTasks[i] = _emptyTaskCompletionSource.Task;
            var topicName = _isPartitionedTopic ? GetPartitionedTopicName(i) : Topic;
            _subReaders[i] = CreateSubReader(topicName);
            monitoringTasks[i] = _subReaders[i].OnStateChangeFrom(ReaderState.Disconnected, _cts.Token).AsTask();
        }

        _allSubReadersAreReady = true;
        _semaphoreSlim.Release();

        while (true)
        {
            await Task.WhenAny(monitoringTasks).ConfigureAwait(false);

            for (var i = 0; i < numberOfSubReaders; ++i)
            {
                var task = monitoringTasks[i];
                if (!task.IsCompleted)
                    continue;

                var state = task.Result;
                states[i] = state;
                monitoringTasks[i] = _subReaders[i].OnStateChangeFrom(state, _cts.Token).AsTask();
            }

            if (!_isPartitionedTopic)
                _state.SetState(states[0]);
            else if (states.Any(x => x == ReaderState.Faulted))
                _state.SetState(ReaderState.Faulted);
            else if (states.All(x => x == ReaderState.Connected))
                _state.SetState(ReaderState.Connected);
            else if (states.All(x => x == ReaderState.ReachedEndOfTopic))
                _state.SetState(ReaderState.ReachedEndOfTopic);
            else if (states.All(x => x == ReaderState.Disconnected))
                _state.SetState(ReaderState.Disconnected);
            else
                _state.SetState(ReaderState.PartiallyConnected);
        }
    }

    public async ValueTask<ReaderState> OnStateChangeTo(ReaderState state, CancellationToken cancellationToken)
        => await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);

    public async ValueTask<ReaderState> OnStateChangeFrom(ReaderState state, CancellationToken cancellationToken)
        => await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);

    public bool IsFinalState()
        => _state.IsFinalState();

    public bool IsFinalState(ReaderState state)
        => _state.IsFinalState(state);

    public async ValueTask<IEnumerable<MessageId>> GetLastMessageIds(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            return new[] { await _subReaders[_subReaderIndex].GetLastMessageId(cancellationToken).ConfigureAwait(false) };

        var getLastMessageIdsTasks = new List<Task<MessageId>>(_numberOfPartitions);

        foreach (var subReader in _subReaders)
        {
            var getLastMessageIdTask = subReader.GetLastMessageId(cancellationToken);
            getLastMessageIdsTasks.Add(getLastMessageIdTask.AsTask());
        }

        //await all of the tasks.
        await Task.WhenAll(getLastMessageIdsTasks).ConfigureAwait(false);

        //collect MessageIds
        var messageIds = new List<MessageId>();
        for (var i = 0; i < _subReaders.Length; i++)
        {
            messageIds.Add(getLastMessageIdsTasks[i].Result);
        }
        return messageIds;
    }

    public bool HasReachedEndOfTopic()
    {
        if (!_allSubReadersAreReady)
            return false;

        if (!_isPartitionedTopic)
            return _subReaders[_subReaderIndex].HasReachedEndOfTopic();

        var numberOfSubReaders = _isPartitionedTopic ? _numberOfPartitions : 1;
        for (int i = 0; i < numberOfSubReaders; i++)
        {
            if (!_subReaders[i].HasReachedEndOfTopic())
                return false;
        }
        return true;
    }

    public async ValueTask<bool> HasMessageAvailable(CancellationToken cancellationToken = default)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            return await _subReaders[_subReaderIndex].HasMessageAvailable(cancellationToken).ConfigureAwait(false);

        var list = _subReaders.Select(x => x.HasMessageAvailable(cancellationToken).AsTask()).ToList();
        while (list.Count > 0)
        {
            var task = await Task.WhenAny(list).ConfigureAwait(false);
            list.Remove(task);

            if (await task.ConfigureAwait(false))
            {
                return true;
            }
        }

        return false;
    }

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
            return await _subReaders[_subReaderIndex].Receive(cancellationToken).ConfigureAwait(false);

        var iterations = 0;
        while (true)
        {
            iterations++;
            _subReaderIndex++;
            if (_subReaderIndex == _subReaders.Length)
                _subReaderIndex = 0;

            var receiveTask = _receiveTasks[_subReaderIndex];
            if (receiveTask == _emptyTaskCompletionSource.Task)
            {
                var receiveTaskValueTask = _subReaders[_subReaderIndex].Receive(cancellationToken);
                if (receiveTaskValueTask.IsCompleted)
                    return receiveTaskValueTask.Result;
                _receiveTasks[_subReaderIndex] = receiveTaskValueTask.AsTask();
            }
            else
            {
                if (receiveTask.IsCompleted)
                {
                    _receiveTasks[_subReaderIndex] = _emptyTaskCompletionSource.Task;
                    return receiveTask.Result;
                }
            }
            if (iterations == _subReaders.Length)
                await Task.WhenAny(_receiveTasks).ConfigureAwait(false);
        }
    }

    public async ValueTask Seek(MessageId messageId, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
        {
            await _subReaders[_subReaderIndex].Seek(messageId, cancellationToken).ConfigureAwait(false);
            return;
        }

        var seekTasks = new List<Task>(_numberOfPartitions);
        foreach (var subReader in _subReaders)
        {
            var getLastMessageIdTask = subReader.Seek(messageId, cancellationToken);
            seekTasks.Add(getLastMessageIdTask.AsTask());
        }
        await Task.WhenAll(seekTasks).ConfigureAwait(false);
    }

    public async ValueTask Seek(ulong publishTime, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitionedTopic)
        {
            await _subReaders[_subReaderIndex].Seek(publishTime, cancellationToken).ConfigureAwait(false);
            return;
        }

        var seekTasks = new List<Task>(_numberOfPartitions);
        foreach (var subReader in _subReaders)
        {
            var getLastMessageIdTask = subReader.Seek(publishTime, cancellationToken);
            seekTasks.Add(getLastMessageIdTask.AsTask());
        }
        await Task.WhenAll(seekTasks).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _cts.Cancel();
        _cts.Dispose();

        await _processManager.DisposeAsync().ConfigureAwait(false);

        foreach (var subReader in _subReaders)
            await subReader.DisposeAsync().ConfigureAwait(false);

        _state.SetState(ReaderState.Closed);
    }

    private static StateManager<ReaderState> CreateStateManager()
        => new(ReaderState.Disconnected, ReaderState.Closed, ReaderState.ReachedEndOfTopic, ReaderState.Faulted);

    private SubReader<TMessage> CreateSubReader(string topic)
    {
        var correlationId = Guid.NewGuid();
        var subscription = $"Reader-{correlationId:N}";
        var subscribe = new CommandSubscribe
        {
            ConsumerName = _readerOptions.ReaderName ?? subscription,
            Durable = false,
            ReadCompacted = _readerOptions.ReadCompacted,
            StartMessageId = _readerOptions.StartMessageId.ToMessageIdData(),
            Subscription = subscription,
            Topic = topic
        };
        var messagePrefetchCount = _readerOptions.MessagePrefetchCount;
        var messageFactory = new MessageFactory<TMessage>(_readerOptions.Schema);
        var batchHandler = new BatchHandler<TMessage>(false, messageFactory);
        var decompressorFactories = CompressionFactories.DecompressorFactories();
        var factory = new ConsumerChannelFactory<TMessage>(correlationId, _processManager, _connectionPool, subscribe, messagePrefetchCount, batchHandler, messageFactory, decompressorFactories, topic);
        var stateManager = new StateManager<ReaderState>(ReaderState.Disconnected, ReaderState.Closed, ReaderState.ReachedEndOfTopic, ReaderState.Faulted);
        var initialChannel = new NotReadyChannel<TMessage>();
        var executor = new Executor(correlationId, _processManager, _exceptionHandler);
        var subReader = new SubReader<TMessage>(correlationId, ServiceUrl, topic, _processManager, initialChannel, executor, stateManager, factory);
        var process = new ReaderProcess(correlationId, stateManager, subReader);
        _processManager.Add(process);
        process.Start();
        return subReader;
    }

    private string GetPartitionedTopicName(int partitionNumber) => $"{Topic}-partition-{partitionNumber}";

    private async Task Guard(CancellationToken cancellationToken)
    {
        if (!_allSubReadersAreReady)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
            _semaphoreSlim.Release();
        }

        if (_faultException is not null)
            throw new ReaderFaultedException(_faultException);
    }
}
