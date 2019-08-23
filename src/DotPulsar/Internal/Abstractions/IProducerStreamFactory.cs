using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal.Abstractions
{
    public interface IProducerStreamFactory
    {
        Task<IProducerStream> CreateStream(IProducerProxy proxy, CancellationToken cancellationToken = default);
    }
}
