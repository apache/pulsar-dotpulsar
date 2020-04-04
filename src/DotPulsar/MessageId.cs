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
    using Internal.PulsarApi;
    using System;

    public sealed class MessageId : IEquatable<MessageId>
    {
        static MessageId()
        {
            Earliest = new MessageId(ulong.MaxValue, ulong.MaxValue, -1, -1);
            Latest = new MessageId(long.MaxValue, long.MaxValue, -1, -1);
        }

        public static MessageId Earliest { get; }
        public static MessageId Latest { get; }

        internal MessageId(MessageIdData messageIdData)
            => Data = messageIdData;

        public MessageId(ulong ledgerId, ulong entryId, int partition, int batchIndex)
            => Data = new MessageIdData
            {
                LedgerId = ledgerId,
                EntryId = entryId,
                Partition = partition,
                BatchIndex = batchIndex
            };

        internal MessageIdData Data { get; }

        public ulong LedgerId => Data.LedgerId;
        public ulong EntryId => Data.EntryId;
        public int Partition => Data.Partition;
        public int BatchIndex => Data.BatchIndex;

        public override bool Equals(object o)
            => o is MessageId id && Equals(id);

        public bool Equals(MessageId other)
            => LedgerId == other.LedgerId && EntryId == other.EntryId && Partition == other.Partition && BatchIndex == other.BatchIndex;

        public static bool operator ==(MessageId x, MessageId y)
            => x.Equals(y);

        public static bool operator !=(MessageId x, MessageId y)
            => !x.Equals(y);

        public override int GetHashCode()
            => HashCode.Combine(LedgerId, EntryId, Partition, BatchIndex);

        public override string ToString()
            => $"LedgerId: {LedgerId}, EntryId: {EntryId}, Partition: {Partition}, BatchIndex: {BatchIndex}";
    }
}
