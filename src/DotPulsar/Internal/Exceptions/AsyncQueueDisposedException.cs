using System;

namespace DotPulsar.Internal.Exceptions
{
    public sealed class AsyncQueueDisposedException : ObjectDisposedException
    {
        public AsyncQueueDisposedException() : base(typeof(AsyncQueue<>).FullName) { }
    }
}
