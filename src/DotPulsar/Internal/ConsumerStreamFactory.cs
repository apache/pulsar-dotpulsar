using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class ConsumerStreamFactory : IConsumerStreamFactory
    {
        private readonly ConnectionPool _connectionTool;
        private readonly IFaultStrategy _faultStrategy;
        private readonly CommandSubscribe _subscribe;
        private readonly uint _messagePrefetchCount;

        public ConsumerStreamFactory(ConnectionPool connectionManager, ConsumerOptions options, IFaultStrategy faultStrategy)
        {
            _connectionTool = connectionManager;
            _faultStrategy = faultStrategy;
            _messagePrefetchCount = options.MessagePrefetchCount;

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

        public ConsumerStreamFactory(ConnectionPool connectionManager, ReaderOptions options, IFaultStrategy faultStrategy)
        {
            _connectionTool = connectionManager;
            _faultStrategy = faultStrategy;
            _messagePrefetchCount = options.MessagePrefetchCount;

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
                    var connection = await _connectionTool.FindConnectionForTopic(_subscribe.Topic, cancellationToken);
                    var response = await connection.Send(_subscribe, proxy);
                    return new ConsumerStream(response.ConsumerId, _messagePrefetchCount, proxy, connection, _faultStrategy, proxy);
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
