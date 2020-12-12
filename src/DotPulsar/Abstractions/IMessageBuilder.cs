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
    /// A message building abstraction.
    /// </summary>
    public interface IMessageBuilder
    {
        /// <summary>
        /// Timestamp as unix time in milliseconds indicating when the message should be delivered to consumers.
        /// </summary>
        IMessageBuilder DeliverAt(long timestamp);

        /// <summary>
        /// Timestamp as UTC DateTime indicating when the message should be delivered to consumers.
        /// </summary>
        IMessageBuilder DeliverAt(DateTime timestamp);

        /// <summary>
        /// Timestamp as DateTimeOffset indicating when the message should be delivered to consumers.
        /// </summary>
        IMessageBuilder DeliverAt(DateTimeOffset timestamp);

        /// <summary>
        /// The event time of the message as unix time in milliseconds.
        /// </summary>
        IMessageBuilder EventTime(ulong eventTime);

        /// <summary>
        /// The event time of the message as an UTC DateTime.
        /// </summary>
        IMessageBuilder EventTime(DateTime eventTime);

        /// <summary>
        /// The event time of the message as a DateTimeOffset.
        /// </summary>
        IMessageBuilder EventTime(DateTimeOffset eventTime);

        /// <summary>
        /// Set the key of the message for routing policy.
        /// </summary>
        IMessageBuilder Key(string key);

        /// <summary>
        /// Set the key of the message for routing policy.
        /// </summary>
        IMessageBuilder KeyBytes(byte[] key);

        /// <summary>
        /// Set the ordering key of the message for message dispatch in SubscriptionType.KeyShared mode.
        /// The partition key will be used if the ordering key is not specified.
        /// </summary>
        IMessageBuilder OrderingKey(byte[] key);

        /// <summary>
        /// Add/Set a property key/value on the message.
        /// </summary>
        IMessageBuilder Property(string key, string value);

        /// <summary>
        /// Set the sequence id of the message.
        /// </summary>
        IMessageBuilder SequenceId(ulong sequenceId);

        /// <summary>
        /// Set the consumer name.
        /// </summary>
        ValueTask<MessageId> Send(ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default);
    }
}
