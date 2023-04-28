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

using DotPulsar.Internal.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class ProducerProcess : Process
{
    private readonly IStateManager<ProducerState> _stateManager;
    private readonly IContainsProducerChannel _producer;
    private readonly AsyncQueue<Func<CancellationToken, Task>> _actionQueue;
    private readonly Task _actionProcessorTask;

    public ProducerProcess(
        Guid correlationId,
        IStateManager<ProducerState> stateManager,
        IContainsProducerChannel producer) : base(correlationId)
    {
        _stateManager = stateManager;
        _producer = producer;
        _actionQueue = new AsyncQueue<Func<CancellationToken, Task>>();
        _actionProcessorTask = ProcessActions(CancellationTokenSource.Token);
    }

    public override async ValueTask DisposeAsync()
    {
        await _actionProcessorTask.ConfigureAwait(false);
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
            var formerState = _stateManager.SetState(ProducerState.Faulted);
            if (formerState != ProducerState.Faulted)
                _actionQueue.Enqueue(async _ => await _producer.ChannelFaulted(Exception!));
            return;
        }

        switch (ChannelState)
        {
            case ChannelState.ClosedByServer:
            case ChannelState.Disconnected:
                _stateManager.SetState(ProducerState.Disconnected);
                _actionQueue.Enqueue(async x =>
                {
                    await _producer.CloseChannel(x).ConfigureAwait(false);
                    await _producer.EstablishNewChannel(x).ConfigureAwait(false);
                });
                return;
            case ChannelState.Connected:
                _actionQueue.Enqueue(async x =>
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

    private async Task ProcessActions(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var func = await _actionQueue.Dequeue(cancellationToken).ConfigureAwait(false);
                await func.Invoke(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }
        }
    }
}
