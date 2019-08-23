using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal.Abstractions
{
    public interface IConsumerStreamFactory
    {
        Task<IConsumerStream> CreateStream(IConsumerProxy proxy, CancellationToken cancellationToken = default);
    }
}
