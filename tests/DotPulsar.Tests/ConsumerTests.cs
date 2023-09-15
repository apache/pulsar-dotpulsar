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

namespace DotPulsar.Tests;

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

[Collection("Integration"), Trait("Category", "Integration")]
public class ConsumerTests
{
    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public ConsumerTests(IntegrationFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetLastMessageId_GivenNonPartitionedTopic_ShouldGetMessageIdFromPartition()
    {
        //Arrange
        const string topicName = "persistent://public/default/consumer-getlastmessageid-given-non-partitioned-topic";
        const string subscriptionName = "subscription-given-given-partitioned-topic";
        const string consumerName = $"consumer-given-partitioned-topic";
        const string content = "test-message";
        const int numberOfMessages = 6;

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, SubscriptionInitialPosition.Earliest, topicName, consumerName, subscriptionName);
        await using var producer = CreateProducer(client, topicName);

        MessageId expected = null!;
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send(content);
            if (i >= 5)
            {
                expected = messageId;
            }
        }

        //Act
        var actual = await consumer.GetLastMessageId();

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetLastMessageId_GivenPartitionedTopic_ShouldThrowException()
    {
        //Arrange
        const string topicName = "persistent://public/default/consumer-getlastmessageid-given-partitioned-topic";
        const string subscriptionName = "subscription-given-given-partitioned-topic";
        const string consumerName = "consumer-given-partitioned-topic";
        const int partitions = 3;
        _fixture.CreatePartitionedTopic(topicName, partitions);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, SubscriptionInitialPosition.Earliest, topicName, consumerName, subscriptionName);

        //Act
        var exception = await Record.ExceptionAsync(() => consumer.GetLastMessageId().AsTask());

