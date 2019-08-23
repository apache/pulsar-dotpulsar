namespace DotPulsar.Exceptions
{
    public sealed class PersistenceException : DotPulsarException
    {
        public PersistenceException(string message) : base(message) { }
    }
}
