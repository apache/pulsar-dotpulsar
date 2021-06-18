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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(nameof(StandaloneClusterTest))]
    public class ProducerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly StandaloneClusterFixture _standaloneClusterFixture;
        private readonly IPulsarService _pulsarService;

        public ProducerTests(ITestOutputHelper outputHelper, StandaloneClusterFixture fixture)
        {
            _testOutputHelper = outputHelper;
            _standaloneClusterFixture = fixture;
            Debug.Assert(fixture.PulsarService != null, "fixture.PulsarService != null");
            _pulsarService = fixture.PulsarService;
        }

        [Fact]
        public async void SimpleProduceConsume_WhenSendingMessagesFromProducer_ThenReceiveMessagesFromConsumer()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"simple-produce-consume{Guid.NewGuid():N}";
            string content = "test-message";

            //Act
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();

            await using var consumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName("test-sub")
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .Create();
            await producer.StateChangedTo(ProducerState.Connected);
            await producer.Send(content);

            //Assert
            Assert.Equal(content, (await consumer.Receive()).Value());
        }

        [Fact]
        public async void SinglePartition_WhenSendMessages_ThenGetMessagesFromSinglePartition()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"single-partitioned-{Guid.NewGuid():N}";
            string content = "test-message";
            var partitions = 3;
            var consumers = new List<IConsumer<string>>();

            await _pulsarService.CreatePartitionedProducer($"persistent/public/default/{topicName}", partitions);

            //Act
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();

            for (var i = 0; i < partitions; ++i)
            {
                consumers.Add(client.NewConsumer(Schema.String)
                    .Topic($"{topicName}-partition-{i}")
                    .SubscriptionName("test-sub")
                    .InitialPosition(SubscriptionInitialPosition.Earliest)
                    .Create());
            }

            await producer.StateChangedTo(ProducerState.Connected);
            await producer.Send(content);

            //Assert
            Assert.Equal(content, (await Task.WhenAny(consumers.Select(c => c.Receive().AsTask()).ToList())).Result.Value());
        }

        [Fact]
        public async void RoundRobinPartition_WhenSendMessages_ThenGetMessagesFromPartitionsInOrder()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"round-robin-partitioned-{Guid.NewGuid():N}";
            string content = "test-message";
            var partitions = 3;
            var consumers = new List<IConsumer<string>>();

            await _pulsarService.CreatePartitionedProducer($"persistent/public/default/{topicName}", partitions);

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
            }

            //Assert
            for (var i = 0; i < partitions; ++i)
            {
                Assert.Equal($"{content}-{i}", (await consumers[i].Receive()).Value());
            }
        }
    }
}
