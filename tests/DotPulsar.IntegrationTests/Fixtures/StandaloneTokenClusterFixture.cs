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

namespace DotPulsar.IntegrationTests.Fixtures;

using Abstraction;
using Services;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

public class StandaloneTokenClusterFixture : PulsarServiceBase
{
    private readonly IMessageSink _messageSink;

    public StandaloneTokenClusterFixture(IMessageSink messageSink) : base(messageSink)
    {
        _messageSink = messageSink;
    }

    public IPulsarService PulsarService => this;

    public override async Task InitializeAsync()
    {
        await TakeDownPulsar(); // clean-up if anything was left running from previous run

        await ProcessAsyncHelper
            .ExecuteShellCommand("docker-compose", "-f docker-compose-standalone-token-tests.yml up -d")
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
            catch (Exception e)
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
        => ProcessAsyncHelper
        .ExecuteShellCommand("docker-compose", "-f docker-compose-standalone-token-tests.yml down")
        .LogFailure(s => MessageSink.OnMessage(new DiagnosticMessage("Error bringing down container: {0}", s)));

    public static async Task<string> GetAuthToken(bool includeExpiry)
    {
        var arguments = "exec pulsar-tokens bin/pulsar tokens create --secret-key file:///appdata/my-secret.key --subject test-user";

        if (includeExpiry)
            arguments += " --expiry-time 10s";

        var tokenCreateRequest = await ProcessAsyncHelper.ExecuteShellCommand("docker", arguments);

        if (!tokenCreateRequest.Completed)
            throw new InvalidOperationException($"Getting token from container failed: {tokenCreateRequest.Output}");

        return tokenCreateRequest.Output;
    }
}
