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

namespace DotPulsar.StressTests.Fixtures;

using DotPulsar.TestHelpers;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

public class StandaloneClusterFixture : IAsyncLifetime
{
    private readonly IMessageSink _messageSink;

    public StandaloneClusterFixture(IMessageSink messageSink)
    {
        _messageSink = messageSink;
    }

    public async Task InitializeAsync()
    {
        await TakeDownPulsar(); // clean-up if anything was left running from previous run

        await ProcessAsyncHelper
            .ExecuteShellCommand("docker-compose", "-f docker-compose-standalone-tests.yml up -d")
            .ThrowOnFailure();

        using var handler = new HttpClientHandler { AllowAutoRedirect = true };
        using var client = new HttpClient(handler);

        var waitTries = 10;
        while (waitTries > 0)
        {
            try
            {
                var requestUri = "http://localhost:54546/metrics/";
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

    public async Task DisposeAsync()
    {
        try
        {
            await TakeDownPulsar();
        }
        catch (Exception e)
        {
            _messageSink.OnMessage(new DiagnosticMessage($"Error taking down pulsar: {e}"));
        }
    }

    private Task TakeDownPulsar()
        => ProcessAsyncHelper
        .ExecuteShellCommand("docker-compose", "-f docker-compose-standalone-tests.yml down")
        .LogFailure(s => _messageSink.OnMessage(new DiagnosticMessage($"Error bringing down container: {s}")));
}
