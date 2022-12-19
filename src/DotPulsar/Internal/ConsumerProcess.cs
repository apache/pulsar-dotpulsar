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

using Abstractions;
using System;
using System.Threading.Tasks;

public sealed class ConsumerProcess : Process
{
    private readonly IStateManager<ConsumerState> _stateManager;
    private readonly IContainsChannel _consumer;
    private readonly bool _isFailoverSubscription;
    private Task? _establishNewChannelTask;

    public ConsumerProcess(
        Guid correlationId,
        IStateManager<ConsumerState> stateManager,
        IContainsChannel consumer,
        bool isFailoverSubscription) : base(correlationId)
    {
        _stateManager = stateManager;
        _consumer = consumer;
        _isFailoverSubscription = isFailoverSubscription;
    }

    public override async ValueTask DisposeAsync()
    {
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
            _stateManager.SetState(ConsumerState.Faulted);
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
                EstablishNewChannel();
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

    private void EstablishNewChannel()
    {
        var token = CancellationTokenSource.Token;
        if (_establishNewChannelTask is null || _establishNewChannelTask.IsCompleted)
            _establishNewChannelTask = Task.Run(() => _consumer.EstablishNewChannel(token).ConfigureAwait(false), token);
    }
}
