namespace DotPulsar
{
    public sealed class ConsumerOptions
    {
        public ConsumerOptions()
        {
            InitialPosition = SubscriptionInitialPosition.Latest;
            MessagePrefetchCount = 1000;
            SubscriptionType = SubscriptionType.Exclusive;
        }

        public string ConsumerName { get; set; }
        public SubscriptionInitialPosition InitialPosition { get; set; }
        public int PriorityLevel { get; set; }
        public uint MessagePrefetchCount { get; set; }
        public string SubscriptionName { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public string Topic { get; set; }
    }
}
