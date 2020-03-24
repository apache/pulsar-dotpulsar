using DotPulsar.Internal;
using System;

namespace DotPulsar.Exceptions
{
    public sealed class ReaderDisposedException : ObjectDisposedException
    {
        public ReaderDisposedException() : base(typeof(Reader).FullName) { }
    }
}
