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

namespace DotPulsar;

using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;

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
    /// Initializes a new instance using the specified ledgerId, entryId, partition, and batchIndex.
    /// </summary>
    public MessageId(ulong ledgerId, ulong entryId, int partition, int batchIndex, string topic = "")
    {
        LedgerId = ledgerId;
        EntryId = entryId;
        Partition = partition;
        BatchIndex = batchIndex;
        Topic = topic;
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

    /// <summary>
    /// Return the full topic name of a message.
    /// </summary>
    public string Topic { get; }

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

        result = String.Compare(Topic, other.Topic, StringComparison.Ordinal);
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
        => other is not null && LedgerId == other.LedgerId && EntryId == other.EntryId && Partition == other.Partition && BatchIndex == other.BatchIndex && Topic == other.Topic;

    public static bool operator ==(MessageId x, MessageId y)
        => ReferenceEquals(x, y) || (x is not null && x.Equals(y));

    public static bool operator !=(MessageId x, MessageId y)
        => !(x == y);

    public override int GetHashCode()
        => HashCode.Combine(LedgerId, EntryId, Partition, BatchIndex, Topic);

    public override string ToString()
    {
        if (Topic == string.Empty)
            return $"{LedgerId}:{EntryId}:{Partition}:{BatchIndex}";
        return $"{LedgerId}:{EntryId}:{Partition}:{BatchIndex}:{Topic}";
    }

    private static bool TryParse(ReadOnlySpan<char> s, out int result)
    {
#if NETSTANDARD2_0
        return Int32.TryParse(s.ToString(), out result);
#else
        return Int32.TryParse(s, out result);
#endif
    }

    private static bool TryParse(ReadOnlySpan<char> s, out ulong result)
    {
#if NETSTANDARD2_0
        return UInt64.TryParse(s.ToString(), out result);
#else
        return UInt64.TryParse(s, out result);
#endif
    }

    /// <summary>
    /// Converts the string representation of a message id to its object equivalent. A return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="s">A string containing a message id to convert.</param>
    /// <param name="result">
    /// When this method returns, contains the MessageId equivalent of the string contained in s, if the conversion succeeded, or MessageId.Earliest if the conversion failed.
    /// The conversion fails if the s parameter is null or Empty, or is not of the correct format.
    /// This parameter is passed uninitialized; any value originally supplied in result will be overwritten.
    /// </param>
    /// <returns> true if the string was converted successfully; otherwise, false. </returns>
    public static bool TryParse(string s, out MessageId result)
    {
        result = Earliest;
        if (string.IsNullOrWhiteSpace(s))
            return false;
        var input = s.AsMemory();
        var startOfNextEntry = 0;
        var index = 0;
        var field = 0;
        ulong ledgerId = 0;
        ulong entryId = 0;
        int partition = 0;
        int batchIndex = 0;
        var topic = string.Empty;

        while (index <= s.Length)
        {
            if (index == s.Length || s[index] == ':')
            {
                var length = index - startOfNextEntry;
                if (length == 0)
                    return false;

                var value = input.Slice(startOfNextEntry, length);
                if (field == 0 && !TryParse(value.Span, out ledgerId))
                    return false;
                else if (field == 1 && !TryParse(value.Span, out entryId))
                    return false;
                else if (field == 2 && !TryParse(value.Span, out partition))
                    return false;
                else if (field == 3)
                {
                    if (!TryParse(value.Span, out batchIndex))
                        return false;
                    if (index + 1 < s.Length)
                        topic = s.Substring(index + 1);
                    break;
                }

                startOfNextEntry = index + 1;
                ++field;
            }

            ++index;
        }

        result = new MessageId(ledgerId, entryId, partition, batchIndex, topic);
        return true;
    }

    internal MessageIdData ToMessageIdData()
    {
        var messageIdData = new MessageIdData();
        messageIdData.MapFrom(this);
        return messageIdData;
    }
}
