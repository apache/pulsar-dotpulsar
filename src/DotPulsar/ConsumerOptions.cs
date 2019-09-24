namespace DotPulsar
{
    public sealed class ConsumerOptions
    {
        internal const SubscriptionInitialPosition DefaultInitialPosition = SubscriptionInitialPosition.Latest;
        internal const uint DefaultMessagePrefetchCount = 1000;
        internal const int DefaultPriorityLevel = 0;
        internal const bool DefaultReadCompacted = false;
        internal const SubscriptionType DefaultSubscriptionType = SubscriptionType.Exclusive;

        public ConsumerOptions(string subscriptionName, string topic)
        {
            InitialPosition = DefaultInitialPosition;
            PriorityLevel = DefaultPriorityLevel;
            MessagePrefetchCount = DefaultMessagePrefetchCount;
            ReadCompacted = DefaultReadCompacted;
            SubscriptionType = DefaultSubscriptionType;
            SubscriptionName = subscriptionName;
            Topic = topic;
        }

        public string? ConsumerName { get; set; }
        public SubscriptionInitialPosition InitialPosition { get; set; }
        public int PriorityLevel { get; set; }
        public uint MessagePrefetchCount { get; set; }
        public bool ReadCompacted { get; set; }
        public string SubscriptionName { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public string Topic { get; set; }
    }
}
