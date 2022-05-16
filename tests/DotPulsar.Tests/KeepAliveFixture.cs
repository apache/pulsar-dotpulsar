namespace DotPulsar.Tests;

using Ductus.FluentDocker.Commands;
using Ductus.FluentDocker.Services;
using System.Linq;
using Xunit.Abstractions;

public class KeepAliveFixture : IntegrationFixture
{
    public KeepAliveFixture(IMessageSink messageSink) : base(messageSink)
    {

    }

    protected override bool IncludeNetwork => true;

    protected override string[] EnvironmentVariables => base.EnvironmentVariables.Concat(new[] { "keepAliveIntervalSeconds=5" }).ToArray();

    public void DisconnectBroker()
    {
        var hosts = new Hosts().Discover();
        var docker = hosts.Single();
        docker.Host.NetworkDisconnect(PulsarContainerName, NetworkAlias, true);
    }

    public void ReconnectBroker()
    {
        var hosts = new Hosts().Discover();
        var docker = hosts.Single();
        docker.Host.NetworkConnect(PulsarContainerName, NetworkAlias, new[] { "pulsar" });
    }
}
