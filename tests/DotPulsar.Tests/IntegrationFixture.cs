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
using Toxiproxy.Net;
using Toxiproxy.Net.Toxics;
using Xunit.Abstractions;
using Xunit.Sdk;

public class IntegrationFixture : IAsyncLifetime
{
    private const string AuthenticationPlugin = "org.apache.pulsar.client.impl.auth.AuthenticationToken";
    private const string SecretKeyPath = "/pulsar/secret.key";
    private const string UserName = "test-user";
    private const int PulsarPort = 6650;
    private const int ToxiProxyControlPort = 8474;
    private const int ToxiProxyPort = 15124;
    private readonly CancellationTokenSource _cts;

    private readonly IMessageSink _messageSink;

    private readonly INetwork _network;
    private readonly IContainer _pulsarCluster;
    private readonly IContainer _toxiProxy;

    private Client _toxiProxyClient;
    private Connection _toxiProxyConnection;
    private Proxy _toxiProxylocalToPulsarCluster;

    private string? _token;

    public IntegrationFixture(IMessageSink messageSink)
    {
        _messageSink = messageSink;
        _cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

        var environmentVariables = new Dictionary<string, string>
        {
            { "PULSAR_PREFIX_tokenSecretKey", $"file://{SecretKeyPath}" },
            { "PULSAR_PREFIX_authenticationRefreshCheckSeconds", "5" },
            { "superUserRoles", UserName },
            { "authenticationEnabled", "true" },
            { "authorizationEnabled", "true" },
            { "authenticationProviders", "org.apache.pulsar.broker.authentication.AuthenticationProviderToken" },
            { "authenticateOriginalAuthData", "false" },
            { "brokerClientAuthenticationPlugin", AuthenticationPlugin },
            { "CLIENT_PREFIX_authPlugin", AuthenticationPlugin }
        };

        var arguments =
            $"bin/pulsar tokens create-secret-key --output {SecretKeyPath} && " +
            $"export brokerClientAuthenticationParameters=token:$(bin/pulsar tokens create --secret-key {SecretKeyPath} --subject {UserName}) && " +
            $"export CLIENT_PREFIX_authParams=$brokerClientAuthenticationParameters && bin/apply-config-from-env.py conf/standalone.conf && " +
            $"bin/apply-config-from-env-with-prefix.py CLIENT_PREFIX_ conf/client.conf && bin/pulsar standalone --no-functions-worker";

        _network = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .Build();

        _toxiProxy = new ContainerBuilder()
            .WithImage("ghcr.io/shopify/toxiproxy:2.7.0")
            .WithPortBinding(ToxiProxyControlPort, true)
            .WithPortBinding(ToxiProxyPort, true)
            .WithHostname("toxiproxy")
            .WithNetwork(_network)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(strategy => strategy.ForPath("/version").ForPort(ToxiProxyControlPort)))
            .Build();

        _pulsarCluster = new ContainerBuilder()
            .WithImage("apachepulsar/pulsar:3.1.1")
            .WithEnvironment(environmentVariables)
            .WithHostname("pulsar")
            .WithNetwork(_network)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted(["/bin/bash", "-c", "bin/pulsar-admin clusters list"]))
            .WithCommand("/bin/bash", "-c", arguments)
            .Build();

        ServiceUrl = new Uri($"pulsar://{_pulsarCluster.Hostname}:{PulsarPort}");
        _toxiProxyConnection = new Connection();
        _toxiProxyClient = _toxiProxyConnection.Client();
        _toxiProxylocalToPulsarCluster = new Proxy();
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
        _toxiProxylocalToPulsarCluster = new Proxy
        {
            Name = "localToPulsarCluster",
            Enabled = true,
            Listen = $"{"0.0.0.0"}:{ToxiProxyPort}",
            Upstream = $"{"pulsar"}:{PulsarPort}"
        };
        await _toxiProxyClient.AddAsync(_toxiProxylocalToPulsarCluster);

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
        var arguments = $"bin/pulsar tokens create --secret-key {SecretKeyPath} --subject {UserName}";

        if (expiryTime != Timeout.InfiniteTimeSpan)
            arguments += $" --expiry-time {expiryTime.TotalSeconds}s";

        var result = await _pulsarCluster.ExecAsync(new[] { "/bin/bash", "-c", arguments }, cancellationToken);

        if (result.ExitCode != 0)
            throw new InvalidOperationException($"Could not create the token: {result.Stderr}");

        return result.Stdout;
    }

    private static string CreateTopicName() => $"persistent://public/default/{Guid.NewGuid():N}";

    public async Task<string> CreateTopic(CancellationToken cancellationToken)
    {
        var topic = CreateTopicName();
        await CreateTopic(topic, cancellationToken);
        return topic;
    }

    public async Task CreateTopic(string topic, CancellationToken cancellationToken)
    {
        var arguments = $"bin/pulsar-admin topics create {topic}";

        var result = await _pulsarCluster.ExecAsync(new[] { "/bin/bash", "-c", arguments }, cancellationToken);

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

        var result = await _pulsarCluster.ExecAsync(new[] { "/bin/bash", "-c", arguments }, cancellationToken);

        if (result.ExitCode != 0)
            throw new Exception($"Could not create the partitioned topic: {result.Stderr}");
    }

    public async Task SimulateDisconnect()
    {
        var timeoutProxy = new TimeoutToxic();
        timeoutProxy.Attributes.Timeout = 1000000000;
        timeoutProxy.Toxicity = 1.0;
        await _toxiProxylocalToPulsarCluster.AddAsync(timeoutProxy);
        await _toxiProxylocalToPulsarCluster.UpdateAsync();
    }

    public async Task RemoveAllToxins()
    {
        var allToxicsAsync = await _toxiProxylocalToPulsarCluster.GetAllToxicsAsync();

        foreach (var toxicBase in allToxicsAsync)
        {
            await _toxiProxylocalToPulsarCluster.RemoveToxicAsync(toxicBase.Name);
        }
    }
}
