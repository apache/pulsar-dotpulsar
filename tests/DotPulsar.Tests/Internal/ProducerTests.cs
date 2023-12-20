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
using System;
using System.Collections.Generic;
using Xunit.Abstractions;

[Collection("Integration"), Trait("Category", "Integration")]
public sealed class ProducerTests : IDisposable
{
    private readonly CancellationTokenSource _cts;
    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public ProducerTests(IntegrationFixture fixture, ITestOutputHelper outputHelper)
    {
        _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        _fixture = fixture;
        _testOutputHelper = outputHelper;
    }

    [Fact]
    public async Task Send_GivenMessageWasSent_ShouldBeConsumable()
    {
        //Arrange
        const string content = "test-message";
        var topicName = await _fixture.CreateTopic();
        await using var client = CreateClient();
        await using var producer = CreateProducer(client, topicName);
        await using var consumer = CreateConsumer(client, topicName);

        //Act
        var messageId = await producer.Send(content, _cts.Token);
        var message = await consumer.Receive(_cts.Token);

        //Assert
        message.MessageId.Should().Be(messageId);
        message.Value().Should().Be(content);
    }

    [Fact]
    public async Task SendChannel_GivenMessageWasSent_ShouldBeConsumable()
    {
        //Arrange
        const string content = "test-message";
        var topicName = await _fixture.CreateTopic();
        await using var client = CreateClient();
        await using var producer = CreateProducer(client, topicName);
        await using var consumer = CreateConsumer(client, topicName);
        var idTask = new TaskCompletionSource<MessageId>(TaskCreationOptions.RunContinuationsAsynchronously);

        ValueTask SetMessageId(MessageId id)
        {
            idTask.SetResult(id);
            return ValueTask.CompletedTask;
        }

        //Act
        await producer.SendChannel.Send(content, SetMessageId, _cts.Token);
        producer.SendChannel.Complete();
        await producer.SendChannel.Completion(_cts.Token);
        var message = await consumer.Receive(_cts.Token);
        var expected = await idTask.Task;

        //Assert
        message.MessageId.Should().Be(expected);
        message.Value().Should().Be(content);
    }

    [Theory]
    [InlineData(ProducerAccessMode.Exclusive, ProducerAccessMode.Exclusive, ProducerState.Connected, ProducerState.Fenced)]
    [InlineData(ProducerAccessMode.Exclusive, ProducerAccessMode.ExclusiveWithFencing, ProducerState.Fenced, ProducerState.Connected)]
    [InlineData(ProducerAccessMode.Exclusive, ProducerAccessMode.WaitForExclusive, ProducerState.Connected, ProducerState.WaitingForExclusive)]
    [InlineData(ProducerAccessMode.Exclusive, ProducerAccessMode.Shared, ProducerState.Connected, ProducerState.Disconnected)] // Rethrow on ProducerBusy to Fault instead of just Disconnect
    [InlineData(ProducerAccessMode.ExclusiveWithFencing, ProducerAccessMode.Exclusive, ProducerState.Connected, ProducerState.Fenced)]
    [InlineData(ProducerAccessMode.ExclusiveWithFencing, ProducerAccessMode.ExclusiveWithFencing, ProducerState.Fenced, ProducerState.Connected)]
    [InlineData(ProducerAccessMode.ExclusiveWithFencing, ProducerAccessMode.WaitForExclusive, ProducerState.Connected, ProducerState.WaitingForExclusive)]
    [InlineData(ProducerAccessMode.ExclusiveWithFencing, ProducerAccessMode.Shared, ProducerState.Connected, ProducerState.Disconnected)]
    [InlineData(ProducerAccessMode.Shared, ProducerAccessMode.Exclusive, ProducerState.Connected, ProducerState.Fenced)]
    //[InlineData(ProducerAccessMode.Shared, ProducerAccessMode.ExclusiveWithFencing, ProducerState.Connected, ProducerState.Fenced)]
    [InlineData(ProducerAccessMode.Shared, ProducerAccessMode.Shared, ProducerState.Connected, ProducerState.Connected)]
    [InlineData(ProducerAccessMode.Shared, ProducerAccessMode.WaitForExclusive, ProducerState.Connected, ProducerState.WaitingForExclusive)]
    [InlineData(ProducerAccessMode.WaitForExclusive, ProducerAccessMode.Exclusive, ProducerState.Connected, ProducerState.Fenced)]
    [InlineData(ProducerAccessMode.WaitForExclusive, ProducerAccessMode.ExclusiveWithFencing, ProducerState.Fenced, ProducerState.Connected)]
    [InlineData(ProducerAccessMode.WaitForExclusive, ProducerAccessMode.WaitForExclusive, ProducerState.Connected, ProducerState.WaitingForExclusive)]
    [InlineData(ProducerAccessMode.WaitForExclusive, ProducerAccessMode.Shared, ProducerState.Connected, ProducerState.Disconnected)] // Rethrow on ProducerBusy to Fault instead of just Disconnect
    public async Task State_GivenMultipleProducersWithDifferentAccessModes_ThenGoToTheExpectedStates(
        ProducerAccessMode accessModeForProducer1,
        ProducerAccessMode accessModeForProducer2,
        ProducerState expectedStateForProducer1,
        ProducerState expectedStateForProducer2)
    {
        //Arrange
        var topicName = await _fixture.CreateTopic();
        await using var client = CreateClient();
        await using var producer1 = CreateProducer(client, topicName, accessModeForProducer1);
        await producer1.OnStateChangeTo(ProducerState.Connected, _cts.Token);

        //Act
        await using var producer2 = CreateProducer(client, topicName, accessModeForProducer2);

        if (accessModeForProducer2 == ProducerAccessMode.ExclusiveWithFencing) // We need to send a message to trigger the state change
        {
            await producer2.OnStateChangeTo(ProducerState.Connected, _cts.Token);

            try
            {
                _ = producer1.Send("test", _cts.Token);
            }
            catch
            {
                //Ignore
            }
        }

        var actualStateForProducer1 = await producer1.OnStateChangeTo(expectedStateForProducer1, _cts.Token);
        var actualStateForProducer2 = await producer2.OnStateChangeTo(expectedStateForProducer2, _cts.Token);

        //Assert
        actualStateForProducer1.Should().Be(expectedStateForProducer1);
        actualStateForProducer2.Should().Be(expectedStateForProducer2);
    }

