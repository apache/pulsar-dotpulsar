using System;

namespace DotPulsar.Internal.Exceptions
{
    public sealed class ConnectionDisposedException : ObjectDisposedException
    {
        public ConnectionDisposedException() : base(typeof(Connection).FullName) { }
    }
}
