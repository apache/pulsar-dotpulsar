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

            var taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            Console.CancelKeyPress += (sender, args) =>
            {
                taskCompletionSource.SetResult(null);
                args.Cancel = true;
            };

            await using var client = PulsarClient.Builder().Build(); //Connecting to pulsar://localhost:6650

            var producer = client.NewProducer()
                .StateChangedHandler(Monitor)
                .Topic(myTopic)
                .Create();

            var cts = new CancellationTokenSource();

            var producing = ProduceMessages(producer, cts.Token);

            Console.WriteLine("Press Ctrl+C to exit");

            await taskCompletionSource.Task;

            cts.Cancel();

            await producing;

            await producer.DisposeAsync();
        }

        private static async Task ProduceMessages(IProducer producer, CancellationToken cancellationToken)
        {
            await Task.Yield();

            var delay = TimeSpan.FromSeconds(5);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var data = DateTime.UtcNow.ToLongTimeString();
                    var bytes = Encoding.UTF8.GetBytes(data);
                    _ = await producer.Send(bytes, cancellationToken);
                    Console.WriteLine("Sent: " + data);
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (OperationCanceledException) // If not using the cancellationToken, then just dispose the producer and catch ObjectDisposedException instead
            { }
        }

        private static void Monitor(ProducerStateChanged stateChanged, CancellationToken cancellationToken)
        {
            var stateMessage = stateChanged.ProducerState switch
            {
                ProducerState.Connected => "is connected",
                ProducerState.Disconnected => "is disconnected",
                ProducerState.Closed => "has closed",
                ProducerState.Faulted => "has faulted",
                _ => $"has an unknown state '{stateChanged.ProducerState}'"
            };

            var topic = stateChanged.Producer.Topic;
            Console.WriteLine($"The producer for topic '{topic}' " + stateMessage);
        }
    }
}
