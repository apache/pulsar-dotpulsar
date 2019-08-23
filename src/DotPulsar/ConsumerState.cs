namespace DotPulsar
{
    public enum ConsumerState : byte
    {
        Active,
        Closed,
        Disconnected,
        Faulted,
        Inactive,
        ReachedEndOfTopic
    }
}
