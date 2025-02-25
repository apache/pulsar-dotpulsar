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

namespace DotPulsar.Tests;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using DotPulsar.Abstractions;
using System.Text;
using System.Text.Json;
using Testcontainers.Pulsar;
using Toxiproxy.Net;
using Xunit.Abstractions;
using Xunit.Sdk;

public class IntegrationFixture : IAsyncLifetime
{
    private const int PulsarPort = 6650;
    private const int ToxiProxyControlPort = 8474;
    private const int ToxiProxyPort = 15124;
    private readonly CancellationTokenSource _cts;
    private readonly IMessageSink _messageSink;
    private readonly INetwork _network;
    private readonly PulsarContainer _pulsarCluster;
    private readonly IContainer _toxiProxy;
    private Client _toxiProxyClient;
    private Connection _toxiProxyConnection;
    private Proxy _toxiProxyPulsarProxy;
    private string? _token;

    public IntegrationFixture(IMessageSink messageSink)
    {
        _messageSink = messageSink;
        _cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

        _network = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .Build();

        _toxiProxy = new ContainerBuilder()
            .WithImage("ghcr.io/shopify/toxiproxy:2.11.0")
            .WithPortBinding(ToxiProxyControlPort, true)
            .WithPortBinding(ToxiProxyPort, true)
            .WithHostname("toxiproxy")
            .WithNetwork(_network)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(strategy => strategy.ForPath("/version").ForPort(ToxiProxyControlPort)))
            .Build();

        _pulsarCluster = new PulsarBuilder()
            .WithAuthentication()
            .WithNetwork(_network)
            .WithHostname("pulsar")
            .Build();

