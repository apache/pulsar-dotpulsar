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

namespace DotPulsar.Extensions;

using DotPulsar.Abstractions;
using DotPulsar.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Extensions for IConsumer.
/// </summary>
public static class ConsumerExtensions
{
    /// <summary>
    /// Acknowledge the consumption of a single message.
    /// </summary>
    public static async ValueTask Acknowledge(this IConsumer consumer, IMessage message, CancellationToken cancellationToken = default)
        => await consumer.Acknowledge(message.MessageId, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Acknowledge the consumption of all the messages in the topic up to and including the provided message.
    /// </summary>
    public static async ValueTask AcknowledgeCumulative(this IConsumer consumer, IMessage message, CancellationToken cancellationToken = default)
        => await consumer.AcknowledgeCumulative(message.MessageId, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Process and auto-acknowledge a message.
    /// </summary>
    public static async ValueTask Process<TMessage>(
        this IConsumer<TMessage> consumer,
        Func<IMessage<TMessage>, CancellationToken, ValueTask> processor,
        ProcessingOptions options,
        CancellationToken cancellationToken = default)
    {
        using var messageProcessor = new MessageProcessor<TMessage>(consumer, processor, options);
        await messageProcessor.Process(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Process and auto-acknowledge a message using the default processing options.
    /// </summary>
    public static async ValueTask Process<TMessage>(
        this IConsumer<TMessage> consumer,
        Func<IMessage<TMessage>, CancellationToken, ValueTask> processor,
        CancellationToken cancellationToken = default)
        => await Process(consumer, processor, new ProcessingOptions(), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Wait for the state to change to a specific state.
    /// </summary>
    /// <returns>
    /// The current state.
    /// </returns>
    /// <remarks>
    /// If the state change to a final state, then all awaiting tasks will complete.
    /// </remarks>
    public static async ValueTask<ConsumerStateChanged> StateChangedTo(this IConsumer consumer, ConsumerState state, CancellationToken cancellationToken = default)
    {
        var newState = await consumer.OnStateChangeTo(state, cancellationToken).ConfigureAwait(false);
        return new ConsumerStateChanged(consumer, newState);
    }

    /// <summary>
    /// Wait for the state to change from a specific state.
    /// </summary>
    /// <returns>
    /// The current state.
    /// </returns>
    /// <remarks>
    /// If the state change to a final state, then all awaiting tasks will complete.
    /// </remarks>
    public static async ValueTask<ConsumerStateChanged> StateChangedFrom(this IConsumer consumer, ConsumerState state, CancellationToken cancellationToken = default)
    {
        var newState = await consumer.OnStateChangeFrom(state, cancellationToken).ConfigureAwait(false);
        return new ConsumerStateChanged(consumer, newState);
    }
}
