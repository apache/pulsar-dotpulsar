using DotPulsar.Internal.PulsarApi;
using System.Buffers;

namespace DotPulsar.Internal
{
    public sealed class SendPackage
    {
        public SendPackage(CommandSend command, PulsarApi.MessageMetadata metadata)
        {
            Command = command;
            Metadata = metadata;
        }

        public CommandSend Command { get; }
        public PulsarApi.MessageMetadata Metadata { get; set; }
        public ReadOnlySequence<byte> Payload { get; set; }
    }
}
