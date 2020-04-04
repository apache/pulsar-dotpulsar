namespace DotPulsar.Exceptions
{
    using Internal;
    using System;

    public sealed class ConsumerDisposedException : ObjectDisposedException
    {
        public ConsumerDisposedException() : base(typeof(Consumer).FullName) { }
    }
}
