using DotPulsar.Abstractions;
using DotPulsar.Internal;

namespace DotPulsar.Extensions
{
    public static class ProducerExtensions
    {
        public static IMessageBuilder NewMessage(this IProducer producer) => new MessageBuilder(producer);
    }
}
