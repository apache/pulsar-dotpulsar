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

namespace DotPulsar.StressTests.Fixtures
{
    using Ductus.FluentDocker.Builders;
    using Ductus.FluentDocker.Services;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class StandaloneClusterFixture : IAsyncLifetime
    {
        private readonly ICompositeService _svc;

        public StandaloneClusterFixture()
        {
            _svc = new Builder()
                .UseContainer()
                .UseCompose()
                .ForceRecreate()
                .FromFile("./docker-compose-standalone-tests.yml")
                .RemoveOrphans()
                .Build();
        }

        public async Task InitializeAsync()
        {
            _svc.Start();

            var waitTries = 10;

            using var handler = new HttpClientHandler { AllowAutoRedirect = true };

            using var client = new HttpClient(handler);

            while (waitTries > 0)
            {
                Console.WriteLine("Waiting for Pulsar to start");

                try
                {
                    await client.GetAsync("http://localhost:54546/metrics/").ConfigureAwait(false);
                    Console.WriteLine("Pulsar has started");
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

        public Task DisposeAsync()
        {
            _svc.Remove(true);
            return Task.CompletedTask;
        }
    }
}
