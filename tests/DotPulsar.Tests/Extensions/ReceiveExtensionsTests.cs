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

namespace DotPulsar.Tests.Extensions;

using DotPulsar.Abstractions;
using DotPulsar.Extensions;

[Collection("Integration"), Trait("Category", "Integration")]
public class ReceiveExtensionsTests
{
    private readonly CancellationTokenSource _cts;
    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public ReceiveExtensionsTests(IntegrationFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task TryReceive_GivenTopicWithMessages_ShouldReturnTrue()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic(_cts.Token);
        await using var client = CreateClient();
        await using var receiver = CreateConsumer(client, topicName);
        await using var producer = CreateProducer(client, topicName);
        await producer.Send("test-message", _cts.Token);
        await producer.Send("test-message", _cts.Token);
        _ = await receiver.Receive(_cts.Token); //Trigger the message flow and wait for the first message to arrive
        await Task.Delay(TimeSpan.FromSeconds(2), _cts.Token); //Wait for the second message

        //Act
        var actual = receiver.TryReceive(out var message);

        //Assert
        actual.ShouldBe(true);
    }

    [Fact]
    public async Task TryReceive_GivenEmptyTopic_ShouldReturnFalse()
    {
        //Arrange
        await using var client = CreateClient();
        await using var receiver = CreateConsumer(client, await _fixture.CreateTopic(_cts.Token));

        //Act
        var actual = receiver.TryReceive(out var message);

        //Assert
        actual.ShouldBe(false);
    }

    private static string CreateSubscriptionName() => $"subscription-{Guid.NewGuid():N}";

    private IConsumer<string> CreateConsumer(IPulsarClient pulsarClient, string topicName)
        => pulsarClient.NewConsumer(Schema.String)
        .InitialPosition(SubscriptionInitialPosition.Earliest)
        .SubscriptionName(CreateSubscriptionName())
        .Topic(topicName)
        .StateChangedHandler(_testOutputHelper.Log)
        .Create();

    private IProducer<string> CreateProducer(IPulsarClient pulsarClient, string topicName)
        => pulsarClient.NewProducer(Schema.String)
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
}
