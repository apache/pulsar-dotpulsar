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

namespace DotPulsar.IntegrationTests
{
    using Abstraction;
    using Extensions;
    using Fixtures;
    using FluentAssertions;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(nameof(StandaloneClusterTest))]
    public class ConsumerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IPulsarService _pulsarService;

        public ConsumerTests(ITestOutputHelper outputHelper, StandaloneClusterFixture fixture)
        {
            _testOutputHelper = outputHelper;
            Debug.Assert(fixture.PulsarService != null, "fixture.PulsarService != null");
            _pulsarService = fixture.PulsarService;
        }

        [Fact]
        public async Task PartitionConsume_WhenSendMessages_ThenGetMessagesFromPartitionedTopic()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"partitioned-consumer-{Guid.NewGuid():N}";
            const string content = "test-message";
            const int partitions = 3;
            const int msgCount = 10;

            await _pulsarService.CreatePartitionedTopic($"persistent/public/default/{topicName}", partitions);

            //Act
            await using var consumer = client.NewConsumer(Schema.String)
                .SubscriptionName("test-sub")
                .Topic(topicName)
                .Create();

            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();
            await producer.StateChangedTo(ProducerState.Connected);

            for (var i = 0; i < msgCount; ++i)
            {
                var msg = $"{content}-{i}";
                await producer.Send(msg);
                _testOutputHelper.WriteLine($"Sent a message to consumer: {msg}");
            }

            //Assert
            for (var i = 0; i < msgCount; ++i)
            {
                var msg = await consumer.Receive();
                msg.Value().Should().Be($"{content}-{i}");
                _testOutputHelper.WriteLine($"Received a message: {msg.Value()}");
                await consumer.Acknowledge(msg.MessageId);
            }
        }

        [Fact]
        public async Task PartitionConsumeSeek_WhenSeekEarliestMessage_ThenGetMessageFromEarliestPosition()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"partitioned-consumer-seek-{Guid.NewGuid():N}";
            const string content = "test-message";
            const int partitions = 3;
            const int msgCount = 10;

            await _pulsarService.CreatePartitionedTopic($"persistent/public/default/{topicName}", partitions);

            //Act
            await using var consumer = client.NewConsumer(Schema.Int32)
                .SubscriptionName("test-sub")
                .Topic(topicName)
                .Create();

            await using var producer = client.NewProducer(Schema.Int32)
                .Topic(topicName)
                .Create();
            await producer.StateChangedTo(ProducerState.Connected);

            for (var i = 0; i < msgCount; ++i)
            {
                await producer.Send(i);
                _testOutputHelper.WriteLine($"Sent a message to consumer: {i}");
            }

            //Assert
            for (var i = 0; i < msgCount; ++i)
            {
                var msg = await consumer.Receive();
                _testOutputHelper.WriteLine($"Received message: {msg.Value()}");
                await consumer.Acknowledge(msg.MessageId);
            }

            await consumer.Seek(MessageId.Earliest);
            _testOutputHelper.WriteLine($"Seek message");

            // The sequential of the partitions for which message is obtained is not guaranteed here
            var message = await consumer.Receive();
            message.Value().Should().BeLessThan(partitions);
            await consumer.Acknowledge(message.MessageId);
        }

        [Fact]
        public async Task PartitionConsumeRedeliver_WhenRedeliverUnackMsg_ThenGetUnackMessages()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"partitioned-consumer-redeliver-{Guid.NewGuid():N}";
            const string content = "test-message";
            const int partitions = 3;
            const int msgCount = 10;

            await _pulsarService.CreatePartitionedTopic($"persistent/public/default/{topicName}", partitions);

            //Act
            await using var consumer = client.NewConsumer(Schema.Int32)
                .SubscriptionName("test-sub")
                .Topic(topicName)
                .Create();

            await using var producer = client.NewProducer(Schema.Int32)
                .Topic(topicName)
                .Create();
            await producer.StateChangedTo(ProducerState.Connected);

            for (var i = 0; i < msgCount; ++i)
            {
                await producer.Send(i);
                _testOutputHelper.WriteLine($"Sent a message to consumer: {i}");
            }

            //Assert
            for (var i = 0; i < msgCount; ++i)
            {
                var msg = await consumer.Receive();
                _testOutputHelper.WriteLine($"Received message: {msg.Value()}");

                if (i < partitions)
                    await consumer.Acknowledge(msg.MessageId);
            }

            await consumer.RedeliverUnacknowledgedMessages();
            _testOutputHelper.WriteLine($"Seek message");

            // The sequential of the partitions for which message is obtained is not guaranteed here
            var message = await consumer.Receive();
            message.Value().Should().BeLessThan(partitions * 2);
            message.Value().Should().BeGreaterOrEqualTo(partitions);
            await consumer.Acknowledge(message.MessageId);
        }

        [Fact]
        public async void ConsumerUnsubscribe_WhenUnsubscribe_ThenConsumerIsInFinalState()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"partitioned-consumer-seek-{Guid.NewGuid():N}";
            const int partitions = 3;

            await _pulsarService.CreatePartitionedTopic($"persistent/public/default/{topicName}", partitions);

            //Act
            await using var consumer = client.NewConsumer(Schema.Int32)
                .SubscriptionName("test-sub")
                .Topic(topicName)
                .Create();

            await consumer.StateChangedTo(ConsumerState.Active);

            await consumer.Unsubscribe();

            //Assert
            var timeoutErrMsg = "operation timeout";

            Func<Task> act = async () =>
            {
                var task = consumer.StateChangedFrom(ConsumerState.Active).AsTask();

                if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5))) != task)
                    throw new TimeoutException(timeoutErrMsg);
            };
            act.Should().NotThrow<TimeoutException>(timeoutErrMsg);
        }
    }
}
