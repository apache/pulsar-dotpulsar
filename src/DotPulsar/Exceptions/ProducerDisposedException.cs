namespace DotPulsar.Exceptions
{
    using Internal;
    using System;

    public sealed class ProducerDisposedException : ObjectDisposedException
    {
        public ProducerDisposedException() : base(typeof(Producer).FullName) { }
    }
}
