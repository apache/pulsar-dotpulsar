using System;
using Metadata = DotPulsar.Internal.PulsarApi.MessageMetadata;

namespace DotPulsar.Internal.Extensions
{
    public static class MessageMetadataExtensions
    {
        public static DateTimeOffset GetDeliverAtTimeAsDateTimeOffset(this Metadata metadata)
            => DateTimeOffset.FromUnixTimeMilliseconds(metadata.DeliverAtTime);

        public static void SetDeliverAtTime(this Metadata metadata, DateTimeOffset timestamp)
            => metadata.DeliverAtTime = timestamp.ToUnixTimeMilliseconds();

        public static DateTimeOffset GetEventTimeAsDateTimeOffset(this Metadata metadata)
            => DateTimeOffset.FromUnixTimeMilliseconds((long)metadata.EventTime);

        public static void SetEventTime(this Metadata metadata, DateTimeOffset timestamp)
            => metadata.EventTime = (ulong)timestamp.ToUnixTimeMilliseconds();

        public static byte[]? GetKeyAsBytes(this Metadata metadata)
            => metadata.PartitionKeyB64Encoded ? Convert.FromBase64String(metadata.PartitionKey) : null;

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
}
