using DotPulsar.Internal;
using System;

namespace DotPulsar.Exceptions
{
    public sealed class ConsumerDisposedException : ObjectDisposedException
    {
        public ConsumerDisposedException() : base(typeof(Consumer).FullName) { }
    }
}
