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
    using System.Threading.Tasks;

    public sealed class ProducerProcess : Process
    {
        private readonly IStateManager<ProducerState> _stateManager;
        private readonly IEstablishNewChannel _producer;

        // The following variables are only used when producing in partitioned topic.
        private readonly IRegisterEvent _processManager;
        private readonly Guid? _partitionedProducerId;
        private readonly uint? _partitionIndex;

        public ProducerProcess(
            Guid correlationId,
            IStateManager<ProducerState> stateManager,
            IEstablishNewChannel producer,
            IRegisterEvent processManager = null!,
            Guid? partitionedProducerId = null,
            uint? partitionIndex = null) : base(correlationId)
        {
            _stateManager = stateManager;
            _producer = producer;
            _processManager = processManager;
            _partitionedProducerId = partitionedProducerId;
            _partitionIndex = partitionIndex;
        }

        public override async ValueTask DisposeAsync()
        {
            SetState(ProducerState.Closed);
            CancellationTokenSource.Cancel();
            await _producer.DisposeAsync().ConfigureAwait(false);
        }

        protected override void CalculateState()
        {
            if (_stateManager.IsFinalState())
                return;

            if (ExecutorState == ExecutorState.Faulted)
            {
                _stateManager.SetState(ProducerState.Faulted);
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

        private void SetState(ProducerState state)
        {
            _stateManager.SetState(state);

            if (_partitionedProducerId.HasValue && _partitionIndex.HasValue)
                _processManager.Register(new PartitionedSubProducerStateChanged(_partitionedProducerId.Value, state, _partitionIndex.Value));
        }
    }
}
