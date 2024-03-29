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

public sealed class ReaderProcess : Process
{
    private readonly IStateManager<ReaderState> _stateManager;
    private readonly IContainsChannel _subReader;

    public ReaderProcess(
        Guid correlationId,
        IStateManager<ReaderState> stateManager,
        IContainsChannel reader) : base(correlationId)
    {
        _stateManager = stateManager;
        _subReader = reader;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync().ConfigureAwait(false);
        _stateManager.SetState(ReaderState.Closed);
    }

    protected override void CalculateState()
    {
        if (_stateManager.IsFinalState())
            return;

        if (ExecutorState == ExecutorState.Faulted)
        {
            var formerState = _stateManager.SetState(ReaderState.Faulted);
            if (formerState != ReaderState.Faulted)
                ActionQueue.Enqueue(async _ => await _subReader.ChannelFaulted(Exception!).ConfigureAwait(false));
            return;
        }

        switch (ChannelState)
        {
            case ChannelState.ClosedByServer:
            case ChannelState.Disconnected:
                _stateManager.SetState(ReaderState.Disconnected);
                ActionQueue.Enqueue(async x =>
                {
                    await _subReader.CloseChannel(x).ConfigureAwait(false);
                    await _subReader.EstablishNewChannel(x).ConfigureAwait(false);
                });
                return;
            case ChannelState.Connected:
                _stateManager.SetState(ReaderState.Connected);
                return;
            case ChannelState.ReachedEndOfTopic:
                _stateManager.SetState(ReaderState.ReachedEndOfTopic);
                return;
        }
    }
}
