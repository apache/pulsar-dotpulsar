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
    using Abstractions;
    using Extensions;
    using Fixtures;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
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
            await consumer.StateChangedTo(ConsumerState.Active);

            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();
            await producer.StateChangedTo(ProducerState.Connected);

            var sentMessages = new List<string>();

            for (var i = 0; i < msgCount; ++i)
            {
                var msg = $"{content}-{i}";
                await producer.Send(msg);
                sentMessages.Add(msg);
                _testOutputHelper.WriteLine($"Sent a message to consumer: {msg}");
            }

            //Assert
            for (var i = 0; i < msgCount; ++i)
            {
                var msg = await consumer.Receive();
                Assert.Contains(msg.Value(), sentMessages);
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
            const int partitions = 3;
            const int msgCount = 10;

            await _pulsarService.CreatePartitionedTopic($"persistent/public/default/{topicName}", partitions);

            //Act
            await using var consumer = client.NewConsumer(Schema.Int32)
                .SubscriptionName("test-sub")
                .Topic(topicName)
                .Create();
            await consumer.StateChangedTo(ConsumerState.Active);

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
            _testOutputHelper.WriteLine($"Received message: {message.Value()}");
            message.Value().Should().BeLessThan(partitions);
            await consumer.Acknowledge(message.MessageId);
        }

        [Fact]
        public async Task PartitionConsumeRedeliver_WhenRedeliverUnackMsg_ThenGetUnackMessages()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"partitioned-consumer-redeliver-{Guid.NewGuid():N}";
            const int partitions = 3;
            const int msgCount = 10;

            await _pulsarService.CreatePartitionedTopic($"persistent/public/default/{topicName}", partitions);

            //Act
            await using var consumer = client.NewConsumer(Schema.Int32)
                .SubscriptionName("test-sub")
                .Topic(topicName)
                .Create();
            await consumer.StateChangedTo(ConsumerState.Active);

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
            _testOutputHelper.WriteLine($"Received message: {message.Value()}");
            message.Value().Should().BeLessThan(partitions * 2);
            message.Value().Should().BeGreaterOrEqualTo(partitions);
            await consumer.Acknowledge(message.MessageId);
        }

        [Fact]
        public async void ConsumerUnsubscribe_WhenUnsubscribe_ThenConsumerIsUnsubscribed()
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

        [Fact]
        public async void MultipleTopicsConsumer_WhenSendMessages_ThenGetMessages()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();

            var topics = new List<string> { $"simple-topic-{Guid.NewGuid():N}", $"simple-topic-{Guid.NewGuid():N}", $"simple-topic-{Guid.NewGuid():N}" };

            //Act
            await using var consumer =
                client.NewConsumer(Schema.String).Topics(topics).SubscriptionName("test-sub").Create();
            await consumer.StateChangedTo(ConsumerState.Active);

            var producers = new List<IProducer<string>>();

            foreach (var topic in topics)
            {
                var producer = client.NewProducer(Schema.String).Topic(topic).Create();
                await producer.StateChangedTo(ProducerState.Connected);
                producers.Add(producer);
            }

            var sentMessages = new List<string>();

            for (var i = 0; i < producers.Count; i++)
            {
                var msg = $"test-message-{i}";
                await producers[i].Send(msg);
                _testOutputHelper.WriteLine($"Sent a message to consumer: {msg}");
                sentMessages.Add(msg);
            }

            // Assert
            foreach (var _ in sentMessages)
            {
                var receive = await consumer.Receive();
                _testOutputHelper.WriteLine($"Received message: {receive.Value()}");
                Assert.Contains(receive.Value(), sentMessages);
            }
        }

        private readonly Random _random = new();

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        [Fact]
        public async void TopicsPatternConsumer_WhenSendMessages_ThenGetMessages()
        {
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            var prefix = $"{RandomString(5)}-topic-";
            var topics = Enumerable.Range(0, 3).Select(_ => $"{prefix}{Guid.NewGuid():N}").ToList();

            var producers = new List<IProducer<string>>();

            foreach (var topic in topics)
            {
                var producer = client.NewProducer(Schema.String).Topic(topic).Create();
                await producer.StateChangedTo(ProducerState.Connected);
                producers.Add(producer);
            }

            await using var consumer = client.NewConsumer(Schema.String)
                .TopicsPattern($"persistent://public/default/{prefix}-*").SubscriptionName("test-sub").Create();
            await consumer.StateChangedTo(ConsumerState.Active);

            var sentMessages = new List<string>();

            for (var i = 0; i < producers.Count; i++)
            {
                var msg = $"test-message-{i}";
                await producers[i].Send(msg);
                _testOutputHelper.WriteLine($"Sent a message to consumer: {msg}");
                sentMessages.Add(msg);
            }

            for (var i = 0; i < producers.Count; i++)
            {
                var receive = await consumer.Receive();
                _testOutputHelper.WriteLine($"Received message: {receive.Value()}");
                Assert.Contains(receive.Value(), sentMessages);
            }
        }
    }
}
