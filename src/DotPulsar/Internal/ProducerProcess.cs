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
    using Events;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ProducerProcess : Process
    {
        private readonly IStateManager<ProducerState> _stateManager;
        private readonly IEstablishNewChannel _producer;

        // The following variables are only used when this is the process for parent producer.
        private readonly IRegisterEvent _processManager;
        private int _partitionsCount;
        private int _connectedProducersCount;
        private int _initialProducersCount;

        // The following variables are only used for sub producer
        private readonly Guid? _partitionedProducerId;

        public ProducerProcess(
            Guid correlationId,
            IStateManager<ProducerState> stateManager,
            IEstablishNewChannel producer,
            IRegisterEvent processManager,
            Guid? partitionedProducerId = null) : base(correlationId)
        {
            _stateManager = stateManager;
            _producer = producer;
            _processManager = processManager;
            _partitionedProducerId = partitionedProducerId;
        }

        public override async ValueTask DisposeAsync()
        {
            SetState(ProducerState.Closed);
            CancellationTokenSource.Cancel();

            await _producer.DisposeAsync().ConfigureAwait(false);
        }

        protected override void HandleExtend(IEvent e)
        {
            switch (e)
            {
                case PartitionedSubProducerStateChanged stateChanged:
                    switch (stateChanged.ProducerState)
                    {
                        case ProducerState.Closed:
                            _stateManager.SetState(ProducerState.Closed);
                            break;
                        case ProducerState.Connected:
                            Interlocked.Increment(ref _connectedProducersCount);
                            break;
                        case ProducerState.Disconnected:
                            // When the sub producer is initialized, the Disconnected event will be triggered.
                            // So we need to first subtract from _initialProducersCount.
                            if (_initialProducersCount == 0)
                                Interlocked.Decrement(ref _connectedProducersCount);
                            else
                                Interlocked.Decrement(ref _initialProducersCount);
                            break;
                        case ProducerState.Faulted:
                            _stateManager.SetState(ProducerState.Faulted);
                            break;
                        case ProducerState.PartiallyConnected: break;
                        default: throw new ArgumentOutOfRangeException();
                    }

                    break;
                case UpdatePartitions updatePartitions:
                    if (updatePartitions.PartitionsCount == 0)
                    {
                        Interlocked.Exchange(ref _initialProducersCount, 1);
                        Interlocked.Exchange(ref _partitionsCount, 1);
                    }
                    else
                    {
                        Interlocked.Add(ref _initialProducersCount, (int) updatePartitions.PartitionsCount - _partitionsCount);
                        Interlocked.Exchange(ref _partitionsCount, (int) updatePartitions.PartitionsCount);
                    }

                    break;
            }
        }

        protected override void CalculateState()
        {
            if (_stateManager.IsFinalState())
                return;

            if (!IsSubProducer()) // parent producer process
            {
                if (_connectedProducersCount <= 0)
                    _stateManager.SetState(ProducerState.Disconnected);
                else if (_connectedProducersCount == _partitionsCount)
                    _stateManager.SetState(ProducerState.Connected);
                else
                    _stateManager.SetState(ProducerState.PartiallyConnected);

                if (_partitionsCount == 0)
                    _ = _producer.EstablishNewChannel(CancellationTokenSource.Token);
                return;
            }

            if (ExecutorState == ExecutorState.Faulted)
            {
                SetState(ProducerState.Faulted);
                return;
            }

            switch (ChannelState)
            {
                case ChannelState.ClosedByServer:
                case ChannelState.Disconnected:
                    SetState(ProducerState.Disconnected);
                    _ = _producer.EstablishNewChannel(CancellationTokenSource.Token);
                    return;
                case ChannelState.Connected:
                    SetState(ProducerState.Connected);
                    return;
            }
        }

        /// <summary>
        /// Check if this is the sub producer process of the partitioned producer.
        /// </summary>
        private bool IsSubProducer()
            => _partitionedProducerId.HasValue;

        private void SetState(ProducerState state)
        {
            _stateManager.SetState(state);

            if (IsSubProducer())
                _processManager.Register(new PartitionedSubProducerStateChanged(_partitionedProducerId!.Value, state));
        }
    }
}
