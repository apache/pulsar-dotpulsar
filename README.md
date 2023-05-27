# DotPulsar

![CI - Unit](https://github.com/apache/pulsar-dotpulsar/workflows/CI%20-%20Unit/badge.svg)

The official .NET client library for [Apache Pulsar](https://pulsar.apache.org/).

DotPulsar is written entirely in C# and implements Apache Pulsar's [binary protocol](https://pulsar.apache.org/docs/en/develop-binary-protocol/).

## What's new?

Have a look at the [changelog](CHANGELOG.md).

## Getting Started

Let's take a look at a "Hello world" example, where we first produce a message and then consume it. Note that the topic and subscription will be created if they don't exist.

First, we need a Pulsar setup. See [Pulsar docs](https://pulsar.apache.org/docs/getting-started-home/) for how to set up a local standalone Pulsar instance.

Install the NuGet package [DotPulsar](https://www.nuget.org/packages/DotPulsar/) and run the follow code example:

```csharp
using DotPulsar;
using DotPulsar.Extensions;

const string myTopic = "persistent://public/default/mytopic";

// connecting to pulsar://localhost:6650
await using var client = PulsarClient.Builder().Build();

// produce a message
await using var producer = client.NewProducer(Schema.String).Topic(myTopic).Create();
await producer.Send("Hello World");

// consume messages
await using var consumer = client.NewConsumer(Schema.String)
    .SubscriptionName("MySubscription")
    .Topic(myTopic)
    .InitialPosition(SubscriptionInitialPosition.Earliest)
    .Create();

await foreach (var message in consumer.Messages())
{
    Console.WriteLine($"Received: {message.Value()}");
    await consumer.Acknowledge(message);
}
```

For a more in-depth tour of the API, please visit the [Wiki](https://github.com/apache/pulsar-dotpulsar/wiki).

## Supported features

- [X] Service discovery
- [X] Automatic reconnect
- [X] TLS connections
- [X] Pulsar Proxy
- [X] Producer - send with custom metadata
- [X] Producer - send with event time, sequence id, and delayed message delivery
- [X] Producer - send with key and ordering key
- [X] Producer - partitioned topics
- [X] Consumer - subscription with initial position and priority level
- [X] Consumer - subscription types exclusive, shared, failover, and key shared
- [X] Consumer - receive and single + cumulative acknowledge
- [X] Consumer/Reader - seek on message-id and publish time
- [X] Consumer - unsubscribe
- [X] Consumer - compacted topics
- [X] Reader API
- [X] Read/Consume/Acknowledge batched messages
- [X] Telemetry
    - [Tracing](https://github.com/apache/pulsar-dotpulsar/wiki/Tracing)
    - [Metrics](https://github.com/apache/pulsar-dotpulsar/wiki/Metrics)
- [X] Authentication
    - TLS Authentication
    - JSON Web Token Authentication
    - Custom Authentication
- [X] [Message compression](https://github.com/apache/pulsar-dotpulsar/wiki/Compression)
    - LZ4
    - ZLIB
    - ZSTD
    - SNAPPY
- [X] Schemas
    - Boolean
    - Bytes (using byte[] and ReadOnlySequence\<byte\>)
    - String (UTF-8, UTF-16, and US-ASCII)
    - INT8, INT16, INT32, and INT64
    - Float and Double
    - Time (using TimeSpan)
    - Timestamp and Date (using DateTime)

For a horizontal comparison with more language-specific clients, see [Client Feature Matrix](https://pulsar.apache.org/client-feature-matrix/).

## Roadmap

Help prioritizing the roadmap is most welcome, so please reach out and tell us what you want and need.

## Join Our Community

Apache Pulsar has a [Slack instance](https://pulsar.apache.org/contact/), and there you'll find us in the #dev-dotpulsar channel.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/apache/pulsar-dotpulsar/tags).

## Authors

* **Daniel Blankensteiner** - *Initial work* - [Danske Commodities](https://github.com/DanskeCommodities)

Contributions are welcomed and greatly appreciated. See also the list of [contributors](https://github.com/apache/pulsar-dotpulsar/contributors) who participated in this project. Read the [CONTRIBUTING](CONTRIBUTING.md) guide for how to participate.

If your contribution adds Pulsar features for C# clients, you need to update both the [Pulsar docs](https://pulsar.apache.org/docs/client-libraries/) and the [Client Feature Matrix](https://pulsar.apache.org/client-feature-matrix/). See [Contribution Guide](https://pulsar.apache.org/contribute/site-intro/#pages) for more details.

## License

This project is licensed under [Apache License, Version 2.0](https://apache.org/licenses/LICENSE-2.0).
