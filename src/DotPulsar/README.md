# DotPulsar

Let's take a look at a "Hello world" example, where we first set up a consumer and then produce a message. Note that the topic and subscription will be created if they don't exist.

### Creating the client

Before creating readers, consumers, and producers, we need to create a client.

```csharp
using DotPulsar;
using DotPulsar.Extensions;

await using var client = PulsarClient.Builder().Build();  // Connecting to pulsar://localhost:6650
```

## Consuming

```csharp
await using var consumer = client.NewConsumer(Schema.String)
    .SubscriptionName("MySubscription")
    .Topic("persistent://public/default/mytopic")
    .InitialPosition(SubscriptionInitialPosition.Earliest)
    .Create();

await foreach (var message in consumer.Messages())
{
    Console.WriteLine($"Received: {message.Value()}");
    await consumer.Acknowledge(message);
}
```

## Producing

```csharp
await using var producer = client.NewProducer(Schema.String).Topic("persistent://public/default/mytopic").Create();
var messageId = await producer.Send("Hello World");
```