    [Fact]
    public async Task SinglePartition_WhenSendMessages_ThenGetMessagesFromSinglePartition()
    {
        //Arrange
        const string content = "test-message";
        const int partitions = 3;
        const int msgCount = 3;
        var topicName = await _fixture.CreatePartitionedTopic(partitions);
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
                _ = await producer.Send(message, _cts.Token);
                _testOutputHelper.Log($"Sent a message: {message}");
            }
        }

        //Assert
        for (var i = 0; i < partitions; ++i)
        {
            var consumer = consumers[i];

            for (var msgIndex = 0; msgIndex < msgCount; ++msgIndex)
            {
                var message = await consumer.Receive(_cts.Token);
                message.Value().Should().Be($"{content}-{i}-{msgIndex}");
            }
        }
    }

    [Fact]
    public async Task RoundRobinPartition_WhenSendMessages_ThenGetMessagesFromPartitionsInOrder()
    {
        //Arrange
        const string content = "test-message";
        const int partitions = 3;
        var consumers = new List<IConsumer<string>>();
        await using var client = CreateClient();
        var topicName = await _fixture.CreatePartitionedTopic(partitions);

        //Act
        await using var producer = CreateProducer(client, topicName);

        for (var i = 0; i < partitions; ++i)
        {
            consumers.Add(CreateConsumer(client, $"{topicName}-partition-{i}"));
            await producer.Send($"{content}-{i}", _cts.Token);
            _testOutputHelper.Log($"Sent a message to consumer [{i}]");
        }

        //Assert
        for (var i = 0; i < partitions; ++i)
        {
            (await consumers[i].Receive(_cts.Token)).Value().Should().Be($"{content}-{i}");
        }
    }

    [Fact]
    public async Task Send_WhenProducingMessagesForOnePartition_ShouldPartitionOnlyBeNegativeOne()
    {
        //Arrange
        const int numberOfMessages = 10;
        var topicName = await _fixture.CreateTopic();

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        //Act
        var produced = await ProduceMessages(producer, numberOfMessages, _cts.Token);
        var consumed = await ConsumeMessages(consumer, numberOfMessages, _cts.Token);

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
        var topicName = await _fixture.CreatePartitionedTopic(partitions);
        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        //Act
        var produced = await ProduceMessages(producer, numberOfMessages, _cts.Token);
        var consumed = await ConsumeMessages(consumer, numberOfMessages, _cts.Token);

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

    private static string CreateSubscriptionName() => $"subscription-{Guid.NewGuid():N}";

    private IProducer<string> CreateProducer(
        IPulsarClient pulsarClient,
        string topicName,
        ProducerAccessMode producerAccessMode = ProducerAccessMode.Shared)
        => pulsarClient.NewProducer(Schema.String)
        .Topic(topicName)
        .ProducerAccessMode(producerAccessMode)
        .StateChangedHandler(_testOutputHelper.Log)
        .Create();

    private IConsumer<string> CreateConsumer(IPulsarClient pulsarClient, string topicName)
        => pulsarClient.NewConsumer(Schema.String)
        .InitialPosition(SubscriptionInitialPosition.Earliest)
        .SubscriptionName(CreateSubscriptionName())
        .Topic(topicName)
        .StateChangedHandler(_testOutputHelper.Log)
        .Create();

    private IPulsarClient CreateClient()
        => PulsarClient
        .Builder()
        .Authentication(_fixture.Authentication)
        .ExceptionHandler(_testOutputHelper.Log)
        .ServiceUrl(_fixture.ServiceUrl)
        .Build();

    public void Dispose() => _cts.Dispose();
}
