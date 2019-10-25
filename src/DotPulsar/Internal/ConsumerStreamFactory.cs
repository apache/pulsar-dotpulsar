using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class ConsumerStreamFactory : IConsumerStreamFactory
    {
        private readonly ConnectionPool _connectionPool;
        private readonly IFaultStrategy _faultStrategy;
        private readonly CommandSubscribe _subscribe;
        private readonly uint _messagePrefetchCount;
        private readonly BatchHandler _batchHandler;

        public ConsumerStreamFactory(ConnectionPool connectionPool, ConsumerOptions options, IFaultStrategy faultStrategy)
        {
            _connectionPool = connectionPool;
            _faultStrategy = faultStrategy;
            _messagePrefetchCount = options.MessagePrefetchCount;
            _batchHandler = new BatchHandler(true);

            _subscribe = new CommandSubscribe
            {
                ConsumerName = options.ConsumerName,
                initialPosition = (CommandSubscribe.InitialPosition)options.InitialPosition,
                PriorityLevel = options.PriorityLevel,
                ReadCompacted = options.ReadCompacted,
                Subscription = options.SubscriptionName,
                Topic = options.Topic,
                Type = (CommandSubscribe.SubType)options.SubscriptionType
            };
        }

        public ConsumerStreamFactory(ConnectionPool connectionPool, ReaderOptions options, IFaultStrategy faultStrategy)
        {
            _connectionPool = connectionPool;
            _faultStrategy = faultStrategy;
            _messagePrefetchCount = options.MessagePrefetchCount;
            _batchHandler = new BatchHandler(false);

            _subscribe = new CommandSubscribe
            {
                ConsumerName = options.ReaderName,
                Durable = false,
                ReadCompacted = options.ReadCompacted,
                StartMessageId = options.StartMessageId.Data,
                Subscription = "Reader-" + Guid.NewGuid().ToString("N"),
                Topic = options.Topic
            };
        }

        public async Task<IConsumerStream> CreateStream(IConsumerProxy proxy, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    var connection = await _connectionPool.FindConnectionForTopic(_subscribe.Topic, cancellationToken);
                    var response = await connection.Send(_subscribe, proxy);
                    return new ConsumerStream(response.ConsumerId, _messagePrefetchCount, proxy, connection, _faultStrategy, proxy, _batchHandler);
                }
                catch (OperationCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw;
                    else
                        await Task.Delay(_faultStrategy.RetryInterval, cancellationToken);
                }
                catch (Exception exception)
                {
                    switch (_faultStrategy.DetermineFaultAction(exception))
                    {
                        case FaultAction.Relookup:
                        case FaultAction.Retry:
                            await Task.Delay(_faultStrategy.RetryInterval, cancellationToken);
                            continue;
                    }

                    throw;
                }
            }
        }
    }
}
