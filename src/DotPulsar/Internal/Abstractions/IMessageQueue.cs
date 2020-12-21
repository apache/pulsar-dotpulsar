namespace DotPulsar.Internal.Abstractions
{
    using System.Threading.Tasks;
    public interface IMessageQueue
    {
        MessageId Acknowledge(MessageId obj);
        MessageId NegativeAcknowledge(MessageId obj);
    }
}