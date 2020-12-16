# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.10.0] - 2020-12-16

### Added

- Added performance improvements when receiving data
- Added the IHandleStateChanged\<TStateChanged\> interface for easier state monitoring
- Added StateChangedHandler methods on ConsumerBuilder, ReaderBuilder and ProducerBuilder for easier state monitoring
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
