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

namespace DotPulsar.Internal.Abstractions;

using DotPulsar.Internal.Events;

public abstract class Process : IProcess
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    protected readonly AsyncQueue<Func<CancellationToken, Task>> ActionQueue;
    private Task? _actionProcessorTask;
    protected ChannelState ChannelState;
    protected ExecutorState ExecutorState;
    protected Exception? Exception;

    protected Process(Guid correlationId)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        ActionQueue = new AsyncQueue<Func<CancellationToken, Task>>();
        ChannelState = ChannelState.Disconnected;
        ExecutorState = ExecutorState.Ok;
        CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }

    public void Start()
    {
        _actionProcessorTask = ProcessActions(_cancellationTokenSource.Token);
        CalculateState();
    }

    public virtual async ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Cancel();
        if (_actionProcessorTask is not null)
            await _actionProcessorTask.ConfigureAwait(false);
    }

    public void Handle(IEvent e)
    {
        switch (e)
        {
            case ExecutorFaulted executorFaulted:
                ExecutorState = ExecutorState.Faulted;
                Exception = executorFaulted.Exception;
                break;
            case ChannelActivated _:
                ChannelState = ChannelState.Active;
                break;
            case ChannelClosedByServer _:
                ChannelState = ChannelState.ClosedByServer;
                break;
            case ChannelConnected _:
                ChannelState = ChannelState.Connected;
                break;
            case ChannelDeactivated _:
                ChannelState = ChannelState.Inactive;
                break;
            case SendReceiptWrongOrdering _:
            case ChannelDisconnected _:
                ChannelState = ChannelState.Disconnected;
                break;
            case ChannelReachedEndOfTopic _:
                ChannelState = ChannelState.ReachedEndOfTopic;
                break;
            case ChannelUnsubscribed _:
                ChannelState = ChannelState.Unsubscribed;
                break;
            case ProducerWaitingForExclusive _:
                ChannelState = ChannelState.WaitingForExclusive;
                break;
        }

        CalculateState();
    }

    protected abstract void CalculateState();

    private async Task ProcessActions(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var func = await ActionQueue.Dequeue(cancellationToken).ConfigureAwait(false);
                await func.Invoke(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }
        }
    }
}
