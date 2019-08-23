namespace DotPulsar.Internal.Abstractions
{
    public interface IEnqueue<T>
    {
        void Enqueue(T item);
    }
}
