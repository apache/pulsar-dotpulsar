namespace DotPulsar.Exceptions
{
    public sealed class InvalidTopicNameException : DotPulsarException
    {
        public InvalidTopicNameException(string message) : base(message) { }
    }
}
