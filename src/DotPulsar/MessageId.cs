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
    using DotPulsar.Internal.Extensions;
    using Internal.PulsarApi;
    using System;

    /// <summary>
    /// Unique identifier of a single message.
    /// </summary>
    public sealed class MessageId : IEquatable<MessageId>, IComparable<MessageId>
    {
        static MessageId()
        {
            Earliest = new MessageId(ulong.MaxValue, ulong.MaxValue, -1, -1);
            Latest = new MessageId(long.MaxValue, long.MaxValue, -1, -1);
        }

        /// <summary>
        /// The oldest message available in the topic.
        /// </summary>
        public static MessageId Earliest { get; }

        /// <summary>
        /// The next message published in the topic.
        /// </summary>
        public static MessageId Latest { get; }

        /// <summary>
        /// Initializes a new instance using the specified ledgerId, entryId, partition and batchIndex.
        /// </summary>
        public MessageId(ulong ledgerId, ulong entryId, int partition, int batchIndex)
        {
            LedgerId = ledgerId;
            EntryId = entryId;
            Partition = partition;
            BatchIndex = batchIndex;
        }

        /// <summary>
        /// The id of the ledger.
        /// </summary>
        public ulong LedgerId { get; }

        /// <summary>
        /// The id of the entry.
        /// </summary>
        public ulong EntryId { get; }

        /// <summary>
        /// The partition.
        /// </summary>
        public int Partition { get; }

        /// <summary>
        /// The batch index.
        /// </summary>
        public int BatchIndex { get; }

        public int CompareTo(MessageId? other)
        {
            if (other is null)
                return 1;

            var result = LedgerId.CompareTo(other.LedgerId);
            if (result != 0)
                return result;

            result = EntryId.CompareTo(other.EntryId);
            if (result != 0)
                return result;

            result = Partition.CompareTo(other.Partition);
            if (result != 0)
                return result;

            return BatchIndex.CompareTo(other.BatchIndex);
        }

        public static bool operator >(MessageId x, MessageId y)
            => x is not null && x.CompareTo(y) >= 1;

        public static bool operator <(MessageId x, MessageId y)
            => x is not null ? x.CompareTo(y) <= -1 : y is not null;

        public static bool operator >=(MessageId x, MessageId y)
            => x is not null ? x.CompareTo(y) >= 0 : y is null;

        public static bool operator <=(MessageId x, MessageId y)
            => x is null || x.CompareTo(y) <= 0;

        public override bool Equals(object? o)
            => o is MessageId id && Equals(id);

        public bool Equals(MessageId? other)
            => other is not null && LedgerId == other.LedgerId && EntryId == other.EntryId && Partition == other.Partition && BatchIndex == other.BatchIndex;

        public static bool operator ==(MessageId x, MessageId y)
            => ReferenceEquals(x, y) || (x is not null && x.Equals(y));

        public static bool operator !=(MessageId x, MessageId y)
            => !(x == y);

        public override int GetHashCode()
            => HashCode.Combine(LedgerId, EntryId, Partition, BatchIndex);

        public override string ToString()
            => $"{LedgerId}:{EntryId}:{Partition}:{BatchIndex}";

        internal MessageIdData ToMessageIdData()
        {
            var messageIdData = new MessageIdData();
            messageIdData.MapFrom(this);
            return messageIdData;
        }
    }
}
