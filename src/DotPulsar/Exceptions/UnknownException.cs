namespace DotPulsar.Exceptions
{
    public sealed class UnknownException : DotPulsarException
    {
        public UnknownException(string message) : base(message) { }
    }
}
