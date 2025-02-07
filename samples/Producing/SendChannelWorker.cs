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

public class SendChannelWorker : BackgroundService
{
    private readonly ILogger _logger;

    public SendChannelWorker(ILogger<SendChannelWorker> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var client = PulsarClient.Builder()
            .ExceptionHandler(_logger.PulsarClientException) // Optional
            .Build();                                        // Connecting to pulsar://localhost:6650

        await using var producer = client.NewProducer(Schema.String)
            .StateChangedHandler(_logger.ProducerChangedState) // Optional
            .Topic("persistent://public/default/mytopic")
            .Create();

        var sendChannel = producer.SendChannel;

        var delay = TimeSpan.FromSeconds(5);

        _logger.LogInformation($"Will start sending messages every {delay.TotalSeconds} seconds with 'SendChannel'");

        while (!stoppingToken.IsCancellationRequested)
        {
            var data = DateTime.UtcNow.ToLongTimeString();
            await sendChannel.Send(data, id =>
            {
                _logger.LogInformation($"Received acknowledgement message with content: '{data}' and got message id: '{id}'");
                return ValueTask.CompletedTask;
            }, stoppingToken);
            _logger.LogInformation($"Sent message with content: '{data}'");
            await Task.Delay(delay, stoppingToken);
        }
        sendChannel.Complete();

        // Wait up to 5 seconds for in flight messages to be delivered before closing
        var shutdownCts = new CancellationTokenSource();
        shutdownCts.CancelAfter(TimeSpan.FromSeconds(5));
        await sendChannel.Completion(shutdownCts.Token);
    }
}
