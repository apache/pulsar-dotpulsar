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

public sealed class ConsumerProcess : Process
{
    private readonly IStateManager<ConsumerState> _stateManager;
    private readonly IContainsChannel _consumer;
    private readonly AsyncQueue<Func<CancellationToken, Task>> _actionQueue;
    private readonly bool _isFailoverSubscription;
    private readonly Task _actionProcessorTask;

    public ConsumerProcess(
        Guid correlationId,
        IStateManager<ConsumerState> stateManager,
        IContainsChannel consumer,
        bool isFailoverSubscription) : base(correlationId)
    {
        _stateManager = stateManager;
        _consumer = consumer;
        _actionQueue = new AsyncQueue<Func<CancellationToken, Task>>();
        _isFailoverSubscription = isFailoverSubscription;
        _actionProcessorTask = ProcessActions(CancellationTokenSource.Token);
    }

    public override async ValueTask DisposeAsync()
    {
        await _actionProcessorTask.ConfigureAwait(false);
        _stateManager.SetState(ConsumerState.Closed);
        CancellationTokenSource.Cancel();
        await _consumer.DisposeAsync().ConfigureAwait(false);
    }

    protected override void CalculateState()
    {
        if (_stateManager.IsFinalState())
            return;

        if (ExecutorState == ExecutorState.Faulted)
        {
            var formerState = _stateManager.SetState(ConsumerState.Faulted);
            if (formerState != ConsumerState.Faulted)
                _actionQueue.Enqueue(async _ => await _consumer.ChannelFaulted(Exception!) );
            return;
        }

        switch (ChannelState)
        {
            case ChannelState.Active:
                _stateManager.SetState(ConsumerState.Active);
                return;
            case ChannelState.Inactive:
                _stateManager.SetState(ConsumerState.Inactive);
                return;
            case ChannelState.ClosedByServer:
            case ChannelState.Disconnected:
                _stateManager.SetState(ConsumerState.Disconnected);
                _actionQueue.Enqueue(async x =>
                {
                    await _consumer.CloseChannel(x).ConfigureAwait(false);
                    await _consumer.EstablishNewChannel(x).ConfigureAwait(false);
                });
                return;
            case ChannelState.Connected:
                if (!_isFailoverSubscription)
                    _stateManager.SetState(ConsumerState.Active);
                return;
            case ChannelState.ReachedEndOfTopic:
                _stateManager.SetState(ConsumerState.ReachedEndOfTopic);
                return;
            case ChannelState.Unsubscribed:
                _stateManager.SetState(ConsumerState.Unsubscribed);
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
