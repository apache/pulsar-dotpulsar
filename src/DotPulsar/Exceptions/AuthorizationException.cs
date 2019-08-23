namespace DotPulsar.Exceptions
{
    public sealed class AuthorizationException : DotPulsarException
    {
        public AuthorizationException(string message) : base(message) { }
    }
}
