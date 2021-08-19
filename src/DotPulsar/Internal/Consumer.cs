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
    using Abstractions;
    using Compression;
    using DotPulsar.Abstractions;
    using DotPulsar.Exceptions;
    using DotPulsar.Extensions;
    using Extensions;
    using Nito.AsyncEx;
    using PulsarApi;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class Consumer<TMessage> : IConsumer<TMessage>, IRegisterEvent
    {
        private readonly IHandleException _exceptionHandler;
        private readonly ConsumerOptions<TMessage> _options;
        private readonly ProcessManager _processManager;
        private readonly IExecute _executor;
        private readonly StateManager<ConsumerState> _state;
        private readonly IConnectionPool _connectionPool;
        private readonly CancellationTokenSource _cts;
        private readonly ConcurrentDictionary<string, IConsumer<TMessage>> _consumers;
        private readonly AsyncReaderWriterLock _lock;

        private List<Task<ConsumerStateChanged>> _monitoringTasks;
        private ConcurrentQueue<IMessage<TMessage>> _messagesQueue;
        private int _consumersCount;
        private int _isDisposed;
        private Exception? _throw;

        public Uri ServiceUrl { get; }
        public string SubscriptionName { get; }
        public string Topic { get; }
        public ISet<string>? TopicNames { get; }
        public string? TopicsPattern { get; }
        public RegexSubscriptionMode RegexSubscriptionMode { get; }
        public uint NumberOfPartitions { get; }
        public bool AutoUpdatePartitions { get; }
        public TimeSpan AutoUpdatePartitionsInterval { get; }

        public Consumer(
            Uri serviceUrl,
            ConsumerOptions<TMessage> options,
            ProcessManager processManager,
            IHandleException exceptionHandler,
            IConnectionPool connectionPool)
        {
            _state = new StateManager<ConsumerState>(ConsumerState.Disconnected, ConsumerState.Closed,
                ConsumerState.ReachedEndOfTopic, ConsumerState.Faulted);
            ServiceUrl = serviceUrl;
            Topic = "ROOT_CONSUMER";
            TopicNames = options.TopicNames;
            TopicsPattern = options.TopicsPattern;
            RegexSubscriptionMode = options.RegexSubscriptionMode;
            SubscriptionName = options.SubscriptionName;
            AutoUpdatePartitions = options.AutoUpdatePartitions;
            AutoUpdatePartitionsInterval = options.AutoUpdatePartitionsInterval;
            _options = options;
            _exceptionHandler = exceptionHandler;
            _processManager = processManager;
            _connectionPool = connectionPool;
            _cts = new CancellationTokenSource();
            _executor = new Executor(Guid.Empty, this, _exceptionHandler);
            _isDisposed = 0;
            _consumers = new ConcurrentDictionary<string, IConsumer<TMessage>>();
            _messagesQueue = new ConcurrentQueue<IMessage<TMessage>>();
            _lock = new AsyncReaderWriterLock();
            _monitoringTasks = new List<Task<ConsumerStateChanged>>();

            if (!string.IsNullOrEmpty(TopicsPattern))
            {
                if (TopicNames != null && TopicNames.Count != 0)
                {
                    throw new ArgumentException("Topic names list must be null when use topicsPattern");
                }
            }
            else
            {
                if (TopicNames == null || TopicNames.Count == 0)
                {
                    throw new ArgumentException("Topic names list cannot be null");
                }
            }

            _ = Setup();
        }

        private async Task Setup()
        {
            await Task.Yield();

            try
            {
                List<string> topics;

                if (!string.IsNullOrEmpty(TopicsPattern))
                {
                    topics = await GetPatternTopic(TopicsPattern!);
                }
                else
                {
                    topics = TopicNames!.ToList();
                }

                await _executor.Execute(() => Monitor(topics), _cts.Token).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (_cts.IsCancellationRequested)
                    return;

                _throw = exception;
                _state.SetState(ConsumerState.Faulted);
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
                Subscription = _options.SubscriptionName,
                Topic = topic,
                Type = (CommandSubscribe.SubType) _options.SubscriptionType
            };
            var messagePrefetchCount = _options.MessagePrefetchCount;
            var messageFactory = new MessageFactory<TMessage>(_options.Schema);
            var batchHandler = new BatchHandler<TMessage>(topic, true, messageFactory);
            var decompressorFactories = CompressionFactories.DecompressorFactories();

            var factory = new ConsumerChannelFactory<TMessage>(correlationId, _processManager, _connectionPool, subscribe, messagePrefetchCount, batchHandler, messageFactory,
                decompressorFactories);
            var stateManager = new StateManager<ConsumerState>(ConsumerState.Disconnected, ConsumerState.Closed, ConsumerState.ReachedEndOfTopic, ConsumerState.Faulted);
            var initialChannel = new NotReadyChannel<TMessage>();
            var executor = new Executor(correlationId, _processManager, _exceptionHandler);
            var consumer = new SubConsumer<TMessage>(correlationId, ServiceUrl, _options.SubscriptionName, topic, _processManager, initialChannel, executor, stateManager, factory);
            var process = new ConsumerProcess(correlationId, stateManager, consumer, _options.SubscriptionType == SubscriptionType.Failover);
            _processManager.Add(process);
            process.Start();
            return consumer;
        }

        private async Task TopicChangesMonitor(IReadOnlyCollection<string> topics)
        {
            var delay = TimeSpan.FromMilliseconds(AutoUpdatePartitionsInterval.TotalMilliseconds);

            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(delay);

                using (_lock.WriterLock())
                {
                    try
                    {
                        var oldTopics = _consumers.Keys;
                        var newTopics = await GenerateSubscriptionTopics(topics);

                        // Not changed
                        if (newTopics.OrderBy(n => n).SequenceEqual(oldTopics.OrderBy(n => n)))
                        {
                            continue;
                        }

                        List<IMessage<TMessage>> newMessagesQueue = _messagesQueue.ToList();
                        List<Task<ConsumerStateChanged>> newMonitoringTasks = _monitoringTasks.ToList();

                        var removeTopics = oldTopics.Where(x => !newTopics.Contains(x)).ToList();

                        removeTopics.ForEach(n =>
                        {
                            _consumers.TryRemove(n, out var consumer);
                            consumer?.DisposeAsync();

                            newMessagesQueue = newMessagesQueue.Where(m => !m.Topic.Equals(n)).ToList();
                            newMonitoringTasks = newMonitoringTasks.Where(m => !m.Result.Consumer.Topic.Equals(n)).ToList();
                        });

                        var addTopics = newTopics.Where(x => !oldTopics.Contains(x)).ToList();

                        addTopics.ForEach(topic =>
                        {
                            var consumer = CreateSubConsumer(topic);
                            _consumers.TryAdd(topic, consumer);
                            newMonitoringTasks!.Add(consumer.StateChangedFrom(ConsumerState.Disconnected, _cts.Token).AsTask());
                        });

                        _monitoringTasks = newMonitoringTasks;
                        _messagesQueue = new ConcurrentQueue<IMessage<TMessage>>(newMessagesQueue);
                        Interlocked.Exchange(ref _consumersCount, _monitoringTasks.Count);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        private async Task Monitor(IEnumerable<string> topics)
        {
            var topicList = topics.ToList();
            var subscriptionTopics = await GenerateSubscriptionTopics(topicList);

            using (_lock.ReaderLock())
            {
                foreach (var topic in subscriptionTopics)
                {
                    var consumer = CreateSubConsumer(topic);
                    _ = _consumers.TryAdd(topic, consumer);
                    _monitoringTasks.Add(consumer.StateChangedFrom(ConsumerState.Disconnected, _cts.Token).AsTask());
                }

                Interlocked.Exchange(ref _consumersCount, _monitoringTasks.Count);
            }

            if (AutoUpdatePartitions)
            {
                _ = _executor.Execute(() => TopicChangesMonitor(topicList)).ConfigureAwait(false);
            }

            var activeConsumers = 0;

            while (true)
            {
                List<Task<ConsumerStateChanged>> monitoringTasks;

                using (_lock.ReaderLock())
                {
                    monitoringTasks = _monitoringTasks.ToList();
                }

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
                        case ConsumerState.Unsubscribed:
                            _state.SetState(ConsumerState.Unsubscribed);
                            return;
                        case ConsumerState.Faulted:
                            _state.SetState(ConsumerState.Faulted);
                            return;
                    }

                    using (_lock.WriterLock())
                    {
                        var index = i;

                        if (monitoringTasks.Count != _monitoringTasks.Count)
                        {
                            index = _monitoringTasks.FindIndex(n
                                => n.Result.Consumer.Topic.Equals(task.Result.Consumer.Topic));
                        }

                        if (index != -1)
                        {
                            _monitoringTasks[i] = task.Result.Consumer.StateChangedFrom(state, _cts.Token).AsTask();
                        }
                    }
                }

                if (activeConsumers == 0)
                    _state.SetState(ConsumerState.Disconnected);
                else if (activeConsumers == monitoringTasks.Count)
                    _state.SetState(ConsumerState.Active);
                else
                    _state.SetState(ConsumerState.PartiallyActive);
            }
        }

        private static GetTopicsUnderNamespaceMode ConvertRegexSubscriptionMode(
            RegexSubscriptionMode regexSubscriptionMode)
        {
            return regexSubscriptionMode switch
            {
                RegexSubscriptionMode.PersistentOnly => GetTopicsUnderNamespaceMode.PERSISTENT,
                RegexSubscriptionMode.NonPersistentOnly => GetTopicsUnderNamespaceMode.NON_PERSISTENT,
                RegexSubscriptionMode.AllTopics => GetTopicsUnderNamespaceMode.ALL,
                _ => throw new ArgumentOutOfRangeException(nameof(regexSubscriptionMode), regexSubscriptionMode, null)
            };
        }

        private async Task<List<string>> GetPatternTopic(string pattern)
        {
            var destination = new TopicName(pattern);
            var mode = ConvertRegexSubscriptionMode(RegexSubscriptionMode);
            NamespaceName namespaceName = destination.GetNamespaceName();

            var pbMode = mode switch
            {
                GetTopicsUnderNamespaceMode.PERSISTENT => CommandGetTopicsOfNamespace.ModeType.Persistent,
                GetTopicsUnderNamespaceMode.NON_PERSISTENT => CommandGetTopicsOfNamespace.ModeType.NonPersistent,
                GetTopicsUnderNamespaceMode.ALL => CommandGetTopicsOfNamespace.ModeType.All,
                _ => throw new ArgumentOutOfRangeException()
            };

            var req = new CommandGetTopicsOfNamespace { Namespace = namespaceName.ToString(), Mode = pbMode };
            var connection = await _connectionPool.GetConnection(ServiceUrl, _cts.Token).ConfigureAwait(false);
            var response = await connection.Send(req, _cts.Token).ConfigureAwait(false);
            response.Expect(BaseCommand.Type.GetTopicsOfNamespaceResponse);

            var topics = response.GetTopicsOfNamespaceResponse.Topics;

            Regex topicPatternRegex = TopicsPattern!.Contains("://")
                ? new Regex(TopicsPattern.Split(new[] { "://" }, StringSplitOptions.None)[1])
                : new Regex(TopicsPattern);

            var filteredTopics = topics.Select(n => new TopicName(n))
                .Select(n => n.ToString())
                .Where(n => topicPatternRegex.IsMatch(n.Split(new[] { "://" }, StringSplitOptions.None)[1]))
                .ToList();

            return filteredTopics;
        }

        private async Task<uint> GetNumberOfPartitions(string topic, CancellationToken cancellationToken)
        {
            var connection = await _connectionPool.FindConnectionForTopic(topic, cancellationToken).ConfigureAwait(false);
            var commandPartitionedMetadata = new CommandPartitionedTopicMetadata { Topic = topic };
            var response = await connection.Send(commandPartitionedMetadata, cancellationToken).ConfigureAwait(false);

            response.Expect(BaseCommand.Type.PartitionedMetadataResponse);

            if (response.PartitionMetadataResponse.Response == CommandPartitionedTopicMetadataResponse.LookupType.Failed)
                response.PartitionMetadataResponse.Throw();

            return response.PartitionMetadataResponse.Partitions;
        }

        private async Task<List<string>> GenerateSubscriptionTopics(IEnumerable<string> originalTopics)
        {
            var topicNumberOfPartitionsDictionary = new Dictionary<string, uint>();

            foreach (var topic in originalTopics)
            {
                var numberOfPartitions = await GetNumberOfPartitions(topic, _cts.Token).ConfigureAwait(false);
                topicNumberOfPartitionsDictionary.Add(topic, numberOfPartitions);
            }

            return topicNumberOfPartitionsDictionary
                .Where(n => n.Value == 0)
                .Select(n => n.Key)
                .Concat(
                    topicNumberOfPartitionsDictionary
                        .Where(n => n.Value != 0)
                        .Select(n =>
                        {
                            return Enumerable.Range(0, Convert.ToInt32(n.Value)).Select(m => $"{n.Key}-partition-{m}");
                        })
                        .SelectMany(n => n)
                ).ToList();
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

        public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
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

                while (!cts.IsCancellationRequested)
                {
                    var done = false;

                    Task<IMessage<TMessage>>[] receiveTasks = _consumers.Values.Select(consumer => consumer.Receive(cts.Token).AsTask()).ToArray();
                    await Task.WhenAny(receiveTasks).ConfigureAwait(false);

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

        public async ValueTask Acknowledge(IMessageId messageId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            await _executor.Execute(() =>
                {
                    ThrowIfNotActive();

                    using (_lock.ReaderLock())
                    {
                        return _consumers[messageId.Topic].Acknowledge(messageId, cancellationToken);
                    }
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask AcknowledgeCumulative(IMessageId messageId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            await _executor.Execute(() =>
                {
                    ThrowIfNotActive();

                    using (_lock.ReaderLock())
                    {
                        return _consumers[messageId.Topic].AcknowledgeCumulative(messageId, cancellationToken);
                    }
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask RedeliverUnacknowledgedMessages(IEnumerable<IMessageId> messageIds, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await _executor.Execute<ValueTask>(async () =>
            {
                ThrowIfNotActive();

                using (_lock.ReaderLock())
                {
                    var tasks = messageIds.Select(n => _consumers[n.Topic].RedeliverUnacknowledgedMessages(cancellationToken).AsTask());
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

        private static bool IsIllegalMultiTopicsMessageId(MessageId messageId)
        {
            //only support earliest/latest
            return !MessageId.Earliest.Equals(messageId) && !MessageId.Latest.Equals(messageId);
        }

        public async ValueTask Seek(MessageId messageId, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await _executor.Execute<ValueTask>(async () =>
            {
                ThrowIfNotActive();

                using (_lock.ReaderLock())
                {
                    if (_consumers.Values.All(n => messageId.Equals(null) && n.NumberOfPartitions != 0) || IsIllegalMultiTopicsMessageId(messageId))
                    {
                        throw new ArgumentException("Illegal messageId can only be earliest/latest.");
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

        public async ValueTask<IMessageId> GetLastMessageId(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            var getLastMessageId = new CommandGetLastMessageId();
            return await _executor.Execute(() => GetLastMessageId(getLastMessageId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private async ValueTask<IMessageId> GetLastMessageId(CommandGetLastMessageId command, CancellationToken cancellationToken)
        {
            ThrowIfNotActive();

            using (_lock.ReaderLock())
            {
                if (_consumers.Count == 1)
                {
                    return await _consumers.Values.First().GetLastMessageId(cancellationToken).ConfigureAwait(false);
                }

                var messageIdMap = new Dictionary<string, IMessageId>();

                foreach (var consumer in _consumers)
                {
                    var m = await consumer.Value.GetLastMessageId(cancellationToken).ConfigureAwait(false);
                    messageIdMap.Add(consumer.Key, m);
                }

                return new MultiMessageId(messageIdMap);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new ConsumerDisposedException(GetType().FullName!);
        }

        private void ThrowIfNotActive()
        {
            if (_state.CurrentState != ConsumerState.Active)
                throw new ConsumerNotActiveException("The consumer is not yet activated.");
        }

        public void Register(IEvent @event) { }
    }
}
