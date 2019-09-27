using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading.Tasks;

namespace DotPulsar.Internal.Abstractions
{
    public interface IProducerStream : IAsyncDisposable
    {
        Task<CommandSendReceipt> Send(ReadOnlyMemory<byte> payload);
        Task<CommandSendReceipt> Send(PulsarApi.MessageMetadata metadata, ReadOnlyMemory<byte> payload);
    }
}
