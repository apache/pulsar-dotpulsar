# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
