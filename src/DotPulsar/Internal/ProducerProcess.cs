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
        private readonly IEstablishNewChannel? _producer;
        private readonly bool _isPartitionedProducer;

        // The following variables are only used when the producer is PartitionedProducer.
        private readonly IRegisterEvent _processManager;
        private int _partitionsCount;
        private int _connectedProducersCount = 0;

        // The following variables are only used for sub producer
        private readonly Guid? _partitionedProducerId;
        private readonly uint? _partitionIndex;

        public ProducerProcess(
            Guid correlationId,
            IStateManager<ProducerState> stateManager,
            IEstablishNewChannel? producer,
            IRegisterEvent processManager,
            Guid? partitionedProducerId = null,
            uint? partitionIndex = null,
            bool isPartitionedProducer = false,
            int partitionsCount = 1) : base(correlationId)
        {
            _stateManager = stateManager;
            _producer = producer;
            _processManager = processManager;
            _isPartitionedProducer = isPartitionedProducer;
            _partitionsCount = partitionsCount;
            _partitionedProducerId = partitionedProducerId;
            _partitionIndex = partitionIndex;
        }

        public override async ValueTask DisposeAsync()
        {
            SetState(ProducerState.Closed);
            CancellationTokenSource.Cancel();

            if (_producer != null)
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
                            Interlocked.Decrement(ref _connectedProducersCount);
                            break;
                        case ProducerState.Faulted:
                            _stateManager.SetState(ProducerState.Faulted);
                            break;
                        case ProducerState.PartiallyConnected: break;
                        default: throw new ArgumentOutOfRangeException();
                    }

                    break;
                case UpdatePartitions updatePartitions:
                    _partitionsCount = (int) updatePartitions.PartitionsCount;
                    break;
            }
        }

        protected override void CalculateState()
        {
            if (_stateManager.IsFinalState())
                return;

            if (_isPartitionedProducer)
            {
                if (_connectedProducersCount == _partitionsCount)
                    _stateManager.SetState(ProducerState.Connected);
                else if (_connectedProducersCount == 0)
                    _stateManager.SetState(ProducerState.Disconnected);
                else
                    _stateManager.SetState(ProducerState.PartiallyConnected);
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
                    _ = _producer!.EstablishNewChannel(CancellationTokenSource.Token);
                    return;
                case ChannelState.Connected:
                    SetState(ProducerState.Connected);
                    return;
            }
        }

        private void SetState(ProducerState state)
        {
            _stateManager.SetState(state);

            // Check if this is the sub producer process of the partitioned producer.
            if (_partitionedProducerId.HasValue && _partitionIndex.HasValue)
                _processManager.Register(new PartitionedSubProducerStateChanged(_partitionedProducerId.Value, state, _partitionIndex.Value));
        }
    }
}
