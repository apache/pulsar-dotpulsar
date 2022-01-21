﻿/*
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

namespace DotPulsar.IntegrationTests.Services;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

public sealed class StandaloneContainerService : PulsarServiceBase
{
    public StandaloneContainerService(IMessageSink messageSink) : base(messageSink) { }

    public override async Task InitializeAsync()
    {
        await TakeDownPulsar(); // clean-up if anything was left running from previous run

        await ProcessAsyncHelper.ExecuteShellCommand("docker-compose", "-f docker-compose-standalone-tests.yml up -d")
            .ThrowOnFailure();

        var waitTries = 10;

        using var handler = new HttpClientHandler { AllowAutoRedirect = true };

        using var client = new HttpClient(handler);

        while (waitTries > 0)
        {
            try
            {
                await client.GetAsync("http://localhost:54546/metrics/").ConfigureAwait(false);
                return;
            }
            catch
            {
                waitTries--;
                await Task.Delay(5000).ConfigureAwait(false);
            }
        }

        throw new Exception("Unable to confirm Pulsar has initialized");
    }

    protected override Task OnDispose() => TakeDownPulsar();

    private Task TakeDownPulsar()
        => ProcessAsyncHelper.ExecuteShellCommand("docker-compose", "-f docker-compose-standalone-tests.yml down")
            .LogFailure(s => MessageSink.OnMessage(new DiagnosticMessage("Error bringing down container: {0}", s)));

    public override Uri GetBrokerUri()
        => new("pulsar://localhost:54545");

    public override Uri GetWebServiceUri()
        => new("http://localhost:54546");
}
