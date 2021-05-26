namespace DotPulsar.Internal.Abstractions
{
    public interface IMessageTracker
    {
        void Add(MessageId messageId);
    }
}
