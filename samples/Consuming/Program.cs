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

namespace Consuming;

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
        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, args) =>
        {
            cts.Cancel();
            args.Cancel = true;
        };

        await using var client = PulsarClient.Builder().Build(); // Connecting to pulsar://localhost:6650

        await using var consumer = client.NewConsumer(Schema.String)
            .StateChangedHandler(Monitor)
            .SubscriptionName("MySubscription")
            .Topic("persistent://public/default/mytopic")
            .Create();

        Console.WriteLine("Press Ctrl+C to exit");

        await ConsumeMessages(consumer, cts.Token);
    }

    private static async Task ConsumeMessages(IConsumer<string> consumer, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var message in consumer.Messages(cancellationToken))
            {
                Console.WriteLine($"Received: {message.Value()}");
                await consumer.Acknowledge(message, cancellationToken);
            }
        }
        catch (OperationCanceledException) { }
    }

    private static void Monitor(ConsumerStateChanged stateChanged, CancellationToken cancellationToken)
    {
        var topic = stateChanged.Consumer.Topic;
        var state = stateChanged.ConsumerState;
        Console.WriteLine($"The consumer for topic '{topic}' changed state to '{state}'");
    }
}
