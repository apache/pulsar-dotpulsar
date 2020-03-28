using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DotPulsar.Stress.Tests.Fixtures
{
    public class StandaloneClusterFixture : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            // clean-up if anything was left running from previous run
            RunProcess("docker-compose", "-f docker-compose-standalone-tests.yml down");
            RunProcess("docker-compose", "-f docker-compose-standalone-tests.yml build");
            RunProcess("docker-compose", "-f docker-compose-standalone-tests.yml up -d");

            int waitTries = 10;

            using var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;
            using var client = new HttpClient(handler);

            while (waitTries > 0)
            {
                try
                {
                    await client.GetAsync("http://localhost:8080/metrics/").ConfigureAwait(false);
                    return;
                }
                catch (Exception ex)
                {
                    waitTries--;
                    await Task.Delay(5000).ConfigureAwait(false);
                }
            }

            throw new Exception("Unable to confirm Pulsar has initialized");
        }

        public async Task DisposeAsync()
        {
            RunProcess("docker-compose", "-f docker-compose-standalone-tests.yml down");
        }

        private void RunProcess(string name, string arguments)
        {
            var processStartInfo = new ProcessStartInfo()
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
            {
                throw new Exception($"Exit code {process.ExitCode} when running process {name} with arguments {arguments}");
            }
        }
    }
}