        //Assert
        exception.Should().BeOfType<NotSupportedException>();
    }

    [Fact]
    public async Task GetLastMessageIds_GivenNonPartitionedTopic_ShouldGetMessageIdFromPartition()
    {
        //Arrange
        const string topicName = "consumer-getlastmessageids-given-non-partitioned-topic";
        const string subscriptionName = "subscription-should_have-3-topics";
        const string consumerName = "consumer-should_have-3-topics";
        const string content = "test-message";
        const int numberOfMessages = 6;

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, SubscriptionInitialPosition.Earliest, topicName, consumerName, subscriptionName);
        await using var producer = CreateProducer(client, topicName);

        var expected = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send(content);
            if (i >= 5)
            {
                expected.Add(messageId);
            }
        }

        //Act
        var actual = await consumer.GetLastMessageIds();

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetLastMessageIds_GivenPartitionedTopic_ShouldGetMessageIdFromAllPartitions()
    {
        //Arrange
        const string topicName = "consumer-getlastmessageids-given-partitioned-topic";
        const string subscriptionName = "subscription-should_have-3-topics";
        const string consumerName = "consumer-should_have-3-topics";
        const string content = "test-message";
        const int numberOfMessages = 6;
        const int partitions = 3;
        _fixture.CreatePartitionedTopic($"persistent://public/default/{topicName}", partitions);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, SubscriptionInitialPosition.Earliest, topicName, consumerName, subscriptionName);
        await using var producer = CreateProducer(client, topicName);

        var expected = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send(content);
            if (i >= 3)
            {
                expected.Add(messageId);
            }
        }

        //Act
        var actual = await consumer.GetLastMessageIds();

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Receive_GivenNonPartitionedTopic_ShouldReceiveAll()
    {
        //Arrange
        const string topicName = "persistent://public/default/consumer-given-topic-with-messages";
        const string subscriptionName = "subscription-given-topic-with-messages";
        const string consumerName = "consumer-given-topic-with-messages";
        const string content = "test-message";
        const int numberOfMessages = 10000;

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, SubscriptionInitialPosition.Earliest, topicName, consumerName, subscriptionName);
        await using var producer = CreateProducer(client, topicName);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        //Act
        var produced = await ProduceMessages(producer, numberOfMessages, cts.Token, content);
        var consumed = await ConsumeMessages(consumer, numberOfMessages, cts.Token);

        //Assert
        consumed.Should().BeEquivalentTo(produced);
    }

    [Fact]
    public async Task Receive_GivenPartitionedTopic_ShouldReceiveAll()
    {
        //Arrange
        const string subscriptionName = "subscription-given-non-partitioned-topic-with-messages";
        const string consumerName = "consumer-given-non-partitioned-topic-with-messages";
        const string topicName = "consumer-with-3-partitions-test";
        const string content = "test-message";
        const int numberOfMessages = 10000;
        const int partitions = 3;

        _fixture.CreatePartitionedTopic($"persistent://public/default/{topicName}", partitions);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, SubscriptionInitialPosition.Earliest, topicName, consumerName, subscriptionName);
        await using var producer = CreateProducer(client, topicName);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        //Act
        var produced = await ProduceMessages(producer, numberOfMessages, cts.Token, content);
        var consumed = await ConsumeMessages(consumer, numberOfMessages, cts.Token);

        //Assert
        consumed.Should().BeEquivalentTo(produced);
    }

    [Fact]
    public async Task Receive_WhenFaultedAfterInvokingReceive_ShouldThrowConsumerFaultedException()
    {
        //Arrange
        var semaphoreSlim = new SemaphoreSlim(1);
        await using var client = PulsarClient.Builder().ExceptionHandler(context =>
        {
            semaphoreSlim.WaitAsync();
            context.Result = FaultAction.Rethrow;
            context.ExceptionHandled = true;
        })
        .ServiceUrl(new Uri("pulsar://nosuchhost")).Build();

        await using var consumer = CreateConsumer(client, SubscriptionInitialPosition.Earliest, "persistent://a/b/c", "cn", "sn");

        var receiveTask = consumer.Receive().AsTask();
        semaphoreSlim.Release();

        //Act
        var exception = await Record.ExceptionAsync(() => receiveTask);

        //Assert
        exception.Should().BeOfType<ConsumerFaultedException>();
    }

    [Fact]
    public async Task Receive_WhenFaultedBeforeInvokingReceive_ShouldThrowConsumerFaultedException()
    {
        //Arrange
        await using var client = PulsarClient.Builder().ExceptionHandler(context =>
        {
            context.Result = FaultAction.Rethrow;
            context.ExceptionHandled = true;
        })
        .ServiceUrl(new Uri("pulsar://nosuchhost")).Build();

        await using var consumer = CreateConsumer(client, SubscriptionInitialPosition.Earliest, "persistent://a/b/c", "cn", "sn");

        await consumer.OnStateChangeTo(ConsumerState.Faulted);

        //Act
        var exception = await Record.ExceptionAsync(() => consumer.Receive().AsTask());

        //Assert
        exception.Should().BeOfType<ConsumerFaultedException>();
    }

    private static async Task<IEnumerable<MessageId>> ProduceMessages(IProducer<string> producer, int numberOfMessages, CancellationToken ct, string content)
    {
        var messageIds = new MessageId[numberOfMessages];

        for (var i = 0; i < numberOfMessages; ++i)
        {
            messageIds[i] = await producer.Send(content, ct);
        }

        return messageIds;
    }

    private static async Task<IEnumerable<MessageId>> ConsumeMessages(IConsumer<string> consumer, int numberOfMessages, CancellationToken ct)
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

    private void LogState(ConsumerStateChanged stateChange)
        => _testOutputHelper.WriteLine($"The consumer for topic '{stateChange.Consumer.Topic}' changed state to '{stateChange.ConsumerState}'");

    private void LogState(ProducerStateChanged stateChange)
        => _testOutputHelper.WriteLine($"The producer for topic '{stateChange.Producer.Topic}' changed state to '{stateChange.ProducerState}'");

    private IProducer<string> CreateProducer(IPulsarClient pulsarClient, string topicName)
        => pulsarClient.NewProducer(Schema.String)
        .Topic(topicName)
        .StateChangedHandler(LogState)
        .Create();

    private IConsumer<string> CreateConsumer(
        IPulsarClient pulsarClient,
        SubscriptionInitialPosition subscriptionInitialPosition,
        string topicName,
        string consumerName,
        string subscriptionName)
        => pulsarClient.NewConsumer(Schema.String)
        .ConsumerName(consumerName)
        .InitialPosition(subscriptionInitialPosition)
        .SubscriptionName(subscriptionName)
        .Topic(topicName)
        .StateChangedHandler(LogState)
        .Create();

    private IPulsarClient CreateClient()
        => PulsarClient
            .Builder()
            .Authentication(AuthenticationFactory.Token(ct => ValueTask.FromResult(_fixture.CreateToken(Timeout.InfiniteTimeSpan))))
            .ExceptionHandler(ec => _testOutputHelper.WriteLine($"Exception: {ec.Exception}"))
            .ServiceUrl(_fixture.ServiceUrl)
            .Build();
}
