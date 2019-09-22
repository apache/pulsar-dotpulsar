namespace DotPulsar.Exceptions
{
    public sealed class ConnectionSecurityException : DotPulsarException
    {
        public ConnectionSecurityException(string message) : base(message) { }
    }
}
