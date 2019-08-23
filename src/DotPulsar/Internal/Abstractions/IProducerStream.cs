using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading.Tasks;

namespace DotPulsar.Internal.Abstractions
{
    public interface IProducerStream : IDisposable
    {
        Task<CommandSendReceipt> Send(PulsarApi.MessageMetadata metadata, ReadOnlyMemory<byte> payload);
    }
}
