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

namespace DotPulsar.Abstractions;

/// <summary>
/// A pulsar client abstraction.
/// </summary>
public interface IPulsarClient : IAsyncDisposable
{
    /// <summary>
    /// Create a producer.
    /// </summary>
    IProducer<TMessage> CreateProducer<TMessage>(ProducerOptions<TMessage> options);

    /// <summary>
    /// Create a consumer.
    /// </summary>
    IConsumer<TMessage> CreateConsumer<TMessage>(ConsumerOptions<TMessage> options);

    /// <summary>
    /// Create a reader.
    /// </summary>
    IReader<TMessage> CreateReader<TMessage>(ReaderOptions<TMessage> options);

    /// <summary>
    /// The client's service url.
    /// </summary>
    public Uri ServiceUrl { get; }
}
