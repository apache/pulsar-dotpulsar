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
    using DotPulsar.Internal.Abstractions;
    using System;
    using System.Buffers;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class PartitionedProducer : IProducer
    {
        private readonly ConcurrentDictionary<int, IProducer> _producers;
        private readonly StateManager<ProducerState> _state;
        private PartitionedTopicMetadata _partitionedTopicMetadata;
        private int _isDisposed;
        private CancellationTokenSource _cancellationTokenSource;
        private int _connectedProducerCount = 0;
        private ReaderWriterLockSlim _metadataLock = new ReaderWriterLockSlim();
        private IMessageRouter _messageRouter;
        private PulsarClient _client;
        private ProducerOptions _options;

        public string Topic { get; }

        public PartitionedProducer(
            string topic,
            StateManager<ProducerState> state,
            ProducerOptions options,
            PartitionedTopicMetadata partitionedTopicMetadata,
            Dictionary<int, IProducer> producers,
            IMessageRouter messageRouter,
            PulsarClient client)
        {
            Topic = topic;
            _state = state;
            _partitionedTopicMetadata = partitionedTopicMetadata;
            _producers = new ConcurrentDictionary<int, IProducer>(producers);
            _isDisposed = 0;
            _messageRouter = messageRouter;
            _client = client;
            _options = options;

            _cancellationTokenSource = new CancellationTokenSource();

            foreach (var producer in _producers.Values)
            {
                _ = MonitorState(producer, null, _cancellationTokenSource.Token);
            }

            _ = UpdatePartitionMetadata();
        }

        private async Task MonitorState(IProducer producer, ProducerState? initialState, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            var state = initialState ?? ProducerState.Disconnected;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var stateChanged = await producer.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);
                    state = stateChanged.ProducerState;

                    switch (state)
                    {
                        case ProducerState.Disconnected:
                            Interlocked.Decrement(ref _connectedProducerCount);
                            _state.SetState(ProducerState.Disconnected);
                            break;
                        case ProducerState.Faulted:
                            Interlocked.Decrement(ref _connectedProducerCount);
                            _state.SetState(ProducerState.Faulted);
                            break;
                        case ProducerState.Closed:
                            Interlocked.Decrement(ref _connectedProducerCount);
                            _state.SetState(ProducerState.Closed);
                            break;
                        case ProducerState.Connected:
                            Interlocked.Increment(ref _connectedProducerCount);
                            break;
                    }

                    _metadataLock.EnterReadLock();
                    if (_connectedProducerCount == _partitionedTopicMetadata.Partitions)
                        _state.SetState(ProducerState.Connected);
                    _metadataLock.ExitReadLock();

                    if (IsFinalState(state))
                        _cancellationTokenSource.Cancel(); // cancel other monitor tasks

                    if (producer.IsFinalState(state))
                        return;
                }
            }
            catch (OperationCanceledException)
            { }
        }

        private async Task UpdatePartitionMetadata()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            for (;;)
            {
                try
                {
                    await Task.Delay(_options.AutoUpdatePartitionsInterval);
                    var newMetadata = await _client.GetPartitionTopicMetadata(Topic, cancellationToken).ConfigureAwait(false);
                    // Not support shrink topic partitions
                    if (newMetadata.Partitions > _partitionedTopicMetadata.Partitions)
                    {
                        int newProducersCount = newMetadata.Partitions - _partitionedTopicMetadata.Partitions;
                        var producers = new ConcurrentDictionary<int, IProducer>(Environment.ProcessorCount, newProducersCount);
                        var newSubproducerTasks = new List<Task>(newProducersCount);
                        for (int i = _partitionedTopicMetadata.Partitions; i < newMetadata.Partitions; i++)
                        {
                            int partID = i;
                            newSubproducerTasks.Add(Task.Run(async () =>
                            {
                                var subproducerOption = _options.Clone() as ProducerOptions;
                                subproducerOption!.Topic = $"{_options.Topic}-partition-{partID}";
                                var producer = _client.CreateProducerWithoutCheckingPartition(subproducerOption);
                                producers[partID] = producer;
                                _ = await producer.StateChangedTo(ProducerState.Connected).ConfigureAwait(false);
                            }));
                        }
                        try
                        {
                            await Task.WhenAll(newSubproducerTasks.ToArray()).ConfigureAwait(false);
                            foreach (var p in producers)
                            {
                                _producers[p.Key] = p.Value;
                                _ = MonitorState(p.Value, ProducerState.Connected, cancellationToken);
                            }

                            _metadataLock.EnterWriteLock();
                            _partitionedTopicMetadata = newMetadata;
                            Interlocked.Add(ref _connectedProducerCount, newProducersCount);
                            _metadataLock.ExitWriteLock();
                        }
                        catch
                        {
                            foreach (var producer in producers.Values)
                                await producer.DisposeAsync().ConfigureAwait(false);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                }
            }
        }

        private IProducer GetProducer(MessageMetadata? message = null)
        {
            int partitionIndex = _messageRouter.ChoosePartition(message, _partitionedTopicMetadata);
            if (partitionIndex < 0 || partitionIndex >= _partitionedTopicMetadata.Partitions)
            {
                throw new ArgumentException($"Illegal partition index chosen by the message routing policy: ${partitionIndex}");
            }

            return _producers[partitionIndex];
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            foreach (var producer in _producers.Values)
            {
                await producer.DisposeAsync().ConfigureAwait(false);
            }

            _metadataLock.Dispose();

            _state.SetState(ProducerState.Closed);
        }

        public bool IsFinalState()
            => _state.IsFinalState();

        public bool IsFinalState(ProducerState state)
            => _state.IsFinalState(state);

        public ValueTask<MessageId> Send(byte[] data, CancellationToken cancellationToken)
            => GetProducer(null).Send(new ReadOnlySequence<byte>(data), cancellationToken);

        public ValueTask<MessageId> Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            => GetProducer(null).Send(new ReadOnlySequence<byte>(data), cancellationToken);

        public ValueTask<MessageId> Send(ReadOnlySequence<byte> data, CancellationToken cancellationToken)
            => GetProducer(null).Send(data, cancellationToken);

        public ValueTask<MessageId> Send(MessageMetadata metadata, byte[] data, CancellationToken cancellationToken)
            => GetProducer(metadata).Send(metadata, new ReadOnlySequence<byte>(data), cancellationToken);

        public ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            => GetProducer(metadata).Send(metadata, new ReadOnlySequence<byte>(data), cancellationToken);

        public ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlySequence<byte> data, CancellationToken cancellationToken)
            => GetProducer(metadata).Send(metadata, data, cancellationToken);

        public async ValueTask<ProducerStateChanged> StateChangedFrom(ProducerState state, CancellationToken cancellationToken = default)
        {
            var newState = await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);
            return new ProducerStateChanged(this, newState);
        }

        public async ValueTask<ProducerStateChanged> StateChangedTo(ProducerState state, CancellationToken cancellationToken = default)
        {
            var newState = await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);
            return new ProducerStateChanged(this, newState);
        }
    }
}
