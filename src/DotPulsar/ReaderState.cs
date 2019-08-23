namespace DotPulsar
{
    public enum ReaderState : byte
    {
        Closed,
        Connected,
        Disconnected,
        Faulted,
        ReachedEndOfTopic
    }
}
