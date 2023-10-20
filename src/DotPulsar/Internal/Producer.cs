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
using DotPulsar.Internal.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed class Producer<TMessage> : IProducer<TMessage>, IRegisterEvent
{
    private readonly string _operationName;
    private readonly KeyValuePair<string, object?>[] _activityTags;
    private readonly KeyValuePair<string, object?>[] _meterTags;
    private readonly bool _attachTraceInfoToMessages;
    private readonly SequenceId _sequenceId;
    private readonly StateManager<ProducerState> _state;
    private readonly IConnectionPool _connectionPool;
    private readonly IHandleException _exceptionHandler;
    private readonly ICompressorFactory? _compressorFactory;
    private readonly ProducerOptions<TMessage> _options;
    private readonly ProcessManager _processManager;
    private readonly ConcurrentDictionary<int, SubProducer> _producers;
    private readonly IMessageRouter _messageRouter;
    private readonly CancellationTokenSource _cts;
    private readonly IExecute _executor;
    private int _isDisposed;
    private int _producerCount;
    private Exception? _faultException;

    public Uri ServiceUrl { get; }
    public string Topic { get; }

    public ISendChannel<TMessage> SendChannel { get; }

    public Producer(
        Uri serviceUrl,
        ProducerOptions<TMessage> options,
        IHandleException exceptionHandler,
        IConnectionPool connectionPool,
        ICompressorFactory? compressorFactory)
    {
        _operationName = $"{options.Topic} send";
        _activityTags = new KeyValuePair<string, object?>[]
        {
            new("messaging.destination", options.Topic),
            new("messaging.destination_kind", "topic"),
            new("messaging.system", "pulsar"),
            new("messaging.url", serviceUrl),
        };
        _meterTags = new KeyValuePair<string, object?>[]
        {
            new("topic", options.Topic)
        };
        _attachTraceInfoToMessages = options.AttachTraceInfoToMessages;
        _sequenceId = new SequenceId(options.InitialSequenceId);
        _state = CreateStateManager();
        ServiceUrl = serviceUrl;
        Topic = options.Topic;
        _isDisposed = 0;
        _options = options;
        _exceptionHandler = exceptionHandler;
        _connectionPool = connectionPool;
        _compressorFactory = compressorFactory;
        _processManager = new ProcessManager();
        _messageRouter = options.MessageRouter;
        _cts = new CancellationTokenSource();
        _executor = new Executor(Guid.Empty, this, _exceptionHandler);
        _producers = new ConcurrentDictionary<int, SubProducer>();
        SendChannel = new SendChannel<TMessage>(this);
        _ = Setup();
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
            _state.SetState(ProducerState.Faulted);
        }
    }

    private async Task Monitor()
    {
        var numberOfPartitions = await _connectionPool.GetNumberOfPartitions(Topic, _cts.Token).ConfigureAwait(false);
        var isPartitionedTopic = numberOfPartitions != 0;
        var numberOfSubProducers = isPartitionedTopic ? numberOfPartitions : 1;
        var monitoringTasks = new Task<ProducerState>[numberOfSubProducers];
        var states = new ProducerState[numberOfSubProducers];

        for (var i = 0; i < numberOfSubProducers; ++i)
        {
            var topicName = isPartitionedTopic ? GetPartitionedTopicName(i) : Topic;
            var producer = CreateSubProducer(topicName, isPartitionedTopic ? i : -1);
            _ = _producers.TryAdd(i, producer);
            monitoringTasks[i] = producer.OnStateChangeFrom(ProducerState.Disconnected, _cts.Token).AsTask();
        }

        Interlocked.Exchange(ref _producerCount, monitoringTasks.Length);

        while (true)
        {
            await Task.WhenAny(monitoringTasks).ConfigureAwait(false);

            for (var i = 0; i < monitoringTasks.Length; ++i)
            {
                var task = monitoringTasks[i];
                if (!task.IsCompleted)
                    continue;

                var state = task.Result;
                states[i] = state;
                monitoringTasks[i] = _producers[i].OnStateChangeFrom(state, _cts.Token).AsTask();
            }

            if (!isPartitionedTopic)
                _state.SetState(states[0]);
            else if (states.Any(x => x == ProducerState.Faulted))
                _state.SetState(ProducerState.Faulted);
            else if (states.Any(x => x == ProducerState.Fenced))
                _state.SetState(ProducerState.Fenced);
            else if (states.All(x => x == ProducerState.Connected))
                _state.SetState(ProducerState.Connected);
            else if (states.All(x => x == ProducerState.Disconnected))
                _state.SetState(ProducerState.Disconnected);
            else if (states.Any(x => x == ProducerState.Disconnected))
                _state.SetState(ProducerState.PartiallyConnected);
            else
                _state.SetState(ProducerState.WaitingForExclusive);
        }
    }

    private SubProducer CreateSubProducer(string topic, int partition)
    {
        var correlationId = Guid.NewGuid();
        var producerName = _options.ProducerName;
        var schema = _options.Schema;
        var producerAccessMode = (PulsarApi.ProducerAccessMode) _options.ProducerAccessMode;
        var factory = new ProducerChannelFactory(correlationId, _processManager, _connectionPool, topic, producerName, producerAccessMode, schema.SchemaInfo, _compressorFactory);
        var stateManager = CreateStateManager();
        var initialChannel = new NotReadyChannel<TMessage>();
        var executor = new Executor(correlationId, _processManager, _exceptionHandler);
        var producer = new SubProducer(correlationId, _processManager, initialChannel, executor, stateManager, factory, partition, _options.MaxPendingMessages, topic);
        var process = new ProducerProcess(correlationId, stateManager, producer);
        _processManager.Add(process);
        process.Start();
        return producer;
    }
    public bool IsFinalState()
        => _state.IsFinalState();

    public bool IsFinalState(ProducerState state)
        => _state.IsFinalState(state);

    public async ValueTask<ProducerState> OnStateChangeTo(ProducerState state, CancellationToken cancellationToken)
        => await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);

    public async ValueTask<ProducerState> OnStateChangeFrom(ProducerState state, CancellationToken cancellationToken)
        => await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _cts.Cancel();
        _cts.Dispose();

        await _processManager.DisposeAsync().ConfigureAwait(false);

        foreach (var subProducer in _producers.Values)
            await subProducer.DisposeAsync().ConfigureAwait(false);

        _state.SetState(ProducerState.Closed);
    }

    private async ValueTask<int> ChoosePartitions(MessageMetadata metadata, CancellationToken cancellationToken)
    {
        if (_producerCount == 0)
        {
            _ = await _state.StateChangedFrom(ProducerState.Disconnected, cancellationToken).ConfigureAwait(false);
            if (_faultException is not null)
                throw new ProducerFaultedException(_faultException);
        }

        if (_producerCount == 1)
            return 0;

        return _messageRouter.ChoosePartition(metadata, _producerCount);
    }

    public async ValueTask<MessageId> Send(MessageMetadata metadata, TMessage message, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<MessageId>();
        var registration = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

        ValueTask OnMessageSent(MessageId messageId)
        {
            tcs.TrySetResult(messageId);
#if NET6_0_OR_GREATER
            return ValueTask.CompletedTask;
#else
            return new ValueTask();
#endif
        }

        try
        {
            await InternalSend(metadata, message, true, OnMessageSent, x => tcs.TrySetException(x), cancellationToken).ConfigureAwait(false);
            return await tcs.Task.ConfigureAwait(false);
        }
        finally
        {
            registration.Dispose();
        }
    }

    public async ValueTask Enqueue(MessageMetadata metadata, TMessage message, Func<MessageId, ValueTask>? onMessageSent = default, CancellationToken cancellationToken = default)
        => await InternalSend(metadata, message, false, onMessageSent, cancellationToken: cancellationToken).ConfigureAwait(false);

    private async ValueTask InternalSend(MessageMetadata metadata, TMessage message, bool sendOpCancelable, Func<MessageId, ValueTask>? onMessageSent = default, Action<Exception>? onFailed = default, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        var autoAssignSequenceId = metadata.SequenceId == 0;
        if (autoAssignSequenceId)
            metadata.SequenceId = _sequenceId.FetchNext();

        var activity = DotPulsarActivitySource.StartProducerActivity(metadata, _operationName, _activityTags);
        if (activity is not null && _attachTraceInfoToMessages)
        {
            metadata[Constants.TraceParent] = activity.Id;
            if (activity.TraceStateString is not null)
                metadata[Constants.TraceState] = activity.TraceStateString;
        }

        var startTimestamp = DotPulsarMeter.MessageSentEnabled ? Stopwatch.GetTimestamp() : 0;

        try
        {
            var partition = await ChoosePartitions(metadata, cancellationToken).ConfigureAwait(false);
            var subProducer = _producers[partition];
            var data = _options.Schema.Encode(message);

            var tcs = new TaskCompletionSource<MessageId>();
            await subProducer.Send(new SendOp(metadata.Metadata, data, tcs, sendOpCancelable ? cancellationToken : CancellationToken.None), cancellationToken).ConfigureAwait(false);

            _ = tcs.Task.ContinueWith(async task =>
            {
                if (startTimestamp != 0)
                    DotPulsarMeter.MessageSent(startTimestamp, _meterTags);

                if (task.IsFaulted || task.IsCanceled)
                {
                    Exception exception = task.IsCanceled ? new OperationCanceledException() : task.Exception!;
                    FailActivity(exception, activity);

                    if (autoAssignSequenceId)
                        metadata.SequenceId = 0;

                    onFailed?.Invoke(exception);
                    return;
                }

                CompleteActivity(task.Result, data.Length, activity);

                try
                {
                    if (onMessageSent is not null)
                        await onMessageSent.Invoke(task.Result).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // ignored
                }
            }, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            FailActivity(exception, activity);

            if (autoAssignSequenceId)
                metadata.SequenceId = 0;

            throw;
        }
    }

    internal async ValueTask WaitForSendQueueEmpty(CancellationToken cancellationToken)
        => await Task.WhenAll(_producers.Values.Select(producer => producer.WaitForSendQueueEmpty(cancellationToken).AsTask())).ConfigureAwait(false);

    private static void CompleteActivity(MessageId messageId, long payloadSize, Activity? activity)
    {
        if (activity is null)
            return;

        if (activity.IsAllDataRequested)
        {
            activity.SetMessageId(messageId);
            activity.SetPayloadSize(payloadSize);
            activity.SetStatus(ActivityStatusCode.Ok);
        }

        activity.Dispose();
    }

    private static void FailActivity(Exception exception, Activity? activity)
    {
        if (activity is null)
            return;

        if (activity.IsAllDataRequested)
            activity.AddException(exception);

        activity.Dispose();
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
            throw new ProducerDisposedException(GetType().FullName!);
    }

    private static StateManager<ProducerState> CreateStateManager()
        => new(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted, ProducerState.Fenced);

    private string GetPartitionedTopicName(int partitionNumber) => $"{Topic}-partition-{partitionNumber}";

    public void Register(IEvent @event) { }
}