        ServiceUrl = new Uri($"pulsar://{_pulsarCluster.Hostname}:{PulsarPort}");
        _toxiProxyConnection = new Connection();
        _toxiProxyClient = _toxiProxyConnection.Client();
        _toxiProxyPulsarProxy = new Proxy();
    }

    public Uri ServiceUrl { get; private set; }

    public IAuthentication Authentication => AuthenticationFactory.Token(ct => ValueTask.FromResult(_token!));

    public async Task DisposeAsync()
    {
        await _pulsarCluster.DisposeAsync();
        await _toxiProxy.DisposeAsync();
        _toxiProxyConnection.Dispose();
        _cts.Dispose();
    }

    public async Task InitializeAsync()
    {
        SubscribeToContainerEvents(_toxiProxy, "Toxiproxy");
        SubscribeToContainerEvents(_pulsarCluster, "Pulsar cluster");
        await _network.CreateAsync(_cts.Token);
        _messageSink.OnMessage(new DiagnosticMessage("Starting Toxiproxy"));
        await _toxiProxy.StartAsync(_cts.Token);
        _messageSink.OnMessage(new DiagnosticMessage("Starting Pulsar Cluster"));
        await _pulsarCluster.StartAsync(_cts.Token);
        _messageSink.OnMessage(new DiagnosticMessage("The containers has initiated. Next, we'll configure Toxiproxy mappings."));

        _toxiProxyConnection = new Connection(_toxiProxy.Hostname, _toxiProxy.GetMappedPublicPort(ToxiProxyControlPort));
        _toxiProxyClient = _toxiProxyConnection.Client();
        _toxiProxyPulsarProxy = new Proxy
        {
            Name = "localToPulsarCluster",
            Enabled = true,
            Listen = $"{"0.0.0.0"}:{ToxiProxyPort}",
            Upstream = $"{"pulsar"}:{PulsarPort}"
        };
        await _toxiProxyClient.AddAsync(_toxiProxyPulsarProxy);

        _messageSink.OnMessage(new DiagnosticMessage("Toxiproxy successfully mapped connections between host and the Pulsar Cluster."));
        ServiceUrl = new Uri($"pulsar://{_toxiProxy.Hostname}:{_toxiProxy.GetMappedPublicPort(ToxiProxyPort)}");
        _messageSink.OnMessage(new DiagnosticMessage("You can connect with: " + ServiceUrl));

        _token = await CreateToken(Timeout.InfiniteTimeSpan, _cts.Token);
    }

    private void HandleClusterStateChange(string containerName, string state) =>
        _messageSink.OnMessage(new DiagnosticMessage($"The {containerName} changed state to: {state}"));

    private void SubscribeToContainerEvents(IContainer container, string containerName)
    {
        container.Created += (_, _) => HandleClusterStateChange(containerName, "Created");
        container.Creating += (_, _) => HandleClusterStateChange(containerName, "Creating");
        container.Started += (_, _) => HandleClusterStateChange(containerName, "Started");
        container.Starting += (_, _) => HandleClusterStateChange(containerName, "Starting");
        container.Stopped += (_, _) => HandleClusterStateChange(containerName, "Stopped");
        container.Stopping += (_, _) => HandleClusterStateChange(containerName, "Stopping");
    }

    public async Task<string> CreateToken(TimeSpan expiryTime, CancellationToken cancellationToken)
    {
        return await _pulsarCluster.CreateAuthenticationTokenAsync(expiryTime, cancellationToken);
    }

    private static string CreateTopicName() => $"persistent://public/default/{Guid.NewGuid():N}";

    public async Task<string> CreateTopic(CancellationToken cancellationToken)
    {
        var topic = CreateTopicName();
        await CreateTopic(topic, cancellationToken);
        return topic;
    }

    public async Task CreateTopics(IEnumerable<string> topics, CancellationToken cancellationToken)
    {
        foreach (var topic in topics)
        {
            await CreateTopic(topic, cancellationToken);
        }
    }

    public async Task CreateTopic(string topic, CancellationToken cancellationToken)
    {
        var arguments = $"bin/pulsar-admin topics create {topic}";

        var result = await _pulsarCluster.ExecAsync(["/bin/bash", "-c", arguments], cancellationToken);

        if (result.ExitCode != 0)
            throw new Exception($"Could not create the topic: {result.Stderr}");
    }

    public async Task<string> CreatePartitionedTopic(int numberOfPartitions, CancellationToken cancellationToken)
    {
        var topic = CreateTopicName();
        await CreatePartitionedTopic(topic, numberOfPartitions, cancellationToken);
        return topic;
    }

    public async Task CreatePartitionedTopic(string topic, int numberOfPartitions, CancellationToken cancellationToken)
    {
        var arguments = $"bin/pulsar-admin topics create-partitioned-topic {topic} -p {numberOfPartitions}";

        var result = await _pulsarCluster.ExecAsync(["/bin/bash", "-c", arguments], cancellationToken);

        if (result.ExitCode != 0)
            throw new Exception($"Could not create the partitioned topic: {result.Stderr}");
    }

    public async Task AddSchemaToExistingTopic(string topic, SchemaInfo pulsarSchemaInfo, CancellationToken cancellationToken)
    {
        var schDef = new
        {
            type = pulsarSchemaInfo.Type.ToString().ToUpper(),
            schema = Encoding.UTF8.GetString(pulsarSchemaInfo.Data),
            properties = new Dictionary<string, string>()
        };
        var schemaFilename = $"{Guid.NewGuid()}.sch";
        var content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(schDef));
        await _pulsarCluster.CopyAsync(content, $"pulsar/{schemaFilename}", ct: cancellationToken);
        var arguments = $"bin/pulsar-admin schemas upload --filename {schemaFilename} {topic}";
        var result = await _pulsarCluster.ExecAsync(["/bin/bash", "-c", arguments], cancellationToken);

        if (result.ExitCode != 0)
            throw new Exception($"Could not upload a schema to topic: {result.Stderr}");
    }

    public async Task<IAsyncDisposable> DisableThePulsarConnection()
    {
        _toxiProxyPulsarProxy.Enabled = false;
        await _toxiProxyPulsarProxy.UpdateAsync();
        return new Releaser(_toxiProxyPulsarProxy);
    }

    private class Releaser : IAsyncDisposable
    {
        private readonly Proxy _proxy;

        public Releaser(Proxy proxy) => _proxy = proxy;

        public async ValueTask DisposeAsync()
        {
            _proxy.Enabled = true;
            await _proxy.UpdateAsync();
        }
    }
}
