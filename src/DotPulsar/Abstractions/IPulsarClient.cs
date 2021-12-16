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
    /// A pulsar client abstraction.
    /// </summary>
    public interface IPulsarClient : IAsyncDisposable
    {
        /// <summary>
        /// Create a producer.
        /// </summary>
        IProducer CreateProducer(ProducerOptions options);

        /// <summary>
        /// Create a consumer.
        /// </summary>
        IConsumer CreateConsumer(ConsumerOptions options);

        /// <summary>
        /// Create parallel workers consumer.
        /// </summary>
        IParallelConsumer CreateParallelConsumer(ParallelConsumerOptions options);

        /// <summary>
        /// Create a reader.
        /// </summary>
        IReader CreateReader(ReaderOptions options);

        /// <summary>
        /// Get the partition topic metadata for a given topic.
        /// </summary>
        ValueTask<PartitionedTopicMetadata> GetPartitionTopicMetadata(string topic, CancellationToken cancellationToken = default);
    }
}
