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

namespace DotPulsar.Tests.Internal;

using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using System.Text.Json;

[Collection("Integration"), Trait("Category", "Integration")]
public sealed class ConnectionPoolTests : IDisposable
{
    private const int ProducerCount = 20;

    private readonly CancellationTokenSource _cts;
    private readonly IntegrationFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public ConnectionPoolTests(IntegrationFixture fixture, ITestOutputHelper outputHelper)
    {
        _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
        _fixture = fixture;
        _testOutputHelper = outputHelper;
    }

    [Fact]
    public async Task Connectivity_WhenManyProducersReconnectSimultaneously_AllShouldShareOneConnection()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic(_cts.Token);
        await using var client = CreateClient();
        var producers = Enumerable.Range(0, ProducerCount).Select(_ => CreateProducer(client, topicName)).ToArray();

        try
        {
            await Task.WhenAll(producers.Select(p => p.State.OnStateChangeTo(ProducerState.Connected, _cts.Token).AsTask()));

            //Act
            await using (await _fixture.DisableThePulsarConnection())
            {
                await Task.WhenAll(producers.Select(p => p.StateChangedTo(ProducerState.Disconnected, _cts.Token).AsTask()));
            }

            await Task.WhenAll(producers.Select(p => p.State.OnStateChangeTo(ProducerState.Connected, _cts.Token).AsTask()));

            //Assert
            using var adminClient = CreateAdminClient();
            var uniqueConnections = await GetUniqueProducerConnectionCount(adminClient, topicName, _cts.Token);
            uniqueConnections.ShouldBe(1);
        }
        finally
        {
            await Task.WhenAll(producers.Select(p => p.DisposeAsync().AsTask()));
        }
    }

    [Fact]
    public async Task Connectivity_WhenManyProducersReconnectTwice_AllShouldShareOneConnection()
    {
        //Arrange
        var topicName = await _fixture.CreateTopic(_cts.Token);
        await using var client = CreateClient();
        var producers = Enumerable.Range(0, ProducerCount).Select(_ => CreateProducer(client, topicName)).ToArray();

        try
        {
            await Task.WhenAll(producers.Select(p => p.State.OnStateChangeTo(ProducerState.Connected, _cts.Token).AsTask()));

            //Act
            for (var cycle = 0; cycle < 2; cycle++)
            {
                await using (await _fixture.DisableThePulsarConnection())
                {
                    await Task.WhenAll(producers.Select(p => p.StateChangedTo(ProducerState.Disconnected, _cts.Token).AsTask()));
                }

                await Task.WhenAll(producers.Select(p => p.State.OnStateChangeTo(ProducerState.Connected, _cts.Token).AsTask()));
            }

            //Assert
            using var adminClient = CreateAdminClient();
            var uniqueConnections = await GetUniqueProducerConnectionCount(adminClient, topicName, _cts.Token);
            uniqueConnections.ShouldBe(1);
        }
        finally
        {
            await Task.WhenAll(producers.Select(p => p.DisposeAsync().AsTask()));
        }
    }

    private static async ValueTask<int> GetUniqueProducerConnectionCount(HttpClient httpClient, string topicName, CancellationToken cancellationToken)
    {
        var topic = topicName.Replace("persistent://", string.Empty);
        using var response = await httpClient.GetAsync($"/admin/v2/persistent/{topic}/stats", cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            return 0;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

        // to comply both Pulsar 3.x and 4.x
        if (!json.RootElement.TryGetProperty("publishers", out var producers) &&
            !json.RootElement.TryGetProperty("producers", out producers))
            return 0;

        return producers.EnumerateArray()
            .Select(p => p.TryGetProperty("address", out var addr) ? addr.GetString() : null)
            .Where(a => a is not null)
            .Distinct()
            .Count();
    }

    private IProducer<string> CreateProducer(IPulsarClient pulsarClient, string topicName)
        => pulsarClient
            .NewProducer(Schema.String)
            .Topic(topicName)
            .StateChangedHandler(_testOutputHelper.Log)
            .Create();

    private IPulsarClient CreateClient()
        => PulsarClient
            .Builder()
            .Authentication(_fixture.Authentication)
            .ExceptionHandler(_testOutputHelper.Log)
            .ServiceUrl(_fixture.ServiceUrl)
            .Build();

    private HttpClient CreateAdminClient() => _fixture.CreateAdminClient();

    public void Dispose() => _cts.Dispose();
}
