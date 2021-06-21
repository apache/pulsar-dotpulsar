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
    using DotPulsar.Internal.Compression;
    using DotPulsar.Internal.PulsarApi;
    using Exceptions;
    using Internal;
    using Internal.Abstractions;
    using Internal.Extensions;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Pulsar client for creating producers, consumers and readers.
    /// </summary>
    public sealed class PulsarClient : IPulsarClient
    {
        private readonly IConnectionPool _connectionPool;
        private readonly ProcessManager _processManager;
        private readonly IHandleException _exceptionHandler;
        private int _isDisposed;

        public Uri ServiceUrl { get; }

        internal PulsarClient(
            IConnectionPool connectionPool,
            ProcessManager processManager,
            IHandleException exceptionHandler,
            Uri serviceUrl)
        {
            _connectionPool = connectionPool;
            _processManager = processManager;
            _exceptionHandler = exceptionHandler;
            ServiceUrl = serviceUrl;
            _isDisposed = 0;
            DotPulsarEventSource.Log.ClientCreated();
        }

        /// <summary>
        /// Get a builder that can be used to configure and build a PulsarClient instance.
        /// </summary>
        public static IPulsarClientBuilder Builder()
            => new PulsarClientBuilder();

        internal async Task<uint> GetNumberOfPartitions(string topic, CancellationToken cancellationToken)
        {
            var connection = await _connectionPool.FindConnectionForTopic(topic, cancellationToken).ConfigureAwait(false);
            var commandPartitionedMetadata = new CommandPartitionedTopicMetadata() { Topic = topic };
            var response = await connection.Send(commandPartitionedMetadata, cancellationToken).ConfigureAwait(false);

            response.Expect(BaseCommand.Type.PartitionedMetadataResponse);

            if (response.PartitionMetadataResponse.Response == CommandPartitionedTopicMetadataResponse.LookupType.Failed)
                response.PartitionMetadataResponse.Throw();

            return response.PartitionMetadataResponse.Partitions;
        }

        /// <summary>
        /// Create a producer.
        /// </summary>
        public IProducer<TMessage> CreateProducer<TMessage>(ProducerOptions<TMessage> options)
        {
            ThrowIfDisposed();

            var correlationId = Guid.NewGuid();
            var executor = new Executor(correlationId, _processManager, _exceptionHandler);
            var stateManager = new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);

            var producer = new Producer<TMessage>(correlationId, ServiceUrl, options.Topic, _processManager, executor, stateManager, options, this);

            if (options.StateChangedHandler is not null)
                _ = StateMonitor.MonitorProducer(producer, options.StateChangedHandler);
            var process = new ProducerProcess(correlationId, stateManager, producer, _processManager);
            _processManager.Add(process);
            return producer;
        }

        /// <summary>
        /// Create a producer internally.
        /// This method is used to create internal producers for partitioned producer.
        /// </summary>
        internal SubProducer<TMessage> NewSubProducer<TMessage>(string topic, ProducerOptions<TMessage> options, IExecute executor, Guid partitionedProducerGuid,
            uint? partitionIndex = null)
        {
            ThrowIfDisposed();

            ICompressorFactory? compressorFactory = null;

            if (partitionIndex.HasValue)
            {
                topic = $"{topic}-partition-{partitionIndex}";
            }

            if (options.CompressionType != CompressionType.None)
            {
                var compressionType = (Internal.PulsarApi.CompressionType) options.CompressionType;
                compressorFactory = CompressionFactories.CompressorFactories().SingleOrDefault(f => f.CompressionType == compressionType);

                if (compressorFactory is null)
                    throw new CompressionException($"Support for {compressionType} compression was not found");
            }

            var correlationId = Guid.NewGuid();

            var producerName = options.ProducerName;
            var schema = options.Schema;
            var initialSequenceId = options.InitialSequenceId;

            var factory = new ProducerChannelFactory(correlationId, _processManager, _connectionPool, topic, producerName, schema.SchemaInfo, compressorFactory);
            var stateManager = new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);
            var initialChannel = new NotReadyChannel<TMessage>();
            var producer = new SubProducer<TMessage>(correlationId, ServiceUrl, topic, initialSequenceId, _processManager, initialChannel, executor, stateManager, factory, schema);

            if (options.StateChangedHandler is not null && !partitionIndex.HasValue) // the StateChangeHandler of the sub producers in partitioned producers should be disabled.
                _ = StateMonitor.MonitorProducer(producer, options.StateChangedHandler);
            var process = new ProducerProcess(correlationId, stateManager, producer, _processManager, partitionedProducerGuid);
            _processManager.Add(process);
            process.Start();
            return producer;
        }

        /// <summary>
        /// Create a consumer.
        /// </summary>
        public IConsumer<TMessage> CreateConsumer<TMessage>(ConsumerOptions<TMessage> options)
        {
            ThrowIfDisposed();

            var correlationId = Guid.NewGuid();
            var executor = new Executor(correlationId, _processManager, _exceptionHandler);
            var subscribe = new CommandSubscribe
            {
                ConsumerName = options.ConsumerName,
                InitialPosition = (CommandSubscribe.InitialPositionType) options.InitialPosition,
                PriorityLevel = options.PriorityLevel,
                ReadCompacted = options.ReadCompacted,
                Subscription = options.SubscriptionName,
                Topic = options.Topic,
                Type = (CommandSubscribe.SubType) options.SubscriptionType
            };
            var messagePrefetchCount = options.MessagePrefetchCount;
            var messageFactory = new MessageFactory<TMessage>(options.Schema);
            var batchHandler = new BatchHandler<TMessage>(true, messageFactory);
            var decompressorFactories = CompressionFactories.DecompressorFactories();
            var factory = new ConsumerChannelFactory<TMessage>(correlationId, _processManager, _connectionPool, subscribe, messagePrefetchCount, batchHandler, messageFactory, decompressorFactories);
            var stateManager = new StateManager<ConsumerState>(ConsumerState.Disconnected, ConsumerState.Closed, ConsumerState.ReachedEndOfTopic, ConsumerState.Faulted);
            var initialChannel = new NotReadyChannel<TMessage>();
            var consumer = new Consumer<TMessage>(correlationId, ServiceUrl, options.SubscriptionName, options.Topic, _processManager, initialChannel, executor, stateManager, factory);
            if (options.StateChangedHandler is not null)
                _ = StateMonitor.MonitorConsumer(consumer, options.StateChangedHandler);
            var process = new ConsumerProcess(correlationId, stateManager, consumer, options.SubscriptionType == SubscriptionType.Failover);
            _processManager.Add(process);
            process.Start();
            return consumer;
        }

        /// <summary>
        /// Create a reader.
        /// </summary>
        public IReader<TMessage> CreateReader<TMessage>(ReaderOptions<TMessage> options)
        {
            ThrowIfDisposed();

            var correlationId = Guid.NewGuid();
            var executor = new Executor(correlationId, _processManager, _exceptionHandler);
            var subscribe = new CommandSubscribe
            {
                ConsumerName = options.ReaderName,
                Durable = false,
                ReadCompacted = options.ReadCompacted,
                StartMessageId = options.StartMessageId.ToMessageIdData(),
                Subscription = $"Reader-{Guid.NewGuid():N}",
                Topic = options.Topic
            };
            var messagePrefetchCount = options.MessagePrefetchCount;
            var messageFactory = new MessageFactory<TMessage>(options.Schema);
            var batchHandler = new BatchHandler<TMessage>(false, messageFactory);
            var decompressorFactories = CompressionFactories.DecompressorFactories();
            var factory = new ConsumerChannelFactory<TMessage>(correlationId, _processManager, _connectionPool, subscribe, messagePrefetchCount, batchHandler, messageFactory, decompressorFactories);
            var stateManager = new StateManager<ReaderState>(ReaderState.Disconnected, ReaderState.Closed, ReaderState.ReachedEndOfTopic, ReaderState.Faulted);
            var initialChannel = new NotReadyChannel<TMessage>();
            var reader = new Reader<TMessage>(correlationId, ServiceUrl, options.Topic, _processManager, initialChannel, executor, stateManager, factory);
            if (options.StateChangedHandler is not null)
                _ = StateMonitor.MonitorReader(reader, options.StateChangedHandler);
            var process = new ReaderProcess(correlationId, stateManager, reader);
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
