﻿/*
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
    using Internal;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A consumer building abstraction.
    /// </summary>
    public interface IConsumerBuilder<TMessage>
    {
        /// <summary>
        /// Set the consumer name. This is optional.
        /// </summary>
        IConsumerBuilder<TMessage> ConsumerName(string name);

        /// <summary>
        /// Set initial position for the subscription. The default is 'Latest'.
        /// </summary>
        IConsumerBuilder<TMessage> InitialPosition(SubscriptionInitialPosition initialPosition);

        /// <summary>
        /// Number of messages that will be prefetched. The default is 1000.
        /// </summary>
        IConsumerBuilder<TMessage> MessagePrefetchCount(uint count);

        /// <summary>
        /// Set the priority level for the shared subscription consumer. The default is 0.
        /// </summary>
        IConsumerBuilder<TMessage> PriorityLevel(int priorityLevel);

        /// <summary>
        /// Whether to read from the compacted topic. The default is 'false'.
        /// </summary>
        IConsumerBuilder<TMessage> ReadCompacted(bool readCompacted);

        /// <summary>
        /// Register a state changed handler.
        /// </summary>
        IConsumerBuilder<TMessage> StateChangedHandler(IHandleStateChanged<ConsumerStateChanged> handler);

        /// <summary>
        /// Set the subscription name for this consumer. This is required.
        /// </summary>
        IConsumerBuilder<TMessage> SubscriptionName(string name);

        /// <summary>
        /// Set the subscription type for this consumer. The default is 'Exclusive'.
        /// </summary>
        IConsumerBuilder<TMessage> SubscriptionType(SubscriptionType type);

        /// <summary>
        /// Set the topic for this consumer. This is required.
        /// </summary>
        IConsumerBuilder<TMessage> Topic(string topic);

        /// <summary>
        /// Create the consumer.
        /// </summary>
        IConsumer<TMessage> Create();

        /// <summary>
        /// Specify a list of topics that this consumer will subscribe on.
        /// </summary>
        /// <param name="topicNames">a list of topic that the consumer will subscribe on</param>
        IConsumerBuilder<TMessage> Topics(IEnumerable<string> topicNames);

        /// <summary>
        /// Specify a pattern for topics that this consumer will subscribe on.
        /// </summary>
        /// <param name="topicsPattern">a regular expression to select a list of topics to subscribe to</param>
        IConsumerBuilder<TMessage> TopicsPattern(string topicsPattern);

        /// <summary>
        /// The subscription mode for TopicsPattern.
        /// </summary>
        IConsumerBuilder<TMessage> RegexSubscriptionMode(RegexSubscriptionMode mode);

        /// <summary>
        /// Controls whether the consumers automatically update partition data, the default is false.
        /// </summary>
        IConsumerBuilder<TMessage> AutoUpdatePartitions(bool enabled);

        /// <summary>
        /// Set the interval of updating partitions, the default is 1 minute.
        /// This only works when autoUpdatePartitions is true.
        /// </summary>
        IConsumerBuilder<TMessage> AutoUpdatePartitionsInterval(TimeSpan timeSpan);
    }
}
