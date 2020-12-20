namespace DotPulsar.Internal.Abstractions
{
    using System.Threading.Tasks;
    public interface IMessageAcksTracker<T>
    {
        T Add(T message);
        T Ack(T message);
        T Nack(T message);
    }
}