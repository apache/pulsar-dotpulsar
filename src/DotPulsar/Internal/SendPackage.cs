using DotPulsar.Internal.PulsarApi;
using System;

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
        public ReadOnlyMemory<byte> Payload { get; set; }
    }
}
