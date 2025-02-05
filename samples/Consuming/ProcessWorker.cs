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
using Extensions;
using System.Threading;

public sealed class ProcessWorker : BackgroundService
{
    private readonly ILogger _logger;

    public ProcessWorker(ILogger<ProcessWorker> logger) => _logger = logger;

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

        _ = consumer.DelayedStateMonitor(       // Recommended way of ignoring the short disconnects expected when working with a distributed system
            ConsumerState.Active,               // Operational state
            TimeSpan.FromSeconds(5),            // The amount of time allowed in non-operational state before we act
            _logger.ConsumerLostConnection,     // Invoked if we are NOT back in operational state after 5 seconds
            _logger.ConsumerRegainedConnection, // Invoked when we are in operational state again
            stoppingToken);

        _logger.LogInformation("Will start receiving messages with 'Process'");

        await consumer.Process(ProcessMessage, stoppingToken);
    }

    private ValueTask ProcessMessage(IMessage<string> message, CancellationToken cancellationToken)
    {
        _logger.OutputMessage(message);
        return ValueTask.CompletedTask;
    }
}
