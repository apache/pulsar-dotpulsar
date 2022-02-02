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

using DotPulsar.TestHelpers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

public sealed class StandaloneFixture : IAsyncLifetime
{
    private readonly string _containerName;
    private readonly Uri _webServiceUri;
    private readonly CancellationTokenSource _cts;
    private readonly IMessageSink _messageSink;

    public StandaloneFixture(IMessageSink messageSink)
    {
        _containerName = "standalone";
        _webServiceUri = new("http://localhost:54548");
        ServiceUrl = new("pulsar://localhost:54547");
        _cts = new CancellationTokenSource();
        _messageSink = messageSink;
    }

    public Uri ServiceUrl { get; }

    public async Task DisposeAsync()
    {
        _cts.Cancel();
        await TakeDownPulsar();
    }

    public async Task InitializeAsync()
    {
        await TakeDownPulsar(); // clean-up if anything was left running from previous run

        await ProcessAsyncHelper
            .ExecuteShellCommand("docker-compose", $"-f {_containerName}.yml up -d")
            .ThrowOnFailure();

        using var handler = new HttpClientHandler { AllowAutoRedirect = true };
        using var client = new HttpClient(handler);

        var token = await GetToken(Timeout.InfiniteTimeSpan);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var waitTries = 10;
        while (waitTries > 0)
        {
            try
            {
                var requestUri = new Uri(_webServiceUri, "metrics/");
                var response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                    return;
            }
            catch (Exception e)
            {
                _messageSink.OnMessage(new DiagnosticMessage("Error trying to fetch metrics: {0}", e));
            }

            waitTries--;
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        throw new Exception("Unable to confirm Pulsar has initialized");
    }

    public async Task<string> GetToken(TimeSpan expiryTime)
    {
        var arguments = $"exec {_containerName} bin/pulsar tokens create --secret-key file:///appdata/my-secret.key --subject test-user";

        if (expiryTime != Timeout.InfiniteTimeSpan)
            arguments += $" --expiry-time {expiryTime.TotalSeconds}s";

        var tokenCreateRequest = await ProcessAsyncHelper.ExecuteShellCommand("docker", arguments);

        if (tokenCreateRequest.Completed)
            return tokenCreateRequest.Output;

        throw new InvalidOperationException($"Getting token from container failed: {tokenCreateRequest.Output}");
    }

    public async Task CreatePartitionedTopic(string restTopic, int numPartitions)
    {
        using var client = new HttpClient();
        var token = await GetToken(Timeout.InfiniteTimeSpan);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var requestUri = new Uri(_webServiceUri, $"admin/v2/{restTopic}/partitions");
        var content = new StringContent(numPartitions.ToString(), Encoding.UTF8, "application/json");
        var response = await client.PutAsync(requestUri, content, _cts.Token);
        if (response.IsSuccessStatusCode)
            return;

        throw new Exception($"Could not create the partition topic. Got status code {response.StatusCode}");
    }

    private async Task TakeDownPulsar()
        => await ProcessAsyncHelper
        .ExecuteShellCommand("docker-compose", $"-f {_containerName}.yml down")
        .LogFailure(s => _messageSink.OnMessage(new DiagnosticMessage($"Error bringing down container: {s}")));
}
