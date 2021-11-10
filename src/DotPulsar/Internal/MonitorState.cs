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
using DotPulsar.Extensions;
using System.Threading.Tasks;

public static class StateMonitor
{
    public static async Task MonitorProducer(IProducer producer, IHandleStateChanged<ProducerStateChanged> handler)
    {
        await Task.Yield();

        var state = ProducerState.Disconnected;

        while (!producer.IsFinalState(state))
        {
            var stateChanged = await producer.StateChangedFrom(state, handler.CancellationToken).ConfigureAwait(false);
            state = stateChanged.ProducerState;
            try
            {
                await handler.OnStateChanged(stateChanged, handler.CancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }
        }
    }

    public static async Task MonitorConsumer(IConsumer consumer, IHandleStateChanged<ConsumerStateChanged> handler)
    {
        await Task.Yield();

        var state = ConsumerState.Disconnected;

        while (!consumer.IsFinalState(state))
        {
            var stateChanged = await consumer.StateChangedFrom(state, handler.CancellationToken).ConfigureAwait(false);
            state = stateChanged.ConsumerState;
            try
            {
                await handler.OnStateChanged(stateChanged, handler.CancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }
        }
    }

    public static async Task MonitorReader(IReader reader, IHandleStateChanged<ReaderStateChanged> handler)
    {
        await Task.Yield();

        var state = ReaderState.Disconnected;

        while (!reader.IsFinalState(state))
        {
            var stateChanged = await reader.StateChangedFrom(state, handler.CancellationToken).ConfigureAwait(false);
            state = stateChanged.ReaderState;
            try
            {
                await handler.OnStateChanged(stateChanged, handler.CancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }
        }
    }
}
