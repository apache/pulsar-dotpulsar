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

namespace DotPulsar.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A consumer abstraction.
    /// </summary>
    public interface IConsumer : IAsyncDisposable
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
        /// Ask whether the current state is final, meaning that it will never change.
        /// </summary>
        /// <returns>
        /// True if it's final and False if it's not.
        /// </returns>
        bool IsFinalState();

        /// <summary>
        /// Ask whether the provided state is final, meaning that it will never change.
        /// </summary>
        /// <returns>
        /// True if it's final and False if it's not.
        /// </returns>
        bool IsFinalState(ConsumerState state);

        /// <summary>
        /// Get an IAsyncEnumerable for consuming messages
        /// </summary>
        IAsyncEnumerable<Message> Messages(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reset the subscription associated with this consumer to a specific MessageId.
        /// </summary>
        ValueTask Seek(MessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Wait for the state to change to a specific state.
        /// </summary>
        /// <returns>
        /// The current state.
        /// </returns>
        /// <remarks>
        /// If the state change to a final state, then all awaiting tasks will complete.
        /// </remarks>
        ValueTask<ConsumerStateChanged> StateChangedTo(ConsumerState state, CancellationToken cancellationToken = default);

        /// <summary>
        /// Wait for the state to change from a specific state.
        /// </summary>
        /// <returns>
        /// The current state.
        /// </returns>
        /// <remarks>
        /// If the state change to a final state, then all awaiting tasks will complete.
        /// </remarks>
        ValueTask<ConsumerStateChanged> StateChangedFrom(ConsumerState state, CancellationToken cancellationToken = default);

        /// <summary>
        /// The topic of the consumer.
        /// </summary>
        string Topic { get; }

        /// <summary>
        /// Unsubscribe the consumer.
        /// </summary>
        ValueTask Unsubscribe(CancellationToken cancellationToken = default);
    }
}
