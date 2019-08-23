namespace DotPulsar.Exceptions
{
    public sealed class TopicTerminatedException : DotPulsarException
    {
        public TopicTerminatedException(string message) : base(message) { }
    }
}
