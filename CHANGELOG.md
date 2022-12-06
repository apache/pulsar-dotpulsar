# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.6.0] - 2022-12-06

### Added

- .NET 7 added as a target framework
- Delay option for OnStateChangeFrom/To for IState and StateChangedFrom/To for IConsumer, IProducer, and IReader
- DelayedStateMonitor extension methods for IState with Func and Action callbacks

### Removed

- .NET Core 3.1 removed as a target framework
- .NET 5.0 removed as a target framework

## [2.5.2] - 2022-11-02

### Changed

- A temporarily broken connection could cause DotPulsar to mark consumers, readers, and producers as disconnected while the broker was unaware of the problem. When reconnecting the broker would claim that the consumer, reader or producer is still active. Therefore the default exception handling will now retry (instead of rethrow/fault) on these two exceptions (waiting for the broker to realize that the existing connections are dead)
    - ConsumerBusyException
    - ProducerBusyException

### Fixed

- Under certain circumstances a double connection attempt could be initiated, resulting in one good connection and a looping connection calling the exception pipeline

## [2.5.1] - 2022-11-01

### Fixed

- The 'Process' extension method for IConsumer\<TMessage\> will now use the cancellation token when acknowledging messages

## [2.5.0] - 2022-10-28

### Added

- The 'Process' extension method for IConsumer\<TMessage\> can now be used for parallel processing of messages. You can control:
    - Whether ordered acknowledgment should be enforced
    - The maximum number of messages that may be processed concurrently
    - The maximum number of messages that may be processed per task
    - The TaskScheduler to use for scheduling tasks
- The trace created when sending a message can now be automatically linked to by traces created when consuming the message. All you have to do is:
    - Make sure 'messaging.trace_parent' and 'messaging.trace_state' are not already in use in the message's metadata (properties)
    - Set 'AttachTraceInfoToMessages' on ProducerOptions or IProducerBuilder when creating a producer
    - Set 'LinkTraces' on the ProcessingOptions passed to the 'Process' extension method for IConsumer\<TMessage\>

## [2.4.1] - 2022-09-16

### Changed

- Retain the original stack trace when an exception is thrown in the Producer

## [2.4.0] - 2022-06-14

### Added

