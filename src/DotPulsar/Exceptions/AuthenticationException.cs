namespace DotPulsar.Exceptions
{
    public sealed class AuthenticationException : DotPulsarException
    {
        public AuthenticationException(string message) : base(message) { }
    }
}
