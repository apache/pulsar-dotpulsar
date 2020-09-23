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
        /// Set the maximum number of messages permitted in a batch. The default is 1000.
        /// </summary>
        IProducerBuilder BatchingMaxMessagesPerBatch(int maxMessagesPerBatch);

        /// <summary>
        /// Set the time period within which the messages sent will be batched. The default is 1 ms.
        /// </summary>
        IProducerBuilder BatchingMaxPublishDelay(TimeSpan maxPublishDelay);

        /// <summary>
        /// Control whether automatic batching of messages is enabled for the producer. The default is 'false'.
        /// </summary>
        IProducerBuilder BatchingEnabled(bool batchingEnabled);

        /// <summary>
        /// Set the maximum number of bytes permitted in a batch. The default is 128KB.
        /// </summary>
        IProducerBuilder BatchingMaxBytes(int batchingMaxBytes);

        /// <summary>
        /// Create the producer.
        /// </summary>
        IProducer Create();
    }
}
