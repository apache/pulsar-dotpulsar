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

namespace Producing;

using DotPulsar;
using DotPulsar.Extensions;
using Extensions;

public sealed class SendWorker : BackgroundService
{
    private readonly ILogger _logger;

    public SendWorker(ILogger<SendWorker> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var client = PulsarClient.Builder()
            .ExceptionHandler(_logger.PulsarClientException) // Optional
            .Build();                                        // Connecting to pulsar://localhost:6650

        await using var producer = client.NewProducer(Schema.String)
            .StateChangedHandler(_logger.ProducerChangedState) // Optional
            .Topic("persistent://public/default/mytopic")
            .Create();

        var delay = TimeSpan.FromSeconds(5);

        _logger.LogInformation($"Will start sending messages every {delay.TotalSeconds} seconds with 'Send'");

        while (!stoppingToken.IsCancellationRequested)
        {
            var data = DateTime.UtcNow.ToLongTimeString();
            var messageId = await producer.Send(data, stoppingToken);
            _logger.LogInformation($"Sent message with content: '{data}' and got message id: '{messageId}'");
            await Task.Delay(delay, stoppingToken);
        }
    }
}
