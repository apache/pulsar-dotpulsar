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
/// The reader building options.
/// </summary>
public sealed class ReaderOptions<TMessage>
{
    /// <summary>
    /// The default message prefetch count.
    /// </summary>
    public static readonly uint DefaultMessagePrefetchCount = 1000;

    /// <summary>
    /// The default of whether to read compacted.
    /// </summary>
    public static readonly bool DefaultReadCompacted = false;

    /// <summary>
    /// Initializes a new instance using the specified startMessageId and topic.
    /// </summary>
    public ReaderOptions(MessageId startMessageId, string topic, ISchema<TMessage> schema)
    {
        MessagePrefetchCount = DefaultMessagePrefetchCount;
        ReadCompacted = DefaultReadCompacted;
        StartMessageId = startMessageId;
        Topic = topic;
        Schema = schema;
    }

    /// <summary>
    /// Number of messages that will be prefetched. The default is 1000.
    /// </summary>
    public uint MessagePrefetchCount { get; set; }

    /// <summary>
    /// Whether to read from the compacted topic. The default is 'false'.
    /// </summary>
    public bool ReadCompacted { get; set; }

    /// <summary>
    /// Set the reader name. This is optional.
    /// </summary>
    public string? ReaderName { get; set; }

    /// <summary>
    /// Set the schema. This is required.
    /// </summary>
    public ISchema<TMessage> Schema { get; set; }

    /// <summary>
    /// The initial reader position is set to the specified message id. This is required.
    /// </summary>
    public MessageId StartMessageId { get; set; }

    /// <summary>
    /// Register a state changed handler. This is optional.
    /// </summary>
    public IHandleStateChanged<ReaderStateChanged>? StateChangedHandler { get; set; }

    /// <summary>
    /// Set the topic for this reader. This is required.
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// The prefix for the subscription being created behind the scenes for the reader. This is optional
    /// It can be necessary to set this if the policy for access is set to SubscriptionPrefix.
    /// </summary>
    public string SubscriptionNamePrefix { get; set; } = string.Empty;
}
