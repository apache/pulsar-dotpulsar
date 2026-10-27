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
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System.Buffers;
using System.Collections;

public sealed class BatchHandler<TMessage>
{
    private readonly object _lock;
    private readonly bool _trackBatches;
    private readonly IMessageFactory<TMessage> _messageFactory;
    private readonly Queue<IMessage<TMessage>> _messages;
    private readonly Dictionary<(ulong LedgerId, ulong EntryId, int Partition), Batch> _batches;

    public BatchHandler(bool trackBatches, IMessageFactory<TMessage> messageFactory)
    {
        _lock = new object();
        _trackBatches = trackBatches;
        _messageFactory = messageFactory;
        _messages = new Queue<IMessage<TMessage>>();
        _batches = new Dictionary<(ulong LedgerId, ulong EntryId, int Partition), Batch>();
    }

    public IMessage<TMessage> Add(MessageIdData messageId, uint redeliveryCount, MessageMetadata metadata, ReadOnlySequence<byte> data)
    {
        var messages = new List<IMessage<TMessage>>(metadata.NumMessagesInBatch);

        long index = 0;

        for (var i = 0; i < metadata.NumMessagesInBatch; ++i)
        {
            var singleMetadataSize = data.ReadUInt32(index, true);
            index += 4;
            var singleMetadata = SingleMessageMetadata.Parser.ParseFrom(data.Slice(index, singleMetadataSize));
            index += singleMetadataSize;
            var singleMessageId = new MessageId(messageId.LedgerId, messageId.EntryId, messageId.Partition, i);
            var message = _messageFactory.Create(singleMessageId, redeliveryCount, data.Slice(index, singleMetadata.PayloadSize), metadata, singleMetadata);
            messages.Add(message);
            index += (uint) singleMetadata.PayloadSize;
        }

        lock (_lock)
        {
            var key = (messageId.LedgerId, messageId.EntryId, messageId.Partition);
            // Redelivery must not replace acknowledgements already recorded for this batch.
            if (_trackBatches && !_batches.ContainsKey(key))
                _batches.Add(key, new Batch(messageId, metadata.NumMessagesInBatch));

            foreach (var message in messages)
            {
                _messages.Enqueue(message);
            }

            return _messages.Dequeue();
        }
    }

    public IMessage<TMessage>? GetNext()
    {
        lock (_lock)
            return _messages.Count == 0 ? null : _messages.Dequeue();
    }

    public void Clear()
    {
        lock (_lock)
        {
            _messages.Clear();
            _batches.Clear();
        }
    }

    public MessageIdData? Acknowledge(MessageIdData messageId)
    {
        lock (_lock)
        {
            var key = (messageId.LedgerId, messageId.EntryId, messageId.Partition);
            if (!_batches.TryGetValue(key, out var batch) || !batch.Acknowledge(messageId.BatchIndex))
                return null;

            _batches.Remove(key);
            return batch.MessageId;
        }
    }

    private sealed class Batch
    {
        private readonly BitArray _acknowledgementIndex;
        private int _remaining;

        public Batch(MessageIdData messageId, int numberOfMessages)
        {
            MessageId = messageId;
            _acknowledgementIndex = new BitArray(numberOfMessages, false);
            _remaining = numberOfMessages;
        }

        public MessageIdData MessageId { get; }

        public bool Acknowledge(int batchIndex)
        {
            if (!_acknowledgementIndex[batchIndex])
            {
                _acknowledgementIndex[batchIndex] = true;
                --_remaining;
            }

            return _remaining == 0;
        }
    }
}
