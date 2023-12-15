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

namespace DotPulsar.Internal.Extensions;

using System.Text;
using Metadata = PulsarApi.MessageMetadata;

public static class MessageMetadataExtensions
{
    // Deliver at
    public static DateTime GetDeliverAtTimeAsDateTime(this Metadata metadata)
        => metadata.GetDeliverAtTimeAsDateTimeOffset().UtcDateTime;

    public static void SetDeliverAtTime(this Metadata metadata, DateTime timestamp)
        => metadata.SetDeliverAtTime(new DateTimeOffset(timestamp));

    public static DateTimeOffset GetDeliverAtTimeAsDateTimeOffset(this Metadata metadata)
        => DateTimeOffset.FromUnixTimeMilliseconds(metadata.DeliverAtTime);

    public static void SetDeliverAtTime(this Metadata metadata, DateTimeOffset timestamp)
        => metadata.DeliverAtTime = timestamp.ToUnixTimeMilliseconds();

    // Event time
    public static DateTime GetEventTimeAsDateTime(this Metadata metadata)
        => metadata.GetEventTimeAsDateTimeOffset().UtcDateTime;

    public static void SetEventTime(this Metadata metadata, DateTime timestamp)
        => metadata.SetEventTime(new DateTimeOffset(timestamp));

    public static DateTimeOffset GetEventTimeAsDateTimeOffset(this Metadata metadata)
        => DateTimeOffset.FromUnixTimeMilliseconds((long) metadata.EventTime);

    public static void SetEventTime(this Metadata metadata, DateTimeOffset timestamp)
        => metadata.EventTime = (ulong) timestamp.ToUnixTimeMilliseconds();

    // Key
    public static byte[]? GetKeyAsBytes(this Metadata metadata)
    {
        if (metadata.PartitionKey is null)
            return null;

        if (metadata.PartitionKeyB64Encoded)
            return Convert.FromBase64String(metadata.PartitionKey);

        return Encoding.UTF8.GetBytes(metadata.PartitionKey);
    }

    public static void SetKey(this Metadata metadata, string? key)
    {
        metadata.PartitionKey = key;
        metadata.PartitionKeyB64Encoded = false;
    }

    public static void SetKey(this Metadata metadata, byte[]? key)
    {
        if (key is null)
            return;

        metadata.PartitionKey = Convert.ToBase64String(key);
        metadata.PartitionKeyB64Encoded = true;
    }
}
