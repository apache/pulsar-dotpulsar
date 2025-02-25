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
using DotPulsar.Tests.Schemas.TestSamples.AvroModels;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

[Collection("Integration"), Trait("Category", "Integration")]
public sealed class ConsumerTests : IDisposable
{
    private readonly CancellationTokenSource _cts;
    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public ConsumerTests(IntegrationFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetLastMessageIds_GivenNonPartitionedTopic_ShouldGetMessageIdFromPartition()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic(_cts.Token);
        const int numberOfMessages = 6;

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
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
        var actual = await consumer.GetLastMessageIds(_cts.Token);

        //Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public async Task GetLastMessageIds_GivenPartitionedTopic_ShouldGetMessageIdFromAllPartitions()
    {
        //Arrange
        const int numberOfMessages = 6;
        const int partitions = 3;
        var topicName = await _fixture.CreatePartitionedTopic(partitions, _cts.Token);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
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
        var actual = await consumer.GetLastMessageIds(_cts.Token);

        //Assert
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetLastMessageIds_GivenEmptyTopic_ShouldBeEqualToMessageIdEarliest()
    {
        //Arrange
        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, await _fixture.CreateTopic(_cts.Token));
        var expected = new List<MessageId> { MessageId.Earliest };

        //Act
        var actual = await consumer.GetLastMessageIds(_cts.Token);

        //Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public async Task Receive_GivenNonPartitionedTopic_ShouldReceiveAll()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic(_cts.Token);
        const int numberOfMessages = 1000;

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        //Act
        var produced = await ProduceMessages(producer, numberOfMessages, "test-message", _cts.Token);
        var consumed = await ConsumeMessages(consumer, numberOfMessages, _cts.Token);

        //Assert
        consumed.ShouldBe(produced);
    }

    [Fact]
    public async Task Receive_GivenPartitionedTopic_ShouldReceiveAll()
    {
        //Arrange
        const int numberOfMessages = 1000;
        const int partitions = 3;

        var topicName = await _fixture.CreatePartitionedTopic(partitions, _cts.Token);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);

        //Act
        var produced = await ProduceMessages(producer, numberOfMessages, "test-message", _cts.Token);
        var consumed = await ConsumeMessages(consumer, numberOfMessages, _cts.Token);

        //Assert
        consumed.ShouldBe(produced, true);
    }

    [Fact]
    public async Task Receive_GivenMultipleTopics_ShouldReceiveAll()
    {
        //Arrange
        const int numberOfMessages = 100;
        const int partitions = 3;

        var topic = await _fixture.CreateTopic(_cts.Token);
        var partitionedTopic = await _fixture.CreatePartitionedTopic(partitions, _cts.Token);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, [topic, partitionedTopic]);
        await using var producer = CreateProducer(client, topic);
        await using var partitionedProducer = CreateProducer(client, partitionedTopic);

        //Act
        var produced = new List<MessageId>();
        produced.AddRange(await ProduceMessages(producer, numberOfMessages, "test-message", _cts.Token));
        produced.AddRange(await ProduceMessages(partitionedProducer, numberOfMessages, "test-message", _cts.Token));
        var consumed = await ConsumeMessages(consumer, produced.Count, _cts.Token);

