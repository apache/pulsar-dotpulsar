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

public sealed class ConsumerProcess : Process
{
    private readonly IStateManager<ConsumerState> _stateManager;
    private readonly IContainsChannel _subConsumer;
    private readonly bool _isFailoverSubscription;

    public ConsumerProcess(
        Guid correlationId,
        IStateManager<ConsumerState> stateManager,
        IContainsChannel consumer,
        bool isFailoverSubscription) : base(correlationId)
    {
        _stateManager = stateManager;
        _subConsumer = consumer;
        _isFailoverSubscription = isFailoverSubscription;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync().ConfigureAwait(false);
        _stateManager.SetState(ConsumerState.Closed);
    }

    protected override void CalculateState()
    {
        if (_stateManager.IsFinalState())
            return;

        if (ExecutorState == ExecutorState.Faulted)
        {
            var formerState = _stateManager.SetState(ConsumerState.Faulted);
            if (formerState != ConsumerState.Faulted)
                ActionQueue.Enqueue(async _ => await _subConsumer.ChannelFaulted(Exception!).ConfigureAwait(false));
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
                ActionQueue.Enqueue(async x =>
                {
                    await _subConsumer.CloseChannel(x).ConfigureAwait(false);
                    await _subConsumer.EstablishNewChannel(x).ConfigureAwait(false);
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
}
