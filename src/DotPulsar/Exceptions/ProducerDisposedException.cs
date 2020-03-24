using DotPulsar.Internal;
using System;

namespace DotPulsar.Exceptions
{
    public sealed class ProducerDisposedException : ObjectDisposedException
    {
        public ProducerDisposedException() : base(typeof(Producer).FullName) { }
    }
}
