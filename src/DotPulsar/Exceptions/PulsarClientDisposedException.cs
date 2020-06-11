namespace DotPulsar.Exceptions
{
    using System;

    public sealed class PulsarClientDisposedException : ObjectDisposedException
    {
        public PulsarClientDisposedException() : base(typeof(PulsarClient).FullName) { }
    }
}
