/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

﻿using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace DotPulsar.Internal
{
    public sealed class BatchHandler
    {
        private readonly bool _trackBatches;
        private readonly Queue<Message> _messages;
        private readonly LinkedList<Batch> _batches;

        public BatchHandler(bool trackBatches)
        {
            _trackBatches = trackBatches;
            _messages = new Queue<Message>();
            _batches = new LinkedList<Batch>();
        }

        public Message Add(MessageIdData messageId, PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> data)
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
                var message = new Message(singleMessageId, metadata, singleMetadata, data.Slice(index, singleMetadata.PayloadSize));
                _messages.Enqueue(message);
                index += (uint)singleMetadata.PayloadSize;
            }

            return _messages.Dequeue();
        }

        public Message? GetNext() => _messages.Count == 0 ? null : _messages.Dequeue();

        public void Clear()
        {
            _messages.Clear();
            _batches.Clear();
        }

        public MessageIdData? Acknowledge(MessageIdData messageId)
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

        private sealed class Batch
        {
            private readonly BitArray _acknowledgementIndex;

            public Batch(MessageIdData messageId, int numberOfMessages)
            {
                MessageId = messageId;
                _acknowledgementIndex = new BitArray(numberOfMessages, false);
            }

            public MessageIdData MessageId { get; }

            public void Acknowledge(int batchIndex) => _acknowledgementIndex.Set(batchIndex, true);

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
