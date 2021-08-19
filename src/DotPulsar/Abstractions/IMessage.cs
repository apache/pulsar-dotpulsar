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
    using System.Buffers;
    using System.Collections.Generic;

    /// <summary>
    /// A message abstraction.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// The id of the message.
        /// </summary>
        MessageId MessageId { get; }

        /// <summary>
        /// The raw payload of the message.
        /// </summary>
        ReadOnlySequence<byte> Data { get; }

        /// <summary>
        /// The name of the producer who produced the message.
        /// </summary>
        string ProducerName { get; }

        /// <summary>
        /// The schema version of the message.
        /// </summary>
        byte[]? SchemaVersion { get; }

        /// <summary>
        /// The sequence id of the message.
        /// </summary>
        ulong SequenceId { get; }

        /// <summary>
        /// The redelivery count (maintained by the broker) of the message.
        /// </summary>
        uint RedeliveryCount { get; }

        /// <summary>
        /// Check whether the message has an event time.
        /// </summary>
        bool HasEventTime { get; }

        /// <summary>
        /// The event time of the message as unix time in milliseconds.
        /// </summary>
        ulong EventTime { get; }

        /// <summary>
        /// The event time of the message as an UTC DateTime.
        /// </summary>
        public DateTime EventTimeAsDateTime { get; }

        /// <summary>
        /// The event time of the message as a DateTimeOffset with an offset of 0.
        /// </summary>
        public DateTimeOffset EventTimeAsDateTimeOffset { get; }

        /// <summary>
        /// Check whether the key been base64 encoded.
        /// </summary>
        bool HasBase64EncodedKey { get; }

        /// <summary>
        /// Check whether the message has a key.
        /// </summary>
        bool HasKey { get; }

        /// <summary>
        /// The key as a string.
        /// </summary>
        string? Key { get; }

        /// <summary>
        /// The key as bytes.
        /// </summary>
        byte[]? KeyBytes { get; }

        /// <summary>
        /// Check whether the message has an ordering key.
        /// </summary>
        bool HasOrderingKey { get; }

        /// <summary>
        /// The ordering key of the message.
        /// </summary>
        byte[]? OrderingKey { get; }

        /// <summary>
        /// The publish time of the message as unix time in milliseconds.
        /// </summary>
        ulong PublishTime { get; }

        /// <summary>
        /// The publish time of the message as an UTC DateTime.
        /// </summary>
        public DateTime PublishTimeAsDateTime { get; }

        /// <summary>
        /// The publish time of the message as a DateTimeOffset with an offset of 0.
        /// </summary>
        public DateTimeOffset PublishTimeAsDateTimeOffset { get; }

        /// <summary>
        /// The properties of the message.
        /// </summary>
        public IReadOnlyDictionary<string, string> Properties { get; }

        /// <summary>
        /// The topic of the message.
        /// </summary>
        public string Topic { get; }
    }
}
