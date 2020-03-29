namespace DotPulsar.Exceptions
{
    using System;
    using Internal;

    public sealed class ConsumerDisposedException : ObjectDisposedException
    {
        public ConsumerDisposedException() : base(typeof(Consumer).FullName) { }
    }
}
