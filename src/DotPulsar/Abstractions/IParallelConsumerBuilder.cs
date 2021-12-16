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

namespace DotPulsar.Abstractions
{
    using System;

    public interface IParallelConsumerBuilder
    {
        /// <summary>
        /// Set the consumer name. This is optional.
        /// </summary>
        IParallelConsumerBuilder ConsumerName(string name);

        /// <summary>
        /// Set initial position for the subscription. The default is 'Latest'.
        /// </summary>
        IParallelConsumerBuilder InitialPosition(SubscriptionInitialPosition initialPosition);

        /// <summary>
        /// Set the priority level for the shared subscription consumer. The default is 0.
        /// </summary>
        IParallelConsumerBuilder PriorityLevel(int priorityLevel);

        /// <summary>
        /// Number of messages that will be prefetched. The default is 1000.
        /// </summary>
        IParallelConsumerBuilder MessagePrefetchCount(uint count);

        /// <summary>
        /// Number of partitions to prefetch messages from. The default is 2.
        /// Number of messages which will be prefetched is MessagePrefetchCount / PartitionPrefetchCount.
        /// This setting is ignored if topic is not partitioned.
        /// </summary>
        IParallelConsumerBuilder PartitionPrefetchCount(uint count);

        /// <summary>
        /// Max number of parallel workers to use when processing messages. The default is 10.
        /// </summary>
        IParallelConsumerBuilder MaxParallelWorkersCount(uint count);

        /// <summary>
        /// Whether to read from the compacted topic. The default is 'false'.
        /// </summary>
        IParallelConsumerBuilder ReadCompacted(bool readCompacted);

        /// <summary>
        /// Set the subscription name for this consumer. This is required.
        /// </summary>
        IParallelConsumerBuilder SubscriptionName(string name);

        /// <summary>
        /// Set the subscription type for this consumer. The default is 'Exclusive'.
        /// </summary>
        IParallelConsumerBuilder SubscriptionType(SubscriptionType type);

        /// <summary>
        /// Set KeyShared subscription policy for consumer (can only be specified if subscription type is KeyShared).
        /// By default, KeyShared subscription use auto split hash range to maintain consumers, and allowOutOfOrderDelivery is false.
        /// </summary>
        IParallelConsumerBuilder KeySharedPolicy(KeySharedPolicy? keySharedPolicy);

        /// <summary>
        /// Set the topic for this consumer. This is required.
        /// </summary>
        IParallelConsumerBuilder Topic(string topic);

        /// <summary>
        /// Set if partitions should be auto updated. Only applies to partitioned topics. The default is true.
        /// </summary>
        IParallelConsumerBuilder AutoUpdatePartitions(bool autoUpdatePartitions);

        /// <summary>
        /// Set partitions auto update interval. Only applies to partitioned topics. The default is 60 seconds.
        /// </summary>
        IParallelConsumerBuilder AutoUpdatePartitionsInterval(int autoUpdatePartitionsInterval);

        /// <summary>
        /// After what time message should be redelivered.
        /// Messages won't be re-delivered sooner than {delay} but can be redelivered later by as much as >= 2 * {delay}.
        /// </summary>
        IParallelConsumerBuilder NegativeAcknowledgeRedeliveryDelay(TimeSpan delay);

        /// <summary>
        /// Create the consumer.
        /// </summary>
        IParallelConsumer Create();
    }
}
