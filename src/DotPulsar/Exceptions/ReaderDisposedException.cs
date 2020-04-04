namespace DotPulsar.Exceptions
{
    using Internal;
    using System;

    public sealed class ReaderDisposedException : ObjectDisposedException
    {
        public ReaderDisposedException() : base(typeof(Reader).FullName) { }
    }
}
