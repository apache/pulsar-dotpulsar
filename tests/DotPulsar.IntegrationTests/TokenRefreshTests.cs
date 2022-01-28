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

namespace DotPulsar.IntegrationTests;

using Abstraction;
using Abstractions;
using Extensions;
using Fixtures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

[Collection(nameof(StandaloneTokenClusterTests))]
public class TokenRefreshTests
{
    public enum TokenTestRefreshType
    {
        Standard,
        FailAtStartup,
        FailOnRefresh,
        TimeoutOnRefresh
    }

    private const string MyTopic = "persistent://public/default/mytopic";
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IPulsarService _pulsarService;

    public TokenRefreshTests(ITestOutputHelper outputHelper, StandaloneTokenClusterFixture fixture)
    {
        _testOutputHelper = outputHelper;
        Debug.Assert(fixture.PulsarService != null, "fixture.PulsarService != null");
        _pulsarService = fixture.PulsarService;
    }

    [InlineData(TokenTestRefreshType.Standard, 0)] // Standard happy path with no token refresh failures
    [InlineData(TokenTestRefreshType.FailAtStartup, 1)] // 1 Failure at startup, not on refresh
    [InlineData(TokenTestRefreshType.FailOnRefresh, 2)] // Fails on refresh which will force a reconnection and fail once more on new connection
    [InlineData(TokenTestRefreshType.TimeoutOnRefresh, 0)] // Connection will be disconnected by server due to slow response to auth challenge
    [Theory]
    public async Task TestExpiryRefresh(TokenTestRefreshType refreshType, int timesToFail)
    {
        var publishingStarted = false;
        var delayedNames = new HashSet<string>();
        ValueTask<string> GetToken(string name, ref int count)
        {
            if (refreshType is TokenTestRefreshType.Standard)
            {
                return GetAuthToken(name);
            }

            if (refreshType is TokenTestRefreshType.FailAtStartup && !publishingStarted && ++count <= timesToFail)
            {
                return ValueTask.FromException<string>(new Exception("Initial Token Failed"));
            }

            if (refreshType is TokenTestRefreshType.FailOnRefresh && publishingStarted && ++count <= timesToFail)
            {
                return ValueTask.FromException<string>(count == 1 ? new Exception("Refresh Failed") : new Exception("Initial Token Failed"));
            }

            if (refreshType is TokenTestRefreshType.TimeoutOnRefresh && publishingStarted && !delayedNames.Contains(name))
            {
                delayedNames.Add(name);
                Task.Delay(6000);
            }

            return GetAuthToken(name);
        }

        var producerTokenCount = 0;
        await using var producerClient = GetPulsarClient("Producer", (ct) => GetToken("Producer", ref producerTokenCount));

        var consumerTokenCount = 0;
        await using var consumerClient = GetPulsarClient("Consumer", (ct) => GetToken("Consumer", ref consumerTokenCount));

        await using var producer = producerClient.NewProducer(Schema.String)
            .Topic(MyTopic)
            .StateChangedHandler(Monitor)
            .Create();

        await using var consumer = consumerClient.NewConsumer(Schema.String)
            .Topic(MyTopic)
            .StateChangedHandler(Monitor)
            .SubscriptionName("test-sub")
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();

        const int messageCount = 20;
        var received = new List<string>(messageCount);

        var publisherTask = Task.Run(async () =>
        {
            for (var i = 0; i < messageCount; i++)
            {
                _testOutputHelper.WriteLine("Trying to publish message for index {0}", i);
                var messageId = await producer.Send(i.ToString());
                publishingStarted = true;
                _testOutputHelper.WriteLine("Published message {0} for index {1}", messageId, i);
                await Task.Delay(1000);
            }
        });

        var consumerTask = Task.Run(async () =>
        {
            for (var i = 0; i < messageCount; i++)
            {
                var message = await consumer.Receive();
                received.Add(message.Value());
            }
        });

        var timeoutTask = Task.Delay(60_000);
        await Task.WhenAny(Task.WhenAll(consumerTask, publisherTask), timeoutTask);
        Assert.False(timeoutTask.IsCompleted);

        var expected = Enumerable.Range(0, messageCount).Select(i => i.ToString()).ToList();
        var missing = expected.Except(received).ToList();

        if (missing.Count > 0)
        {
            Assert.True(false, $"Missing values: {string.Join(",", missing)}");
        }
    }

    private IPulsarClient GetPulsarClient(string name, Func<CancellationToken, ValueTask<string>> tokenFactory)
        => PulsarClient.Builder()
            .Authentication(AuthenticationFactory.Token(tokenFactory))
            .RetryInterval(TimeSpan.FromSeconds(1))
            .ExceptionHandler(ec =>
            {
                _testOutputHelper.WriteLine("Error (handled={0}) occurred in {1} client: {2}", ec.ExceptionHandled, name, ec.Exception);
            })
            .ServiceUrl(_pulsarService.GetBrokerUri()).Build();

    private async ValueTask<string> GetAuthToken(string name)
    {
        var result = await StandaloneTokenClusterFixture.GetAuthToken(true);
        _testOutputHelper.WriteLine("{0} received token {1}", name, result);
        return result;
    }

    private void Monitor(ProducerStateChanged stateChanged, CancellationToken cancellationToken)
    {
        var stateMessage = stateChanged.ProducerState switch
        {
            ProducerState.Connected => "is connected",
            ProducerState.Disconnected => "is disconnected",
            ProducerState.PartiallyConnected => "is partially connected",
            ProducerState.Closed => "has closed",
            ProducerState.Faulted => "has faulted",
            _ => $"has an unknown state '{stateChanged.ProducerState}'"
        };

        var topic = stateChanged.Producer.Topic;
        _testOutputHelper.WriteLine($"The producer for topic '{topic}' " + stateMessage);
    }

    private void Monitor(ConsumerStateChanged stateChanged, CancellationToken cancellationToken)
    {
        var stateMessage = stateChanged.ConsumerState switch
        {
            ConsumerState.Active => "is active",
            ConsumerState.Inactive => "is inactive",
            ConsumerState.Disconnected => "is disconnected",
            ConsumerState.Closed => "has closed",
            ConsumerState.ReachedEndOfTopic => "has reached end of topic",
            ConsumerState.Faulted => "has faulted",
            _ => $"has an unknown state '{stateChanged.ConsumerState}'"
        };

        var topic = stateChanged.Consumer.Topic;
        _testOutputHelper.WriteLine($"The consumer for topic '{topic}' " + stateMessage);
    }
}
