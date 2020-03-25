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

using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using System;
using System.Buffers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reading
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string myTopic = "persistent://public/default/mytopic";

            await using var client = PulsarClient.Builder().Build(); //Connecting to pulsar://localhost:6650

            var reader = client.NewReader()
                .StartMessageId(MessageId.Earliest)
                .Topic(myTopic)
                .Create();


            var monitoring = Monitor(reader);

            var cts = new CancellationTokenSource();

            var reading = ReadMessages(reader, cts.Token);

            Console.WriteLine("Press a key to exit");
            
            _ = Console.ReadKey();

            cts.Cancel();

            await reading.ConfigureAwait(false);

            await reader.DisposeAsync().ConfigureAwait(false);

            await monitoring.ConfigureAwait(false);
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
            catch(OperationCanceledException)
            {
                return;
            }
        }

        private static async Task Monitor(IReader reader)
        {
            await Task.Yield();

            var state = ReaderState.Disconnected;

            while (true)
            {
                state = await reader.StateChangedFrom(state).ConfigureAwait(false);

                var stateMessage = state switch
                {
                    ReaderState.Connected => "The reader is connected",
                    ReaderState.Disconnected => "The reader is disconnected",
                    ReaderState.Closed => "The reader has closed",
                    ReaderState.ReachedEndOfTopic => "The reader has reached end of topic",
                    ReaderState.Faulted => "The reader has faulted",
                    _ => $"The reader has an unknown state '{state}'"
                };

                Console.WriteLine(stateMessage);

                if (reader.IsFinalState(state))
                    return;
            }
        }
    }
}
