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
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A consumer building abstraction.
    /// </summary>
    public interface IConsumerBuilder
    {
        /// <summary>
        /// Set the consumer name. This is optional.
        /// </summary>
        IConsumerBuilder ConsumerName(string name);

        /// <summary>
        /// Set initial position for the subscription. The default is 'Latest'.
        /// </summary>
        IConsumerBuilder InitialPosition(SubscriptionInitialPosition initialPosition);

        /// <summary>
        /// Number of messages that will be prefetched. The default is 1000.
        /// </summary>
        IConsumerBuilder MessagePrefetchCount(uint count);

        /// <summary>
        /// Set the priority level for the shared subscription consumer. The default is 0.
        /// </summary>
        IConsumerBuilder PriorityLevel(int priorityLevel);

        /// <summary>
        /// Whether to read from the compacted topic. The default is 'false'.
        /// </summary>
        IConsumerBuilder ReadCompacted(bool readCompacted);

        /// <summary>
        /// Register a state changed handler.
        /// </summary>
        IConsumerBuilder StateChangedHandler(IHandleStateChanged<ConsumerStateChanged> handler);

        /// <summary>
        /// Register a state changed handler.
        /// </summary>
        IConsumerBuilder StateChangedHandler(Action<ConsumerStateChanged, CancellationToken> handler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Register a state changed handler.
        /// </summary>
        IConsumerBuilder StateChangedHandler(Func<ConsumerStateChanged, CancellationToken, ValueTask> handler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set the subscription name for this consumer. This is required.
        /// </summary>
        IConsumerBuilder SubscriptionName(string name);

        /// <summary>
        /// Set the subscription type for this consumer. The default is 'Exclusive'.
        /// </summary>
        IConsumerBuilder SubscriptionType(SubscriptionType type);

        /// <summary>
        /// Set the topic for this consumer. This is required.
        /// </summary>
        IConsumerBuilder Topic(string topic);

        /// <summary>
        /// Create the consumer.
        /// </summary>
        IConsumer Create();
    }
}