- Support for [Zstd compression](https://github.com/apache/pulsar-dotpulsar/wiki/Compression) via the [ZstdSharp.Port](https://www.nuget.org/packages/ZstdSharp.Port) NuGet package

### Fixed

- The presence of [ZstdNet](https://www.nuget.org/packages/ZstdNet) on a non-Windows system will cause DotPulsar to fault consumers, producers, and readers

## [2.3.1] - 2022-04-20

### Changed

- Performance improvements, especially when consuming/reading

## [2.3.0] - 2022-03-18

### Added

- Metrics (the meter name is 'DotPulsar')
    - dotpulsar.client.count - number of active clients (gauge)
    - dotpulsar.connection.count - number of active connections (gauge)
    - dotpulsar.reader.count - number of active readers (gauge with 'topic' tag)
    - dotpulsar.consumer.count - number of active consumers (gauge with 'topic' tag)
    - dotpulsar.producer.count - number of active producers (gauge with 'topic' tag)
    - dotpulsar.producer.send.duration - Measures the duration for sending a message (histogram with 'topic' tag)
    - dotpulsar.consumer.process.duration - Measures the duration for processing a message (histogram with 'topic' and 'subscription' tags)

### Changed

- Adding a property to MessageMetadata with a key or value of null will throw an ArgumentNullException

## [2.2.0] - 2022-02-04

### Added

- Extension methods for IConsumerBuilder, IProducerBuilder, and IReaderBuilder for setting a StateChangedHandler without a cancellation token

### Changed

- The following internal exceptions are now public
    - ChannelNotReadyException (should be ignored when logging exceptions)
    - ConsumerNotFoundException (from the broker, but indicates an internal problem)
    - ServiceNotReadyException (the broker is not ready)
    - TooManyRequestsException (the broker is getting too many requests)

## [2.1.0] - 2022-02-02

### Added

- Support for on the fly authentication (AuthChallenge)
- The IAuthentication interface for implementing custom authentication
- IPulsarClientBuilder.Authentication(IAuthentication authentication) for using custom authentication
- The AuthenticationFactory with support for:
    - Token(string token)
    - Token(Func\<CancellationToken, ValueTask\<string\>\> tokenSupplier)

### Deprecated

- IPulsarClientBuilder.AuthenticateUsingToken(string token) is marked as obsolete. Use Authentication(AuthenticationFactory.Token(...)) instead

## [2.0.1] - 2021-11-12

### Changed

- Only the version number changed. Something went wrong (in the .NET 6 target) when building the NuGet package for release 2.0.0

## [2.0.0] - 2021-11-12

## Added

- .NET 6 added as a target framework
- [Tracing](https://github.com/apache/pulsar-dotpulsar/wiki/Tracing) support following the [guidelines](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md) from the [OpenTelemetry](https://opentelemetry.io/) project
    - Sending a message will create a producer trace and add tracing metadata to the message
    - The 'Process' extension method for IConsumer\<TMessage\> is no longer experimental and will create a consumer trace
    - The 'GetConversationId' extension method for IMessage has been added
    - The 'SetConversationId' and 'GetConversationId' extension methods for MessageMetadata have been added
- The client will send a 'ping' if there has been no activity on the connection for 30 seconds. This default can be changed by setting the 'KeepAliveInterval' on the IPulsarClientBuilder

### Changed

- **Breaking**: Sending a message without metadata is now an extension method and therefore no longer part of the ISend\<TMessage\> (and thereby IProducer\<TMessage\>) interface
- IMessageRouter: ChoosePartition(MessageMetadata? messageMetadata, int numberOfPartitions) -> ChoosePartition(MessageMetadata messageMetadata, int numberOfPartitions)
- The default behavior for the IOException has changed from 'Rethrow' to 'Retry'
- The default behavior for the MetadataException has changed from 'Retry' to 'Rethrow', meaning that it will fault the consumer, reader, and/or producer

## [1.1.2] - 2021-07-05

### Fixed

- The partitioned producer didn't pass metadata to the message router

## [1.1.1] - 2021-07-05

### Fixed

- The producer ignored message metadata provided by the user

## [1.1.0] - 2021-06-29

### Added

- The producer now supports partitioned topics
- The IMessageRouter interface with the RoundRobinPartitionRouter (default) and SinglePartitionRouter implementations
- The producer builder accepts a custom implementation of IMessageRouter

### Changed

- The producer state can now be 'PartiallyConnected'
- The KeyBytes property on MessageMetadata returned null if the key was set via a string. Now it will return string keys as UTF8 bytes

### Fixed

- Autogenerate a consumer and reader name when it's not explicitly set by the user

## [1.0.2] - 2021-04-30

### Fixed

- Closing a consumer or reader while the broker is streaming messages could take down the connection causing other consumers, readers, and producers of the connection to reconnect
- In some circumstances, the protocol bytes could be misread leading to wrong message parsing

## [1.0.1] - 2021-03-30

### Fixed

- Creating a producer with a schema for a non-existing topic failed

## [1.0.0] - 2021-03-17

### Added

- A number of resilience, correctness, and performance improvements
- The optional listener name can be set via the PulsarClientBuilder
- *Experimental*: Added an extension method for IConsumer that will 'Process' and auto-acknowledge messages while creating an Activity (useful for doing tracing)
- Schema support for the following types
    - Boolean
    - Bytes (using byte[] and ReadOnlySequence\<byte\>)
    - String (UTF-8, UTF-16, and US-ASCII)
    - INT8, INT16, INT32, and INT64
    - Float and Double
    - Time (using TimeSpan)
    - Timestamp and Date (using DateTime)

### Changed

- **Breaking**: Building a producer will now create an IProducer\<T\>\
  The non-generic IProducer interface is still there, but messages can only be sent (ISend) with IProducer\<T\>
- **Breaking**: Building a reader or consumer will now create an IConsumer\<T\> or IReader\<T\>\
  The non-generic IReader and IConsumer are still there, but messages can only be consumed/read (IReceive) with IConsumer\<T\> and IReader\<T\>
- **Breaking**: Receiving a message with now return an IMessage\<T\> instead of the Message class (which is now internal)\
  The non-generic IMessage can be used if 'Value()' (decoding the 'Data' bytes) is not used (when just handling raw messages)
- **Breaking**: The message builder is now generic
- Setting an Action and Func StateChangedHandler on the ConsumerBuilder, ReaderBuilder, and ProducerBuilder are now extension methods
- Setting an Action and Func ExceptionHandler on the PulsarClientBuilder are now extension methods

### Fixed

- When the broker sends a CommandClose[Producer/Consumer] all in-flight (and following) requests to the broker are ignored.\
  Even though we reconnect the consumer, reader, or producer the tasks for the in-flight requests will hang as long as the connection is kept alive by other producers/consumers/readers.\
  This situation is now handled and the requests will be sent again on the new connection.

## [0.11.0] - 2021-02-21

### Added

- The Consumer and Reader now share the IReceive interface for receiving a single message: ValueTask\<Message\> Receive(CancellationToken cancellationToken)
- The Consumer and Reader now share the ISeek interface for seeking on message-id and publish time
- The Consumer and Reader now share the IGetLastMessageId interface for getting the last message-id on a topic
- The Consumer, Reader, and Producer now share the IState interface adding 'OnStateChangeFrom' and 'OnStateChangeTo'
- The PulsarClient, Consumer, Reader, and Producer now have the read-only property 'ServiceUrl'
- The Consumer now have the read-only property 'SubscriptionName'
- All message compression types are now supported (listed below). Please read this [compression how-to](https://github.com/apache/pulsar-dotpulsar/wiki/Compression)
    - LZ4
    - SNAPPY
    - ZLIB
    - ZSTD

### Changed

- MessageId.ToString() now returns a string matching that of other clients: "{LedgerId}:{EntryId}:{Partition}:{BatchIndex}"
- A lot of methods were made into extension methods and now require a using statement for 'DotPulsar.Extensions'
    - Producer.StateChangedTo(ProducerState state, CancellationToken cancellationToken)
    - Producer.StateChangedFrom(ProducerState state, CancellationToken cancellationToken)
    - Producer.Send(byte[] data, CancellationToken cancellationToken)
    - Producer.Send(ReadOnlyMemory\<byte\> data, CancellationToken cancellationToken)
    - Producer.Send(MessageMetadata metadata, byte[] data, CancellationToken cancellationToken)
    - Producer.Send(MessageMetadata metadata, ReadOnlyMemory\<byte\> data, CancellationToken cancellationToken)
    - Consumer.Acknowledge(Message message, CancellationToken cancellationToken)
    - Consumer.AcknowledgeCumulative(Message message, CancellationToken cancellationToken)
    - Consumer.StateChangedTo(ConsumerState state, CancellationToken cancellationToken)
    - Consumer.StateChangedFrom(ConsumerState state, CancellationToken cancellationToken)
    - Consumer.Messages(CancellationToken cancellationToken)
    - Consumer.Seek(DateTime publishTime, CancellationToken cancellationToken)
    - Consumer.Seek(DateTimeOffset publishTime, CancellationToken cancellationToken)
    - Reader.StateChangedTo(ReaderState state, CancellationToken cancellationToken)
    - Reader.StateChangedFrom(ReaderState state, CancellationToken cancellationToken)
    - Reader.Messages(CancellationToken cancellationToken)
    - Reader.Seek(DateTime publishTime, CancellationToken cancellationToken)
    - Reader.Seek(DateTimeOffset publishTime, CancellationToken cancellationToken)

### Fixed

- Before the Consumer and Reader would throw an ArgumentOutOfRangeException if they encountered a compressed message. Now they will throw a CompressionException if the compression type is not supported
- DotPulsarEventSource (performance counters) was only enabled for .NET Standard 2.1. Now it's enabled for all target frameworks except .NET Standard 2.0

## [0.10.1] - 2020-12-23

### Added

- MessageId implements IComparable\<MessageId\>

### Fixed

- Do not throw exceptions when disposing consumers, readers, or producers

## [0.10.0] - 2020-12-16

### Added

- Added performance improvements when receiving data
- Added the IHandleStateChanged\<TStateChanged\> interface for easier state monitoring
- Added StateChangedHandler methods on ConsumerBuilder, ReaderBuilder, and ProducerBuilder for easier state monitoring
- Added StateChangedHandler property on ConsumerOptions, ReaderOptions, and ProducerOptions for easier state monitoring
- Added four new DotPulsarExceptions: InvalidTransactionStatusException, NotAllowedException, TransactionConflictException and TransactionCoordinatorNotFoundException
- Added properties on Message to read EventTime and PublishTime as DateTime
- Added methods on the IMessageBuilder to set EventTime and DeliverAt using DateTime
- Added properties on MessageMetadata to set EventTime and DeliverAtTime using DateTime
- Added seeking by MessageId on the Reader
- Added seeking by message publish time on the Consumer and Reader
- Added GetLastMessageId on the Reader

### Changed

- The protobuf-net dependency is upgraded from 2.4.6 to 3.0.73 to get support for ReadOnlySequence\<byte\>

### Fixed

- Reconnection issues when seeking

## [0.9.7] - 2020-12-04

### Added

- Added an ExceptionHandler method on the IPulsarClientBuilder taking an Action\<ExceptionContext\> for easy sync exception handling
- Added .NET 5 as a target framework and started using C# 9.0
- Added performance improvements when sending and receiving data
- The default values for ConsumerOptions, ReaderOptions, and ProducerOptions are now public

## [0.9.6] - 2020-10-15

### Fixed

- Fixed missing metadata properties in batched messages containing only one message
- Fixed potential torn reads in EventCounters
