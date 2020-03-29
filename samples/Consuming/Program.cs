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

namespace Consuming
{
    using System;
    using System.Buffers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DotPulsar;
    using DotPulsar.Abstractions;
    using DotPulsar.Extensions;

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            const string myTopic = "persistent://public/default/mytopic";

            await using var client = PulsarClient.Builder().Build(); //Connecting to pulsar://localhost:6650

            var consumer = client.NewConsumer()
                .SubscriptionName("MySubscription")
                .Topic(myTopic)
                .Create();

            var monitoring = Monitor(consumer);

            var cts = new CancellationTokenSource();

            var consuming = ConsumeMessages(consumer, cts.Token);

            Console.WriteLine("Press a key to exit");

            _ = Console.ReadKey();

            cts.Cancel();

            await consuming.ConfigureAwait(false);

            await consumer.DisposeAsync().ConfigureAwait(false);

            await monitoring.ConfigureAwait(false);
        }

        private static async Task ConsumeMessages(IConsumer consumer, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                await foreach (var message in consumer.Messages(cancellationToken))
                {
                    var data = Encoding.UTF8.GetString(message.Data.ToArray());
                    Console.WriteLine("Received: " + data);
                    await consumer.Acknowledge(message, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { }
        }

        private static async Task Monitor(IConsumer consumer)
        {
            await Task.Yield();

            var state = ConsumerState.Disconnected;

            while (true)
            {
                state = await consumer.StateChangedFrom(state).ConfigureAwait(false);

                var stateMessage = state switch
                {
                    ConsumerState.Active => "The consumer is active",
                    ConsumerState.Inactive => "The consumer is inactive",
                    ConsumerState.Disconnected => "The consumer is disconnected",
                    ConsumerState.Closed => "The consumer has closed",
                    ConsumerState.ReachedEndOfTopic => "The consumer has reached end of topic",
                    ConsumerState.Faulted => "The consumer has faulted",
                    _ => $"The consumer has an unknown state '{state}'"
                };

                Console.WriteLine(stateMessage);

                if (consumer.IsFinalState(state))
                    return;
            }
        }
    }
}
