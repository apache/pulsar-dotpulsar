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
    using DotPulsar.Internal.PulsarApi;
    using System.Buffers;
    using System.Collections.Generic;

    public static class MessageFactory
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

        public static Message Create(
            MessageId messageId,
            uint redeliveryCount,
            MessageMetadata metadata,
            ReadOnlySequence<byte> data)
        {
            return new Message(
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
                numMessagesInBatch: metadata.NumMessagesInBatch);
        }

        public static Message Create(
            MessageId messageId,
            uint redeliveryCount,
            MessageMetadata metadata,
            SingleMessageMetadata singleMetadata,
            ReadOnlySequence<byte> data)
        {
            return new Message(
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
                numMessagesInBatch: metadata.NumMessagesInBatch);
        }

        /// <summary>
        /// Intended for testing.
        /// </summary>
        public static Message Create(
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
            int numMessagesInBatch) => new Message(messageId, data, producerName, sequenceId, redeliveryCount, eventTime, publishTime, properties, hasBase64EncodedKey, key, orderingKey, numMessagesInBatch);
    }
}
