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

namespace DotPulsar.IntegrationTests.Fixtures
{
    using DotNet.Testcontainers.Containers.Builders;
    using DotNet.Testcontainers.Containers.Modules;
    using DotNet.Testcontainers.Containers.WaitStrategies;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class StandaloneClusterFixture : IAsyncLifetime
    {
        private TestcontainersContainer? _pulsarContainer;
        public async Task InitializeAsync()
        {
            _pulsarContainer = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("apachepulsar/pulsar:2.7.0")
                .WithName("pulsar-standalone")
                .WithPortBinding(8080)
                .WithPortBinding(6650)
                .WithExposedPort(6650)
                .WithCommand("/bin/bash", "-c",
                    "bin/apply-config-from-env.py conf/standalone.conf && bin/pulsar standalone -nss -nfw")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6650))
                .Build();

            await _pulsarContainer.StartAsync();

        }

        public async Task DisposeAsync()
        {
            if (_pulsarContainer != null)
            {
                await _pulsarContainer.StopAsync();
                await _pulsarContainer.DisposeAsync().ConfigureAwait(false);
            }
        }

        private static void TakeDownPulsar()
            => RunProcess("docker-compose", "-f docker-compose-standalone-tests.yml down");

        private static void RunProcess(string name, string arguments)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = name,
                Arguments = arguments
            };

            processStartInfo.Environment["TAG"] = "test";
            processStartInfo.Environment["CONFIGURATION"] = "Debug";
            processStartInfo.Environment["COMPUTERNAME"] = Environment.MachineName;

            var process = Process.Start(processStartInfo);
            if (process is null)
                throw new Exception("Process.Start returned null");

            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception($"Exit code {process.ExitCode} when running process {name} with arguments {arguments}");
        }
    }
}
