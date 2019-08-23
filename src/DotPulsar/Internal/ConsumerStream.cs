using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class ConsumerStream : IConsumerStream
    {
        private readonly ulong _id;
        private readonly IDequeue<MessagePackage> _dequeue;
        private readonly Connection _connection;
        private readonly IFaultStrategy _faultStrategy;
        private readonly IConsumerProxy _proxy;
        private readonly CommandFlow _commandFlow;
        private uint _sendWhenZero;
        private bool _firstBatch;

        public ConsumerStream(ulong id, uint messagePrefetchCount, IDequeue<MessagePackage> dequeue, Connection connection, IFaultStrategy faultStrategy, IConsumerProxy proxy)
        {
            _id = id;
            _dequeue = dequeue;
            _connection = connection;
            _faultStrategy = faultStrategy;
            _proxy = proxy;
            _commandFlow = new CommandFlow { ConsumerId = id, MessagePermits = messagePrefetchCount };
            _sendWhenZero = 0;
            _firstBatch = true;
        }

        public async Task<Message> Receive(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (_sendWhenZero == 0) //TODO should sending the flow command be handled on other thread and thereby not slow down the consumer?
                {
                    await _connection.Send(_commandFlow);

                    if (_firstBatch)
                    {
                        _commandFlow.MessagePermits = (uint)Math.Ceiling(_commandFlow.MessagePermits * 0.5);
                        _firstBatch = false;
                    }

                    _sendWhenZero = _commandFlow.MessagePermits;
                }

                var messagePackage = await _dequeue.Dequeue(cancellationToken);
                _sendWhenZero--;

                try
                {
                    return Serializer.Deserialize(messagePackage);
                }
                catch (ChecksumException)
                {
                    var ack = new CommandAck
                    {
                        Type = CommandAck.AckType.Individual,
                        validation_error = CommandAck.ValidationError.ChecksumMismatch
                    };
                    ack.MessageIds.Add(messagePackage.Command.MessageId);
                    await Send(ack);
                }
            }
        }

        public async Task Send(CommandAck command)
        {
            try
            {
                command.ConsumerId = _id;
                await _connection.Send(command);
            }
            catch (Exception exception)
            {
                OnException(exception);
                throw;
            }
        }

        public async Task<CommandSuccess> Send(CommandUnsubscribe command)
        {
            try
            {
                command.ConsumerId = _id;
                var response = await _connection.Send(command);
                response.Expect(BaseCommand.Type.Success);
                return response.Success;
            }
            catch (Exception exception)
            {
                OnException(exception);
                throw;
            }
        }

        public async Task<CommandSuccess> Send(CommandSeek command)
        {
            try
            {
                command.ConsumerId = _id;
                var response = await _connection.Send(command);
                response.Expect(BaseCommand.Type.Success);
                return response.Success;
            }
            catch (Exception exception)
            {
                OnException(exception);
                throw;
            }
        }

        public async Task<CommandGetLastMessageIdResponse> Send(CommandGetLastMessageId command)
        {
            try
            {
                command.ConsumerId = _id;
                var response = await _connection.Send(command);
                response.Expect(BaseCommand.Type.GetLastMessageIdResponse);
                return response.GetLastMessageIdResponse;
            }
            catch (Exception exception)
            {
                OnException(exception);
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _connection.Send(new CommandCloseConsumer { ConsumerId = _id }).Wait();
            }
            catch
            {
                // Ignore
            }
        }

        private void OnException(Exception exception)
        {
            if (_faultStrategy.DetermineFaultAction(exception) == FaultAction.Relookup)
                _proxy.Disconnected();
        }
    }
}
