# DotPulsar

![CI - Unit](https://github.com/apache/pulsar-dotpulsar/workflows/CI%20-%20Unit/badge.svg)

The official .NET/C# client library for [Apache Pulsar](https://pulsar.apache.org/).

DotPulsar is written entirely in C# and implements Apache Pulsar's [binary protocol](https://pulsar.apache.org/docs/en/develop-binary-protocol/).

## What's new?

Have a look at the [changelog](CHANGELOG.md).

## Getting Started

Let's take a look at a "Hello world" example, where we first produce a message and then consume it.

Install the NuGet package [DotPulsar](https://www.nuget.org/packages/DotPulsar/) and copy/paste the code below (you will be needing using declarations for 'DotPulsar' and 'DotPulsar.Extensions').

```csharp
const string myTopic = "persistent://public/default/mytopic";

await using var client = PulsarClient.Builder()
                                     .Build(); //Connecting to pulsar://localhost:6650

var producer = client.NewProducer()
                     .Topic(myTopic)
                     .Create();

_ = await producer.Send(Encoding.UTF8.GetBytes("Hello World"));

var consumer = client.NewConsumer()
                     .SubscriptionName("MySubscription")
                     .Topic(myTopic)
                     .Create();

await foreach (var message in consumer.Messages())
{
    Console.WriteLine("Received: " + Encoding.UTF8.GetString(message.Data.ToArray()));
    await consumer.Acknowledge(message);
}
```

For a more in-depth tour of the API, please visit the [Wiki](https://github.com/apache/pulsar-dotpulsar/wiki).

## Supported features

- [X] Service discovery
- [X] Automatic reconnect
- [X] TLS connections
- [X] TLS Authentication
- [X] JSON Web Token Authentication
- [X] Producer send with custom metadata
- [X] Producer send with event time, sequence id, and delayed message delivery
- [X] Producer send with key and ordering key
- [X] Consumer subscription with initial position and priority level
- [X] Consumer subscription types exclusive, shared, failover, and key shared
- [X] Consumer receive and single + cumulative acknowledge
- [X] Consumer and Reader seek on message-id and publish time
- [X] Consumer unsubscribe
- [X] Consume compacted topics
- [X] Reader API
- [X] Read/Consume/Acknowledge batched messages
- [X] Pulsar Proxy
- [X] [LZ4 message compression](https://github.com/apache/pulsar-dotpulsar/wiki/Compression)
- [X] [ZLIB message compression](https://github.com/apache/pulsar-dotpulsar/wiki/Compression)
- [X] [ZSTD message compression](https://github.com/apache/pulsar-dotpulsar/wiki/Compression)
- [X] [SNAPPY message compression](https://github.com/apache/pulsar-dotpulsar/wiki/Compression)

## Roadmap

Help prioritizing the roadmap is most welcome, so please reach out and tell us what you want and need.

## Join Our Community

Apache Pulsar has a [Slack instance](https://pulsar.apache.org/contact/) and there you'll find us in the #dev-dotpulsar channel. Just waiting for you to pop by :-)

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/apache/pulsar-dotpulsar/tags).

## Authors

* **Daniel Blankensteiner** - *Initial work* - [Danske Commodities](https://github.com/danske-commodities)

See also the list of [contributors](https://github.com/apache/pulsar-dotpulsar/contributors) who participated in this project.

## License

This project is licensed under the Apache License Version 2.0 - see the [LICENSE](LICENSE) file for details.
