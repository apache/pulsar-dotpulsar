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
    private Task<IMessage<TMessage>>[] _receiveTaskQueueForSubReaders;
    private int _subReaderIndex;
    private bool _isPartitioned;
    private int _numberOfPartitions;
    private int _isDisposed;
    private Exception? _faultException;

    public Uri ServiceUrl { get; }
    public string Topic { get; }

    public Reader(
        Uri serviceUrl,
        ReaderOptions<TMessage> readerOptions,
        ProcessManager processManager,
        IHandleException exceptionHandler,
        IConnectionPool connectionPool)
    {
        ServiceUrl = serviceUrl;
        Topic = readerOptions.Topic;
        _readerOptions = readerOptions;
        _connectionPool = connectionPool;
        _processManager = processManager;
        _exceptionHandler = exceptionHandler;
        _semaphoreSlim = new SemaphoreSlim(1);
        _state = CreateStateManager();
        _receiveTaskQueueForSubReaders = Array.Empty<Task<IMessage<TMessage>>>();
        _cts = new CancellationTokenSource();
        _executor = new Executor(Guid.Empty, _processManager, _exceptionHandler);
        _isDisposed = 0;
        _subReaders = null!;

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
            _state.SetState(ReaderState.Faulted);
        }
    }

    private async Task Monitor()
    {
        await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
        _numberOfPartitions = Convert.ToInt32(await _connectionPool.GetNumberOfPartitions(Topic, _cts.Token).ConfigureAwait(false));
        _isPartitioned = _numberOfPartitions != 0;
        _receiveTaskQueueForSubReaders = new Task<IMessage<TMessage>>[_numberOfPartitions];
        var readerStateTasks = new Task<ReaderState>[_numberOfPartitions];
        ReaderState[] subReaderStates;

        for (var i = 0; i < _receiveTaskQueueForSubReaders.Length; i++)
        {
            _receiveTaskQueueForSubReaders[i] = _emptyTaskCompletionSource.Task;
        }

        if (_isPartitioned)
        {
            _subReaderIndex = -1;
            _subReaders = new SubReader<TMessage>[_numberOfPartitions];
            subReaderStates = new ReaderState[_numberOfPartitions];
            for (var partition = 0; partition < _numberOfPartitions; partition++)
            {
                var partitionedTopicName = GetPartitionedTopicName(partition);
                _subReaders[partition] = CreateSubReader(partitionedTopicName);
                readerStateTasks[partition] = _subReaders[partition].OnStateChangeFrom(ReaderState.Disconnected, _cts.Token).AsTask();
            }
        }
        else
        {
            _subReaderIndex = 0;
            readerStateTasks = new Task<ReaderState>[1];
            subReaderStates = new ReaderState[1];
            _subReaders = new SubReader<TMessage>[1];
            _subReaders[0] = CreateSubReader(Topic);
            readerStateTasks[0] = _subReaders[0].OnStateChangeFrom(ReaderState.Disconnected, _cts.Token).AsTask();
        }
        _allSubReadersAreReady = true;
        _semaphoreSlim.Release();

        while (true)
        {
            await Task.WhenAny(readerStateTasks).ConfigureAwait(false);

            for (var i = 0; i < readerStateTasks.Length; ++i)
            {
                var task = readerStateTasks[i];
                if (!task.IsCompleted)
                    continue;

                var state = task.Result;
                subReaderStates[i] = state;

                readerStateTasks[i] = _subReaders[i].OnStateChangeFrom(state, _cts.Token).AsTask();
            }

            if (subReaderStates.Any(x => x == ReaderState.Faulted))
                _state.SetState(ReaderState.Faulted);
            else if (subReaderStates.All(x => x == ReaderState.Connected))
                _state.SetState(ReaderState.Connected);
            else if (subReaderStates.All(x => x == ReaderState.Disconnected))
                _state.SetState(ReaderState.Disconnected);
            else if (subReaderStates.All(x => x == ReaderState.ReachedEndOfTopic))
                _state.SetState(ReaderState.ReachedEndOfTopic);
            else if (subReaderStates.Length > 1) //States for a partitioned topic
            {
                if (subReaderStates.Any(x => x == ReaderState.Connected) && subReaderStates.Any(x => x == ReaderState.Disconnected))
                    _state.SetState(ReaderState.PartiallyConnected);
            }
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

    [Obsolete("GetLastMessageId is obsolete. Please use GetLastMessageIds instead.")]
    public async ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitioned)
            return await _subReaders[_subReaderIndex].GetLastMessageId(cancellationToken).ConfigureAwait(false);
        throw new NotSupportedException("GetLastMessageId can't be used on partitioned topics. Please use GetLastMessageIds");
    }

    public async ValueTask<IEnumerable<MessageId>> GetLastMessageIds(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitioned)
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

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitioned)
            return await _subReaders[_subReaderIndex].Receive(cancellationToken).ConfigureAwait(false);

        var iterations = 0;
        while (true)
        {
            iterations++;
            _subReaderIndex++;
            if (_subReaderIndex == _subReaders.Length)
                _subReaderIndex = 0;

            var receiveTask = _receiveTaskQueueForSubReaders[_subReaderIndex];
            if (receiveTask == _emptyTaskCompletionSource.Task)
            {
                var receiveTaskValueTask = _subReaders[_subReaderIndex].Receive(cancellationToken);
                if (receiveTaskValueTask.IsCompleted)
                    return receiveTaskValueTask.Result;
                _receiveTaskQueueForSubReaders[_subReaderIndex] = receiveTaskValueTask.AsTask();
            }
            else
            {
                if (receiveTask.IsCompleted)
                {
                    _receiveTaskQueueForSubReaders[_subReaderIndex] = _emptyTaskCompletionSource.Task;
                    return receiveTask.Result;
                }
            }
            if (iterations == _subReaders.Length)
                await Task.WhenAny(_receiveTaskQueueForSubReaders).ConfigureAwait(false);
        }
    }

    public async ValueTask Seek(MessageId messageId, CancellationToken cancellationToken)
    {
        await Guard(cancellationToken).ConfigureAwait(false);

        if (!_isPartitioned)
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

        if (!_isPartitioned)
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

        _state.SetState(ReaderState.Closed);

        foreach (var subConsumer in _subReaders)
        {
            await subConsumer.DisposeAsync().ConfigureAwait(false);
        }
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
    }
}
