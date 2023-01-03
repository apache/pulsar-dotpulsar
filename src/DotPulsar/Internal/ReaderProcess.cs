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

public sealed class ReaderProcess : Process
{
    private readonly IStateManager<ReaderState> _stateManager;
    private readonly IContainsChannel _reader;
    private Task? _establishNewChannelTask;

    public ReaderProcess(
        Guid correlationId,
        IStateManager<ReaderState> stateManager,
        IContainsChannel reader) : base(correlationId)
    {
        _stateManager = stateManager;
        _reader = reader;
    }

    public override async ValueTask DisposeAsync()
    {
        _stateManager.SetState(ReaderState.Closed);
        CancellationTokenSource.Cancel();
        await _reader.DisposeAsync().ConfigureAwait(false);
    }

    protected override void CalculateState()
    {
        if (_stateManager.IsFinalState())
            return;

        if (ExecutorState == ExecutorState.Faulted)
        {
            _stateManager.SetState(ReaderState.Faulted);
            return;
        }

        switch (ChannelState)
        {
            case ChannelState.ClosedByServer:
            case ChannelState.Disconnected:
                _stateManager.SetState(ReaderState.Disconnected);
                EstablishNewChannel();
                return;
            case ChannelState.Connected:
                _stateManager.SetState(ReaderState.Connected);
                return;
            case ChannelState.ReachedEndOfTopic:
                _stateManager.SetState(ReaderState.ReachedEndOfTopic);
                return;
        }
    }

    private void EstablishNewChannel()
    {
        var token = CancellationTokenSource.Token;
        if (_establishNewChannelTask is null || _establishNewChannelTask.IsCompleted)
            _establishNewChannelTask = Task.Run(() => _reader.EstablishNewChannel(token).ConfigureAwait(false), token);
    }
}
