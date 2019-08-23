namespace DotPulsar.Exceptions
{
    public sealed class IncompatibleSchemaException : DotPulsarException
    {
        public IncompatibleSchemaException(string message) : base(message) { }
    }
}
