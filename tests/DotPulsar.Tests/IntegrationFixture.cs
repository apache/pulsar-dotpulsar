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

using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class IntegrationFixture : IAsyncLifetime
{
    private const string AuthenticationPlugin = "org.apache.pulsar.client.impl.auth.AuthenticationToken";
    private const string SecretKeyPath = "/pulsar/secret.key";
    private const string UserName = "test-user";
    private const int Port = 6650;

    private readonly IContainerService _cluster;

    public IntegrationFixture()
    {
        var environmentVariables = new[]
        {
            $"PULSAR_PREFIX_tokenSecretKey=file://{SecretKeyPath}",
            "PULSAR_PREFIX_authenticationRefreshCheckSeconds=5",
            $"superUserRoles={UserName}",
            "authenticationEnabled=true",
            "authorizationEnabled=true",
            "authenticationProviders=org.apache.pulsar.broker.authentication.AuthenticationProviderToken",
            "authenticateOriginalAuthData=false",
            $"brokerClientAuthenticationPlugin={AuthenticationPlugin}",
            $"CLIENT_PREFIX_authPlugin={AuthenticationPlugin}",
        };

        var arguments = "\"" +
            $"bin/pulsar tokens create-secret-key --output {SecretKeyPath} && " +
            $"export brokerClientAuthenticationParameters=token:$(bin/pulsar tokens create --secret-key {SecretKeyPath} --subject {UserName}) && " +
            "export CLIENT_PREFIX_authParams=$brokerClientAuthenticationParameters && " +
            "bin/apply-config-from-env.py conf/standalone.conf && " +
            "bin/apply-config-from-env-with-prefix.py CLIENT_PREFIX_ conf/client.conf && " +
            "bin/pulsar standalone --no-functions-worker"
            + "\"";

        _cluster = new Builder()
            .UseContainer()
            .UseImage("apachepulsar/pulsar:2.9.2")
            .WithEnvironment(environmentVariables)
            .ExposePort(Port)
            .Command("/bin/bash -c", arguments)
            .Build();

        ServiceUrl = new Uri("pulsar:://localhost:6650");
    }

    public Uri ServiceUrl { get; private set; }

    public Task DisposeAsync()
    {
        _cluster.Dispose();
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        _cluster.Start();
        _cluster.WaitForMessageInLogs("Successfully updated the policies on namespace public/default", int.MaxValue);
        var endpoint = _cluster.ToHostExposedEndpoint($"{Port}/tcp");
        ServiceUrl = new Uri($"pulsar://localhost:{endpoint.Port}");
        return Task.CompletedTask;
    }

    public string CreateToken(TimeSpan expiryTime)
    {
        var arguments = $"bin/pulsar tokens create --secret-key {SecretKeyPath} --subject {UserName}";

        if (expiryTime != Timeout.InfiniteTimeSpan)
            arguments += $" --expiry-time {expiryTime.TotalSeconds}s";

        var result = _cluster.Execute(arguments);

        if (!result.Success)
            throw new InvalidOperationException($"Could not create the token: {result.Error}");

        return result.Data[0];
    }

    public void CreatePartitionedTopic(string topic, int numberOfPartitions)
    {
        var arguments = $"bin/pulsar-admin topics create-partitioned-topic {topic} -p {numberOfPartitions}";

        var result = _cluster.Execute(arguments);

        if (!result.Success)
            throw new Exception($"Could not create the partitioned topic: {result.Error}");
    }
}
