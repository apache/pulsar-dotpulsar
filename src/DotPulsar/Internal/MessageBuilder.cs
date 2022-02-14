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

namespace DotPulsar.Internal;

using DotPulsar.Abstractions;
using Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class MessageBuilder<TMessage> : IMessageBuilder<TMessage>
{
    private readonly IProducer<TMessage> _producer;
    internal readonly MessageMetadata _metadata;

    public MessageBuilder(IProducer<TMessage> producer)
    {
        _producer = producer;
        _metadata = new MessageMetadata();
    }

    public IMessageBuilder<TMessage> DeliverAt(long timestamp)
    {
        _metadata.Metadata.DeliverAtTime = timestamp;
        return this;
    }

    public IMessageBuilder<TMessage> DeliverAt(DateTime timestamp)
    {
        _metadata.Metadata.SetDeliverAtTime(timestamp);
        return this;
    }

    public IMessageBuilder<TMessage> DeliverAt(DateTimeOffset timestamp)
    {
        _metadata.Metadata.SetDeliverAtTime(timestamp);
        return this;
    }

    public IMessageBuilder<TMessage> EventTime(ulong eventTime)
    {
        _metadata.Metadata.EventTime = eventTime;
        return this;
    }

    public IMessageBuilder<TMessage> EventTime(DateTime eventTime)
    {
        _metadata.Metadata.SetEventTime(eventTime);
        return this;
    }

    public IMessageBuilder<TMessage> EventTime(DateTimeOffset eventTime)
    {
        _metadata.Metadata.SetEventTime(eventTime);
        return this;
    }

    public IMessageBuilder<TMessage> Key(string key)
    {
        _metadata.Metadata.SetKey(key);
        return this;
    }

    public IMessageBuilder<TMessage> KeyBytes(byte[] key)
    {
        _metadata.Metadata.SetKey(key);
        return this;
    }

    public IMessageBuilder<TMessage> OrderingKey(byte[] key)
    {
        _metadata.Metadata.OrderingKey = key;
        return this;
    }

    public IMessageBuilder<TMessage> Property(string key, string value)
    {
        _metadata[key] = value;
        return this;
    }

    public IMessageBuilder<TMessage> SchemaVersion(byte[] schemaVersion)
    {
        _metadata.Metadata.SchemaVersion = schemaVersion;
        return this;
    }

    public IMessageBuilder<TMessage> SequenceId(ulong sequenceId)
    {
        _metadata.Metadata.SequenceId = sequenceId;
        return this;
    }

    public async ValueTask<MessageId> Send(TMessage message, CancellationToken cancellationToken)
        => await _producer.Send(_metadata, message, cancellationToken).ConfigureAwait(false);
}
