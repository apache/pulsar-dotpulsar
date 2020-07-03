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
    using Extensions;
    using PulsarApi;
    using System.Buffers;
    using System.Collections;
    using System.Collections.Generic;

    public sealed class BatchHandler
    {
        private readonly object _lock;
        private readonly bool _trackBatches;
        private readonly Queue<Message> _messages;
        private readonly LinkedList<Batch> _batches;

        public BatchHandler(bool trackBatches)
        {
            _lock = new object();
            _trackBatches = trackBatches;
            _messages = new Queue<Message>();
            _batches = new LinkedList<Batch>();
        }

        public Message Add(MessageIdData messageId, uint redeliveryCount, MessageMetadata metadata, ReadOnlySequence<byte> data)
        {
            lock (_lock)
            {
                if (_trackBatches)
                    _batches.AddLast(new Batch(messageId, metadata.NumMessagesInBatch));

                long index = 0;

                for (var i = 0; i < metadata.NumMessagesInBatch; ++i)
                {
                    var singleMetadataSize = data.ReadUInt32(index, true);
                    index += 4;
                    var singleMetadata = Serializer.Deserialize<SingleMessageMetadata>(data.Slice(index, singleMetadataSize));
                    index += singleMetadataSize;
                    var singleMessageId = new MessageId(messageId.LedgerId, messageId.EntryId, messageId.Partition, i);
                    var message = new Message(singleMessageId, redeliveryCount, metadata, singleMetadata, data.Slice(index, singleMetadata.PayloadSize));
                    _messages.Enqueue(message);
                    index += (uint) singleMetadata.PayloadSize;
                }

                return _messages.Dequeue();
            }
        }

        public Message? GetNext()
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
                foreach (var batch in _batches)
                {
                    if (messageId.LedgerId != batch.MessageId.LedgerId ||
                        messageId.EntryId != batch.MessageId.EntryId ||
                        messageId.Partition != batch.MessageId.Partition)
                        continue;

                    batch.Acknowledge(messageId.BatchIndex);

                    if (batch.IsAcknowledged())
                    {
                        _batches.Remove(batch);
                        return batch.MessageId;
                    }

                    break;
                }

                return null;
            }
        }

        private sealed class Batch
        {
            private readonly BitArray _acknowledgementIndex;

            public Batch(MessageIdData messageId, int numberOfMessages)
            {
                MessageId = messageId;
                _acknowledgementIndex = new BitArray(numberOfMessages, false);
            }

            public MessageIdData MessageId { get; }

            public void Acknowledge(int batchIndex)
                => _acknowledgementIndex.Set(batchIndex, true);

            public bool IsAcknowledged()
            {
                for (var i = 0; i < _acknowledgementIndex.Length; i++)
                {
                    if (!_acknowledgementIndex[i])
                        return false;
                }

                return true;
            }
        }
    }
}
