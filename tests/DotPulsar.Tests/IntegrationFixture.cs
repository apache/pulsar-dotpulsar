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
using DotPulsar.Abstractions;
using System;
using Xunit.Abstractions;
using Xunit.Sdk;

public class IntegrationFixture : IAsyncLifetime
{
    private const string AuthenticationPlugin = "org.apache.pulsar.client.impl.auth.AuthenticationToken";
    private const string SecretKeyPath = "/pulsar/secret.key";
    private const string UserName = "test-user";
    private const int Port = 6650;

    private readonly IMessageSink _messageSink;
    private readonly IContainer _cluster;

    private string? _token;

    public IntegrationFixture(IMessageSink messageSink)
    {
        _messageSink = messageSink;

        var environmentVariables = new Dictionary<string, string>()
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

        var arguments = $"bin/pulsar tokens create-secret-key --output {SecretKeyPath} &&" +
                             $" export brokerClientAuthenticationParameters=token:$(bin/pulsar tokens create --secret-key {SecretKeyPath} --subject {UserName}) &&" +
                             $" export CLIENT_PREFIX_authParams=$brokerClientAuthenticationParameters && bin/apply-config-from-env.py conf/standalone.conf &&" +
                             $" bin/apply-config-from-env-with-prefix.py CLIENT_PREFIX_ conf/client.conf && bin/pulsar standalone --no-functions-worker";

        _cluster = new ContainerBuilder()
            .WithImage("apachepulsar/pulsar:3.1.1")
            .WithEnvironment(environmentVariables)
            .WithPortBinding(Port,Port)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(@"\[test-user] Created namespace public/default"))
            .WithCommand("/bin/bash", "-c", arguments)
            .Build();

        ServiceUrl = new Uri("pulsar://localhost:6650");
    }

    public Uri ServiceUrl { get; private set; }

    public IAuthentication Authentication => AuthenticationFactory.Token(ct => ValueTask.FromResult(_token!));

    public Task DisposeAsync() => _cluster.DisposeAsync().AsTask();

    private void HandleClusterStateChange(string state)
    {
        _messageSink.OnMessage(new DiagnosticMessage($"The Pulsar cluster changed state to: {state}"));
    }

    public async Task InitializeAsync()
    {
        _cluster.Created += (_, _) => HandleClusterStateChange("Created");
        _cluster.Creating += (_, _) => HandleClusterStateChange("Creating");
        _cluster.Started += (_, _) => HandleClusterStateChange("Started");
        _cluster.Starting += (_, _) => HandleClusterStateChange("Starting");
        _cluster.Stopped += (_, _) => HandleClusterStateChange("Stopped");
        _cluster.Stopping += (_, _) => HandleClusterStateChange("Stopping");

        _messageSink.OnMessage(new DiagnosticMessage("Starting container service"));
        await _cluster.StartAsync();
        _messageSink.OnMessage(new DiagnosticMessage("Container service started. Waiting for message in logs"));
        _messageSink.OnMessage(new DiagnosticMessage("Got message, will now get endpoint"));
        var endpoint = _cluster.GetMappedPublicPort(Port);
        _messageSink.OnMessage(new DiagnosticMessage($"Endpoint opened at {endpoint}"));
        ServiceUrl = new Uri($"pulsar://localhost:{Port}");
        _token = await CreateToken(Timeout.InfiniteTimeSpan);
    }

    public async Task<string> CreateToken(TimeSpan expiryTime)
    {
        var arguments = $"bin/pulsar tokens create --secret-key {SecretKeyPath} --subject {UserName}";

        if (expiryTime != Timeout.InfiniteTimeSpan)
            arguments += $" --expiry-time {expiryTime.TotalSeconds}s";

        var result = await _cluster.ExecAsync(new[] { "/bin/bash", "-c" , arguments });

        if (result.ExitCode != 0)
            throw new InvalidOperationException($"Could not create the token: {result.Stderr}");

        return result.Stdout;
    }

    private static string CreateTopicName() => $"persistent://public/default/{Guid.NewGuid():N}";

    public async Task<string> CreateTopic()
    {
        var topic = CreateTopicName();
        await CreateTopic(topic);
        return topic;
    }

    public async Task CreateTopic(string topic)
    {
        var arguments = $"bin/pulsar-admin topics create {topic}";

        var result = await _cluster.ExecAsync(new[] { "/bin/bash", "-c" , arguments });

        if (result.ExitCode != 0)
            throw new Exception($"Could not create the topic: {result.Stderr}");
    }

    public async Task<string> CreatePartitionedTopic(int numberOfPartitions)
    {
        var topic = CreateTopicName();
        await CreatePartitionedTopic(topic, numberOfPartitions);
        return topic;
    }

    public async Task CreatePartitionedTopic(string topic, int numberOfPartitions)
    {
        var arguments = $"bin/pulsar-admin topics create-partitioned-topic {topic} -p {numberOfPartitions}";

        var result = await _cluster.ExecAsync(new[] { "/bin/bash", "-c" , arguments });

        if (result.ExitCode != 0)
            throw new Exception($"Could not create the partitioned topic: {result.Stderr}");
    }
}
