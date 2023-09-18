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
public class ReaderTests
{
    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public ReaderTests(IntegrationFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetLastMessageId_GivenEmptyTopic_ShouldBeEqualToMessageIdEarliest()
    {
        //Arrange
        const string topicName = $"reader-{nameof(GetLastMessageId_GivenEmptyTopic_ShouldBeEqualToMessageIdEarliest)}";
        await using var client = CreateClient();
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);

        //Act
        var actual = await reader.GetLastMessageId();

        //Assert
        actual.Should().BeEquivalentTo(MessageId.Earliest);
    }

    [Fact]
    public async Task GetLastMessageId_GivenNonPartitionedTopic_ShouldGetMessageId()
    {
        //Arrange
        const string topicName = "persistent://public/default/reader-non-partitioned-topic-get-last-MessageId";
        const string content = "test-message";
        const int numberOfMessages = 6;

        await using var client = CreateClient();
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);
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
        var actual = await reader.GetLastMessageId();

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetLastMessageId_GivenPartitionedTopic_ShouldThrowException()
    {
        //Arrange
        const string topicName = "persistent://public/default/reader-partitioned-topic-get-last-MessageId";
        const int partitions = 3;
        _fixture.CreatePartitionedTopic(topicName, partitions);

        await using var client = CreateClient();
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);

        //Act
        var exception = await Record.ExceptionAsync(() => reader.GetLastMessageId().AsTask());

        //Assert
        exception.Should().BeOfType<NotSupportedException>();
    }

    [Fact]
    public async Task GetLastMessageIds_GivenEmptyTopic_ShouldBeEqualToMessageIdEarliest()
    {
        //Arrange
        const string topicName = $"reader-{nameof(GetLastMessageIds_GivenEmptyTopic_ShouldBeEqualToMessageIdEarliest)}";
        await using var client = CreateClient();
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);
        var expected = new List<MessageId>() { MessageId.Earliest };

        //Act
        var actual = await reader.GetLastMessageIds();

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetLastMessageIds_GivenNonPartitionedTopic_ShouldGetMessageIdFromPartition()
    {
        //Arrange
        const string topicName = "reader-non-partitioned-topic-get-last-MessageIds";
        const string content = "test-message";
        const int numberOfMessages = 6;

        await using var client = CreateClient();
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);
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
        var actual = await reader.GetLastMessageIds();

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetLastMessageIds_GivenPartitionedTopic_ShouldGetMessageIdsFromPartitions()
    {
        //Arrange
        const string topicName = "reader-partitioned-topic-get-last-MessageIds";
        const string content = "test-message";
        const int numberOfMessages = 6;
        const int partitions = 3;
        _fixture.CreatePartitionedTopic($"persistent://public/default/{topicName}", partitions);

        await using var client = CreateClient();
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);
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
        var actual = await reader.GetLastMessageIds();

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Receive_GivenNonPartitionedTopic_ShouldReceiveAll()
    {
        //Arrange
        const string topicName = "Receive_GivenNonPartitionedTopicWithMessages_ShouldReceiveAll";
        const int numberOfMessages = 10;

        await using var client = CreateClient();
        await using var producer = CreateProducer(client, topicName);
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);

        var expected = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send("test-message");
            expected.Add(messageId);
        }

        //Act
        var actual = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await reader.Receive();
            actual.Add(messageId.MessageId);
        }

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Receive_GivenPartitionedTopic_ShouldReceiveAll()
    {
        //Arrange
        const string topicName = "reader-with-3-partitions-test";
        const int numberOfMessages = 50;
        const int partitions = 3;
        _fixture.CreatePartitionedTopic($"persistent://public/default/{topicName}", partitions);

        await using var client = CreateClient();
        await using var producer = CreateProducer(client, topicName);
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);

        var expected = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send("test-message");
            expected.Add(messageId);
        }

        //Act
        var actual = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await reader.Receive();
            actual.Add(messageId.MessageId);
        }

        //Assert
        actual.Should().BeEquivalentTo(expected);
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

        await using var reader = CreateReader(client, MessageId.Earliest, "persistent://a/b/c");

        var receiveTask = reader.Receive().AsTask();
        semaphoreSlim.Release();

        //Act
        var exception = await Record.ExceptionAsync(() => receiveTask);

        //Assert
        exception.Should().BeOfType<ReaderFaultedException>();
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

        await using var reader = CreateReader(client, MessageId.Earliest, "persistent://a/b/c");

        await reader.OnStateChangeTo(ReaderState.Faulted);

        //Act
        var exception = await Record.ExceptionAsync(() => reader.Receive().AsTask());

        //Assert
        exception.Should().BeOfType<ReaderFaultedException>();
    }

    private void LogState(ReaderStateChanged stateChange)
        => _testOutputHelper.WriteLine($"The reader for topic '{stateChange.Reader.Topic}' changed state to '{stateChange.ReaderState}'");

    private void LogState(ProducerStateChanged stateChange)
        => _testOutputHelper.WriteLine($"The producer for topic '{stateChange.Producer.Topic}' changed state to '{stateChange.ProducerState}'");

    private IProducer<String> CreateProducer(IPulsarClient pulsarClient, string topicName) => pulsarClient.NewProducer(Schema.String)
        .Topic(topicName)
        .StateChangedHandler(LogState)
        .Create();

    private IReader<String> CreateReader(IPulsarClient pulsarClient, MessageId messageId, string topicName) => pulsarClient.NewReader(Schema.String)
        .StartMessageId(messageId)
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
