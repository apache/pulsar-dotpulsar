namespace DotPulsar.Exceptions
{
    public sealed class ChecksumException : DotPulsarException
    {
        public ChecksumException(string message) : base(message) { }
    }
}
