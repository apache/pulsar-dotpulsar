using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal.Abstractions
{
    public interface IDequeue<T>
    {
        Task<T> Dequeue(CancellationToken cancellationToken = default);
    }
}
