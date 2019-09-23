namespace DotPulsar
{
    public enum SubscriptionType : byte
    {
        Exclusive = 0,
        Shared = 1,
        Failover = 2,
        //KeyShared = 3 Disabled. Needs protocol version update and testing
    }
}
