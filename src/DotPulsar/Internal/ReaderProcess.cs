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

public sealed class ReaderProcess : Process
{
    private readonly IStateManager<ReaderState> _stateManager;
    private readonly IContainsChannel _reader;
    private readonly AsyncQueue<Func<CancellationToken, Task>> _actionQueue;
    private readonly Task _actionProcessorTask;

    public ReaderProcess(
        Guid correlationId,
        IStateManager<ReaderState> stateManager,
        IContainsChannel reader) : base(correlationId)
    {
        _stateManager = stateManager;
        _reader = reader;
        _actionQueue = new AsyncQueue<Func<CancellationToken, Task>>();
        _actionProcessorTask = ProcessActions(CancellationTokenSource.Token);
    }

    public override async ValueTask DisposeAsync()
    {
        await _actionProcessorTask.ConfigureAwait(false);
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
            var formerState = _stateManager.SetState(ReaderState.Faulted);
            if (formerState != ReaderState.Faulted)
                _actionQueue.Enqueue(async _ => await _reader.ChannelFaulted(Exception!) );
            return;
        }

        switch (ChannelState)
        {
            case ChannelState.ClosedByServer:
            case ChannelState.Disconnected:
                _stateManager.SetState(ReaderState.Disconnected);
                _actionQueue.Enqueue(async x =>
                {
                    await _reader.CloseChannel(x).ConfigureAwait(false);
                    await _reader.EstablishNewChannel(x).ConfigureAwait(false);
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
