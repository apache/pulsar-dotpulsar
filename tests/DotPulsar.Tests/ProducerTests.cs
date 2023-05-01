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
        await using var client = CreateClient();
        var topicName = $"simple-produce-consume{Guid.NewGuid():N}";
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
    public async Task SimpleProduceConsume_WhenSendingWithChannel_ThenReceiveMessagesFromConsumer()
    {
        //Arrange
        await using var client = CreateClient();
        var topicName = $"simple-produce-consume{Guid.NewGuid():N}";
        const string content = "test-message";
        const int msgCount = 3;

        //Act
        await using var producer = client.NewProducer(Schema.String)
            .Topic(topicName)
            .Create();

        await using var consumer = client.NewConsumer(Schema.String)
            .Topic(topicName)
            .SubscriptionName("test-sub")
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();

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
        await using var client = CreateClient();
        var topicName = $"producer-access-mode{Guid.NewGuid():N}";
        var cts = new CancellationTokenSource(TestTimeout);

        await using var producer1 = client.NewProducer(Schema.String)
            .StateChangedHandler(x => _testOutputHelper.WriteLine($"Producer 1 changed to state: {x.ProducerState}"))
            .ProducerAccessMode(accessMode)
            .Topic(topicName)
            .Create();
        await producer1.OnStateChangeTo(ProducerState.Connected, cts.Token);

        //Act
        await using var producer2 = client.NewProducer(Schema.String)
            .StateChangedHandler(x => _testOutputHelper.WriteLine($"Producer 2 changed to state: {x.ProducerState}"))
            .ProducerAccessMode(accessMode)
            .Topic(topicName)
            .Create();

        var result = await producer2.OnStateChangeTo(expectedState, cts.Token);

        //Assert
        result.Should().Be(expectedState);
    }

    [Fact]
    public async Task TwoProducers_WhenUsingExclusiveWithFencing_ThenExcludeExisting()
    {
        //Arrange
        await using var client = CreateClient();
        var topicName = $"producer-access-mode{Guid.NewGuid():N}";
        var cts = new CancellationTokenSource(TestTimeout);

        await using var producer1 = client.NewProducer(Schema.String)
            .StateChangedHandler(x => _testOutputHelper.WriteLine($"Producer 1 changed to state: {x.ProducerState}"))
            .ProducerAccessMode(ProducerAccessMode.ExclusiveWithFencing)
            .Topic(topicName)
            .Create();
        await producer1.OnStateChangeTo(ProducerState.Connected, cts.Token);

        //Act
        await using var producer2 = client.NewProducer(Schema.String)
            .StateChangedHandler(x => _testOutputHelper.WriteLine($"Producer 2 changed to state: {x.ProducerState}"))
            .ProducerAccessMode(ProducerAccessMode.ExclusiveWithFencing)
            .Topic(topicName)
            .Create();
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
        await using var client = CreateClient();
        var topicName = $"producer-access-mode{Guid.NewGuid():N}";
        var cts = new CancellationTokenSource(TestTimeout);

        await using var producer1 = client.NewProducer(Schema.String)
            .StateChangedHandler(x => _testOutputHelper.WriteLine($"Producer 1 changed to state: {x.ProducerState}"))
            .ProducerAccessMode(accessMode1)
            .Topic(topicName)
            .Create();
        await producer1.OnStateChangeTo(ProducerState.Connected, cts.Token);

        //Act
        await using var producer2 = client.NewProducer(Schema.String)
            .StateChangedHandler(x => _testOutputHelper.WriteLine($"Producer 2 changed to state: {x.ProducerState}"))
            .ProducerAccessMode(accessMode2)
            .Topic(topicName)
            .Create();

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
        var topicName = $"single-partitioned-{Guid.NewGuid():N}";
        _fixture.CreatePartitionedTopic($"persistent://public/default/{topicName}", partitions);
        await using var client = CreateClient();

        //Act
        var consumers = new List<IConsumer<string>>();
        for (var i = 0; i < partitions; ++i)
        {
            consumers.Add(client.NewConsumer(Schema.String)
                .Topic($"{topicName}-partition-{i}")
                .SubscriptionName("test-sub")
                .InitialPosition(SubscriptionInitialPosition.Earliest)
                .Create());
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

        var topicName = $"round-robin-partitioned-{Guid.NewGuid():N}";
        const string content = "test-message";
        const int partitions = 3;
        var consumers = new List<IConsumer<string>>();

        _fixture.CreatePartitionedTopic($"persistent://public/default/{topicName}", partitions);

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
            await producer.Send($"{content}-{i}");
            _testOutputHelper.WriteLine($"Sent a message to consumer [{i}]");
        }

        //Assert
        for (var i = 0; i < partitions; ++i)
        {
            (await consumers[i].Receive()).Value().Should().Be($"{content}-{i}");
        }
    }

    private IPulsarClient CreateClient()
        => PulsarClient
            .Builder()
            .Authentication(AuthenticationFactory.Token(ct => ValueTask.FromResult(_fixture.CreateToken(Timeout.InfiniteTimeSpan))))
            .ExceptionHandler(ec => _testOutputHelper.WriteLine($"Exception: {ec.Exception}"))
            .ServiceUrl(_fixture.ServiceUrl)
            .Build();
}
