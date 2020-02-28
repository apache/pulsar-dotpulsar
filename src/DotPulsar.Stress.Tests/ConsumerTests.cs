using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotPulsar.Extensions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotPulsar.Stress.Tests
{
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

            var consumed = await consume;
            var produced = await produce;

            //Assert
            consumed.Should().BeEquivalentTo(produced);

            Task<MessageId[]> ProduceMessages(CancellationToken ct)
                => Enumerable.Range(1, numberOfMessages)
                    .Select(async n => await producer.Send(Encoding.UTF8.GetBytes($"Sent #{n} at {DateTimeOffset.UtcNow:s}"), ct))
                    .WhenAll();

            async Task<List<MessageId>> ConsumeMessages(CancellationToken ct)
            {
                var ids = new List<MessageId>(numberOfMessages);

                await foreach (var message in consumer.Messages(ct))
                {
                    ids.Add(message.MessageId);

                    if (ids.Count != numberOfMessages) continue;

                    await consumer.AcknowledgeCumulative(message, ct);

                    break;
                }

                return ids;
            }
        }
    }
}