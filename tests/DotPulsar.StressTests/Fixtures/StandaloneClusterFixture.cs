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
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class StandaloneClusterFixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            TakeDownPulsar(); // clean-up if anything was left running from previous run

            RunProcess("docker-compose", "-f docker-compose-standalone-tests.yml up -d");

            var waitTries = 10;

            using var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };

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

        public Task DisposeAsync()
        {
            TakeDownPulsar();
            return Task.CompletedTask;
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

            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception($"Exit code {process.ExitCode} when running process {name} with arguments {arguments}");
        }
    }
}
