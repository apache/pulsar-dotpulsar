namespace DotPulsar.Exceptions
{
    public sealed class ProducerBlockedQuotaExceededException : DotPulsarException
    {
        public ProducerBlockedQuotaExceededException(string message) : base(message) { }
    }
}
