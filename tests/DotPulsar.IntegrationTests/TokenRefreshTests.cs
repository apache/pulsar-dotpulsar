namespace DotPulsar.IntegrationTests;

using Abstraction;
using Abstractions;
using Extensions;
using Fixtures;
using Internal;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

[Collection(nameof(StandaloneTokenClusterTest))]
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

    public TokenRefreshTests(ITestOutputHelper outputHelper, TokenClusterFixture fixture)
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
        var initialRefreshCount = DotPulsarEventSource.Log.TokenRefreshCount;

        var publishingStarted = false;
        var delayedNames = new HashSet<string>();
        Task<string> GetToken(string name, ref int count)
        {
            if (refreshType is TokenTestRefreshType.Standard)
            {
                return GetAuthToken(name);
            }

            if (refreshType is TokenTestRefreshType.FailAtStartup && !publishingStarted && ++count <= timesToFail)
            {
                return Task.FromException<string>(new Exception("Initial Token Failed"));
            }

            if (refreshType is TokenTestRefreshType.FailOnRefresh && publishingStarted && ++count <= timesToFail)
            {
                return Task.FromException<string>(count == 1 ? new Exception("Refresh Failed") : new Exception("Initial Token Failed"));
            }

            if (refreshType is TokenTestRefreshType.TimeoutOnRefresh && publishingStarted && !delayedNames.Contains(name))
            {
                delayedNames.Add(name);
                return Task.Delay(6000).ContinueWith(_ => GetAuthToken(name)).Unwrap();
            }

            return GetAuthToken(name);
        }

        var producerTokenCount = 0;
        await using var producerClient = GetPulsarClient("Producer", ()
            => GetToken("Producer", ref producerTokenCount));

        var consumerTokenCount = 0;
        await using var consumerClient = GetPulsarClient("Consumer", ()
            => GetToken("Consumer", ref consumerTokenCount));

        var producer = CreateProducer(producerClient);

        var consumer = consumerClient.NewConsumer(Schema.String)
            .Topic(MyTopic)
            .SubscriptionName("test-sub")
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();

        var received = new List<string>();
        const int messageCount = 20;

        var publisherTask = Task.Run(async () =>
        {
            for (var i = 0; i < messageCount; i++)
            {
                _testOutputHelper.WriteLine("Trying to publish message for index {0}", i);
                var messageId = await producer.Send(Encoding.UTF8.GetBytes(i.ToString()));
                publishingStarted = true;
                _testOutputHelper.WriteLine("Published message {0} for index {1}", messageId, i);

                await Task.Delay(1000);
            }
        });

        var consumerTask = Task.Run(async () =>
        {
            await consumer.OnStateChangeTo(ConsumerState.Active);
            for (int j = 0; j < messageCount; j++)
            {
                var message = await consumer.Receive();
                received.Add(Encoding.UTF8.GetString(message.Data));
            }
        });

        var all = Task.WhenAll(consumerTask, publisherTask);
        var timeoutTask = Task.Delay(60_000);
        var result = await Task.WhenAny(all, timeoutTask);
        Assert.True(result != timeoutTask);

        if (refreshType is TokenTestRefreshType.Standard)
        {
            Assert.True(DotPulsarEventSource.Log.TokenRefreshCount > initialRefreshCount);
        }

        var expected = Enumerable.Range(0, messageCount).Select(i => i.ToString()).ToList();
        var missing = expected.Except(received).ToList();

        if (missing.Count > 0)
        {
            Assert.True(false, $"Missing values: {string.Join(",", missing)}");
        }
    }

    private static IProducer<ReadOnlySequence<byte>> CreateProducer(IPulsarClient producerClient)
        => producerClient.NewProducer()
            .Topic(MyTopic)
            .Create();

    private IPulsarClient GetPulsarClient(string name, Func<Task<string>> tokenFactory)
        => PulsarClient.Builder()
            .AuthenticateUsingToken(tokenFactory)
            .RetryInterval(TimeSpan.FromSeconds(1))
            .ExceptionHandler(ec => _testOutputHelper.WriteLine("Error (handled={0}) occurred in {1} client: {2}", ec.ExceptionHandled, name, ec.Exception))
            .ServiceUrl(_pulsarService.GetBrokerUri()).Build();

    private async Task<string> GetAuthToken(string name)
    {
        var result = await TokenClusterFixture.GetAuthToken(true);
        _testOutputHelper.WriteLine("{0} received token {1}", name, result);
        return result;
    }
}
