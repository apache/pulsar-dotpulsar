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

    public class PartitionedProducerProcess : IProcess
    {
        private readonly IStateManager<ProducerState> _stateManager;

        private int _partitionsCount;
        private int _connectedProducersCount = 0;

        public PartitionedProducerProcess(Guid correlationId,
            IStateManager<ProducerState> stateManager,
            int partitionsCount)
        {
            CorrelationId = correlationId;
            _stateManager = stateManager;
            _partitionsCount = partitionsCount;
        }

        public ValueTask DisposeAsync()
            => throw new NotImplementedException();

        public Guid CorrelationId { get; }

        public void Start()
            => throw new NotImplementedException();

        public void Handle(IEvent e)
        {
            if (_stateManager.IsFinalState())
                return;

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
                case ProducerDisposed:
                    _stateManager.SetState(ProducerState.Closed);
                    break;
            }

            if (_stateManager.IsFinalState())
                return;

            if (_connectedProducersCount == _partitionsCount)
                _stateManager.SetState(ProducerState.Connected);
            else if (_connectedProducersCount == 0)
                _stateManager.SetState(ProducerState.Disconnected);
            else
                _stateManager.SetState(ProducerState.PartiallyConnected);
        }
    }
}
