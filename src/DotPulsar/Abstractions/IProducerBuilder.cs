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
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A producer building abstraction.
    /// </summary>
    public interface IProducerBuilder
    {
        /// <summary>
        /// Set the compression type. The default is 'None'.
        /// </summary>
        IProducerBuilder CompressionType(CompressionType compressionType);

        /// <summary>
        /// Set the initial sequence id. The default is 0.
        /// </summary>
        IProducerBuilder InitialSequenceId(ulong initialSequenceId);

        /// <summary>
        /// Set the producer name. This is optional.
        /// </summary>
        IProducerBuilder ProducerName(string name);

        /// <summary>
        /// Register a state changed handler.
        /// </summary>
        IProducerBuilder StateChangedHandler(IHandleStateChanged<ProducerStateChanged> handler);

        /// <summary>
        /// Register a state changed handler.
        /// </summary>
        IProducerBuilder StateChangedHandler(Action<ProducerStateChanged, CancellationToken> handler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Register a state changed handler.
        /// </summary>
        IProducerBuilder StateChangedHandler(Func<ProducerStateChanged, CancellationToken, ValueTask> handler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set the topic for this producer. This is required.
        /// </summary>
        IProducerBuilder Topic(string topic);

        /// <summary>
        /// Create the producer.
        /// </summary>
        IProducer Create();
    }
}
