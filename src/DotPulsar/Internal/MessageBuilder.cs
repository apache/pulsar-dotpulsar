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
    using DotPulsar.Extensions;
    using Extensions;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class MessageBuilder : IMessageBuilder
    {
        private readonly IProducer _producer;
        private readonly MessageMetadata _metadata;

        public MessageBuilder(IProducer producer)
        {
            _producer = producer;
            _metadata = new MessageMetadata();
        }

        public IMessageBuilder DeliverAt(long timestamp)
        {
            _metadata.Metadata.DeliverAtTime = timestamp;
            return this;
        }

        public IMessageBuilder DeliverAt(DateTime timestamp)
        {
            _metadata.Metadata.SetDeliverAtTime(timestamp);
            return this;
        }

        public IMessageBuilder DeliverAt(DateTimeOffset timestamp)
        {
            _metadata.Metadata.SetDeliverAtTime(timestamp);
            return this;
        }

        public IMessageBuilder EventTime(ulong eventTime)
        {
            _metadata.Metadata.EventTime = eventTime;
            return this;
        }

        public IMessageBuilder EventTime(DateTime eventTime)
        {
            _metadata.Metadata.SetEventTime(eventTime);
            return this;
        }

        public IMessageBuilder EventTime(DateTimeOffset eventTime)
        {
            _metadata.Metadata.SetEventTime(eventTime);
            return this;
        }

        public IMessageBuilder Key(string key)
        {
            _metadata.Metadata.SetKey(key);
            return this;
        }

        public IMessageBuilder KeyBytes(byte[] key)
        {
            _metadata.Metadata.SetKey(key);
            return this;
        }

        public IMessageBuilder OrderingKey(byte[] key)
        {
            _metadata.Metadata.OrderingKey = key;
            return this;
        }

        public IMessageBuilder Property(string key, string value)
        {
            _metadata[key] = value;
            return this;
        }

        public IMessageBuilder SequenceId(ulong sequenceId)
        {
            _metadata.Metadata.SequenceId = sequenceId;
            return this;
        }

        public async ValueTask<MessageId> Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            => await _producer.Send(_metadata, data, cancellationToken).ConfigureAwait(false);
    }
}
