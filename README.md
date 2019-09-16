# DotPulsar

Native .NET/C# client library for [Apache Pulsar](https://pulsar.apache.org/).

DotPulsar is written entirely in C# and implements Apache Pulsar's [binary protocol](https://pulsar.apache.org/docs/en/develop-binary-protocol/). Other options was using the [C++ client library](https://pulsar.apache.org/docs/en/client-libraries-cpp/) (which is what the [Python client](https://pulsar.apache.org/docs/en/client-libraries-python/) and [Go client](https://pulsar.apache.org/docs/en/client-libraries-go/) do) or build on top of the [WebSocket API](https://pulsar.apache.org/docs/en/client-libraries-websocket/). We decided to implement the binary protocol to gain full control and maximize portability and performance.

DotPulsar's API is strongly inspired by Apache Pulsar's official [Java client](https://pulsar.apache.org/docs/en/client-libraries-java/), but a 100% match is not a goal.

## Getting Started

Let's take a look at a "Hello world" example, where we first produce a message and then consume it.

Install the NuGet package [DotPulsar](https://www.nuget.org/packages/DotPulsar/) and copy/paste the code below (you will be needing using declarations for 'DotPulsar' and 'DotPulsar.Extensions').

```csharp
const string myTopic = "persistent://public/default/mytopic";

using var client = PulsarClient.Builder().Build(); //Connecting to localhost:6650

var producer = client.NewProducer()
                     .Topic(myTopic)
                     .Create();
await producer.Send(Encoding.UTF8.GetBytes("Hello World"));

var consumer = client.NewConsumer()
                     .SubscriptionName("MySubscription")
                     .Topic(myTopic)
                     .Create();
var message = await consumer.Receive();
Console.WriteLine("Received: " + Encoding.UTF8.GetString(message.Data.ToArray()));
await consumer.Acknowledge(message);
```

For a more in-depth tour of the API, please visit the [Wiki](https://github.com/danske-commodities/dotpulsar/wiki).

## Supported features

- [X] Service discovery
- [X] Automatic reconnect
- [X] Producer send with custom metadata
- [X] Producer send with event time, sequence id and delayed message delivery
- [X] Producer send with key and ordering key
- [X] Consumer subscription with initial position and priority level
- [X] Consumer subscription types exclusive, shared, failover and key shared
- [X] Consumer receive and single + cumulative acknowledge
- [X] Consumer seek
- [X] Consumer unsubscribe
- [X] Consume compacted topics
- [X] Reader API

## Roadmap

1.0.0

* Move to IAsyncDisposable and IAsyncEnumerable (will mean moving to .NET Standard 2.1)
* Consider using ValueTask instead of Task

X.X.X //Future

* Schema
* Authentication/Authorization (TLS Authentication, Athenz, Kerberos)
* Partitioned topics
* Message compression (LZ4, ZLIB, ZSTD, SNAPPY)
* Multi-topic subscriptions
* Connection encryption
* Message encryption
* Batching
* CommandConsumerStats/CommandConsumerStatsResponse
* CommandGetTopicsOfNamespace/CommandGetTopicsOfNamespaceResponse
* CommandPartitionedTopicMetadata/CommandPartitionedTopicMetadataResponse

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
