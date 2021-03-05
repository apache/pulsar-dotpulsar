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
    using DotPulsar.Schemas;
    using Internal;
    using System.Buffers;

    /// <summary>
    /// Extensions for IPulsarClient.
    /// </summary>
    public static class PulsarClientExtensions
    {
        /// <summary>
        /// Get a builder that can be used to configure and build a Producer instance.
        /// </summary>
        public static IProducerBuilder<ReadOnlySequence<byte>> NewProducer(this IPulsarClient pulsarClient)
            => new ProducerBuilder<ReadOnlySequence<byte>>(pulsarClient, new ByteSequenceSchema());

        /// <summary>
        /// Get a builder that can be used to configure and build a Consumer instance.
        /// </summary>
        public static IConsumerBuilder<ReadOnlySequence<byte>> NewConsumer(this IPulsarClient pulsarClient)
            => new ConsumerBuilder<ReadOnlySequence<byte>>(pulsarClient, new ByteSequenceSchema());

        /// <summary>
        /// Get a builder that can be used to configure and build a Reader instance.
        /// </summary>
        public static IReaderBuilder<ReadOnlySequence<byte>> NewReader(this IPulsarClient pulsarClient)
            => new ReaderBuilder<ReadOnlySequence<byte>>(pulsarClient, new ByteSequenceSchema());

        /// <summary>
        /// Get a builder that can be used to configure and build a Producer instance.
        /// </summary>
        public static IProducerBuilder<TMessage> NewProducer<TMessage>(this IPulsarClient pulsarClient, ISchema<TMessage> schema)
            => new ProducerBuilder<TMessage>(pulsarClient, schema);

        /// <summary>
        /// Get a builder that can be used to configure and build a Consumer instance.
        /// </summary>
        public static IConsumerBuilder<TMessage> NewConsumer<TMessage>(this IPulsarClient pulsarClient, ISchema<TMessage> schema)
            => new ConsumerBuilder<TMessage>(pulsarClient, schema);

        /// <summary>
        /// Get a builder that can be used to configure and build a Reader instance.
        /// </summary>
        public static IReaderBuilder<TMessage> NewReader<TMessage>(this IPulsarClient pulsarClient, ISchema<TMessage> schema)
            => new ReaderBuilder<TMessage>(pulsarClient, schema);
    }
}
