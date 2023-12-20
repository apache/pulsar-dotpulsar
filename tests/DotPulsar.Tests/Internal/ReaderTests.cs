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
using System;
using System.Collections.Generic;
using Xunit.Abstractions;

[Collection("Integration"), Trait("Category", "Integration")]
public sealed class ReaderTests : IDisposable
{
    private readonly CancellationTokenSource _cts;
    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public ReaderTests(IntegrationFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetLastMessageIds_GivenEmptyTopic_ShouldBeEqualToMessageIdEarliest()
    {
        //Arrange
        await using var client = CreateClient();
        await using var reader = CreateReader(client, MessageId.Earliest, await _fixture.CreateTopic());
        var expected = new List<MessageId>() { MessageId.Earliest };

        //Act
        var actual = await reader.GetLastMessageIds(_cts.Token);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetLastMessageIds_GivenNonPartitionedTopic_ShouldGetMessageIdFromPartition()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic();
        const int numberOfMessages = 6;

        await using var client = CreateClient();
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);
        await using var producer = CreateProducer(client, topicName);

        var expected = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send("test-message", _cts.Token);
            if (i >= 5)
            {
                expected.Add(messageId);
            }
        }
        //Act
        var actual = await reader.GetLastMessageIds(_cts.Token);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetLastMessageIds_GivenPartitionedTopic_ShouldGetMessageIdsFromPartitions()
    {
        //Arrange
        const int numberOfMessages = 6;
        const int partitions = 3;
        var topicName = await _fixture.CreatePartitionedTopic(partitions);

        await using var client = CreateClient();
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);
        await using var producer = CreateProducer(client, topicName);

        var expected = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send("test-message", _cts.Token);
            if (i >= 3)
            {
                expected.Add(messageId);
            }
        }

        //Act
        var actual = await reader.GetLastMessageIds(_cts.Token);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Receive_GivenNonPartitionedTopic_ShouldReceiveAll()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic();
        const int numberOfMessages = 10;

        await using var client = CreateClient();
        await using var producer = CreateProducer(client, topicName);
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);

        var expected = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send("test-message", _cts.Token);
            expected.Add(messageId);
        }

        //Act
        var actual = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await reader.Receive(_cts.Token);
            actual.Add(messageId.MessageId);
        }

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Receive_GivenPartitionedTopic_ShouldReceiveAll()
    {
        //Arrange
        const int numberOfMessages = 50;
        const int partitions = 3;
        var topicName = await _fixture.CreatePartitionedTopic(partitions);

        await using var client = CreateClient();
        await using var producer = CreateProducer(client, topicName);
        await using var reader = CreateReader(client, MessageId.Earliest, topicName);

        var expected = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await producer.Send("test-message", _cts.Token);
            expected.Add(messageId);
        }

        //Act
        var actual = new List<MessageId>();
        for (var i = 0; i < numberOfMessages; i++)
        {
            var messageId = await reader.Receive(_cts.Token);
            actual.Add(messageId.MessageId);
        }

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Receive_WhenFaultedAfterInvokingReceive_ShouldThrowReaderFaultedException()
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

        await using var reader = CreateReader(client, MessageId.Earliest, await _fixture.CreateTopic());

        var receiveTask = reader.Receive(_cts.Token).AsTask();
        semaphoreSlim.Release();

        //Act
        var exception = await Record.ExceptionAsync(() => receiveTask);

        //Assert
        exception.Should().BeOfType<ReaderFaultedException>();
    }

    [Fact]
    public async Task Receive_WhenFaultedBeforeInvokingReceive_ShouldThrowReaderFaultedException()
    {
        //Arrange
        await using var client = PulsarClient.Builder().ExceptionHandler(context =>
        {
            context.Result = FaultAction.Rethrow;
            context.ExceptionHandled = true;
        })
        .ServiceUrl(new Uri("pulsar://nosuchhost")).Build();

        await using var reader = CreateReader(client, MessageId.Earliest, await _fixture.CreateTopic());

        await reader.OnStateChangeTo(ReaderState.Faulted, _cts.Token);

        //Act
        var exception = await Record.ExceptionAsync(() => reader.Receive(_cts.Token).AsTask());

        //Assert
        exception.Should().BeOfType<ReaderFaultedException>();
    }

    private IProducer<string> CreateProducer(IPulsarClient pulsarClient, string topicName)
        => pulsarClient.NewProducer(Schema.String)
        .Topic(topicName)
        .StateChangedHandler(_testOutputHelper.Log)
        .Create();

    private IReader<string> CreateReader(IPulsarClient pulsarClient, MessageId messageId, string topicName)
        => pulsarClient.NewReader(Schema.String)
        .StartMessageId(messageId)
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
