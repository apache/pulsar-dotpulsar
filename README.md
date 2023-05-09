# DotPulsar

![CI - Unit](https://github.com/apache/pulsar-dotpulsar/workflows/CI%20-%20Unit/badge.svg)

The official .NET client library for [Apache Pulsar](https://pulsar.apache.org/).

DotPulsar is written entirely in C# and implements Apache Pulsar's [binary protocol](https://pulsar.apache.org/docs/en/develop-binary-protocol/).

## What's new?

Have a look at the [changelog](CHANGELOG.md).

## Getting Started

Let's take a look at a "Hello world" example, where we first produce a message and then consume it. Note that the topic and subscription will be created if they don't exist.

First, we need a Pulsar setup. Have a look [here](https://pulsar.apache.org/docs/en/standalone-docker/) to see how to setup a local standalone Pulsar instance.
Install the NuGet package [DotPulsar](https://www.nuget.org/packages/DotPulsar/) and copy/paste the code below (you will be needing using declarations for 'DotPulsar' and 'DotPulsar.Extensions').

```csharp
const string myTopic = "persistent://public/default/mytopic";

await using var client = PulsarClient.Builder()
                                     .Build(); // Connecting to pulsar://localhost:6650

await using var producer = client.NewProducer(Schema.String)
                                 .Topic(myTopic)
                                 .Create();

_ = await producer.Send("Hello World"); // Send a message and ignore the returned MessageId

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
- [X] Producer send with custom metadata
- [X] Producer send with event time, sequence id, and delayed message delivery
- [X] Producer send with key and ordering key
- [X] Producer for partitioned topics
- [X] Consumer subscription with initial position and priority level
- [X] Consumer subscription types exclusive, shared, failover, and key shared
- [X] Consumer receive and single + cumulative acknowledge
- [X] Consumer and Reader seek on message-id and publish time
- [X] Consumer unsubscribe
- [X] Consume compacted topics
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

## Roadmap

Help prioritizing the roadmap is most welcome, so please reach out and tell us what you want and need.

## Join Our Community

Apache Pulsar has a [Slack instance](https://pulsar.apache.org/contact/) and there you'll find us in the #dev-dotpulsar channel.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/apache/pulsar-dotpulsar/tags).

## Authors

* **Daniel Blankensteiner** - *Initial work* - [Danske Commodities](https://github.com/DanskeCommodities)

See also the list of [contributors](https://github.com/apache/pulsar-dotpulsar/contributors) who participated in this project.

## License

This project is licensed under the Apache License Version 2.0 - see the [LICENSE](LICENSE) file for details.
