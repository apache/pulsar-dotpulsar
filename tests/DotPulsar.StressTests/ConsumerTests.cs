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

namespace DotPulsar.StressTests
{
    using DotPulsar.Abstractions;
    using DotPulsar.Extensions;
    using Fixtures;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(nameof(StandaloneClusterTest))]
    public class ConsumerTests
    {
        private readonly ITestOutputHelper _output;

        public ConsumerTests(ITestOutputHelper output)
            => _output = output;

        [Theory]
        [InlineData(10000)]
        public async Task Messages_GivenTopicWithMessages_ShouldConsumeAll(int numberOfMessages)
        {
            //Arrange
            var testRunId = Guid.NewGuid().ToString("N");

            var topic = $"persistent://public/default/consumer-tests-{testRunId}";

            await using var client = PulsarClient.Builder()
                .ExceptionHandler(new XunitExceptionHandler(_output))
                .ServiceUrl(new Uri("pulsar://localhost:54545"))
                .Build();

            await using var consumer = client.NewConsumer(Schema.ByteArray)
                .ConsumerName($"consumer-{testRunId}")
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionName($"subscription-{testRunId}")
                .Topic(topic)
                .Create();

            await using var producer = client.NewProducer(Schema.ByteArray)
                .ProducerName($"producer-{testRunId}")
                .Topic(topic)
                .Create();

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            //Act
            var produced = await ProduceMessages(producer, numberOfMessages, cts.Token);
            var consumed = await ConsumeMessages(consumer, numberOfMessages, cts.Token);

            //Assert
            consumed.Should().BeEquivalentTo(produced);
        }

        private static async Task<IEnumerable<MessageId>> ProduceMessages(IProducer<byte[]> producer, int numberOfMessages, CancellationToken ct)
        {
            var messageIds = new MessageId[numberOfMessages];

            for (var i = 0; i < numberOfMessages; ++i)
            {
                var data = Encoding.UTF8.GetBytes($"Sent #{i} at {DateTimeOffset.UtcNow:s}");
                messageIds[i] = await producer.Send(data, ct);
            }

            return messageIds;
        }

        private static async Task<IEnumerable<MessageId>> ConsumeMessages(IConsumer<byte[]> consumer, int numberOfMessages, CancellationToken ct)
        {
            var messageIds = new List<MessageId>(numberOfMessages);

            await foreach (var message in consumer.Messages(ct))
            {
                messageIds.Add(message.MessageId);

                if (messageIds.Count != numberOfMessages)
                    continue;

                await consumer.AcknowledgeCumulative(message, ct);

                break;
            }

            return messageIds;
        }
    }
}
