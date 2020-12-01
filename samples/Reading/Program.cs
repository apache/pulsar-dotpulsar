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

namespace Reading
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

            var reader = client.NewReader()
                .StartMessageId(MessageId.Earliest)
                .Topic(myTopic)
                .Create();

            var monitoring = Monitor(reader);

            var cts = new CancellationTokenSource();

            var reading = ReadMessages(reader, cts.Token);

            Console.WriteLine("Press Ctrl+C to exit");

            await taskCompletionSource.Task;

            cts.Cancel();

            await reading;

            await reader.DisposeAsync();

            await monitoring;
        }

        private static async Task ReadMessages(IReader reader, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                await foreach (var message in reader.Messages(cancellationToken))
                {
                    var data = Encoding.UTF8.GetString(message.Data.ToArray());
                    Console.WriteLine("Received: " + data);
                }
            }
            catch (OperationCanceledException) { }
        }

        private static async Task Monitor(IReader reader)
        {
            await Task.Yield();

            var state = ReaderState.Disconnected;

            while (true)
            {
                var stateChanged = await reader.StateChangedFrom(state);
                state = stateChanged.ReaderState;

                var stateMessage = state switch
                {
                    ReaderState.Connected => "is connected",
                    ReaderState.Disconnected => "is disconnected",
                    ReaderState.Closed => "has closed",
                    ReaderState.ReachedEndOfTopic => "has reached end of topic",
                    ReaderState.Faulted => "has faulted",
                    _ => $"has an unknown state '{state}'"
                };

                var topic = stateChanged.Reader.Topic;
                Console.WriteLine($"The reader for topic '{topic}' " + stateMessage);

                if (reader.IsFinalState(state))
                    return;
            }
        }
    }
}
