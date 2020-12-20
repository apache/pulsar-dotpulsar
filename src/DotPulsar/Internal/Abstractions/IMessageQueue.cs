namespace DotPulsar.Internal.Abstractions
{
    using System.Threading.Tasks;
    public interface IMessageQueue<T> : IDequeue<T>
    {
        T Acknowledge(T obj);
        T NegativeAcknowledge(T obj);
    }
}