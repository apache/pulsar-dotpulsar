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

namespace DotPulsar.StressTests
{
    using Extensions;
    using Fixtures;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Abstractions;

    [Collection(nameof(StandaloneClusterTest))]
    public class ConnectionTests
    {
        private readonly ITestOutputHelper _output;

        public ConnectionTests(ITestOutputHelper output)
            => _output = output;

        [Theory]
        [InlineData("pulsar://localhost:54545")] // test that we can connect directly to a broker
        [InlineData("pulsar://localhost:6666")] // test that we can connect through a reverse proxy (NOT a pulsar proxy)
        public async Task ConnectionHandshake_GivenValidServiceUrls_ShouldEstablishConnection(string serviceUrl)
        {
            //Arrange
            var testRunId = Guid.NewGuid().ToString("N");

            var topic = $"persistent://public/default/consumer-tests-{testRunId}";

            var builder = PulsarClient.Builder()
                .ExceptionHandler(new XunitExceptionHandler(_output));

            if (!string.IsNullOrEmpty(serviceUrl))
                builder.ServiceUrl(new Uri(serviceUrl));

            await using var client = builder.Build();

            await using var producer = client.NewProducer(Schema.Bytes)
                .ProducerName($"producer-{testRunId}")
                .Topic(topic)
                .Create();

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            //Act & Assert
            await producer.StateChangedTo(ProducerState.Connected, cts.Token);
        }
    }
}
