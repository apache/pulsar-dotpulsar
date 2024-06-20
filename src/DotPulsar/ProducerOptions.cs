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

namespace DotPulsar;

using DotPulsar.Abstractions;

/// <summary>
/// The producer building options.
/// </summary>
public sealed class ProducerOptions<TMessage>
{
    /// <summary>
    /// The default compression type.
    /// </summary>
    public static readonly CompressionType DefaultCompressionType = CompressionType.None;

    /// <summary>
    /// The default initial sequence id.
    /// </summary>
    public static readonly ulong DefaultInitialSequenceId = 0;

    /// <summary>
    /// The default producer access mode.
    /// </summary>
    public static readonly ProducerAccessMode DefaultProducerAccessMode = ProducerAccessMode.Shared;

    /// <summary>
    /// The default encryption keys.
    /// </summary>
    public static readonly List<string> DefaultEncryptionKeys = new();

    /// <summary>
    /// The default crypto key reader.
    /// </summary>
    public static readonly ICryptoKeyReader DefaultCryptoKeyReader = null;

    /// <summary>
    /// The default action to take when message encryption fails.
    /// </summary>
    public static readonly ProducerCryptoFailureAction DefaultCryptoFailureAction = ProducerCryptoFailureAction.Fail;

    /// <summary>
    /// Initializes a new instance using the specified topic.
    /// </summary>
    public ProducerOptions(string topic, ISchema<TMessage> schema)
    {
        AttachTraceInfoToMessages = false;
        CompressionType = DefaultCompressionType;
        InitialSequenceId = DefaultInitialSequenceId;
        ProducerAccessMode = DefaultProducerAccessMode;
        Topic = topic;
        Schema = schema;
        MessageRouter = new RoundRobinPartitionRouter();
        EncryptionKeys = DefaultEncryptionKeys;
        CryptoKeyReader = DefaultCryptoKeyReader;
        CryptoFailureAction = DefaultCryptoFailureAction;
    }

    /// <summary>
    /// Whether to attach the sending trace's parent and state to the outgoing messages metadata. The default is 'false'.
    /// </summary>
    public bool AttachTraceInfoToMessages { get; set; }

    /// <summary>
    /// Set the crypto key reader.
    /// </summary>
    public ICryptoKeyReader? CryptoKeyReader { get; set; }

    /// <summary>
    /// Set the encryption keys.
    /// </summary>
    public List<string> EncryptionKeys { get; set; }

    /// <summary>
    /// Set the action to take when a crypto operation fails. The default is 'Fail'.
    /// </summary>
    public ProducerCryptoFailureAction CryptoFailureAction { get; set; }

    /// <summary>
    /// Set the compression type. The default is 'None'.
    /// </summary>
    public CompressionType CompressionType { get; set; }

    /// <summary>
    /// Set the initial sequence id. The default is 0.
    /// </summary>
    public ulong InitialSequenceId { get; set; }

    /// <summary>
    /// Set the producer access mode. The default is 'Shared'
    /// </summary>
    public ProducerAccessMode ProducerAccessMode { get; set; }

    /// <summary>
    /// Set the producer name. This is optional.
    /// </summary>
    public string? ProducerName { get; set; }

    /// <summary>
    /// Set the schema. This is required.
    /// </summary>
    public ISchema<TMessage> Schema { get; set; }

    /// <summary>
    /// Register a state changed handler. This is optional.
    /// </summary>
    public IHandleStateChanged<ProducerStateChanged>? StateChangedHandler { get; set; }

    /// <summary>
    /// Set the topic for this producer. This is required.
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// Set the message router. The default router is the Round Robin partition router.
    /// </summary>
    public IMessageRouter MessageRouter { get; set; }

    /// <summary>
    /// Set the max size of the queue holding the messages pending to receive an acknowledgment from the broker.
    /// </summary>
    public uint MaxPendingMessages { get; set; }
}
