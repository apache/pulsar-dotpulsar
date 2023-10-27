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

using DotPulsar.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public sealed class PingPongHandler : IState<PingPongHandlerState>, IAsyncDisposable
{
    private readonly CancellationTokenSource _cts;
    private readonly StateManager<PingPongHandlerState> _stateManager;
    private readonly TimeSpan _keepAliveInterval;
    private long _lastCommand;
    private bool _waitForPong;

    public PingPongHandler(TimeSpan keepAliveInterval)
    {
        _cts = new CancellationTokenSource();
        _stateManager = new StateManager<PingPongHandlerState>(PingPongHandlerState.Active, PingPongHandlerState.TimedOut, PingPongHandlerState.Closed);
        _keepAliveInterval = keepAliveInterval;
        _lastCommand = Stopwatch.GetTimestamp();
        _ = Task.Factory.StartNew(() => Watch());
    }

    public void Incoming(BaseCommand.Type _)
    {
        Interlocked.Exchange(ref _lastCommand, Stopwatch.GetTimestamp());
        _waitForPong = false;
    }

    private TimeSpan GetElapsedTimeSinceLastCommand()
    {
        var lastCommand = Interlocked.Read(ref _lastCommand);
        var now = Stopwatch.GetTimestamp();
        var elapsedTicks = now - lastCommand;
        if (elapsedTicks > 0)
            return TimeSpan.FromSeconds(elapsedTicks / Stopwatch.Frequency);
        return TimeSpan.Zero;
    }

    private async void Watch()
    {
        var waitFor = _keepAliveInterval;

        while (!_cts.IsCancellationRequested)
        {
            await Task.Delay(waitFor).ConfigureAwait(false);
            var elapsed = GetElapsedTimeSinceLastCommand();
            if (elapsed > _keepAliveInterval)
            {
                if (_waitForPong)
                {
                    _stateManager.SetState(PingPongHandlerState.TimedOut);
                    return;
                }

                _waitForPong = true;
                _stateManager.SetState(PingPongHandlerState.ThresholdExceeded);
                waitFor = _keepAliveInterval;
            }
            else
            {
                _stateManager.SetState(PingPongHandlerState.Active);
                waitFor = _keepAliveInterval.Subtract(elapsed);
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _stateManager.SetState(PingPongHandlerState.Closed);
        return new ValueTask();
    }

    public bool IsFinalState() => _stateManager.IsFinalState();

    public bool IsFinalState(PingPongHandlerState state) => _stateManager.IsFinalState(state);

    public async ValueTask<PingPongHandlerState> OnStateChangeTo(PingPongHandlerState state, CancellationToken cancellationToken = default)
        => await _stateManager.StateChangedTo(state, cancellationToken).ConfigureAwait(false);

    public async ValueTask<PingPongHandlerState> OnStateChangeFrom(PingPongHandlerState state, CancellationToken cancellationToken = default)
        => await _stateManager.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);
}
