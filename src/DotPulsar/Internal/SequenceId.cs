namespace DotPulsar.Internal
{
    public sealed class SequenceId
    {
        public SequenceId(ulong initialSequenceId)
        {
            Current = initialSequenceId;
            if (initialSequenceId > 0)
                Increment();
        }

        public ulong Current { get; private set; }

        public void Increment() => ++Current;
    }
}
