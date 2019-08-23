namespace DotPulsar.Exceptions
{
    public sealed class UnsupportedVersionException : DotPulsarException
    {
        public UnsupportedVersionException(string message) : base(message) { }
    }
}
