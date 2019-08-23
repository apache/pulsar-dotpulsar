using DotPulsar.Internal.PulsarApi;
using System;

namespace DotPulsar.Internal
{
    public sealed class SendPackage
    {
        public SendPackage(CommandSend command) => Command = command;

        public CommandSend Command { get; }
        public PulsarApi.MessageMetadata Metadata { get; set; }
        public ReadOnlyMemory<byte> Payload { get; set; }
    }
}
