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
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(nameof(StandaloneClusterTest))]
    public class ProducerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IPulsarService _pulsarService;

        public ProducerTests(ITestOutputHelper outputHelper, StandaloneClusterFixture fixture)
        {
            _testOutputHelper = outputHelper;
            Debug.Assert(fixture.PulsarService != null, "fixture.PulsarService != null");
            _pulsarService = fixture.PulsarService;
        }

        [Fact]
        public async Task SimpleProduceConsume_WhenSendingMessagesToProducer_ThenReceiveMessagesFromConsumer()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"simple-produce-consume{Guid.NewGuid():N}";
            const string content = "test-message";

            //Act
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();

            await using var consumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName("test-sub")
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .Create();

            await producer.Send(content);
            _testOutputHelper.WriteLine($"Sent a message: {content}");

            //Assert
            (await consumer.Receive()).Value().Should().Be(content);
        }

        [Fact]
        public async Task SinglePartition_WhenSendMessages_ThenGetMessagesFromSinglePartition()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"single-partitioned-{Guid.NewGuid():N}";
            const string content = "test-message";
            const int partitions = 3;
            const int msgCount = 3;
            var consumers = new List<IConsumer<string>>();

            await _pulsarService.CreatePartitionedTopic($"persistent/public/default/{topicName}", partitions);

            //Act
            for (var i = 0; i < partitions; ++i)
            {
                await using var producer = client.NewProducer(Schema.String)
                    .Topic(topicName)
                    .MessageRouter(new SinglePartitionRouter(i))
                    .Create();

                for (var msgIndex = 0; msgIndex < msgCount; ++msgIndex)
                {
                    await producer.Send($"{content}-{i}-{msgIndex}");
                    _testOutputHelper.WriteLine($"Sent a message: {content}-{i}-{msgIndex}");
                }
            }

            for (var i = 0; i < partitions; ++i)
            {
                consumers.Add(client.NewConsumer(Schema.String)
                    .Topic($"{topicName}-partition-{i}")
                    .SubscriptionName("test-sub")
                    .InitialPosition(SubscriptionInitialPosition.Earliest)
                    .Create());
            }

            var msg = await consumers[1].GetLastMessageId();

            //Assert
            for (var i = 0; i < partitions; ++i)
            for (var msgIndex = 0; msgIndex < msgCount; ++msgIndex)
                (await consumers[i].Receive()).Value().Should().Be($"{content}-{i}-{msgIndex}");
        }

        [Fact]
        public async Task RoundRobinPartition_WhenSendMessages_ThenGetMessagesFromPartitionsInOrder()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"round-robin-partitioned-{Guid.NewGuid():N}";
            const string content = "test-message";
            const int partitions = 3;
            var consumers = new List<IConsumer<string>>();

            await _pulsarService.CreatePartitionedTopic($"persistent/public/default/{topicName}", partitions);

            //Act
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();
            await producer.StateChangedTo(ProducerState.Connected);

            for (var i = 0; i < partitions; ++i)
            {
                consumers.Add(client.NewConsumer(Schema.String)
                    .Topic($"{topicName}-partition-{i}")
                    .SubscriptionName("test-sub")
                    .InitialPosition(SubscriptionInitialPosition.Earliest)
                    .Create());
                await producer.Send($"{content}-{i}");
                _testOutputHelper.WriteLine($"Sent a message to consumer [{i}]");
            }

            //Assert
            for (var i = 0; i < partitions; ++i)
            {
                (await consumers[i].Receive()).Value().Should().Be($"{content}-{i}");
            }
        }
    }
}
