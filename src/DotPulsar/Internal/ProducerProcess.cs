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
    using System;
    using System.Threading.Tasks;

    public sealed class ProducerProcess : Process
    {
        private readonly IStateManager<ProducerState> _stateManager;
        private readonly IProducerChannelFactory _factory;
        private readonly Producer _producer;

        public ProducerProcess(
            Guid correlationId,
            IStateManager<ProducerState> stateManager,
            IProducerChannelFactory factory,
            Producer producer) : base(correlationId)
        {
            _stateManager = stateManager;
            _factory = factory;
            _producer = producer;
        }

        public override async ValueTask DisposeAsync()
        {
            _stateManager.SetState(ProducerState.Closed);
            CancellationTokenSource.Cancel();
            await _producer.DisposeAsync().ConfigureAwait(false);
        }

        protected override void CalculateState()
        {
            if (_producer.IsFinalState())
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
                    _stateManager.SetState(ProducerState.Disconnected);
                    SetupChannel();
                    return;
                case ChannelState.Connected:
                    _stateManager.SetState(ProducerState.Connected);
                    return;
            }
        }

        private async void SetupChannel()
        {
            try
            {
                var channel = await _factory.Create(CancellationTokenSource.Token).ConfigureAwait(false);
                await _producer.SetChannel(channel).ConfigureAwait(false);
            }
            catch
            {
                // ignored
            }
        }
    }
}
