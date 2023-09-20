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
using DotPulsar.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

[Collection("Integration"), Trait("Category", "Integration")]
public class ProducerTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(30);
    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public ProducerTests(IntegrationFixture fixture, ITestOutputHelper outputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = outputHelper;
    }

    [Fact]
    public async Task SimpleProduceConsume_WhenSendingMessagesToProducer_ThenReceiveMessagesFromConsumer()
    {
        //Arrange
        const string content = "test-message";
        var topicName = CreateTopicName();
        await using var client = CreateClient();
        await using var producer = CreateProducer(client, topicName);
        await using var consumer = CreateConsumer(client, topicName);

        //Act
        await producer.Send(content);
        var message = await consumer.Receive();

        //Assert
        message.Value().Should().Be(content);
    }

    [Fact]
    public async Task SimpleProduceConsume_WhenSendingWithChannel_ThenReceiveMessagesFromConsumer()
    {
        //Arrange
        const string content = "test-message";
        var topicName = CreateTopicName();
        await using var client = CreateClient();
        await using var producer = CreateProducer(client, topicName);
        await using var consumer = CreateConsumer(client, topicName);
        const int msgCount = 3;

        //Act
        for (var i = 0; i < msgCount; i++)
        {
            await producer.SendChannel.Send(content);
            _testOutputHelper.WriteLine($"Sent a message: {content}");
        }

        producer.SendChannel.Complete();
        await producer.SendChannel.Completion();

        //Assert
        for (ulong i = 0; i < msgCount; i++)
        {
            var received = await consumer.Receive();
            received.SequenceId.Should().Be(i);
            received.Value().Should().Be(content);
        }
    }

    [Theory]
    [InlineData(ProducerAccessMode.Shared, ProducerState.Connected)]
    [InlineData(ProducerAccessMode.Exclusive, ProducerState.Fenced)]
    [InlineData(ProducerAccessMode.WaitForExclusive, ProducerState.WaitingForExclusive)]
    public async Task TwoProducers_WhenConnectingSecond_ThenGoToExpectedState(ProducerAccessMode accessMode, ProducerState expectedState)
    {
        //Arrange
        var topicName = $"producer-access-mode{Guid.NewGuid():N}";
        var cts = new CancellationTokenSource(TestTimeout);
        await using var client = CreateClient();

        //Act
        await using var producer1 = CreateProducer(client, topicName, accessMode);
        _ = await producer1.OnStateChangeTo(ProducerState.Connected, cts.Token);

        await using var producer2 = CreateProducer(client, topicName, accessMode);
        var actualState = await producer2.OnStateChangeTo(expectedState, cts.Token);

        //Assert
        actualState.Should().Be(expectedState);
    }

    [Fact]
    public async Task TwoProducers_WhenUsingExclusiveWithFencing_ThenExcludeExisting()
    {
        //Arrange
        await using var client = CreateClient();
        var topicName = CreateTopicName();
        var cts = new CancellationTokenSource(TestTimeout);

        await using var producer1 = CreateProducer(client, topicName, ProducerAccessMode.ExclusiveWithFencing);
        await producer1.OnStateChangeTo(ProducerState.Connected, cts.Token);

        //Act
        await using var producer2 = CreateProducer(client, topicName, ProducerAccessMode.ExclusiveWithFencing);
        await producer2.OnStateChangeTo(ProducerState.Connected, cts.Token);

        try
        {
            // We need to send a message to trigger the disconnect
            await producer1.Send(topicName, cts.Token);
        }
        catch
        {
            //Ignore
        }

        var result = await producer1.OnStateChangeTo(ProducerState.Fenced, cts.Token);

        //Assert
        result.Should().Be(ProducerState.Fenced);
    }

    [Theory]
    [InlineData(ProducerAccessMode.Exclusive, ProducerAccessMode.Shared, ProducerState.Connected, ProducerState.Disconnected)]
    [InlineData(ProducerAccessMode.Shared, ProducerAccessMode.Exclusive, ProducerState.Connected, ProducerState.Fenced)]
    [InlineData(ProducerAccessMode.Shared, ProducerAccessMode.WaitForExclusive, ProducerState.Connected, ProducerState.WaitingForExclusive)]
    [InlineData(ProducerAccessMode.Exclusive, ProducerAccessMode.WaitForExclusive, ProducerState.Connected, ProducerState.WaitingForExclusive)]
    public async Task TwoProducers_WhenUsingDifferentAccessModes_ThenGoToExpectedStates(ProducerAccessMode accessMode1, ProducerAccessMode accessMode2, ProducerState producerState1, ProducerState producerState2)
    {
        //Arrange
        var topicName = CreateTopicName();
        var cts = new CancellationTokenSource(TestTimeout);
        await using var client = CreateClient();

        await using var producer1 = CreateProducer(client, topicName, accessMode1);
        await producer1.OnStateChangeTo(ProducerState.Connected, cts.Token);

        //Act
        await using var producer2 = CreateProducer(client, topicName, accessMode2);

        var result1 = await producer1.OnStateChangeTo(producerState1, cts.Token);
        var result2 = await producer2.OnStateChangeTo(producerState2, cts.Token);

        //Assert
        result1.Should().Be(producerState1);
        result2.Should().Be(producerState2);
    }

    [Fact]
    public async Task SinglePartition_WhenSendMessages_ThenGetMessagesFromSinglePartition()
    {
        //Arrange
        const string content = "test-message";
        const int partitions = 3;
        const int msgCount = 3;
        var topicName = CreateTopicName();
        _fixture.CreatePartitionedTopic(topicName, partitions);
        await using var client = CreateClient();

        //Act
        var consumers = new List<IConsumer<string>>();
        for (var i = 0; i < partitions; ++i)
        {
            consumers.Add(CreateConsumer(client, $"{topicName}-partition-{i}"));
        }

        for (var i = 0; i < partitions; ++i)
        {
            await using var producer = client.NewProducer(Schema.String)
                .Topic(topicName)
                .MessageRouter(new SinglePartitionRouter(i))
                .Create();

            for (var msgIndex = 0; msgIndex < msgCount; ++msgIndex)
            {
                var message = $"{content}-{i}-{msgIndex}";
                _ = await producer.Send(message);
                _testOutputHelper.WriteLine($"Sent a message: {message}");
            }
        }

        //Assert
        for (var i = 0; i < partitions; ++i)
        {
            var consumer = consumers[i];

            for (var msgIndex = 0; msgIndex < msgCount; ++msgIndex)
            {
                var message = await consumer.Receive();
                message.Value().Should().Be($"{content}-{i}-{msgIndex}");
            }
        }
    }

    [Fact]
    public async Task RoundRobinPartition_WhenSendMessages_ThenGetMessagesFromPartitionsInOrder()
    {
        //Arrange
        await using var client = CreateClient();

        var topicName = CreateTopicName();
        const string content = "test-message";
        const int partitions = 3;
        var consumers = new List<IConsumer<string>>();

        _fixture.CreatePartitionedTopic(topicName, partitions);

        //Act
        await using var producer = CreateProducer(client, topicName);

        for (var i = 0; i < partitions; ++i)
        {
            consumers.Add(CreateConsumer(client, $"{topicName}-partition-{i}"));
            await producer.Send($"{content}-{i}");
            _testOutputHelper.WriteLine($"Sent a message to consumer [{i}]");
        }

        //Assert
        for (var i = 0; i < partitions; ++i)
        {
            (await consumers[i].Receive()).Value().Should().Be($"{content}-{i}");
        }
    }

    [Fact]
    public async Task Send_WhenProducingMessagesForOnePartition_ShouldPartitionOnlyBeNegativeOne()
    {
        //Arrange
        const int numberOfMessages = 10;
        var topicName = CreateTopicName();

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        //Act
        var produced = await ProduceMessages(producer, numberOfMessages, cts.Token);
        var consumed = await ConsumeMessages(consumer, numberOfMessages, cts.Token);

        //Assert
        var foundNonNegativeOne = false;
        foreach (var messageId in produced)
        {
            if (messageId.Partition.Equals(-1) != true)
                foundNonNegativeOne = true;
        }
        foreach (var messageId in consumed)
        {
            if (messageId.Partition.Equals(-1) != true)
                foundNonNegativeOne = true;
        }

        foundNonNegativeOne.Should().Be(false);
    }

    [Fact]
    public async Task Send_WhenProducingMessagesForFourPartitions_ShouldPartitionBeDifferentThanNegativeOne()
    {
        //Arrange
        const int numberOfMessages = 10;
        const int partitions = 4;
        var topicName = CreateTopicName();

        _fixture.CreatePartitionedTopic(topicName, partitions);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        //Act
        var produced = await ProduceMessages(producer, numberOfMessages, cts.Token);
        var consumed = await ConsumeMessages(consumer, numberOfMessages, cts.Token);

        //Assert
        var foundNonNegativeOne = false;
        foreach (var messageId in produced)
        {
            if (!messageId.Partition.Equals(-1))
                foundNonNegativeOne = true;
        }
        foreach (var messageId in consumed)
        {
            if (!messageId.Partition.Equals(-1))
                foundNonNegativeOne = true;
        }

        foundNonNegativeOne.Should().Be(true);
    }

    private static async Task<IEnumerable<MessageId>> ProduceMessages(IProducer<string> producer, int numberOfMessages, CancellationToken ct)
    {
        var messageIds = new MessageId[numberOfMessages];

        for (var i = 0; i < numberOfMessages; ++i)
        {
            messageIds[i] = await producer.Send($"Sent #{i} at {DateTimeOffset.UtcNow:s}", ct);
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

    private IProducer<string> CreateProducer(
        IPulsarClient pulsarClient,
        string? topicName = null,
        ProducerAccessMode producerAccessMode = ProducerAccessMode.Shared)
        =>  pulsarClient.NewProducer(Schema.String)
        .Topic(topicName is null ? CreateTopicName() : topicName)
        .ProducerAccessMode(producerAccessMode)
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
