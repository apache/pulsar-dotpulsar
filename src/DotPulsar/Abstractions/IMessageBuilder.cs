using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A message building abstraction.
    /// </summary>
    public interface IMessageBuilder
    {
        /// <summary>
        /// Absolute timestamp indicating when the message should be delivered to consumers.
        /// </summary>
        IMessageBuilder DeliverAt(long timestamp);

        /// <summary>
        /// Set the event time of the message.
        /// </summary>
        IMessageBuilder EventTime(ulong eventTime);

        /// <summary>
        /// Set the key of the message for routing policy.
        /// </summary>
        IMessageBuilder Key(string key);

        /// <summary>
        /// Set the key of the message for routing policy.
        /// </summary>
        IMessageBuilder KeyBytes(byte[] key);

        /// <summary>
        /// Set the ordering key of the message for message dispatch in SubscriptionType.KeyShared mode.
        /// The partition key will be used if the ordering key is not specified.
        /// </summary>
        IMessageBuilder OrderingKey(byte[] key);

        /// <summary>
        /// Add/Set a property key/value on the message.
        /// </summary>
        IMessageBuilder Property(string key, string value);

        /// <summary>
        /// Set the sequence id of the message.
        /// </summary>
        IMessageBuilder SequenceId(ulong sequenceId);

        /// <summary>
        /// Set the consumer name.
        /// </summary>
        ValueTask<MessageId> Send(ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default);
    }
}
