using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A message building abstraction
    /// </summary>
    public interface IMessageBuilder
    {
        /// <summary>
        /// Set the event time of the message
        /// </summary>
        IMessageBuilder EventTime(ulong eventTime);

        /// <summary>
        /// Add/Set a property key/value on the message
        /// </summary>
        IMessageBuilder Property(string key, string value);

        /// <summary>
        /// Set the sequence id of the message
        /// </summary>
        IMessageBuilder SequenceId(ulong sequenceId);

        /// <summary>
        /// Set the consumer name
        /// </summary>
        Task<MessageId> Send(ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default);
    }
}
