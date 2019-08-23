namespace DotPulsar.Exceptions
{
    public sealed class PulsarClientClosedException : DotPulsarException
    {
        public PulsarClientClosedException() : base("Client has closed") { }
    }
}
