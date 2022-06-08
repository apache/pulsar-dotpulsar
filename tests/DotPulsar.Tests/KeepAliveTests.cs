namespace DotPulsar.Tests;

using Abstractions;
using Extensions;
using Consumer;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Builders;
using Ductus.FluentDocker.Services.Extensions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

[Collection("KeepAlive"), Trait("Category", "KeepAlive")]
public class KeepAliveTests
{
    private readonly KeepAliveFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public KeepAliveTests(KeepAliveFixture fixture, ITestOutputHelper outputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = outputHelper;
    }

    [Fact]
    public async Task TestNetworkDisconnection()
    {
        var token = _fixture.CreateToken(Timeout.InfiniteTimeSpan);

        using var consumer = new Builder()
            .UseContainer()
            .UseImage("mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim")
            .UseNetwork(_fixture.NetworkAlias)
            .Mount(Path.Combine(Environment.CurrentDirectory, "linux-x64"), "/app", MountType.ReadOnly)
            .Command("/app/DotPulsar.Consumer", "pulsar://pulsar:6650", token)
            .Build()
            .Start();

        var reconnected = false;
        var logsTask = Task.Run(() =>
        {
            var logs = consumer.Logs(true);

            while (!logs.IsFinished)
            {
                var line = logs.TryRead(45_000); // Do a read with timeout

                if (line != null)
                {
                    _testOutputHelper.WriteLine(line);
                }
            }

            _testOutputHelper.WriteLine("Logs completed");
        });

        try
        {
            consumer.WaitForMessageInLogs(Program.ConnectedAwaitingDisconnection, 30_000);
            _testOutputHelper.WriteLine("Severing network");
            _fixture.DisconnectBroker();

            consumer.WaitForMessageInLogs(Program.DisconnectedAwaitingReconnection, 30_000);
            _testOutputHelper.WriteLine("Reconnecting network");
            _fixture.ReconnectBroker();
            reconnected = true;
            consumer.WaitForMessageInLogs(Program.Reconnected, 30_000);
        }
        finally
        {
            if (!reconnected)
            {
                _fixture.ReconnectBroker();
                await AssertReconnection();
            }

            consumer.Dispose();
        }

        await logsTask;
    }

    private async Task AssertReconnection()
    {
        await using var client = CreateClient();
        string topicName = $"round-robin-partitioned-{Guid.NewGuid():N}";
        await using var producer = client.NewProducer(Schema.String)
            .Topic(topicName)
            .Create();

        var timeout = Task.Delay(TimeSpan.FromSeconds(30));
        var result = await Task.WhenAny(timeout, producer.StateChangedTo(ProducerState.Connected).AsTask());

        await producer.Send("message");

        if (result == timeout)
        {
            throw new Exception("Timeout waiting for active status");
        }
    }

    private IPulsarClient CreateClient()
        => PulsarClient
            .Builder()
            .Authentication(AuthenticationFactory.Token(ct => ValueTask.FromResult(_fixture.CreateToken(Timeout.InfiniteTimeSpan))))
            .ExceptionHandler(ec => _testOutputHelper.WriteLine($"Exception: {ec.Exception}"))
            .ServiceUrl(_fixture.ServiceUrl)
            .Build();
}
