using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Exceptions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class NotReadyStream : IConsumerStream, IProducerStream
    {
        public void Dispose() { }

        public Task<Message> Receive(CancellationToken cancellationToken) => throw GetException();

        public Task Send(CommandAck command) => throw GetException();

        public Task<CommandSuccess> Send(CommandUnsubscribe command) => throw GetException();

        public Task<CommandSuccess> Send(CommandSeek command) => throw GetException();

        public Task<CommandGetLastMessageIdResponse> Send(CommandGetLastMessageId command) => throw GetException();

        public Task<CommandSendReceipt> Send(PulsarApi.MessageMetadata metadata, ReadOnlyMemory<byte> payload) => throw GetException();

        private Exception GetException() => new StreamNotReadyException();
    }
}
