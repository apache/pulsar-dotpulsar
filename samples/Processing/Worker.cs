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

namespace Processing;

using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var client = PulsarClient.Builder().Build(); //Connecting to pulsar://localhost:6650

        await using var consumer = client.NewConsumer(Schema.String)
            .StateChangedHandler(Monitor, cancellationToken)
            .SubscriptionName("MySubscription")
            .Topic("persistent://public/default/mytopic")
            .Create();

        await consumer.Process(ProcessMessage, cancellationToken);
    }

    private ValueTask ProcessMessage(IMessage<string> message, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Received: {message.Value()}");
        return ValueTask.CompletedTask;
    }

    private void Monitor(ConsumerStateChanged stateChanged, CancellationToken cancellationToken)
    {
        var stateMessage = stateChanged.ConsumerState switch
        {
            ConsumerState.Active => "is active",
            ConsumerState.Inactive => "is inactive",
            ConsumerState.Disconnected => "is disconnected",
            ConsumerState.Closed => "has closed",
            ConsumerState.ReachedEndOfTopic => "has reached end of topic",
            ConsumerState.Faulted => "has faulted",
            _ => $"has an unknown state '{stateChanged.ConsumerState}'"
        };

        var topic = stateChanged.Consumer.Topic;
        _logger.LogInformation($"The consumer for topic '{topic}' {stateMessage}");
    }
}
