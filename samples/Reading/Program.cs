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

namespace Reading;

using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

internal static class Program
{
    private static async Task Main()
    {
        const string myTopic = "persistent://public/default/mytopic";

        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, args) =>
        {
            cts.Cancel();
            args.Cancel = true;
        };

        await using var client = PulsarClient.Builder().Build(); //Connecting to pulsar://localhost:6650

        await using var reader = client.NewReader(Schema.String)
            .StartMessageId(MessageId.Earliest)
            .StateChangedHandler(Monitor)
            .Topic(myTopic)
            .Create();

        Console.WriteLine("Press Ctrl+C to exit");

        await ReadMessages(reader, cts.Token);
    }

    private static async Task ReadMessages(IReader<string> reader, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var message in reader.Messages(cancellationToken))
            {
                Console.WriteLine("Received: " + message.Value());
            }
        }
        catch (OperationCanceledException) { }
    }

    private static void Monitor(ReaderStateChanged stateChanged, CancellationToken cancellationToken)
    {
        var stateMessage = stateChanged.ReaderState switch
        {
            ReaderState.Connected => "is connected",
            ReaderState.Disconnected => "is disconnected",
            ReaderState.Closed => "has closed",
            ReaderState.ReachedEndOfTopic => "has reached end of topic",
            ReaderState.Faulted => "has faulted",
            _ => $"has an unknown state '{stateChanged.ReaderState}'"
        };

        var topic = stateChanged.Reader.Topic;
        Console.WriteLine($"The reader for topic '{topic}' {stateMessage}");
    }
}
