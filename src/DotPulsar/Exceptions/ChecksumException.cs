namespace DotPulsar.Exceptions
{
    public sealed class ChecksumException : DotPulsarException
    {
        public ChecksumException(string message) : base(message) { }

        public ChecksumException(uint expectedChecksum, uint actualChecksum) : base($"Checksum mismatch. Excepted {expectedChecksum} but was actually {actualChecksum}") { }
    }
}
