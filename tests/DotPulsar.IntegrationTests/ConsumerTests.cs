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
    using DotPulsar.Abstractions;
    using Extensions;
    using Fixtures;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
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
        public async Task GivesDoesNotAckWithinTimout_WhenAnotherConsumerReceives_ThenOtherConsumerShouldReceiveMessage()
        {
            //Arrange
            var ackTimeoutInMs = 1000;
            var additionalAllowedTimeToWaitInMs = 100;
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"ack-timeout-{Guid.NewGuid():N}";
            string subscriptionName = "test-sub";
            const string content = "test-message";

            //Act
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();
            await producer.Send(content);
            await using var consumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .AcknowledgementTimeout(TimeSpan.FromMilliseconds(ackTimeoutInMs))
                .Create();
            await using var otherConsumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .Create();
            (await consumer.Receive()).Value().Should().Be(content);

            //Assert
            var cancellationToken = new CancellationTokenSource(ackTimeoutInMs + additionalAllowedTimeToWaitInMs).Token;
            (await otherConsumer.Receive(cancellationToken)).Value().Should().Be(content);
        }

        [Fact]
        public async Task GivenAcksWithinTimout_WhenAnotherConsumerReceives_ThenOtherConsumerShouldNotReceiveMessage()
        {
            //Arrange
            var ackTimeoutInMs = 1000;
            var additionalAllowedTimeToWaitInMs = 500;
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"ack-timeout-{Guid.NewGuid():N}";
            string subscriptionName = "test-sub";
            const string content = "test-message";

            //Act
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();
            await producer.Send(content);
            await using var consumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .AcknowledgementTimeout(TimeSpan.FromMilliseconds(ackTimeoutInMs))
                .Create();
            await using var otherConsumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .Create();
            await consumer.Acknowledge(await consumer.Receive());
            var cancellationToken = new CancellationTokenSource(ackTimeoutInMs + additionalAllowedTimeToWaitInMs).Token;
            Func<Task> act = async () => { (await otherConsumer.Receive(cancellationToken)).Value().Should().Be(content); };

            //Assert
            await act.Should().ThrowAsync<TaskCanceledException>();
        }

        [Fact]
        public async Task GivenNegativeAck_WhenConsumerReceives_ThenItShouldReceiveMessageAfterNegativeAcknoledgementDelay()
        {
            //Arrange
            var nackDelay = 1000;
            var additionalAllowedTimeToWaitInMs = 100;
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"nack-delay-{Guid.NewGuid():N}";
            string subscriptionName = "test-sub";
            const string content = "test-message";

            //Act
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();
            await producer.Send(content);
            await using var consumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .NegativeAcknowledgementRedeliveryDelay(TimeSpan.FromMilliseconds(nackDelay))
                .Create();
            await using var otherConsumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .Create();
            consumer.NegativeAcknowledge((await consumer.Receive()).MessageId);

            //Assert
            var cancellationToken = new CancellationTokenSource(nackDelay + additionalAllowedTimeToWaitInMs).Token;
            (await otherConsumer.Receive(cancellationToken)).Value().Should().Be(content);
        }

        [Fact]
        public async Task GivenNegativeAck_WhenAnotherConsumerReceives_ThenItShouldNotReceiveTheSameMessage()
        {
            //Arrange
            var nackDelay = 2000;
            var maxTimeToWait = nackDelay - 100;
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"nack-delay-{Guid.NewGuid():N}";
            string subscriptionName = "test-sub";
            const string content = "test-message";

            //Act
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();
            await producer.Send(content);
            await using var consumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .NegativeAcknowledgementRedeliveryDelay(TimeSpan.FromMilliseconds(nackDelay))
                .Create();
            await using var otherConsumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .Create();
            consumer.NegativeAcknowledge((await consumer.Receive()).MessageId);
            var cancellationToken = new CancellationTokenSource(maxTimeToWait).Token;
            Func<Task> act = async () => { await otherConsumer.Receive(cancellationToken); };

            //Assert
            await act.Should().ThrowAsync<TaskCanceledException>();
        }

        [Fact]
        public async Task GivenMessagesAreProducedInBatch_WhenAConsumerReceives_ThenShouldReceiveAsBatch()
        {
            //Arrange
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"batch-send-and-receive-{Guid.NewGuid():N}";
            string subscriptionName = "test-sub";
            var content = new string[] { "test-1", "test-2", "test-3" };

            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();
            await using var consumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .Create();

            //Act
            foreach (var value in content)
                _ = await producer.Send(value);
            var cts = new CancellationTokenSource(1000);
            var messages = new List<IMessage<string>>
            {
                await consumer.Receive(cts.Token),
                await consumer.Receive(cts.Token),
                await consumer.Receive(cts.Token)
            };

            //Assert
            messages.Should().HaveCount(content.Count());
            messages.Select(m => m.Value()).Should().Equal(content);
            messages.Select(m => m.MessageId.BatchIndex).Distinct().Should().HaveCount(1);
        }

        [Fact]
        public async Task GivenMessagesAreProducedInBatch_WhenAConsumerAcksOneMessage_ThenShouldReceiveBatch()
        {
            //Arrange
            var ackTimeoutInMs = 1000;
            var additionalAllowedTimeToWaitInMs = 100;
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"batched-ack-timeout-{Guid.NewGuid():N}";
            var content = new string[] { "test-1", "test-2", "test-3" };
            string subscriptionName = "test-sub";
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();
            await using var consumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .AcknowledgementTimeout(TimeSpan.FromMilliseconds(ackTimeoutInMs))
                .Create();
            await using var otherConsumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .Create();

            //Act
            foreach (var value in content)
                _ = await producer.Send(value);
            var cts = new CancellationTokenSource(1000);
            var messages = new List<IMessage<string>>
            {
                await consumer.Receive(cts.Token),
                await consumer.Receive(cts.Token),
                await consumer.Receive(cts.Token)
            };
            await consumer.Acknowledge(messages[1]);

            //Assert
            var cancellationToken = new CancellationTokenSource(ackTimeoutInMs + additionalAllowedTimeToWaitInMs).Token;
            (await otherConsumer.Receive(cancellationToken)).Value().Should().Be(content[0]);
            (await otherConsumer.Receive(cancellationToken)).Value().Should().Be(content[2]);
        }

        [Fact]
        public async Task GivenMessagesAreProducedInBatch_WhenAConsumerNegativeAcksOneMessage_ThenOtherConsumerShouldReceiveMessagesAfterRedeliveryDelay()
        {
            //Arrange
            var nackDelay = 1000;
            var additionalAllowedTimeToWaitInMs = 100;
            await using var client = PulsarClient.Builder().ServiceUrl(_pulsarService.GetBrokerUri()).Build();
            string topicName = $"batch-nacking-{Guid.NewGuid():N}";
            var content = new string[] { "test-1", "test-2", "test-3" };
            string subscriptionName = "test-sub";
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .Create();
            await using var consumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .NegativeAcknowledgementRedeliveryDelay(TimeSpan.FromMilliseconds(nackDelay))
                .Create();
            await using var otherConsumer = client.NewConsumer(Schema.String)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .SubscriptionType(SubscriptionType.Shared)
                .Create();

            //Act
            foreach (var value in content)
                _ = await producer.Send(value);
            var cts = new CancellationTokenSource(1000);
            var messages = new List<IMessage<string>>
            {
                await consumer.Receive(cts.Token),
                await consumer.Receive(cts.Token),
                await consumer.Receive(cts.Token)
            };
            consumer.NegativeAcknowledge(messages[1].MessageId);

            //Assert
            var cancellationToken = new CancellationTokenSource(nackDelay + additionalAllowedTimeToWaitInMs).Token;
            (await otherConsumer.Receive(cancellationToken)).Value().Should().Be(content[1]);
        }
    }
}
