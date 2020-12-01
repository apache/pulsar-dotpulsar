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
    using DotPulsar;
    using DotPulsar.Abstractions;
    using DotPulsar.Extensions;
    using System;
    using System.Buffers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            const string myTopic = "persistent://public/default/mytopic";

            var taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            Console.CancelKeyPress += (sender, args) =>
            {
                taskCompletionSource.SetResult(null);
                args.Cancel = true;
            };

            await using var client = PulsarClient.Builder().Build(); //Connecting to pulsar://localhost:6650

            var consumer = client.NewConsumer()
                .SubscriptionName("MySubscription")
                .Topic(myTopic)
                .Create();

            var monitoring = Monitor(consumer);

            var cts = new CancellationTokenSource();

            var consuming = ConsumeMessages(consumer, cts.Token);

            Console.WriteLine("Press Ctrl+C to exit");

            await taskCompletionSource.Task;

            cts.Cancel();

            await consuming;

            await consumer.DisposeAsync();

            await monitoring;
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
                    await consumer.Acknowledge(message, cancellationToken);
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
                var stateChanged = await consumer.StateChangedFrom(state);
                state = stateChanged.ConsumerState;

                var stateMessage = state switch
                {
                    ConsumerState.Active => "is active",
                    ConsumerState.Inactive => "is inactive",
                    ConsumerState.Disconnected => "is disconnected",
                    ConsumerState.Closed => "has closed",
                    ConsumerState.ReachedEndOfTopic => "has reached end of topic",
                    ConsumerState.Faulted => "has faulted",
                    _ => $"has an unknown state '{state}'"
                };

                var topic = stateChanged.Consumer.Topic;
                Console.WriteLine($"The consumer for topic '{topic}' " + stateMessage);

                if (consumer.IsFinalState(state))
                    return;
            }
        }
    }
}
