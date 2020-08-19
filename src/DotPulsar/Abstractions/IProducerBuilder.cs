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
    /// <summary>
    /// A producer building abstraction.
    /// </summary>
    public interface IProducerBuilder
    {
        /// <summary>
        /// Set the producer name. This is optional.
        /// </summary>
        IProducerBuilder ProducerName(string name);

        /// <summary>
        /// Set the initial sequence id. The default is 0.
        /// </summary>
        IProducerBuilder InitialSequenceId(ulong initialSequenceId);

        /// <summary>
        /// Set the topic for this producer. This is required.
        /// </summary>
        IProducerBuilder Topic(string topic);

        /// <summary>
        /// Set the message router for this producer. This is optional.
        /// </summary>
        IProducerBuilder MessageRouter(IMessageRouter router);

        /// <summary>
        /// If enabled, partitioned producer will automatically discover new partitions at runtime.
        /// Only for the partitioned producer implementation.
        /// Default is true.
        /// </summary>
        /// <param name="autoUpdate">Whether to auto discover the partition configuration changes</param>
        /// <returns>The producer builder instance</returns>
        IProducerBuilder AutoUpdatePartitions(bool autoUpdate);

        /// <summary>
        /// Set the interval of updating partitions. This is in second.
        /// Only for the partitioned producer implementation. 
        /// </summary>
        /// <param name="interval">The interval of updating partitions</param>
        /// <returns>The producer builder instance</returns>
        IProducerBuilder AutoUpdatePartitionsInterval(int interval);

        /// <summary>
        /// Create the producer.
        /// </summary>
        IProducer Create();
    }
}
