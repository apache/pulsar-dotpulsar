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
using System;
using System.Threading;
using System.Threading.Tasks;

public abstract class Process : IProcess
{
    protected readonly CancellationTokenSource CancellationTokenSource;
    protected ChannelState ChannelState;
    protected ExecutorState ExecutorState;
    protected Exception? Exception;

    protected Process(Guid correlationId)
    {
        CancellationTokenSource = new CancellationTokenSource();
        ChannelState = ChannelState.Disconnected;
        ExecutorState = ExecutorState.Ok;
        CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
    protected ulong? TopicEpoch { get; private set; }

    public void Start()
        => CalculateState();

    public abstract ValueTask DisposeAsync();

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
            case ProducerChannelConnected producerChannelConnected:
                TopicEpoch = producerChannelConnected.TopicEpoch;
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
}
