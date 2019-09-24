using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal.Abstractions
{
    public interface IConsumerStream : IAsyncDisposable
    {
        Task Send(CommandAck command);
        Task<CommandSuccess> Send(CommandUnsubscribe command);
        Task<CommandSuccess> Send(CommandSeek command);
        Task<CommandGetLastMessageIdResponse> Send(CommandGetLastMessageId command);
        Task<Message> Receive(CancellationToken cancellationToken = default);
    }
}
