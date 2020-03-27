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

using DotPulsar.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotPulsar.Stress.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace DotPulsar.Stress.Tests
{
    [Collection(nameof(StandaloneClusterTest))]
    public class ConsumerTests
    {
        private readonly ITestOutputHelper _output;

        public ConsumerTests(ITestOutputHelper output) => _output = output;

        [Theory]
        [InlineData(1000)]
        public async Task Messages_GivenTopicWithMessages_ShouldConsumeAll(int numberOfMessages)
        {
            //Arrange
            var testRunId = Guid.NewGuid().ToString("N");

            var topic = $"persistent://public/default/consumer-tests-{testRunId}";

            await using var client = PulsarClient.Builder()
                .ExceptionHandler(new XunitExceptionHandler(_output))
                .Build(); //Connecting to pulsar://localhost:6650

            await using var consumer = client.NewConsumer()
                .ConsumerName($"consumer-{testRunId}")
                .SubscriptionName($"subscription-{testRunId}")
                .Topic(topic)
                .Create();

            await using var producer = client.NewProducer()
                .ProducerName($"producer-{testRunId}")
                .Topic(topic)
                .Create();

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            //Act
            var consume = ConsumeMessages(cts.Token);
            var produce = ProduceMessages(cts.Token);

            var consumed = await consume.ConfigureAwait(false);
            var produced = await produce.ConfigureAwait(false);

            //Assert
            consumed.Should().BeEquivalentTo(produced);

            Task<MessageId[]> ProduceMessages(CancellationToken ct)
                => Enumerable.Range(1, numberOfMessages)
                    .Select(async n => await producer.Send(Encoding.UTF8.GetBytes($"Sent #{n} at {DateTimeOffset.UtcNow:s}"), ct).ConfigureAwait(false))
                    .WhenAll();

            async Task<List<MessageId>> ConsumeMessages(CancellationToken ct)
            {
                var ids = new List<MessageId>(numberOfMessages);

                await foreach (var message in consumer.Messages(ct))
                {
                    ids.Add(message.MessageId);

                    if (ids.Count != numberOfMessages) continue;

                    await consumer.AcknowledgeCumulative(message, ct).ConfigureAwait(false);

                    break;
                }

                return ids;
            }
        }
    }
}