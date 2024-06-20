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
using DotPulsar.Internal.Extensions;
using Microsoft.Extensions.ObjectPool;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

public sealed class MessageProcessor<TMessage> : IDisposable
{
    private readonly string _operationName;
    private readonly KeyValuePair<string, object?>[] _activityTags;
    private readonly KeyValuePair<string, object?>[] _meterTags;
    private readonly IConsumer<TMessage> _consumer;
    private readonly Func<IMessage<TMessage>, CancellationToken, ValueTask> _processor;
    private readonly LinkedList<Task> _processorTasks;
    private readonly ConcurrentQueue<ProcessInfo> _processingQueue;
    private readonly SemaphoreSlim _receiveLock;
    private readonly SemaphoreSlim _acknowledgeLock;
    private readonly ObjectPool<ProcessInfo> _processInfoPool;
    private readonly bool _linkTraces;
    private readonly bool _ensureOrderedAcknowledgment;
    private readonly int _maxDegreeOfParallelism;
    private readonly int _maxMessagesPerTask;
    private readonly TaskScheduler _taskScheduler;

    public MessageProcessor(
        IConsumer<TMessage> consumer,
        Func<IMessage<TMessage>, CancellationToken, ValueTask> processor,
        ProcessingOptions options)
    {
        if (options.EnsureOrderedAcknowledgment &&
            (consumer.SubscriptionType == SubscriptionType.Shared ||
            consumer.SubscriptionType == SubscriptionType.KeyShared))
            throw new ProcessingException("Ordered acknowledgment can not be ensuring with shared subscription types");

        const string operation = "process";
        _operationName = $"{consumer.Topic} {operation}";

        _activityTags =
        [
            new("messaging.destination", consumer.Topic),
            new("messaging.destination_kind", "topic"),
            new("messaging.operation", operation),
            new("messaging.system", "pulsar"),
            new("messaging.url", consumer.ServiceUrl),
            new("messaging.pulsar.subscription", consumer.SubscriptionName)
        ];

        _meterTags =
        [
            new("topic", consumer.Topic),
            new("subscription", consumer.SubscriptionName)
        ];

        _consumer = consumer;
        _processor = processor;
        _processorTasks = new LinkedList<Task>();
        _processingQueue = new ConcurrentQueue<ProcessInfo>();
        _receiveLock = new SemaphoreSlim(1, 1);
        _acknowledgeLock = new SemaphoreSlim(1, 1);
        _processInfoPool = new DefaultObjectPool<ProcessInfo>(new DefaultPooledObjectPolicy<ProcessInfo>());

        _linkTraces = options.LinkTraces;
        _ensureOrderedAcknowledgment = options.EnsureOrderedAcknowledgment;
        _maxDegreeOfParallelism = options.MaxDegreeOfParallelism;
        _maxMessagesPerTask = options.MaxMessagesPerTask;
        _taskScheduler = options.TaskScheduler;
    }

    public void Dispose()
    {
        _receiveLock.Dispose();
        _acknowledgeLock.Dispose();
    }

    public async ValueTask Process(CancellationToken cancellationToken)
    {
        for (var i = 1; i < _maxDegreeOfParallelism; ++i)
        {
            StartNewProcessorTask(cancellationToken);
        }

        while (true)
        {
            StartNewProcessorTask(cancellationToken);
            var completedTask = await Task.WhenAny(_processorTasks).ConfigureAwait(false);
            if (completedTask.IsFaulted)
                ExceptionDispatchInfo.Capture(completedTask.Exception!.InnerException!).Throw();
            _processorTasks.Remove(completedTask);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private async ValueTask Processor(CancellationToken cancellationToken)
    {
        var messagesProcessed = 0;

        var processInfo = new ProcessInfo();

        var needToEnsureOrderedAcknowledgement = _ensureOrderedAcknowledgment && _maxDegreeOfParallelism > 1;
        var isUnbounded = _maxMessagesPerTask == ProcessingOptions.Unbounded;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (needToEnsureOrderedAcknowledgement)
            {
                processInfo = _processInfoPool.Get();
                await _receiveLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            }

            var message = await _consumer.Receive(cancellationToken).ConfigureAwait(false);

            if (needToEnsureOrderedAcknowledgement)
            {
                processInfo.MessageId = message.MessageId;
                processInfo.IsProcessed = false;
                _processingQueue.Enqueue(processInfo);
                _receiveLock.Release();
            }

            var activity = DotPulsarActivitySource.StartConsumerActivity(message, _operationName, _activityTags, _linkTraces);
            if (activity is not null && activity.IsAllDataRequested)
            {
                activity.SetMessageId(message.MessageId);
                activity.SetPayloadSize(message.Data.Length);
                activity.SetStatus(ActivityStatusCode.Ok);
            }

            var startTimestamp = DotPulsarMeter.MessageProcessedEnabled ? Stopwatch.GetTimestamp() : 0;

            try
            {
                await _processor(message, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (activity is not null && activity.IsAllDataRequested)
                    activity.AddException(exception);
            }

            if (startTimestamp != 0)
                DotPulsarMeter.MessageProcessed(startTimestamp, _meterTags);

            activity?.Dispose();

            if (needToEnsureOrderedAcknowledgement)
            {
                await _acknowledgeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                processInfo.IsProcessed = true;
                await AcknowledgeProcessedMessages(cancellationToken).ConfigureAwait(false);
                _acknowledgeLock.Release();
            }
            else
                await _consumer.Acknowledge(message.MessageId, cancellationToken).ConfigureAwait(false);

            if (!isUnbounded && ++messagesProcessed == _maxMessagesPerTask)
                return;
        }
    }

    private async ValueTask AcknowledgeProcessedMessages(CancellationToken cancellationToken)
    {
        var messagesToAcknowledge = 0;
        var messageId = MessageId.Earliest;

        while (_processingQueue.TryPeek(out var processInfo))
        {
            if (!processInfo.IsProcessed)
                break;

            ++messagesToAcknowledge;

            if (_processingQueue.TryDequeue(out processInfo))
            {
                messageId = processInfo.MessageId;
                _processInfoPool.Return(processInfo);
            }
        }

        if (messagesToAcknowledge == 1)
            await _consumer.Acknowledge(messageId, cancellationToken).ConfigureAwait(false);
        else if (messagesToAcknowledge > 1)
            await _consumer.AcknowledgeCumulative(messageId, cancellationToken).ConfigureAwait(false);
    }

    private void StartNewProcessorTask(CancellationToken cancellationToken)
    {
        var processorTask = Task.Factory.StartNew(
            async () => await Processor(cancellationToken).ConfigureAwait(false),
            cancellationToken,
            TaskCreationOptions.DenyChildAttach,
            _taskScheduler).Unwrap();

        _processorTasks.AddLast(processorTask);
    }

    private sealed class ProcessInfo
    {
        public ProcessInfo()
        {
            MessageId = MessageId.Earliest;
            IsProcessed = false;
        }

        public MessageId MessageId { get; set; }
        public bool IsProcessed { get; set; }
    }
}
