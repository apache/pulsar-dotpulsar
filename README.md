# DotPulsar

Native .NET/C# client library for [Apache Pulsar](https://pulsar.apache.org/)

## Getting Started

DotPulsar is written entirely in C# and implement Apache Pulsar's [binary protocol](https://pulsar.apache.org/docs/en/develop-binary-protocol/). Other options was using the [C++ client library](https://pulsar.apache.org/docs/en/client-libraries-cpp/) (which is what the [Python client](https://pulsar.apache.org/docs/en/client-libraries-python/) and [Go client](https://pulsar.apache.org/docs/en/client-libraries-go/) do) or build on top of the [WebSocket API](https://pulsar.apache.org/docs/en/client-libraries-websocket/). We decided to implement the binary protocol in order to gain full control and maximize portability and performance.

DotPulsar's API is strongly inspired by Apache Pulsar's official [Java client](https://pulsar.apache.org/docs/en/client-libraries-java/), but a 100% match is not goal.

Let's see how to produce, consume and read messages.

### Producing messages

Producers can be created via the extension method show below, which follows the API from the Java client:

```csharp
var client = PulsarClient.Builder().Build();
var producer = client.NewProducer().Topic("persistent://public/default/mytopic").Create();
await producer.Send(System.Text.Encoding.UTF8.GetBytes("Hello World"));
```

If you are not a fan of extensions methods and builders, then there is another option:

```csharp
var client = PulsarClient.Builder().Build();
var producerOptions = new ProducerOptions
{
    ProducerName = "MyProducer",
    Topic = "persistent://public/default/mytopic"
};
var producer = client.CreateProducer(producerOptions);
```

In the above you only specify the data being sent, but you can also specify metadata:

```csharp
var data = Encoding.UTF8.GetBytes("Hello World");
var messageId = await producer.NewMessage()
                              .Property("SomeKey", "SomeValue") //EventTime and SequenceId can also be set
                              .Send(data);
```

If you are not a fan of extensions methods and builders, then there is another option:

```csharp
var data = Encoding.UTF8.GetBytes("Hello World");
var metadata = new MessageMetadata(); //EventTime and SequenceId can be set via properties
metadata["SomeKey"] = "SomeValue";
var messageId = await producer.Send(metadata, data));
```

### Consuming messages

Consumers can be created via the extension method show below, which follows the API from the Java client:

```csharp
var client = PulsarClient.Builder().Build();
var consumer = client.NewConsumer()
                     .SubscriptionName("MySubscription")
                     .Topic("persistent://public/default/mytopic")
                     .Create();
var message = await consumer.Receive();
Console.WriteLine("Received Message: " + Encoding.UTF8.GetString(message.Data.ToArray()));
await consumer.Acknowledge(message);
```

If you are not a fan of extensions methods and builders, then there is another option:

```csharp
var client = PulsarClient.Builder().Build();
var consumerOptions = new ConsumerOptions
{
    SubscriptionName = "MySubscription",
    Topic = "persistent://public/default/mytopic"
};
var consumer = client.CreateConsumer(consumerOptions);
```

### Reading messages

Readers can be created via the extension method show below, which follows the API from the Java client:

```csharp
var client = PulsarClient.Builder().Build();
var reader = client.NewReader()
                   .StartMessageId(MessageId.Earliest)
                   .Topic("persistent://public/default/mytopic")
                   .Create();
var message = await reader.Receive();
Console.WriteLine("Received Message: " + Encoding.UTF8.GetString(message.Data.ToArray()));
```

If you are not a fan of extensions methods and builders, then there is another option:

```csharp
var client = PulsarClient.Builder().Build();
var readerOptions = new ReaderOptions
{
    StartMessageId = MessageId.Earliest,
    Topic = "persistent://public/default/mytopic"
};
var reader = client.CreateReader(readerOptions);
```

## Monitoring state

Consumers, producers and readers all have states that can be monitored. Let's have a look at what states they can have.

### Consumer states

* Active (All is well)
* Inactive (All is well. The subscription type is 'Failover' and you are not the active consumer)
* Closed (The consumer or PulsarClient has been disposed)
* Disconnected (The connection was lost and attempts are being made to reconnect)
* Faulted (An unrecoverable error has occurred)
* ReachedEndOfTopic (No more messages will be delivered)

### Producer states

* Closed (The producer or PulsarClient has been disposed)
* Connected (All is well)
* Disconnected (The connection was lost and attempts are being made to reconnect)
* Faulted (An unrecoverable error has occurred)

### Reader states

* Closed (The reader or PulsarClient has been disposed)
* Connected: (All is well)
* Disconnected (The connection was lost and attempts are being made to reconnect)
* Faulted (An unrecoverable error has occurred)
* ReachedEndOfTopic (No more messages will be delivered)

### How to

Monitoring the state is easy, so let's see how to monitor when a consumer changes state:

```csharp
private static async Task MonitorConsumerState(IConsumer consumer, CancellationToken cancellationToken)
{
	var state = ConsumerState.Disconnected;

	while (true)
	{
		state = await consumer.StateChangedFrom(state, cancellationToken);

		switch (state)
		{
			case ConsumerState.Active:
				Console.WriteLine("Consumer is active");
				break;
			case ConsumerState.Inactive:
				Console.WriteLine("Consumer is inactive");
				break;
			case ConsumerState.Disconnected:
				Console.WriteLine("Consumer is disconnected");
				break;
			case ConsumerState.Closed:
				Console.WriteLine("Consumer has closed");
				break;
			case ConsumerState.ReachedEndOfTopic:
				Console.WriteLine("Consumer has reached end of topic");
				break;
			case ConsumerState.Faulted:
				Console.WriteLine("Consumer has faulted");
				break;
		}

		if (consumer.IsFinalState(state))
			return;
	}
}
```

Here the variable 'state' will contained to new state. You can both monitor going From (StateChangedFrom) and To (StateChangedTo) a state. 
Some states are final, meaning the state can no longer change. For consumers 'Closed', 'Faulted' and 'ReachedEndOfTopic' are final states. When the consumer enter a final state, all monitoring tasks are completed. So if you e.g. are monitoring going to 'Diconnected' and the consumer is 'Closed', then you task will complete and return 'Closed'.

## Built With

* [protobuf-net](https://github.com/mgravell/protobuf-net) - Provides simple access to fast and efficient "Protocol Buffers" serialization from .NET applications
* [System.IO.Pipelines](https://www.nuget.org/packages/System.IO.Pipelines/) - Single producer single consumer byte buffer management

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/danske-commodities/dotpulsar/tags).

## Authors

* **Daniel Blankensteiner** - *Initial work* - [Danske Commodities](https://github.com/danske-commodities)

See also the list of [contributors](https://github.com/danske-commodities/dotpulsar/contributors) who participated in this project.

## License

This project is licensed under the Apache License Version 2.0 - see the [LICENSE](LICENSE) file for details.

## Roadmap

1.0.0

* Move to IAsyncDisposable and IAsyncEnumerable (will mean moving to .NET Standard 2.1)

X.X.X //Future

* Schemas
* Authentication/Authorization (TLS Authentication, Athenz, Kerberos)
* Partitioned topics
* Topic compaction
* Message compression (LZ4, ZLIB, ZSTD, SNAPPY)
* Multi-topic subscriptions
* Connection encryption
* Message encryption
* Batching
* CommandConsumerStats/CommandConsumerStatsResponse
* CommandGetTopicsOfNamespace/CommandGetTopicsOfNamespaceResponse
* CommandPartitionedTopicMetadata/CommandPartitionedTopicMetadataResponse
