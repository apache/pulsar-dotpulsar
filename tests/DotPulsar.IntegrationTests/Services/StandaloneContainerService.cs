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

namespace DotPulsar.IntegrationTests.Services
{
    using Ductus.FluentDocker.Builders;
    using Ductus.FluentDocker.Services;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public sealed class StandaloneContainerService : PulsarServiceBase
    {
        private readonly ICompositeService _svc;

        public StandaloneContainerService()
        {
            _svc = new Builder()
                .UseContainer()
                .UseCompose()
                .ForceRecreate()
                .FromFile("./docker-compose-standalone-tests.yml")
                .RemoveOrphans()
                .Build();
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync().ConfigureAwait(false);

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

        public override async Task DisposeAsync()
        {
            await base.DisposeAsync().ConfigureAwait(false);
            TakeDownPulsar();
        }

        private void TakeDownPulsar()
        {
            _svc.Remove(true);
        }

        public override Uri GetBrokerUri()
            => new("pulsar://localhost:54545");

        public override Uri GetWebServiceUri()
            => new("http://localhost:54546");
    }
}
