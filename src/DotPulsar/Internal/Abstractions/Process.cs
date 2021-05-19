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

namespace DotPulsar.Internal.Abstractions
{
    using Events;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class Process : IProcess
    {
        protected readonly CancellationTokenSource CancellationTokenSource;
        protected ChannelState ChannelState;
        protected ExecutorState ExecutorState;

        protected Process(Guid correlationId)
        {
            CancellationTokenSource = new CancellationTokenSource();
            ChannelState = ChannelState.Disconnected;
            ExecutorState = ExecutorState.Ok;
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }

        public void Start()
            => CalculateState();

        public abstract ValueTask DisposeAsync();

        public void Handle(IEvent e)
        {
            switch (e)
            {
                case ExecutorFaulted _:
                    ExecutorState = ExecutorState.Faulted;
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
                case ChannelDisconnected _:
                    ChannelState = ChannelState.Disconnected;
                    break;
                case ChannelReachedEndOfTopic _:
                    ChannelState = ChannelState.ReachedEndOfTopic;
                    break;
                case ChannelUnsubscribed _:
                    ChannelState = ChannelState.Unsubscribed;
                    break;
                default:HandleExtend(e);
                    break;
            }

            CalculateState();
        }

        protected abstract void CalculateState();

        protected virtual void HandleExtend(IEvent e)
            => throw new NotImplementedException();
    }
}
