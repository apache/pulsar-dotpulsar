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
    using Internal;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A consumer abstraction.
    /// </summary>
    public interface IConsumer : IGetLastMessageId, ISeek, IState<ConsumerState>, IAsyncDisposable
    {
        /// <summary>
        /// Acknowledge the consumption of a single message using the MessageId.
        /// </summary>
        ValueTask Acknowledge(IMessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acknowledge the consumption of all the messages in the topic up to and including the provided MessageId.
        /// </summary>
        ValueTask AcknowledgeCumulative(IMessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// The consumer's service url.
        /// </summary>
        public Uri ServiceUrl { get; }

        /// <summary>
        /// The consumer's subscription name.
        /// </summary>
        public string SubscriptionName { get; }

        /// <summary>
        /// The consumer's topic.
        /// </summary>
        string Topic { get; }

        /// <summary>
        /// The consumer's topic list.
        /// </summary>
        ISet<string>? TopicNames { get; }

        /// <summary>
        /// The consumer's topic pattern.
        /// </summary>
        string? TopicsPattern { get; }

        /// <summary>
        /// The consumer's topic partition number.
        /// </summary>
        uint NumberOfPartitions { get; }

        /// <summary>
        /// Controls whether the consumers automatically update partition data.
        /// </summary>
        bool AutoUpdatePartitions { get; }

        /// <summary>
        /// Set the interval of updating partitions, the default is 1 minute.
        /// This only works when autoUpdatePartitions is true.
        /// </summary>
        TimeSpan AutoUpdatePartitionsInterval { get; }

        /// <summary>
        /// The subscription mode for TopicsPattern.
        /// </summary>
        RegexSubscriptionMode RegexSubscriptionMode { get; }

        /// <summary>
        /// Unsubscribe the consumer.
        /// </summary>
        ValueTask Unsubscribe(CancellationToken cancellationToken = default);

        /// <summary>
        /// Redeliver the pending messages that were pushed to this consumer that are not yet acknowledged.
        /// </summary>
        ValueTask RedeliverUnacknowledgedMessages(IEnumerable<IMessageId> messageIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Redeliver all pending messages that were pushed to this consumer that are not yet acknowledged.
        /// </summary>
        ValueTask RedeliverUnacknowledgedMessages(CancellationToken cancellationToken = default);
    }
}
