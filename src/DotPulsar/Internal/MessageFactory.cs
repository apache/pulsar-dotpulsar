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

namespace DotPulsar.Internal
{
    using Abstractions;
    using DotPulsar.Abstractions;
    using PulsarApi;
    using System.Buffers;
    using System.Collections.Generic;

    public sealed class MessageFactory<TValue> : IMessageFactory<TValue>
    {
        private static readonly Dictionary<string, string> _empty;

        static MessageFactory() => _empty = new Dictionary<string, string>();

        private static IReadOnlyDictionary<string, string> FromKeyValueList(List<KeyValue> keyValues)
        {
            if (keyValues.Count == 0)
                return _empty;

            var dictionary = new Dictionary<string, string>(keyValues.Count);

            for (var i = 0; i < keyValues.Count; ++i)
            {
                var keyValue = keyValues[i];
                dictionary.Add(keyValue.Key, keyValue.Value);
            }

            return dictionary;
        }

        private readonly ISchema<TValue> _schema;

        public MessageFactory(ISchema<TValue> schema)
            => _schema = schema;

        public IMessage<TValue> Create(string topic, MessageId messageId, uint redeliveryCount, ReadOnlySequence<byte> data, MessageMetadata metadata, SingleMessageMetadata? singleMetadata = null)
        {
            if (singleMetadata is null)
                return Create(topic, messageId, redeliveryCount, metadata, data);

            return Create(topic, messageId, redeliveryCount, metadata, singleMetadata, data);
        }

        private Message<TValue> Create(
            string topic,
            MessageId messageId,
            uint redeliveryCount,
            MessageMetadata metadata,
            ReadOnlySequence<byte> data)
        {
            return new Message<TValue>(
                topic: topic,
                messageId: messageId,
                data: data,
                producerName: metadata.ProducerName,
                sequenceId: metadata.SequenceId,
                redeliveryCount: redeliveryCount,
                eventTime: metadata.EventTime,
                publishTime: metadata.PublishTime,
                properties: FromKeyValueList(metadata.Properties),
                hasBase64EncodedKey: metadata.PartitionKeyB64Encoded,
                key: metadata.PartitionKey,
                orderingKey: metadata.OrderingKey,
                schemaVersion: metadata.SchemaVersion,
                _schema);
        }

        private Message<TValue> Create(
            string topic,
            MessageId messageId,
            uint redeliveryCount,
            MessageMetadata metadata,
            SingleMessageMetadata singleMetadata,
            ReadOnlySequence<byte> data)
        {
            return new Message<TValue>(
                topic: topic,
                messageId: messageId,
                data: data,
                producerName: metadata.ProducerName,
                sequenceId: singleMetadata.SequenceId,
                redeliveryCount: redeliveryCount,
                eventTime: singleMetadata.EventTime,
                publishTime: metadata.PublishTime,
                properties: FromKeyValueList(singleMetadata.Properties),
                hasBase64EncodedKey: singleMetadata.PartitionKeyB64Encoded,
                key: singleMetadata.PartitionKey,
                orderingKey: singleMetadata.OrderingKey,
                schemaVersion: metadata.SchemaVersion,
                _schema);
        }
    }
}
