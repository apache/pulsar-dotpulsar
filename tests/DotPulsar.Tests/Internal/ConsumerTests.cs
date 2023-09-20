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

namespace DotPulsar.Tests.Internal;

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
    public async Task GetLastMessageId_GivenEmptyTopic_ShouldBeEqualToMessageIdEarliest()
    {
        //Arrange
        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client);

        //Act
        var actual = await consumer.GetLastMessageId();

        //Assert
        actual.Should().BeEquivalentTo(MessageId.Earliest);
    }

    [Fact]
    public async Task GetLastMessageId_GivenNonPartitionedTopic_ShouldGetMessageIdFromPartition()
    {
        //Arrange
        var topicName = CreateTopicName();
        const int numberOfMessages = 6;

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        MessageId expected = null!;
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send("test-message");
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
        var topicName = CreateTopicName();
        const int partitions = 3;
        _fixture.CreatePartitionedTopic(topicName, partitions);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);

        //Act
        var exception = await Record.ExceptionAsync(() => consumer.GetLastMessageId().AsTask());

        //Assert
        exception.Should().BeOfType<NotSupportedException>();
    }

    [Fact]
    public async Task GetLastMessageIds_GivenNonPartitionedTopic_ShouldGetMessageIdFromPartition()
    {
        //Arrange
        var topicName = CreateTopicName();
        const int numberOfMessages = 6;

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        var expected = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send("test-message");
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
        var topicName = CreateTopicName();
        const int numberOfMessages = 6;
        const int partitions = 3;
        _fixture.CreatePartitionedTopic(topicName, partitions);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        var expected = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send("test-message");
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
    public async Task GetLastMessageIds_GivenEmptyTopic_ShouldBeEqualToMessageIdEarliest()
    {
        //Arrange
        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client);
        var expected = new List<MessageId>() { MessageId.Earliest };

        //Act
        var actual = await consumer.GetLastMessageIds();

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Receive_GivenNonPartitionedTopic_ShouldReceiveAll()
    {
        //Arrange
        var topicName = CreateTopicName();
        const int numberOfMessages = 10000;

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        //Act
        var produced = await ProduceMessages(producer, numberOfMessages, "test-message", cts.Token);
        var consumed = await ConsumeMessages(consumer, numberOfMessages, cts.Token);

        //Assert
        consumed.Should().BeEquivalentTo(produced);
    }

    [Fact]
    public async Task Receive_GivenPartitionedTopic_ShouldReceiveAll()
    {
        //Arrange
        var topicName = CreateTopicName();
        const int numberOfMessages = 1000;
        const int partitions = 3;

        _fixture.CreatePartitionedTopic(topicName, partitions);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        //Act
        var produced = await ProduceMessages(producer, numberOfMessages, "test-message", cts.Token);
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

        await using var consumer = CreateConsumer(client);

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

        await using var consumer = CreateConsumer(client);

        await consumer.OnStateChangeTo(ConsumerState.Faulted);

        //Act
        var exception = await Record.ExceptionAsync(() => consumer.Receive().AsTask());

        //Assert
        exception.Should().BeOfType<ConsumerFaultedException>();
    }

    private static async Task<IEnumerable<MessageId>> ProduceMessages(IProducer<string> producer, int numberOfMessages, string content, CancellationToken ct)
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

    private static string CreateTopicName() => $"persistent://public/default/{Guid.NewGuid():N}";
    private static string CreateConsumerName() => $"consumer-{Guid.NewGuid():N}";
    private static string CreateSubscriptionName() => $"subscription-{Guid.NewGuid():N}";

    private IProducer<string> CreateProducer(IPulsarClient pulsarClient, string? topicName = null)
        => pulsarClient.NewProducer(Schema.String)
        .Topic(topicName is null ? CreateTopicName() : topicName)
        .StateChangedHandler(LogState)
        .Create();

    private IConsumer<string> CreateConsumer(IPulsarClient pulsarClient, string? topicName = null)
        => pulsarClient.NewConsumer(Schema.String)
        .ConsumerName(CreateConsumerName())
        .InitialPosition(SubscriptionInitialPosition.Earliest)
        .SubscriptionName(CreateSubscriptionName())
        .Topic(topicName is null ? CreateTopicName() : topicName)
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
