using System;

namespace DotPulsar.Internal.Exceptions
{
    public sealed class AsyncLockDisposedException : ObjectDisposedException
    {
        public AsyncLockDisposedException() : base(typeof(AsyncLock).FullName) { }
    }
}
