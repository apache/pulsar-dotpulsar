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
    using Internal.Extensions;
    using Internal.PulsarApi;
    using System;

    /// <summary>
    /// The message metadata builder.
    /// </summary>
    public sealed class MessageMetadata
    {
        public MessageMetadata()
            => Metadata = new Internal.PulsarApi.MessageMetadata();

        internal readonly Internal.PulsarApi.MessageMetadata Metadata;

        /// <summary>
        /// The delivery time of the message (unix time in milliseconds).
        /// </summary>
        public long DeliverAtTime
        {
            get => Metadata.DeliverAtTime;
            set => Metadata.DeliverAtTime = value;
        }

        /// <summary>
        /// The delivery time of the message.
        /// </summary>
        public DateTimeOffset DeliverAtTimeAsDateTimeOffset
        {
            get => Metadata.GetDeliverAtTimeAsDateTimeOffset();
            set => Metadata.SetDeliverAtTime(value);
        }

        /// <summary>
        /// The event time of the message (unix time in milliseconds).
        /// </summary>
        public ulong EventTime
        {
            get => Metadata.EventTime;
            set => Metadata.EventTime = value;
        }

        /// <summary>
        /// The event time of the message.
        /// </summary>
        public DateTimeOffset EventTimeAsDateTimeOffset
        {
            get => Metadata.GetEventTimeAsDateTimeOffset();
            set => Metadata.SetEventTime(value);
        }

        /// <summary>
        /// The key of the message as a string.
        /// </summary>
        public string? Key
        {
            get => Metadata.PartitionKey;
            set => Metadata.SetKey(value);
        }

        /// <summary>
        /// The key of the message as bytes.
        /// </summary>
        public byte[]? KeyBytes
        {
            get => Metadata.GetKeyAsBytes();
            set => Metadata.SetKey(value);
        }

        /// <summary>
        /// The ordering key of the message.
        /// </summary>
        public byte[]? OrderingKey
        {
            get => Metadata.OrderingKey;
            set => Metadata.OrderingKey = value;
        }

        /// <summary>
        /// The properties of the message.
        /// </summary>
        public string? this[string key]
        {
            get
            {
                for (var i = 0; i < Metadata.Properties.Count; ++i)
                {
                    var prop = Metadata.Properties[i];

                    if (prop.Key == key)
                        return prop.Value;
                }

                return null;
            }
            set
            {
                for (var i = 0; i < Metadata.Properties.Count; ++i)
                {
                    var prop = Metadata.Properties[i];

                    if (prop.Key != key)
                        continue;

                    prop.Value = value;

                    return;
                }

                Metadata.Properties.Add(new KeyValue { Key = key, Value = value });
            }
        }

        /// <summary>
        /// The sequence id of the message.
        /// </summary>
        public ulong SequenceId
        {
            get => Metadata.SequenceId;
            set => Metadata.SequenceId = value;
        }
    }
}
