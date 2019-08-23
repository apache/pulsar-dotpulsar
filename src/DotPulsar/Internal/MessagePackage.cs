using DotPulsar.Internal.PulsarApi;
using System.Buffers;

namespace DotPulsar.Internal
{
    public sealed class MessagePackage
    {
        public MessagePackage(CommandMessage command, ReadOnlySequence<byte> data)
        {
            Command = command;
            Data = data;
        }

        public CommandMessage Command { get; }
        public ReadOnlySequence<byte> Data { get; }
    }
}
