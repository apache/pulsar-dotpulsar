using DotPulsar.Internal.PulsarApi;
using System;
using System.Buffers;
using System.Threading.Tasks;

namespace DotPulsar.Internal.Abstractions
{
    public interface IProducerStream : IAsyncDisposable
    {
        Task<CommandSendReceipt> Send(ReadOnlySequence<byte> payload);
        Task<CommandSendReceipt> Send(PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> payload);
    }
}
