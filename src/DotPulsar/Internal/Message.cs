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

namespace DotPulsar.Internal
{
    using DotPulsar.Abstractions;
    using System;
    using System.Buffers;
    using System.Collections.Generic;

    public sealed class Message<TValue> : IMessage<TValue>
    {
        private readonly ISchema<TValue> _schema;

        internal Message(
            string topic,
            MessageId messageId,
            ReadOnlySequence<byte> data,
            string producerName,
            ulong sequenceId,
            uint redeliveryCount,
            ulong eventTime,
            ulong publishTime,
            IReadOnlyDictionary<string, string> properties,
            bool hasBase64EncodedKey,
            string? key,
            byte[]? orderingKey,
            byte[]? schemaVersion,
            ISchema<TValue> schema)
        {
            Topic = topic;
            MessageId = messageId;
            Data = data;
            ProducerName = producerName;
            SequenceId = sequenceId;
            RedeliveryCount = redeliveryCount;
            EventTime = eventTime;
            PublishTime = publishTime;
            Properties = properties;
            HasBase64EncodedKey = hasBase64EncodedKey;
            Key = key;
            OrderingKey = orderingKey;
            SchemaVersion = schemaVersion;
            _schema = schema;
        }

        public MessageId MessageId { get; }

        public ReadOnlySequence<byte> Data { get; }

        public string ProducerName { get; }

        public byte[]? SchemaVersion { get; }

        public ulong SequenceId { get; }

        public uint RedeliveryCount { get; }

        public bool HasEventTime => EventTime != 0;

        public ulong EventTime { get; }

        public DateTime EventTimeAsDateTime => EventTimeAsDateTimeOffset.UtcDateTime;

        public DateTimeOffset EventTimeAsDateTimeOffset => DateTimeOffset.FromUnixTimeMilliseconds((long) EventTime);

        public bool HasBase64EncodedKey { get; }

        public bool HasKey => Key is not null;

        public string? Key { get; }

        public byte[]? KeyBytes => Key is not null ? Convert.FromBase64String(Key) : null;

        public bool HasOrderingKey => OrderingKey is not null;

        public byte[]? OrderingKey { get; }

        public ulong PublishTime { get; }

        public DateTime PublishTimeAsDateTime => PublishTimeAsDateTimeOffset.UtcDateTime;

        public DateTimeOffset PublishTimeAsDateTimeOffset => DateTimeOffset.FromUnixTimeMilliseconds((long) PublishTime);

        public IReadOnlyDictionary<string, string> Properties { get; }

        public TValue Value() => _schema.Decode(Data);

        public string Topic { get; }
    }
}
