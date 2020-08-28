/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar
{
    using Abstractions;
    using DotPulsar.Internal.PulsarApi;
    using DotPulsar.Internal.Extensions;
    using Exceptions;
    using Internal;
    using Internal.Abstractions;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    /// <summary>
    /// Pulsar client for creating producers, consumers and readers.
    /// </summary>
    public sealed class PulsarClient : IPulsarClient
    {
        private readonly IConnectionPool _connectionPool;
        private readonly ProcessManager _processManager;
        private readonly IHandleException _exceptionHandler;
        private int _isDisposed;

        public PulsarClient(IConnectionPool connectionPool, ProcessManager processManager, IHandleException exceptionHandler)
        {
            _connectionPool = connectionPool;
            _processManager = processManager;
            _exceptionHandler = exceptionHandler;
            _isDisposed = 0;
            DotPulsarEventSource.Log.ClientCreated();
        }

        /// <summary>
        /// Get a builder that can be used to configure and build a PulsarClient instance.
        /// </summary>
        public static IPulsarClientBuilder Builder()
            => new PulsarClientBuilder();

        /// <summary>
        /// Create a producer.
        /// </summary>
        public IProducer CreateProducer(ProducerOptions options)
        {
            ThrowIfDisposed();
            var partitionedTopicMetadata = GetPartitionTopicMetadata(options.Topic).Result;
            if (partitionedTopicMetadata.Partitions > 0)
            {
                var stateManager = new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);
                var producers = new Dictionary<int, IProducer>();
                for (int i = 0; i < partitionedTopicMetadata.Partitions; i++)
                {
                    var subproducerOption = options.Clone() as ProducerOptions;
                    subproducerOption!.Topic = $"{options.Topic}-partition-{i}";
                    producers[i] = CreateProducerWithoutCheckingPartition(subproducerOption);
                }
                var producer = new PartitionedProducer(options.Topic, stateManager, options, partitionedTopicMetadata, producers, options.MessageRouter, this);
                return producer;
            }
            else
            {
                return CreateProducerWithoutCheckingPartition(options);
            }
        }

        internal IProducer CreateProducerWithoutCheckingPartition(ProducerOptions options)
        {
            var correlationId = Guid.NewGuid();
            var executor = new Executor(correlationId, _processManager, _exceptionHandler);
            var factory = new ProducerChannelFactory(correlationId, _processManager, _connectionPool, executor, options);
            var stateManager = new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);
            var producer = new Producer(correlationId, options.Topic, options.InitialSequenceId, _processManager, new NotReadyChannel(), executor, stateManager);
            var process = new ProducerProcess(correlationId, stateManager, factory, producer);
            _processManager.Add(process);
            process.Start();
            return producer;
        }

        public async ValueTask<PartitionedTopicMetadata> GetPartitionTopicMetadata(string topic, CancellationToken cancellationToken = default)
        {
            var connection = await _connectionPool.FindConnectionForTopic(topic, cancellationToken).ConfigureAwait(false);
            var commandPartitionedMetadata = new CommandPartitionedTopicMetadata()
            {
                Topic = topic
            };
            var response = await connection.Send(commandPartitionedMetadata, cancellationToken).ConfigureAwait(false);

            response.Expect(BaseCommand.Type.PartitionedMetadataResponse);

            if (response.PartitionMetadataResponse.Response == CommandPartitionedTopicMetadataResponse.LookupType.Failed)
                response.PartitionMetadataResponse.Throw();

            return new PartitionedTopicMetadata((int) response.PartitionMetadataResponse.Partitions);
        }

        /// <summary>
        /// Create a consumer.
        /// </summary>
        public IConsumer CreateConsumer(ConsumerOptions options)
        {
            ThrowIfDisposed();

            PartitionedTopicMetadata partitionedTopicMetadata = GetPartitionTopicMetadata(options.Topic).Result;

            if (partitionedTopicMetadata.Partitions > 0)
            {
                var consumers = new Dictionary<int, Consumer>();

                try
                {
                    var batchHandler = new BatchHandler(true);
                    uint prefetchCount = Math.Max(1U, (uint) (options.MessagePrefetchCount / partitionedTopicMetadata.Partitions));

                    for (int partitionIndex = 0; partitionIndex < partitionedTopicMetadata.Partitions; partitionIndex++)
                    {
                        var partitionTopicConsumerOptions = new ConsumerOptions(options, $"{options.Topic}-partition-{partitionIndex}", prefetchCount);
                        consumers.Add(partitionIndex, CreateConsumerWithoutCheckingPartition(batchHandler, partitionTopicConsumerOptions));
                    }

                    var stateManager = new StateManager<ConsumerState>(ConsumerState.Disconnected, ConsumerState.Closed, ConsumerState.ReachedEndOfTopic, ConsumerState.Faulted);
                    return new PartitionedConsumer(options.Topic,
                                                   stateManager,
                                                   options,
                                                   partitionedTopicMetadata,
                                                   consumers,
                                                   batchHandler,
                                                   this,
                                                   new Internal.Timer());
                }
                catch
                {
                    foreach (IConsumer consumer in consumers.Values)
                    {
                        // dispose should be pretty fast (thus usage of ValueTask instead of Task)
                        consumer.DisposeAsync().GetAwaiter().GetResult();
                    }

                    throw;
                }
            }
            else
            {
                return CreateConsumerWithoutCheckingPartition(new BatchHandler(true), options);
            }
        }

        internal Consumer CreateConsumerWithoutCheckingPartition(BatchHandler batchHandler, ConsumerOptions options)
        {
            var correlationId = Guid.NewGuid();
            var executor = new Executor(correlationId, _processManager, _exceptionHandler);
            var factory = new ConsumerChannelFactory(correlationId, _processManager, _connectionPool, executor, batchHandler, options);
            var stateManager = new StateManager<ConsumerState>(ConsumerState.Disconnected, ConsumerState.Closed, ConsumerState.ReachedEndOfTopic, ConsumerState.Faulted);
            var consumer = new Consumer(correlationId, options.Topic, _processManager, new NotReadyChannel(), executor, stateManager, factory);
            var process = new ConsumerProcess(correlationId, stateManager, factory, consumer, options.SubscriptionType == SubscriptionType.Failover);
            _processManager.Add(process);
            process.Start();
            return consumer;
        }

        /// <summary>
        /// Create a reader.
        /// </summary>
        public IReader CreateReader(ReaderOptions options)
        {
            ThrowIfDisposed();
            var correlationId = Guid.NewGuid();
            var executor = new Executor(correlationId, _processManager, _exceptionHandler);
            var factory = new ReaderChannelFactory(correlationId, _processManager, _connectionPool, executor, options);
            var stateManager = new StateManager<ReaderState>(ReaderState.Disconnected, ReaderState.Closed, ReaderState.ReachedEndOfTopic, ReaderState.Faulted);
            var reader = new Reader(correlationId, options.Topic, _processManager, new NotReadyChannel(), executor, stateManager);
            var process = new ReaderProcess(correlationId, stateManager, factory, reader);
            _processManager.Add(process);
            process.Start();
            return reader;
        }

        /// <summary>
        /// Dispose the client and all its producers, consumers and readers.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            if (_processManager is IAsyncDisposable disposable)
                await disposable.DisposeAsync().ConfigureAwait(false);

            DotPulsarEventSource.Log.ClientDisposed();
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new PulsarClientDisposedException();
        }
    }
}
