namespace DotPulsar.Internal
{
    using System.Threading.Tasks;
    using Abstractions;
    using DotPulsar.Abstractions;

    public class InactiveUnackedMessageTracker : IUnackedMessageTracker
    {
        public InactiveUnackedMessageTracker()
        {
        }

        public void Ack(MessageId messageId)
        {
            return;
        }

        public void Add(MessageId messageId)
        {
            return;
        }

        public Task Start(IConsumer consumer) => Task.CompletedTask;

        public void Dispose() {
            return;
        }

    }
}