        //Assert
        consumed.ShouldBe(produced, true);
    }

    [Fact]
    public async Task Receive_GivenTopicsPattern_ShouldReceiveAll()
    {
        //Arrange
        var match1 = $"persistent://public/default/match-{Guid.NewGuid():N}";
        var match2 = $"persistent://public/default/match-{Guid.NewGuid():N}";
        var nomatch1 = $"non-persistent://public/default/match-{Guid.NewGuid():N}";
        const string nomatch2 = "persistent://public/default/nomatch";

        await _fixture.CreateTopics([match1, match2, nomatch1, nomatch2], _cts.Token);

        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, new Regex(@"persistent://public/default/match.*"));
        await using var producer1 = CreateProducer(client, match1);
        await using var producer2 = CreateProducer(client, match2);
        await using var producer3 = CreateProducer(client, nomatch1);
        await using var producer4 = CreateProducer(client, nomatch2);

        //Act
        var produced = new List<MessageId>();
        produced.AddRange(await ProduceMessages(producer1, 10, "test message", _cts.Token));
        produced.AddRange(await ProduceMessages(producer2, 10, "test message", _cts.Token));
        _ = await ProduceMessages(producer3, 10, "test message", _cts.Token);
        _ = await ProduceMessages(producer4, 10, "test message", _cts.Token);
        var consumed = await ConsumeMessages(consumer, produced.Count, _cts.Token);

        //Assert
        consumed.ShouldBe(produced, true);
    }

    [Fact]
    public async Task Receive_GivenTopicsPatternWithNoMatches_ShouldFaultConsumer()
    {
        //Arrange
        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, new Regex(@"persistent://public/default/nosuchtopics.*"));

        //Act
        var exception = await Record.ExceptionAsync(consumer.Receive(_cts.Token).AsTask);

        //Assert
        exception.ShouldBeOfType<ConsumerFaultedException>();
    }

    [Fact]
    public async Task Receive_GivenInvalidTopicsPattern_ShouldFaultConsumer()
    {
        //Arrange
        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, new Regex(@"invalid://public/default/match.*"));

        //Act
        var exception = await Record.ExceptionAsync(consumer.Receive(_cts.Token).AsTask);

        //Assert
        exception.ShouldBeOfType<ConsumerFaultedException>();
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

        await using var consumer = CreateConsumer(client, await _fixture.CreateTopic(_cts.Token));

        var receiveTask = consumer.Receive(_cts.Token);
        semaphoreSlim.Release();

        //Act
        var exception = await Record.ExceptionAsync(receiveTask.AsTask);

        //Assert
        exception.ShouldBeOfType<ConsumerFaultedException>();
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

        await using var consumer = CreateConsumer(client, await _fixture.CreateTopic(_cts.Token));

        await consumer.State.OnStateChangeTo(ConsumerState.Faulted, _cts.Token);

        //Act
        var exception = await Record.ExceptionAsync(consumer.Receive(_cts.Token).AsTask);

        //Assert
        exception.ShouldBeOfType<ConsumerFaultedException>();
    }
    [Fact]
    public async Task Receive_WhenReceivingToTopicWithSchemaAndReceiverHasWrongSchema_ShouldThrowException()
    {
        var topicName = await _fixture.CreateTopic(_cts.Token);
        var pulsarSchema = Schema.AvroISpecificRecord<AvroSampleModel>();
        await _fixture.AddSchemaToExistingTopic(topicName, pulsarSchema.SchemaInfo, _cts.Token);
        var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName, Schema.String);
        await using var producer = CreateProducer(client, topicName, pulsarSchema);
        await producer.Send(new AvroSampleModel(), _cts.Token);
        var exception = await Record.ExceptionAsync(consumer.Receive().AsTask);
        exception.ShouldBeOfType<IncompatibleSchemaException>();
    }
    [Fact]
    public async Task Receive_WhenReceivingToTopicWithSchemaAndReceiverHasRightSchema_ShouldBeAbleToRecieve()
    {
        var topicName = await _fixture.CreateTopic(_cts.Token);
        var pulsarSchema = Schema.AvroISpecificRecord<AvroSampleModel>();
        await _fixture.AddSchemaToExistingTopic(topicName, pulsarSchema.SchemaInfo, _cts.Token);
        var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName, pulsarSchema);
        await using var producer = CreateProducer(client, topicName, pulsarSchema);
        var modelProduced = new AvroSampleModel();
        await producer.Send(modelProduced, _cts.Token);
        var consumed = await consumer.Receive(_cts.Token);
        consumed.Value().Name.ShouldBe(modelProduced.Name);
        consumed.Value().Surname.ShouldBe(modelProduced.Surname);
        consumed.Value().Age.ShouldBe(modelProduced.Age);
    }

    [Fact]
    public async Task Connectivity_WhenInitiallyConnectedWithNoMessagesThenGoesDown_ShouldBeAbleToReceiveWhenUpAgain()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic(_cts.Token);
        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);
        await consumer.StateChangedTo(ConsumerState.Active, _cts.Token);
        var receiveTask = consumer.Receive(_cts.Token);
        await using (await _fixture.DisableThePulsarConnection())
        {
            await consumer.StateChangedTo(ConsumerState.Disconnected, _cts.Token);
        }
        await consumer.StateChangedTo(ConsumerState.Active, _cts.Token);
        await ProduceMessages(producer, 1, "test-message", _cts.Token);

        //Act
        var exception = await Record.ExceptionAsync(receiveTask.AsTask);

        //Assert
        exception.ShouldBeNull();
    }

    [Fact]
    public async Task Connectivity_WhenConnectionIsInitiallyDownAndComesUp_ShouldBeAbleToReceive()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic(_cts.Token);
        await using var client = CreateClient();
        await using var producer = CreateProducer(client, topicName);
        await ProduceMessages(producer, 1, "test-message", _cts.Token);
        var connectionDown = await _fixture.DisableThePulsarConnection();
        await using var consumer = CreateConsumer(client, topicName);

        //Act
        await connectionDown.DisposeAsync();
        await consumer.StateChangedTo(ConsumerState.Active, _cts.Token);
        var exception = await Record.ExceptionAsync(consumer.Receive(_cts.Token).AsTask);

        //Assert
        exception.ShouldBeNull();
    }

    [Fact]
    public async Task Connectivity_WhenConnectionIsInitiallyDown_ShouldBeAbleToDispose()
    {
        //Arrange
        await using var connectionDown = await _fixture.DisableThePulsarConnection();
        await using var client = CreateClient();
        await using var consumer = CreateConsumer(client, await _fixture.CreateTopic(_cts.Token));

        //Act
        var exception = await Record.ExceptionAsync(consumer.DisposeAsync().AsTask);

        //Assert
        exception.ShouldBeNull();
    }

    [Fact]
    public async Task Connectivity_WhenConnectionIsInitiallyUpAndGoesDown_ShouldBeAbleToDispose()
    {
        //Arrange
        await using var client = CreateClient();
        var consumer = CreateConsumer(client, await _fixture.CreateTopic(_cts.Token));
        await consumer.State.OnStateChangeTo(ConsumerState.Active, _cts.Token);

        //Act
        await using var connectionDown = await _fixture.DisableThePulsarConnection();
        await consumer.StateChangedTo(ConsumerState.Disconnected, _cts.Token);
        var exception = await Record.ExceptionAsync(consumer.DisposeAsync().AsTask);

        //Assert
        exception.ShouldBeNull();
    }

    [Fact]
    public async Task Connectivity_WhenConnectionIsInitiallyUpAndReconnects_ShouldBeAbleToReceive()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic(_cts.Token);
        await using var client = CreateClient();
        await using var producer = CreateProducer(client, topicName);
        await using var consumer = CreateConsumer(client, topicName);
        await ProduceMessages(producer, 1, "test-message", _cts.Token);
        await consumer.State.OnStateChangeTo(ConsumerState.Active, _cts.Token);

        //Act
        await using (await _fixture.DisableThePulsarConnection())
        {
            await consumer.StateChangedTo(ConsumerState.Disconnected, _cts.Token);
        }
        await consumer.State.OnStateChangeTo(ConsumerState.Active, _cts.Token);
        var exception = await Record.ExceptionAsync(consumer.Receive(_cts.Token).AsTask);

        //Assert
        exception.ShouldBeNull();
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

    private static string CreateSubscriptionName() => $"subscription-{Guid.NewGuid():N}";

    private IProducer<T> CreateProducer<T>(
        IPulsarClient pulsarClient,
        string topicName,
        ISchema<T> schema)
        => pulsarClient.NewProducer(schema)
        .Topic(topicName)
        .StateChangedHandler(_testOutputHelper.Log)
        .Create();
    private IProducer<string> CreateProducer(
        IPulsarClient pulsarClient,
        string topicName,
        ProducerAccessMode producerAccessMode = ProducerAccessMode.Shared)
        => CreateProducer(pulsarClient, topicName, Schema.String);


    private IConsumer<string> CreateConsumer(IPulsarClient pulsarClient, string topicName)
        => CreateConsumer(pulsarClient, topicName, Schema.String);
    private IConsumer<T> CreateConsumer<T>(IPulsarClient pulsarClient, string topicName, ISchema<T> schema)
       => pulsarClient.NewConsumer(schema)
       .InitialPosition(SubscriptionInitialPosition.Earliest)
       .SubscriptionName(CreateSubscriptionName())
       .Topic(topicName)
       .StateChangedHandler(_testOutputHelper.Log)
       .Create();

    private IConsumer<string> CreateConsumer(IPulsarClient pulsarClient, IEnumerable<string> topics)
        => pulsarClient.NewConsumer(Schema.String)
        .InitialPosition(SubscriptionInitialPosition.Earliest)
        .SubscriptionName(CreateSubscriptionName())
        .Topics(topics)
        .StateChangedHandler(_testOutputHelper.Log)
        .Create();

    private IConsumer<string> CreateConsumer(IPulsarClient pulsarClient, Regex topicsPattern)
        => pulsarClient.NewConsumer(Schema.String)
        .InitialPosition(SubscriptionInitialPosition.Earliest)
        .SubscriptionName(CreateSubscriptionName())
        .TopicsPattern(topicsPattern)
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
