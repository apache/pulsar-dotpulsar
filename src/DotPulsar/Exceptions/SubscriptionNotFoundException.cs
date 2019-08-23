namespace DotPulsar.Exceptions
{
    public sealed class SubscriptionNotFoundException : DotPulsarException
    {
        public SubscriptionNotFoundException(string message) : base(message) { }
    }
}
