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
using System.Buffers;
using System.Text.Json;
using Xunit.Abstractions;

[Collection("Integration"), Trait("Category", "Integration")]
public sealed class FlowTests
{
    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public FlowTests(IntegrationFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Should_Not_Increase_Permits_On_TryReceive()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic(CancellationToken.None);
        var subscription = CreateSubscriptionName();
        var maxPrefetch = 2;

        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
        using var httpClient = CreateAdminClient();
        await using var pulsarClient = CreateClient();
        await using var consumer = CreateConsumer(pulsarClient, topicName, subscription, (uint)maxPrefetch);
        await using var producer = CreateProducer(pulsarClient, topicName);

        await consumer.StateChangedTo(ConsumerState.Active, cts.Token);

        // Wait until we get our first message
        await producer.Send([1], cts.Token);
        var message = await consumer.Receive(cts.Token);
        await consumer.Acknowledge(message, cts.Token);

        //Act
        var maxPermits = 0L;
        for (int i = 0; i < maxPrefetch * 5; i++)
        {
            consumer.TryReceive(out _).ShouldBe(false);
            await Task.Delay(50, cts.Token);
            var permits = await GetPermits(httpClient, topicName, subscription, cts.Token);
            maxPermits = Math.Max(maxPermits, permits);
        }

        //Assert
        Assert.True(maxPermits <= maxPrefetch, $"availablePermits increased above the threshold of {maxPrefetch} to {maxPermits}");
    }

    private static async ValueTask<long> GetPermits(HttpClient httpClient, string topic, string subscription, CancellationToken cancellationToken)
    {
        topic = topic.Replace("persistent://", string.Empty);
        using var response = await httpClient.GetAsync($"/admin/v2/persistent/{topic}/stats", cancellationToken).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!json.RootElement.TryGetProperty("subscriptions", out var subscriptionsProperty))
            {
                return 0;
            }

            if (!subscriptionsProperty.TryGetProperty(subscription, out var subscriptionProperty))
            {
                return 0;
            }

            if (subscriptionProperty.TryGetProperty("consumers", out var consumersProperty))
            {
                foreach (var consumer in consumersProperty.EnumerateArray())
                {
                    if (consumer.TryGetProperty("availablePermits", out var permitsProperty))
                    {
                        var permits = permitsProperty.GetInt64();
                        if (permits > 0)
                        {
                            return permits;
                        }
                    }
                }
            }
        }

        return 0;
    }

    private static string CreateSubscriptionName() => $"subscription-{Guid.NewGuid():N}";

    private IProducer<ReadOnlySequence<byte>> CreateProducer(IPulsarClient pulsarClient, string topicName)
        => pulsarClient.NewProducer(Schema.ByteSequence)
        .Topic(topicName)
        .StateChangedHandler(_testOutputHelper.Log)
        .Create();

    private IConsumer<ReadOnlySequence<byte>> CreateConsumer(IPulsarClient pulsarClient, string topicName, string subscription, uint maxPrefetch)
        => pulsarClient.NewConsumer(Schema.ByteSequence)
       .InitialPosition(SubscriptionInitialPosition.Earliest)
       .SubscriptionName(subscription)
       .Topic(topicName)
       .StateChangedHandler(_testOutputHelper.Log)
       .MessagePrefetchCount(maxPrefetch)
       .Create();

    private IPulsarClient CreateClient()
        => PulsarClient
        .Builder()
        .Authentication(_fixture.Authentication)
        .ExceptionHandler(_testOutputHelper.Log)
        .ServiceUrl(_fixture.ServiceUrl)
        .Build();

    private HttpClient CreateAdminClient() => new()
    {
        BaseAddress = _fixture.AdminUrl,
        DefaultRequestHeaders =
        {
            Authorization = _fixture.AuthorizationHeader
        }
    };
}
