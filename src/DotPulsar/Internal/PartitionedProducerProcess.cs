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
    using DotPulsar.Abstractions;
    using Events;
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class PartitionedProducerProcess : IProcess
    {
        private readonly IStateManager<PartitionedProducerState> _stateManager;

        /// <summary>
        /// The producers group is shared between PartitionedProducerProcess and PartitionedProducer.
        /// </summary>
        private readonly ConcurrentDictionary<uint, IProducer> _producersGroup;

        private uint _connectedProducersCount = 0;

        public PartitionedProducerProcess(Guid correlationId, IStateManager<PartitionedProducerState> stateManager, ConcurrentDictionary<uint, IProducer> producersGroup)
        {
            CorrelationId = correlationId;
            _stateManager = stateManager;
            _producersGroup = producersGroup;
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

            if (e is PartitionedSubProducerStateChanged stateChanged)
            {
                switch (stateChanged.ProducerState)
                {
                    case ProducerState.Closed:
                        _stateManager.SetState(PartitionedProducerState.Closed);
                        break;
                    case ProducerState.Connected:
                        _connectedProducersCount++;
                        break;
                    case ProducerState.Disconnected:
                        _connectedProducersCount--;
                        break;
                    case ProducerState.Faulted:
                        _stateManager.SetState(PartitionedProducerState.Faulted);
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            if (_stateManager.IsFinalState())
                return;

            if (_connectedProducersCount == _producersGroup.Count)
                _stateManager.SetState(PartitionedProducerState.Connected);
            else if (_connectedProducersCount == 0)
                _stateManager.SetState(PartitionedProducerState.Disconnected);
            else
                _stateManager.SetState(PartitionedProducerState.PartiallyConnected);
        }
    }
}
