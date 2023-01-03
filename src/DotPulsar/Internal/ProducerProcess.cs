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
using System.Threading;
using System.Threading.Tasks;

public sealed class ProducerProcess : Process
{
    private readonly IStateManager<ProducerState> _stateManager;
    private readonly IContainsChannel _producer;
    private Task? _reEstablishChannelTask;

    public ProducerProcess(
        Guid correlationId,
        IStateManager<ProducerState> stateManager,
        IContainsChannel producer) : base(correlationId)
    {
        _stateManager = stateManager;
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
        if (_stateManager.IsFinalState())
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
            case ChannelState.WrongAckOrdering:
                _stateManager.SetState(ProducerState.Disconnected);
                ReestablishChannel(CancellationTokenSource.Token);
                return;
            case ChannelState.Connected:
                _stateManager.SetState(ProducerState.Connected);
                return;
        }
    }

    private void ReestablishChannel(CancellationToken token)
    {
        if (_reEstablishChannelTask is null || _reEstablishChannelTask.IsCompleted)
        {
            _reEstablishChannelTask = Task
                .Run(() => _producer.CloseChannel(token).ConfigureAwait(false), token)
                .ContinueWith(_ => _producer.EstablishNewChannel(token).ConfigureAwait(false), token);
        }
    }
}
