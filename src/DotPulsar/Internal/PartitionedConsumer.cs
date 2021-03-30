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
    using DotPulsar.Abstractions;
    using DotPulsar.Exceptions;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public class PartitionedConsumer : IConsumer
    {
        private int _isDisposed;

        private PartitionedTopicMetadata _partitionedTopicMetadata;

        private readonly object _statusLock = new object();

        private readonly ConsumerOptions _options;
        private readonly BatchHandler _batchHandler;
        private readonly PulsarClient _pulsarClient;
        private readonly StateManager<ConsumerState> _state;
        private readonly ConcurrentDictionary<int, ConsumerUsageData> _consumers;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public PartitionedConsumer(string topic,
                                   StateManager<ConsumerState> state,
                                   ConsumerOptions options,
                                   PartitionedTopicMetadata partitionedTopicMetadata,
                                   Dictionary<int, Consumer> consumers,
                                   BatchHandler batchHandler,
                                   PulsarClient pulsarClient)
        {
            Topic = topic;

            _state = state;
            _options = options;
            _batchHandler = batchHandler;
            _pulsarClient = pulsarClient;
            _partitionedTopicMetadata = partitionedTopicMetadata;

            _consumers = new ConcurrentDictionary<int, ConsumerUsageData>(consumers.Select(x => new KeyValuePair<int, ConsumerUsageData>(x.Key, new ConsumerUsageData(x.Value))));

            _cancellationTokenSource = new CancellationTokenSource();

            foreach (KeyValuePair<int, ConsumerUsageData> consumerUsageDataWithPartitionKey in _consumers)
            {
                _ = MonitorState(consumerUsageDataWithPartitionKey.Value.Consumer, consumerUsageDataWithPartitionKey.Key, _cancellationTokenSource.Token);
            }

            _ = UpdatePartitionMetadata();
        }

        #region IConsumer

        public string Topic { get; }

        public ValueTask Acknowledge(Message message, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return GetConsumer(message.MessageId.Partition).Acknowledge(message, cancellationToken);
        }

        public ValueTask Acknowledge(MessageId messageId, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return GetConsumer(messageId.Partition).Acknowledge(messageId, cancellationToken);
        }

        public ValueTask AcknowledgeCumulative(Message message, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return GetConsumer(message.MessageId.Partition).AcknowledgeCumulative(message, cancellationToken);
        }

        public ValueTask AcknowledgeCumulative(MessageId messageId, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return GetConsumer(messageId.Partition).AcknowledgeCumulative(messageId, cancellationToken);
        }

        public ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken = default)
        {
            // TODO
            // TODO
            // TODO for now this method is not supported. Java has special MultiMessageId class. But in Java they don't use MessageId properties
            // TODO in the same way it is done in C#
            // TODO
            // TODO
            throw new NotSupportedException();
        }

        public bool IsFinalState()
        {
            ThrowIfDisposed();

            return _state.IsFinalState();
        }

        public bool IsFinalState(ConsumerState state)
        {
            ThrowIfDisposed();

            return _state.IsFinalState(state);
        }

        public async IAsyncEnumerable<Message> Messages([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            List<Task<Message>> pendingConsumers = new List<Task<Message>>(_consumers.Count);

            // it is safe to abort this method at any point with exception
            // as messages remaining in the queue weren't returned to the caller
            // so they weren't acknowledged. next time caller restarts consuming messages
            // all un-acknowledged messages will be loaded again.

            for (;;)
            {
                bool waitForTasksToComplete = true;

                foreach (ConsumerUsageData consumerUsageData in _consumers.Values)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Consumer consumer = consumerUsageData.Consumer;

                    if (consumerUsageData.PendingTask.HasValue)
                    {
                        // we end up here only when long running Task<T> finished execution

                        if (consumerUsageData.PendingTask.Value.IsCompleted)
                        {
                            if (!consumerUsageData.PendingTask.Value.IsFaulted &&
                                !consumerUsageData.PendingTask.Value.IsCanceled)
                            {
                                Message message = consumerUsageData.PendingTask.Value.Result;
                                yield return message;
                            }

                            // we want to return one message per partition in each iteration
                            // so we consume messages as evenly as possible
                            // (also we want to avoid waiting on Task<T> as much as possible
                            // see branch below when consumerUsageData.PendingTask == false)

                            consumerUsageData.PendingTask = null;

                            // as at least one task completed without wait we don't need to wait
                            // and check result again in case await isn't needed (converting to Task<T> is
                            // an expensive operation)

                            waitForTasksToComplete = false;
                        }
                        else
                        {
                            // this partition consumer has Task<T> which hasn't completed yet
                            // so skip it
                        }
                    }
                    else
                    {
                        // receive next message

                        consumerUsageData.PendingTask = consumer.Receive(cancellationToken);

                        // ValueTask might complete synchronously and no waiting is needed
                        // so we need to check if it's completed

                        if (consumerUsageData.PendingTask.Value.IsCompleted)
                        {
                            if (!consumerUsageData.PendingTask.Value.IsFaulted &&
                                !consumerUsageData.PendingTask.Value.IsCanceled)
                            {
                                Message message = consumerUsageData.PendingTask.Value.Result;
                                yield return message;
                            }

                            // we want to return one message per partition in each iteration
                            // so we consume messages as evenly as possible
                            // (also we want to avoid waiting on Task<T> as much as possible)

                            consumerUsageData.PendingTask = null;

                            // as at least one task completed without wait we don't need to wait
                            // and check result again in case await isn't needed (converting to Task<T> is
                            // an expensive operation)

                            waitForTasksToComplete = false;
                        }
                        else
                        {
                            // sadly we encountered long operation and we need to wait on it
                            // using Task.WhenAny

                            pendingConsumers.Add(consumerUsageData.PendingTask.Value.AsTask());
                        }
                    }
                }

                if (waitForTasksToComplete)
                {
                    await Task.WhenAny(pendingConsumers)
                              .ConfigureAwait(false);
                }

                pendingConsumers.RemoveAll(x => x.IsCompleted);
            }
        }

        public async ValueTask RedeliverUnacknowledgedMessages(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            foreach (var messageIdsByPartitionIndex in messageIds.GroupBy(x => x.Partition))
            {
                await GetConsumer(messageIdsByPartitionIndex.Key).RedeliverUnacknowledgedMessages(messageIdsByPartitionIndex, cancellationToken)
                                                                 .ConfigureAwait(false);
            }
        }

        public async ValueTask RedeliverUnacknowledgedMessages(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            foreach (ConsumerUsageData consumerUsageData in _consumers.Values)
            {
                await consumerUsageData.Consumer.RedeliverUnacknowledgedMessages(cancellationToken)
                                                .ConfigureAwait(false);
            }
        }

        public ValueTask Seek(MessageId messageId, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Seek operation is not supported for partitioned topics");

        public async ValueTask<ConsumerStateChanged> StateChangedFrom(ConsumerState state, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            var newState = await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);
            return new ConsumerStateChanged(this, newState);
        }

        public async ValueTask<ConsumerStateChanged> StateChangedTo(ConsumerState state, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            var newState = await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);
            return new ConsumerStateChanged(this, newState);
        }

        public async ValueTask Unsubscribe(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            foreach (ConsumerUsageData consumerUsageData in _consumers.Values)
            {
                await consumerUsageData.Consumer.Unsubscribe(cancellationToken).ConfigureAwait(false);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            foreach (ConsumerUsageData consumerUsageData in _consumers.Values)
            {
                await consumerUsageData.Consumer.DisposeAsync().ConfigureAwait(false);
            }
        }

        #endregion

        private async Task MonitorState(Consumer consumer, int partitionIndex, CancellationToken cancellationToken)
        {
            await Task.Yield();

            var state = ConsumerState.Disconnected;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var stateChanged = await consumer.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);
                    state = stateChanged.ConsumerState;

                    if (IsFinalState(state))
                    {
                        _state.SetState(state);
                        _cancellationTokenSource.Cancel(); // cancel other monitor tasks
                    }
                    else
                    {
                        lock (_statusLock)
                        {
                            if (_consumers.TryGetValue(partitionIndex, out ConsumerUsageData consumerUsageData))
                            {
                                consumerUsageData.IsActive = state == ConsumerState.Active;
                            }
                            _state.SetState(_consumers.Any(x => !x.Value.IsActive) ? ConsumerState.Disconnected : ConsumerState.Active);
                        }
                    }

                    if (consumer.IsFinalState(state))
                        return;
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task UpdatePartitionMetadata()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            for (;;)
            {
                try
                {
                    await Task.Delay(_options.AutoUpdatePartitionsInterval, cancellationToken);

                    var newPartitionedTopicMetadata = await _pulsarClient.GetPartitionTopicMetadata(Topic, cancellationToken)
                                                                         .ConfigureAwait(false);

                    // Pulsar doesn't support removing parttions at the moment

                    if (newPartitionedTopicMetadata.Partitions > _partitionedTopicMetadata.Partitions)
                    {
                        uint prefetchCount = Math.Max(1U, (uint) (_options.MessagePrefetchCount / newPartitionedTopicMetadata.Partitions));

                        // need to update existing consumers prefetch count to avoid using too much memory
                        // update will happen on next flow command execution though

                        foreach (ConsumerUsageData consumerUsageData in _consumers.Values)
                        {
                            consumerUsageData.Consumer.UpdateMessagePrefetchCount(prefetchCount, cancellationToken);
                        }

                        // construct new consumers

                        for (int partitionIndex = _consumers.Count; partitionIndex < newPartitionedTopicMetadata.Partitions; partitionIndex++)
                        {
                            var partitionTopicConsumerOptions = new ConsumerOptions(_options, $"{_options.Topic}-partition-{partitionIndex}", prefetchCount);
                            Consumer partitionTopicConsumer = _pulsarClient.CreateConsumerWithoutCheckingPartition(_batchHandler, partitionTopicConsumerOptions);

                            // it is safe to have partial failure as we only add new partition consumers and we add them
                            // in order (increasing partitionIndex)
                            // if exception is thrown then next time we will try add more partition consumers

                            if (_consumers.TryAdd(partitionIndex, new ConsumerUsageData(partitionTopicConsumer)))
                            {
                                _ = MonitorState(partitionTopicConsumer, partitionIndex, _cancellationTokenSource.Token);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // as this is a monitor task which is being excuted on timer we ignore all exceptions
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new ConsumerDisposedException();
        }

        private Consumer GetConsumer(int partitionId)
        {
            ThrowIfDisposed();

            if (_consumers.TryGetValue(partitionId, out ConsumerUsageData result))
            {
                return result.Consumer;
            }
            else
            {
                throw new ArgumentException($"Illegal partition index in received message: ${partitionId}");
            }
        }
    }
}
