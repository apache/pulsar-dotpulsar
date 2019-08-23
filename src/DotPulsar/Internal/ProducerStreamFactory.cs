using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class ProducerStreamFactory : IProducerStreamFactory
    {
        private readonly ConnectionPool _connectionTool;
        private readonly ProducerOptions _options;
        private readonly IFaultStrategy _faultStrategy;
        private readonly SequenceId _sequenceId;

        public ProducerStreamFactory(ConnectionPool connectionManager, ProducerOptions options, IFaultStrategy faultStrategy)
        {
            _connectionTool = connectionManager;
            _options = options;
            _faultStrategy = faultStrategy;
            _sequenceId = new SequenceId(options.InitialSequenceId);
        }

        public async Task<IProducerStream> CreateStream(IProducerProxy proxy, CancellationToken cancellationToken)
        {
            var commandProducer = new CommandProducer
            {
                ProducerName = _options.ProducerName,
                Topic = _options.Topic
            };

            while (true)
            {
                try
                {
                    var connection = await _connectionTool.FindConnectionForTopic(_options.Topic, cancellationToken);
                    var response = await connection.Send(commandProducer, proxy);
                    return new ProducerStream(response.ProducerId, response.ProducerName, _sequenceId, connection, _faultStrategy, proxy);
                }
                catch (OperationCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw;
                    else
                        await Task.Delay(_faultStrategy.TimeToWait, cancellationToken);
                }
                catch (Exception exception)
                {
                    switch (_faultStrategy.DetermineFaultAction(exception))
                    {
                        case FaultAction.Relookup:
                        case FaultAction.Retry:
                            await Task.Delay(_faultStrategy.TimeToWait, cancellationToken);
                            continue;
                    }

                    throw;
                }
            }
        }
    }
}
