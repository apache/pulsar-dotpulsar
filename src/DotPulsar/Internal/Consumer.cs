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
    using PulsarApi;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly ConcurrentDictionary<int, IConsumer<TMessage>> _consumers;
        private ConcurrentQueue<IMessage<TMessage>> _messagesQueue;
        private bool _isPartitionedTopic = false;
        private int _consumersCount;
        private int _isDisposed;
        private Exception? _throw;

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
            _state = new StateManager<ConsumerState>(ConsumerState.Disconnected, ConsumerState.Closed, ConsumerState.ReachedEndOfTopic, ConsumerState.Faulted);
            ServiceUrl = serviceUrl;
            Topic = options.Topic;
            SubscriptionName = options.SubscriptionName;
            _options = options;
            _exceptionHandler = exceptionHandler;
            _processManager = processManager;
            _connectionPool = connectionPool;
            _cts = new CancellationTokenSource();
            _executor = new Executor(Guid.Empty, this, _exceptionHandler);
            _isDisposed = 0;
            _consumers = new ConcurrentDictionary<int, IConsumer<TMessage>>();
            _messagesQueue = new ConcurrentQueue<IMessage<TMessage>>();

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
            var batchHandler = new BatchHandler<TMessage>(true, messageFactory);
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

        private async Task Monitor()
        {
            var numberOfPartitions = await GetNumberOfPartitions(Topic, _cts.Token).ConfigureAwait(false);
            _isPartitionedTopic = numberOfPartitions != 0;
            var monitoringTasks = new Task<ConsumerStateChanged>[_isPartitionedTopic ? numberOfPartitions : 1];

            var topic = Topic;

            for (var partition = 0; partition < monitoringTasks.Length; ++partition)
            {
                if (_isPartitionedTopic)
                    topic = $"{Topic}-partition-{partition}";

                var consumer = CreateSubConsumer(topic);
                _ = _consumers.TryAdd(partition, consumer);
                monitoringTasks[partition] = consumer.StateChangedFrom(ConsumerState.Disconnected, _cts.Token).AsTask();
            }

            Interlocked.Exchange(ref _consumersCount, monitoringTasks.Length);

            var activeConsumers = 0;

            while (true)
            {
                await Task.WhenAny(monitoringTasks).ConfigureAwait(false);

                for (var i = 0; i < monitoringTasks.Length; ++i)
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

                    monitoringTasks[i] = task.Result.Consumer.StateChangedFrom(state, _cts.Token).AsTask();
                }

                if (activeConsumers == 0)
                    _state.SetState(ConsumerState.Disconnected);
                else if (activeConsumers == monitoringTasks.Length)
                    _state.SetState(ConsumerState.Active);
                else
                    _state.SetState(ConsumerState.PartiallyActive);
            }
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

            foreach (var consumer in _consumers.Values)
            {
                await consumer.DisposeAsync().ConfigureAwait(false);
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

            if (_messagesQueue.TryDequeue(out IMessage<TMessage> message))
                return message;

            var cts = new CancellationTokenSource();
            Task<IMessage<TMessage>>[] receiveTasks = _consumers.Values.Select(consumer => consumer.Receive(cts.Token).AsTask()).ToArray();
            await Task.WhenAny(receiveTasks).ConfigureAwait(false);

            receiveTasks.Where(t => t.IsCompleted).ToList().ForEach(t =>
                _messagesQueue.Enqueue(t.Result)
            );

            cts.Cancel();
            cts.Dispose();

            _messagesQueue.TryDequeue(out message);
            return message;
        }

        public async ValueTask Acknowledge(MessageId messageId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            await _executor.Execute(() =>
                {
                    ThrowIfNotActive();
                    return _consumers[_isPartitionedTopic ? messageId.Partition : 0].Acknowledge(messageId, cancellationToken);
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask AcknowledgeCumulative(MessageId messageId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            await _executor.Execute(() =>
                {
                    ThrowIfNotActive();
                    return _consumers[_isPartitionedTopic ? messageId.Partition : 0].AcknowledgeCumulative(messageId, cancellationToken);
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask RedeliverUnacknowledgedMessages(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await _executor.Execute<ValueTask>(async () =>
            {
                ThrowIfNotActive();

                var tasks = messageIds.ToList().Select(messageId =>
                    _consumers[_isPartitionedTopic ? messageId.Partition : 0].RedeliverUnacknowledgedMessages(cancellationToken).AsTask()
                );
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask RedeliverUnacknowledgedMessages(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await _executor.Execute<ValueTask>(async () =>
            {
                ThrowIfNotActive();

                var tasks = _consumers.Values.ToList().Select(consumer =>
                    consumer.RedeliverUnacknowledgedMessages(cancellationToken).AsTask()
                );
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask Unsubscribe(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            await _executor.Execute<ValueTask>(async () =>
            {
                ThrowIfNotActive();

                var tasks = _consumers.Values.ToList().Select(consumer =>
                    consumer.Unsubscribe(cancellationToken).AsTask()
                );
                await Task.WhenAll(tasks).ConfigureAwait(false);
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

                if (_isPartitionedTopic && messageId.Equals(null) || IsIllegalMultiTopicsMessageId(messageId))
                {
                    throw new ArgumentException("Illegal messageId, messageId can only be earliest/latest.");
                }

                var tasks = _consumers.Values.ToList().Select(consumer =>
                    consumer.Seek(messageId, cancellationToken).AsTask()
                );
                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask Seek(ulong publishTime, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await _executor.Execute<ValueTask>(async () =>
            {
                ThrowIfNotActive();

                var tasks = _consumers.Values.ToList().Select(consumer =>
                    consumer.Seek(publishTime, cancellationToken).AsTask()
                );
                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
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

            if (_isPartitionedTopic)
                throw new NotImplementedException();
            return await _consumers[0].GetLastMessageId(cancellationToken).ConfigureAwait(false);
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
