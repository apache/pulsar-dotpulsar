namespace DotPulsar.Exceptions
{
    using System;
    using Internal;

    public sealed class ProducerDisposedException : ObjectDisposedException
    {
        public ProducerDisposedException() : base(typeof(Producer).FullName) { }
    }
}
