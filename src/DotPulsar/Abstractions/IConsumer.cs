using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A consumer abstraction.
    /// </summary>
    public interface IConsumer : IStateChanged<ConsumerState>, IAsyncDisposable
    {
        /// <summary>
        /// Acknowledge the consumption of a single message.
        /// </summary>
        ValueTask Acknowledge(Message message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acknowledge the consumption of a single message using the MessageId.
        /// </summary>
        ValueTask Acknowledge(MessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acknowledge the consumption of all the messages in the topic up to and including the provided message.
        /// </summary>
        ValueTask AcknowledgeCumulative(Message message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acknowledge the consumption of all the messages in the topic up to and including the provided MessageId.
        /// </summary>
        ValueTask AcknowledgeCumulative(MessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the MessageId of the last message on the topic.
        /// </summary>
        ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get an IAsyncEnumerable for consuming messages
        /// </summary>
        IAsyncEnumerable<Message> Messages(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reset the subscription associated with this consumer to a specific MessageId.
        /// </summary>
        ValueTask Seek(MessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unsubscribe the consumer.
        /// </summary>
        ValueTask Unsubscribe(CancellationToken cancellationToken = default);
    }
}
