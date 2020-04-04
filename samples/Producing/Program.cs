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

namespace Producing
{
    using DotPulsar;
    using DotPulsar.Abstractions;
    using DotPulsar.Extensions;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            const string myTopic = "persistent://public/default/mytopic";

            await using var client = PulsarClient.Builder().Build(); //Connecting to pulsar://localhost:6650

            var producer = client.NewProducer().Topic(myTopic).Create();

            var monitoring = Monitor(producer);

            var cts = new CancellationTokenSource();

            var producing = ProduceMessages(producer, cts.Token);

            Console.WriteLine("Press a key to exit");

            _ = Console.ReadKey();

            cts.Cancel();

            await producing.ConfigureAwait(false);

            await producer.DisposeAsync().ConfigureAwait(false);

            await monitoring.ConfigureAwait(false);
        }

        private static async Task ProduceMessages(IProducer producer, CancellationToken cancellationToken)
        {
            await Task.Yield();

            var delay = TimeSpan.FromSeconds(5);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var data = Encoding.UTF8.GetBytes("Sent " + DateTime.UtcNow);
                    _ = await producer.Send(data, cancellationToken).ConfigureAwait(false);
                    await Task.Delay(delay).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) // If not using the cancellationToken, then just dispose the producer and catch ObjectDisposedException instead
            { }
        }

        private static async Task Monitor(IProducer producer)
        {
            await Task.Yield();

            var state = ProducerState.Disconnected;

            while (true)
            {
                var stateChanged = await producer.StateChangedFrom(state).ConfigureAwait(false);
                state = stateChanged.ProducerState;

                var stateMessage = state switch
                {
                    ProducerState.Connected => "is connected",
                    ProducerState.Disconnected => "is disconnected",
                    ProducerState.Closed => "has closed",
                    ProducerState.Faulted => "has faulted",
                    _ => $"has an unknown state '{state}'"
                };

                var topic = stateChanged.Producer.Topic;
                Console.WriteLine($"The producer for topic '{topic}' " + stateMessage);

                if (producer.IsFinalState(state))
                    return;
            }
        }
    }
}
