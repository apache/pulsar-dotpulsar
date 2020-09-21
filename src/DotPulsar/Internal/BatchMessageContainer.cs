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
    using DotPulsar.Internal.Abstractions;
    using System.Buffers;
    using System.Collections.Generic;

    public sealed class BatchMessageContainer : IBatchMessageContainer
    {
        public MessageMetadata MessageMetadata {  get; private set; } = new MessageMetadata();
        public Queue<Message> Messages { get; private set; } = new Queue<Message>();
        private SequenceBuilder<byte> payload = new SequenceBuilder<byte>();
        private int maxMessagesInBatch = 0;
        private int numMessagesInBatch = 0;
        private long maxBatchBytesSize = 0;
        private long currentBatchSize = 0;
        private ulong highestSequenceId = 0;
        
        public bool Add(Message message)
        {
            if (++numMessagesInBatch == 1)
            {
                MessageMetadata.SequenceId = message.SequenceId;
            }

            currentBatchSize += message.Data.Length;
            Messages.Enqueue(message);

            highestSequenceId = message.SequenceId;

            return IsBatchFull();
        }

        private bool IsBatchFull()
        {
            return (maxBatchBytesSize > 0 && currentBatchSize >= maxBatchBytesSize)
                || (maxMessagesInBatch > 0 && numMessagesInBatch >= maxMessagesInBatch);
        }

        public void Clear()
        {
            MessageMetadata = new MessageMetadata();
            Messages.Clear();
            payload = new SequenceBuilder<byte>();
            numMessagesInBatch = 0;
            currentBatchSize = 0;
        }

        public int GetNumMessagesInBatch()
        {
            throw new System.NotImplementedException();
        }

        public bool HaveEnoughSpace(Message message)
        {
            var messageSize = message.Data.Length;
            return (maxBatchBytesSize > 0 && (currentBatchSize + messageSize) >= maxBatchBytesSize)
                || (maxMessagesInBatch > 0 && (numMessagesInBatch + 1) >= maxMessagesInBatch);
        }

        public bool IsEmpty()
        {
            return currentBatchSize == 0;
        }
    }
}
