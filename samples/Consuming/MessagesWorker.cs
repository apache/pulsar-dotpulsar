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
using DotPulsar.Extensions;
using Extensions;

public sealed class MessagesWorker : BackgroundService
{
    private readonly ILogger _logger;

    public MessagesWorker(ILogger<MessagesWorker> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var client = PulsarClient.Builder()
            .ExceptionHandler(_logger.PulsarClientException) // Optional
            .Build();                                        // Connecting to pulsar://localhost:6650

        await using var consumer = client.NewConsumer(Schema.String)
            .StateChangedHandler(_logger.ConsumerChangedState) // Optional
            .SubscriptionName("MySubscription")
            .Topic("persistent://public/default/mytopic")
            .Create();

        _logger.LogInformation("Will start receiving messages with 'Messages'");

        await foreach (var message in consumer.Messages(stoppingToken))
        {
            _logger.OutputMessage(message);
            await consumer.Acknowledge(message, stoppingToken);
        }
    }
}
