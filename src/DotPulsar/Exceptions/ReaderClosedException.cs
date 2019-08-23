namespace DotPulsar.Exceptions
{
    public sealed class ReaderClosedException : DotPulsarException
    {
        public ReaderClosedException() : base("Reader has closed") { }
    }
}
