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
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class ProducerBuilderExtensions
    {
        /// <summary>
        /// Register a state changed handler.
        /// </summary>
        public static IProducerBuilder<TMessage> StateChangedHandler<TMessage>(
            this IProducerBuilder<TMessage> builder,
            Action<ProducerStateChanged, CancellationToken> handler,
            CancellationToken cancellationToken = default)
        {
            builder.StateChangedHandler(new ActionStateChangedHandler<ProducerStateChanged>(handler, cancellationToken));
            return builder;
        }

        /// <summary>
        /// Register a state changed handler.
        /// </summary>
        public static IProducerBuilder<TMessage> StateChangedHandler<TMessage>(
            this IProducerBuilder<TMessage> builder,
            Func<ProducerStateChanged, CancellationToken, ValueTask> handler,
            CancellationToken cancellationToken = default)
        {
            builder.StateChangedHandler(new FuncStateChangedHandler<ProducerStateChanged>(handler, cancellationToken));
            return builder;
        }
    }
}
