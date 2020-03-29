namespace DotPulsar.Exceptions
{
    using System;
    using Internal;

    public sealed class ReaderDisposedException : ObjectDisposedException
    {
        public ReaderDisposedException() : base(typeof(Reader).FullName) { }
    }
}
