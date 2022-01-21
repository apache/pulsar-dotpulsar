namespace DotPulsar.IntegrationTests.Fixtures;

using Abstraction;
using Services;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

public class TokenClusterFixture : PulsarServiceBase
{
    private readonly IMessageSink _messageSink;

    public TokenClusterFixture(IMessageSink messageSink) : base(messageSink)
    {
        _messageSink = messageSink;
    }

    public IPulsarService PulsarService => this;

    public override async Task InitializeAsync()
    {
        await TakeDownPulsar(); // clean-up if anything was left running from previous run

        await ProcessAsyncHelper.ExecuteShellCommand("docker-compose", "-f docker-compose-standalone-token-tests.yml up -d")
            .ThrowOnFailure();

        var waitTries = 10;

        using var handler = new HttpClientHandler { AllowAutoRedirect = true };

        using var client = new HttpClient(handler);

        var token = await GetAuthToken(false);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        while (waitTries > 0)
        {
            try
            {
                await client.GetAsync($"{PulsarService.GetWebServiceUri()}/metrics/").ConfigureAwait(false);
                return;
            }
            catch(Exception e)
            {
                _messageSink.OnMessage(new DiagnosticMessage("Error trying to fetch metrics: {0}", e));
                waitTries--;
                await Task.Delay(5000).ConfigureAwait(false);
            }
        }

        throw new Exception("Unable to confirm Pulsar has initialized");
    }

    protected override async Task OnDispose()
        => await TakeDownPulsar();

    public override Uri GetBrokerUri() => new("pulsar://localhost:54547");

    public override Uri GetWebServiceUri() => new("http://localhost:54548");

    private Task TakeDownPulsar()
        => ProcessAsyncHelper.ExecuteShellCommand("docker-compose", "-f docker-compose-standalone-token-tests.yml down")
            .LogFailure(s => MessageSink.OnMessage(new DiagnosticMessage("Error bringing down container: {0}", s)));

    public static async Task<string> GetAuthToken(bool includeExpiry)
    {
        var arguments = "exec pulsar-tokens bin/pulsar tokens create --secret-key file:///appdata/my-secret.key --subject test-user";

        if (includeExpiry)
        {
            arguments += " --expiry-time 10s";
        }

        var result = await ProcessAsyncHelper.ExecuteShellCommand("docker",
            arguments);

        if (!result.Completed)
        {
            throw new InvalidOperationException($"Getting token from container failed{Environment.NewLine}{result.Output}");
        }

        return result.Output;
    }
}
