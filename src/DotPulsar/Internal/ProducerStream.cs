using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class ProducerStream : IProducerStream
    {
        private readonly PulsarApi.MessageMetadata _cachedMetadata;
        private readonly SendPackage _cachedSendPackage;
        private readonly ulong _id;
        private readonly string _name;
        private readonly SequenceId _sequenceId;
        private readonly Connection _connection;
        private readonly IFaultStrategy _faultStrategy;
        private readonly IProducerProxy _proxy;

        public ProducerStream(ulong id, string name, SequenceId sequenceId, Connection connection, IFaultStrategy faultStrategy, IProducerProxy proxy)
        {
            _cachedMetadata = new PulsarApi.MessageMetadata();
            _cachedSendPackage = new SendPackage(new CommandSend { ProducerId = id, NumMessages = 1 }, _cachedMetadata);
            _id = id;
            _name = name;
            _sequenceId = sequenceId;
            _connection = connection;
            _faultStrategy = faultStrategy;
            _proxy = proxy;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _connection.Send(new CommandCloseProducer { ProducerId = _id });
            }
            catch
            {
                // Ignore
            }
        }

        public async Task<CommandSendReceipt> Send(ReadOnlyMemory<byte> payload) => await Send(_cachedMetadata, payload);

        public async Task<CommandSendReceipt> Send(PulsarApi.MessageMetadata metadata, ReadOnlyMemory<byte> payload)
        {
            try
            {
                metadata.PublishTime = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                metadata.ProducerName = _name;

                if (metadata.SequenceId == 0)
                    metadata.SequenceId = _sequenceId.Current;

                _cachedSendPackage.Command.SequenceId = metadata.SequenceId;
                _cachedSendPackage.Metadata = metadata;
                _cachedSendPackage.Payload = payload;

                var response = await _connection.Send(_cachedSendPackage);
                response.Expect(BaseCommand.Type.SendReceipt); //TODO find out if we should increment on SendError
                _sequenceId.Increment();
                return response.SendReceipt;
            }
            catch (Exception exception)
            {
                if (_faultStrategy.DetermineFaultAction(exception) == FaultAction.Relookup)
                    _proxy.Disconnected();
                throw;
            }
        }
    }
}
