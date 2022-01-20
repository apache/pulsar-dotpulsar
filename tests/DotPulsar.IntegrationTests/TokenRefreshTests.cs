namespace DotPulsar.IntegrationTests;

using Abstraction;
using Extensions;
using Fixtures;
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
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IPulsarService _pulsarService;

    public TokenRefreshTests(ITestOutputHelper outputHelper, TokenClusterFixture fixture)
    {
        _testOutputHelper = outputHelper;
        Debug.Assert(fixture.PulsarService != null, "fixture.PulsarService != null");
        _pulsarService = fixture.PulsarService;
    }

    [Fact]
    public async Task TestExpiryRefresh()
    {
        const string myTopic = "persistent://public/default/mytopic";

        await using var client = PulsarClient.Builder()
            .AuthenticateUsingToken(GetAuthToken)
            .ServiceUrl(_pulsarService.GetBrokerUri()).Build();

        var producer = client.NewProducer()
            .Topic(myTopic)
            .Create();

        var consumer = client.NewConsumer(Schema.String)
            .Topic(myTopic)
            .SubscriptionName("test-sub")
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .Create();

        var received = new List<string>();
        const int messageCount = 50;

        var consumerTask = Task.Run(async () =>
        {
            for (int j = 0; j < messageCount; j++)
            {
                var message = await consumer.Receive();
                received.Add(Encoding.UTF8.GetString(message.Data));
            }
        });

        var publisherTask = Task.Run(async () =>
        {
            for (var i = 0; i < messageCount; i++)
            {
                _testOutputHelper.WriteLine("Trying to publish message for index {0}", i);
                var messageId = await producer.Send(Encoding.UTF8.GetBytes(i.ToString()));
                _testOutputHelper.WriteLine("Published message {0} for index {1}", messageId, i);

                await Task.Delay(2000);
            }
        });

        await Task.WhenAll(publisherTask, consumerTask);

        Assert.Equal(Enumerable.Range(0, messageCount).Select(i => i.ToString()), received);
    }

    private async Task<string> GetAuthToken()
    {
        var result = await TokenClusterFixture.GetAuthToken();
        _testOutputHelper.WriteLine("Received token {0}", result);
        return result;
    }
}
