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
        await using var client = PulsarClient.Builder()
            .ExceptionHandler(context => _logger.PulsarClientException(context))
            .Build(); // Connecting to pulsar://localhost:6650

        await using var consumer = client.NewConsumer(Schema.String)
            .StateChangedHandler(consumerStateChanged => _logger.ConsumerChangedState(consumerStateChanged))
            .SubscriptionName("MySubscription")
            .Topic("persistent://public/default/mytopic")
            .Create();

        var options = new ProcessingOptions
        {
            MaxDegreeOfParallelism = 5
        };
        await consumer.Process(ProcessMessage, options, cancellationToken);
    }

    private ValueTask ProcessMessage(IMessage<string> message, CancellationToken cancellationToken)
    {
        _logger.OutputMessage(message);
        return ValueTask.CompletedTask;
    }
}
