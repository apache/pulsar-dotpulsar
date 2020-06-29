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
    using Abstractions;
    using Internal;

    public static class PulsarClientExtensions
    {
        /// <summary>
        /// Get a builder that can be used to configure and build a Producer instance.
        /// </summary>
        public static IProducerBuilder NewProducer(this IPulsarClient pulsarClient)
            => new ProducerBuilder(pulsarClient);

        /// <summary>
        /// Get a builder that can be used to configure and build a Consumer instance.
        /// </summary>
        public static IConsumerBuilder NewConsumer(this IPulsarClient pulsarClient)
            => new ConsumerBuilder(pulsarClient);

        /// <summary>
        /// Get a builder that can be used to configure and build a Reader instance.
        /// </summary>
        public static IReaderBuilder NewReader(this IPulsarClient pulsarClient)
            => new ReaderBuilder(pulsarClient);
    }
}
