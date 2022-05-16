namespace DotPulsar.Tests;

using System.Linq;
using Xunit.Abstractions;

public class KeepAliveFixture : IntegrationFixture
{
    public KeepAliveFixture(IMessageSink messageSink) : base(messageSink)
    {

    }

    protected override bool IncludeNetwork => true;

    protected override string[] EnvironmentVariables => base.EnvironmentVariables.Concat(new[] { "keepAliveIntervalSeconds=5" }).ToArray();
}
