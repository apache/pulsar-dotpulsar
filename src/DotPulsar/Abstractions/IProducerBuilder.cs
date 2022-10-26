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
/// A producer building abstraction.
/// </summary>
public interface IProducerBuilder<TMessage>
{
    /// <summary>
    /// Whether to attach the sending trace's parent and state to the outgoing messages metadata. The default is 'false'.
    /// </summary>
    IProducerBuilder<TMessage> AttachTraceInfoToMessages(bool attachTraceInfoToMessages);

    /// <summary>
    /// Set the compression type. The default is 'None'.
    /// </summary>
    IProducerBuilder<TMessage> CompressionType(CompressionType compressionType);

    /// <summary>
    /// Set the initial sequence id. The default is 0.
    /// </summary>
    IProducerBuilder<TMessage> InitialSequenceId(ulong initialSequenceId);

    /// <summary>
    /// Set the producer name. This is optional.
    /// </summary>
    IProducerBuilder<TMessage> ProducerName(string name);

    /// <summary>
    /// Register a state changed handler.
    /// </summary>
    IProducerBuilder<TMessage> StateChangedHandler(IHandleStateChanged<ProducerStateChanged> handler);

    /// <summary>
    /// Set the topic for this producer. This is required.
    /// </summary>
    IProducerBuilder<TMessage> Topic(string topic);

    /// <summary>
    /// Set the message router for this producer. The default is RoundRobinPartitionRouter.
    /// </summary>
    IProducerBuilder<TMessage> MessageRouter(IMessageRouter messageRouter);

    /// <summary>
    /// Create the producer.
    /// </summary>
    IProducer<TMessage> Create();
}
