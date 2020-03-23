using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.IntegrationTests
{
    public class IntegrationTestFixture : IDisposable
    {
        public IntegrationTestFixture()
        {
            // clean-up if anything was left running from previous run
            RunProcess("docker-compose", "down --rmi local --remove-orphans");
            RunProcess("docker-compose", "build");
            RunProcess("docker-compose", "up -d");

            int waitTries = 10;
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;
            var client = new HttpClient(handler);

            while (waitTries > 0)
            {
                try
                {
                    client.GetAsync("http://localhost:8080/metrics/").GetAwaiter().GetResult();
                    return;
                }
                catch (Exception ex)
                {
                    waitTries--;
                    Task.Delay(5000).GetAwaiter().GetResult();
                }
            }

            throw new Exception("Unable to confirm Pulsar has initialized");
        }

        public void Dispose()
        {
            RunProcess("docker-compose", "down --rmi local");
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
