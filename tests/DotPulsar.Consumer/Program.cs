namespace DotPulsar.Consumer;

using Abstractions;
using Extensions;
using Internal;
using System.Net.Sockets;

public static class Program
{
    public const string ConnectedAwaitingDisconnection = "Successfully connected, awaiting disconnection";
    public const string DisconnectedAwaitingReconnection = "Successfully disconnected, awaiting reconnection";
    public const string Reconnected = "Successfully reconnected";

    public static async Task<int> Main(string[] args)
    {
        Console.WriteLine($"Running with url {args[0]}");
        var client = CreateClient(args[0], args[1]);

        var testRunId = Guid.NewGuid().ToString("N");

        var topic = $"persistent://public/default/consumer-tests-{testRunId}";
        await using var consumer = client.NewConsumer(Schema.ByteArray)
            .ConsumerName($"consumer-{testRunId}")
            .InitialPosition(SubscriptionInitialPosition.Earliest)
            .SubscriptionName($"subscription-{testRunId}")
            .StateChangedHandler(changed => Console.WriteLine($"Consumer state changed to {changed.ConsumerState}"))
            .Topic(topic)
            .Create();

        var timeout = Task.Delay(TimeSpan.FromSeconds(30));
        var result = await Task.WhenAny(timeout, consumer.StateChangedTo(ConsumerState.Active).AsTask());

        if (result == timeout)
        {
            Console.WriteLine("Timed out waiting for active status");
            return -1;
        }

        Console.WriteLine(ConnectedAwaitingDisconnection);
        timeout = Task.Delay(TimeSpan.FromSeconds(30));
        result = await Task.WhenAny(timeout, consumer.StateChangedTo(ConsumerState.Disconnected).AsTask());

        if (result == timeout)
        {
            Console.WriteLine("Timed out waiting for disconnected status");
            return -2;
        }

        Console.WriteLine(DisconnectedAwaitingReconnection);
        timeout = Task.Delay(TimeSpan.FromSeconds(30));
        result = await Task.WhenAny(timeout, consumer.StateChangedTo(ConsumerState.Active).AsTask());

        if (result == timeout)
        {
            Console.WriteLine("Timed out waiting for reconnected status");
            return -3;
        }

        Console.WriteLine(Reconnected);
        return 0;
    }

    private static IPulsarClient CreateClient(string url, string token)
        => PulsarClient
            .Builder()
            .Authentication(AuthenticationFactory.Token(_ => ValueTask.FromResult(token)))
            .KeepAliveInterval(TimeSpan.FromSeconds(5))
            .ServerResponseTimeout(TimeSpan.FromSeconds(10))
            .ExceptionHandler(new TestExceptionHandler())
            .ServiceUrl(new Uri(url))
            .Build();
}

internal class TestExceptionHandler : IHandleException
{
    private readonly IHandleException _inner = new DefaultExceptionHandler(TimeSpan.FromSeconds(5));

    public ValueTask OnException(ExceptionContext exceptionContext)
    {
        Console.WriteLine("Exception occurred: {0}", exceptionContext.Exception.Message);

        // This occurs when reconnecting the docker network, it takes some time for the alias to be reestablished
        if (exceptionContext.Exception is SocketException se && se.Message.Contains("Name or service not known"))
        {
            exceptionContext.ExceptionHandled = true;
            exceptionContext.Result = FaultAction.Retry;
        }
        else
        {
            _inner.OnException(exceptionContext);
        }

        return ValueTask.CompletedTask;
    }
}
