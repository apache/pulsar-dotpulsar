/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar
{
    /// <summary>
    /// The consumer building options.
    /// </summary>
    public sealed class ConsumerOptions
    {
        /// <summary>
        /// The default initial position.
        /// </summary>
        public static readonly SubscriptionInitialPosition DefaultInitialPosition = SubscriptionInitialPosition.Latest;

        /// <summary>
        /// The default message prefetch count.
        /// </summary>
        public static readonly uint DefaultMessagePrefetchCount = 1000;

        /// <summary>
        /// The default priority level.
        /// </summary>
        public static readonly int DefaultPriorityLevel = 0;

        /// <summary>
        /// The default of whether to read compacted.
        /// </summary>
        public static readonly bool DefaultReadCompacted = false;

        /// <summary>
        /// The default subscription type.
        /// </summary>
        public static readonly SubscriptionType DefaultSubscriptionType = SubscriptionType.Exclusive;

        /// <summary>
        /// Whether to auto subscribe to new partitions.
        /// </summary>
        public const bool DefaultAutoUpdatePartitions = true;

        /// <summary>
        /// The default interval to check for new partitions to subscribe to.
        /// </summary>
        public const int DefaultAutoUpdatePartitionsInterval = 60;

        public ConsumerOptions(string subscriptionName, string topic)
        {
            InitialPosition = DefaultInitialPosition;
            PriorityLevel = DefaultPriorityLevel;
            MessagePrefetchCount = DefaultMessagePrefetchCount;
            ReadCompacted = DefaultReadCompacted;
            SubscriptionType = DefaultSubscriptionType;
            SubscriptionName = subscriptionName;
            AutoUpdatePartitions = DefaultAutoUpdatePartitions;
            AutoUpdatePartitionsInterval = DefaultAutoUpdatePartitionsInterval;
            Topic = topic;
        }

        public ConsumerOptions(ConsumerOptions options, string topic, uint messagePrefetchCount)
        {
            Topic = topic;
            MessagePrefetchCount = messagePrefetchCount;

            InitialPosition = options.InitialPosition;
            PriorityLevel = options.PriorityLevel;
            ReadCompacted = options.ReadCompacted;
            SubscriptionType = options.SubscriptionType;
            SubscriptionName = options.SubscriptionName;
            AutoUpdatePartitions = options.AutoUpdatePartitions;
            AutoUpdatePartitionsInterval = options.AutoUpdatePartitionsInterval;
        }

        /// <summary>
        /// Set the consumer name. This is optional.
        /// </summary>
        public string? ConsumerName { get; set; }

        /// <summary>
        /// Set initial position for the subscription. The default is 'Latest'.
        /// </summary>
        public SubscriptionInitialPosition InitialPosition { get; set; }

        /// <summary>
        /// Set the priority level for the shared subscription consumer. The default is 0.
        /// </summary>
        public int PriorityLevel { get; set; }

        /// <summary>
        /// Number of messages that will be prefetched. The default is 1000.
        /// </summary>
        public uint MessagePrefetchCount { get; set; }

        /// <summary>
        /// Whether to read from the compacted topic. The default is 'false'.
        /// </summary>
        public bool ReadCompacted { get; set; }

        /// <summary>
        /// Set the subscription name for this consumer. This is required.
        /// </summary>
        public string SubscriptionName { get; set; }

        /// <summary>
        /// Set the subscription type for this consumer. The default is 'Exclusive'.
        /// </summary>
        public SubscriptionType SubscriptionType { get; set; }

        /// <summary>
        /// Set the topic for this consumer. This is required.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// True if you want new partitiones to be consumed, false otherwise
        /// </summary>
        public bool AutoUpdatePartitions { get; set; }

        /// <summary>
        /// Time interval which determines frequency with which we are going to check for new partitions (in seconds).
        /// </summary>
        public int AutoUpdatePartitionsInterval { get; set; }
    }
}
