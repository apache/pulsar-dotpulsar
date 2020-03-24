using System;

namespace DotPulsar.Internal.Exceptions
{
    public sealed class PulsarStreamDisposedException : ObjectDisposedException
    {
        public PulsarStreamDisposedException() : base(typeof(PulsarStream).FullName) { }
    }
}
