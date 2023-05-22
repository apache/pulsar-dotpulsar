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

namespace DotPulsar.Internal;

using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using System;
using System.Threading.Tasks;

public sealed class ProducerProcess : Process
{
    private readonly IStateManager<ProducerState> _stateManager;
    private readonly IContainsProducerChannel _producer;

    public ProducerProcess(
        Guid correlationId,
        IStateManager<ProducerState> stateManager,
        IContainsProducerChannel producer) : base(correlationId)
    {
        _stateManager = stateManager;
        _producer = producer;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync().ConfigureAwait(false);
        _stateManager.SetState(ProducerState.Closed);
        CancellationTokenSource.Cancel();
        await _producer.DisposeAsync().ConfigureAwait(false);
    }

    protected override void CalculateState()
    {
        if (_stateManager.IsFinalState())
            return;

        if (ExecutorState == ExecutorState.Faulted)
        {
            ProducerState newState = Exception! is ProducerFencedException ? ProducerState.Fenced : ProducerState.Faulted;
            var formerState = _stateManager.SetState(newState);
            if (formerState != ProducerState.Faulted && formerState != ProducerState.Fenced)
                ActionQueue.Enqueue(async _ => await _producer.ChannelFaulted(Exception!));
            return;
        }

        switch (ChannelState)
        {
            case ChannelState.ClosedByServer:
            case ChannelState.Disconnected:
                _stateManager.SetState(ProducerState.Disconnected);
                ActionQueue.Enqueue(async x =>
                {
                    await _producer.CloseChannel(x).ConfigureAwait(false);
                    await _producer.EstablishNewChannel(x).ConfigureAwait(false);
                });
                return;
            case ChannelState.Connected:
                ActionQueue.Enqueue(async x =>
                {
                    await _producer.ActivateChannel(TopicEpoch, x).ConfigureAwait(false);
                    _stateManager.SetState(ProducerState.Connected);
                });
                return;
            case ChannelState.WaitingForExclusive:
                _stateManager.SetState(ProducerState.WaitingForExclusive);
                return;
        }
    }
}
