using System;

namespace DotPulsar.Exceptions
{
    public sealed class PulsarClientDisposedException : ObjectDisposedException
    {
        public PulsarClientDisposedException() : base(typeof(PulsarClient).FullName) { }
    }
}
