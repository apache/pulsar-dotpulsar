namespace DotPulsar.Exceptions
{
    public sealed class TopicNotFoundException : DotPulsarException
    {
        public TopicNotFoundException(string message) : base(message) { }
    }
}
