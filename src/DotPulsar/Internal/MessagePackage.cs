using DotPulsar.Internal.PulsarApi;
using System.Buffers;

namespace DotPulsar.Internal
{
    public struct MessagePackage
    {
        public MessagePackage(MessageIdData messageId, ReadOnlySequence<byte> data)
        {
            MessageId = messageId;
            Data = data;
        }

        public MessageIdData MessageId { get; }
        public ReadOnlySequence<byte> Data { get; }
    }
}
