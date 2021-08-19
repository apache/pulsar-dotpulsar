namespace DotPulsar.Internal
{
    public enum RegexSubscriptionMode : byte
    {
        /// <summary>
        /// Only subscribe to persistent topics.
        /// </summary>
        PersistentOnly,

        /// <summary>
        /// Only subscribe to non-persistent topics.
        /// </summary>
        NonPersistentOnly,

        /// <summary>
        /// Subscribe to both persistent and non-persistent topics.
        /// </summary>
        AllTopics
    }
}
