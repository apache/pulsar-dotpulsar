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

namespace DotPulsar.Extensions
{
    using DotPulsar.Abstractions;
    using DotPulsar.Internal;
    using DotPulsar.Internal.Extensions;
    using System;
    using System.Collections.Generic;
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
            CancellationToken cancellationToken = default)
        {
            const string operation = "process";
            var operationName = $"{consumer.Topic} {operation}";

            var tags = new KeyValuePair<string, object?>[]
            {
                new KeyValuePair<string, object?>("messaging.destination", consumer.Topic),
                new KeyValuePair<string, object?>("messaging.destination_kind", "topic"),
                new KeyValuePair<string, object?>("messaging.operation", operation),
                new KeyValuePair<string, object?>("messaging.system", "pulsar"),
                new KeyValuePair<string, object?>("messaging.url", consumer.ServiceUrl),
                new KeyValuePair<string, object?>("messaging.pulsar.subscription", consumer.SubscriptionName)
            };

            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await consumer.Receive(cancellationToken).ConfigureAwait(false);

                var activity = DotPulsarActivitySource.StartConsumerActivity(message, operationName, tags);

                if (activity is not null && activity.IsAllDataRequested)
                {
                    activity.SetMessageId(message.MessageId);
                    activity.SetPayloadSize(message.Data.Length);
                    activity.SetStatusCode("OK");
                }

                try
                {
                    await processor(message, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (activity is not null && activity.IsAllDataRequested)
                        activity.AddException(exception);
                }

                activity?.Dispose();

                await consumer.Acknowledge(message, cancellationToken).ConfigureAwait(false);
            }
        }

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
}
