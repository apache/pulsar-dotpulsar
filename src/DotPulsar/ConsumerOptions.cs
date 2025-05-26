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
using System.Text.RegularExpressions;

/// <summary>
/// The consumer building options.
/// </summary>
public sealed class ConsumerOptions<TMessage>
{
    /// <summary>
    /// The default initial position.
    /// </summary>
    public static readonly SubscriptionInitialPosition DefaultInitialPosition = SubscriptionInitialPosition.Latest;

    /// <summary>
    /// The default message prefetch count.
    /// </summary>
    public static readonly uint DefaultMessagePrefetchCount = 1000;

    /// <summary>
    /// The default priority level.
    /// </summary>
    public static readonly int DefaultPriorityLevel = 0;

    /// <summary>
    /// The default of whether to read compacted.
    /// </summary>
    public static readonly bool DefaultReadCompacted = false;

    /// <summary>
    /// The default regex subscription mode.
    /// </summary>
    public static readonly RegexSubscriptionMode DefaultRegexSubscriptionMode = DotPulsar.RegexSubscriptionMode.Persistent;

    /// <summary>
    /// The default of whether to replicate the subscription's state.
    /// </summary>
    public static readonly bool DefaultReplicateSubscriptionState = false;

    /// <summary>
    /// The default subscription type.
    /// </summary>
    public static readonly SubscriptionType DefaultSubscriptionType = SubscriptionType.Exclusive;

    private readonly HashSet<string> _topics;

    private ConsumerOptions(string subscriptionName, ISchema<TMessage> schema, string topic, Regex? topicsPattern, IEnumerable<string> topics)
    {
        InitialPosition = DefaultInitialPosition;
        PriorityLevel = DefaultPriorityLevel;
        MessagePrefetchCount = DefaultMessagePrefetchCount;
        ReadCompacted = DefaultReadCompacted;
        RegexSubscriptionMode = DefaultRegexSubscriptionMode;
        ReplicateSubscriptionState = DefaultReplicateSubscriptionState;
        SubscriptionType = DefaultSubscriptionType;
        SubscriptionProperties = [];
        SubscriptionName = subscriptionName;
        Schema = schema;
        Topic = topic;
        _topics = [];
        foreach (var t in topics)
        {
            _topics.Add(t);
        }
        TopicsPattern = topicsPattern;
    }

    /// <summary>
    /// Initializes a new instance using the specified subscription name, topic and schema.
    /// </summary>
    public ConsumerOptions(string subscriptionName, string topic, ISchema<TMessage> schema)
        : this(subscriptionName, schema, topic, null, Array.Empty<string>()) { }

    /// <summary>
    /// Initializes a new instance using the specified subscription name, topics and schema.
    /// </summary>
    public ConsumerOptions(string subscriptionName, IEnumerable<string> topics, ISchema<TMessage> schema)
        : this(subscriptionName, schema, string.Empty, null, topics) { }

    /// <summary>
    /// Initializes a new instance using the specified subscription name, topics pattern and schema.
    /// </summary>
    public ConsumerOptions(string subscriptionName, Regex topicsPattern, ISchema<TMessage> schema)
        : this(subscriptionName, schema, string.Empty, topicsPattern, Array.Empty<string>()) { }

    /// <summary>
    /// Set the consumer name. This is optional.
    /// </summary>
    public string? ConsumerName { get; set; }

    /// <summary>
    /// Set initial position for the subscription. The default is 'Latest'.
    /// </summary>
    public SubscriptionInitialPosition InitialPosition { get; set; }

    /// <summary>
    /// Number of messages that will be prefetched. The default is 1000.
    /// </summary>
    public uint MessagePrefetchCount { get; set; }

    /// <summary>
    /// Set the priority level for the shared subscription consumer. The default is 0.
    /// </summary>
    public int PriorityLevel { get; set; }

    /// <summary>
    /// Whether to read from the compacted topic. The default is 'false'.
    /// </summary>
    public bool ReadCompacted { get; set; }

    /// <summary>
    /// Determines which topics this consumer should be subscribed to - Persistent, Non-Persistent, or both. The default is 'Persistent'.
    /// </summary>
    public RegexSubscriptionMode RegexSubscriptionMode { get; set; }

    /// <summary>
    /// Whether to replicate the subscription's state across clusters (when using geo-replication). The default is 'false'.
    /// </summary>
    public bool ReplicateSubscriptionState { get; set; }

    /// <summary>
    /// Set the schema. This is required.
    /// </summary>
    public ISchema<TMessage> Schema { get; set; }

    /// <summary>
    /// Register a state changed handler. This is optional.
    /// </summary>
    public IHandleStateChanged<ConsumerStateChanged>? StateChangedHandler { get; set; }

    /// <summary>
    /// Set the subscription name for this consumer. This is required.
    /// </summary>
    public string SubscriptionName { get; set; }

    /// <summary>
    /// Add/Set the subscription's properties. This is optional.
    /// </summary>
    public Dictionary<string, string> SubscriptionProperties { get; set; }

    /// <summary>
    /// Set the subscription type for this consumer. The default is 'Exclusive'.
    /// </summary>
    public SubscriptionType SubscriptionType { get; set; }

    /// <summary>
    /// Set the topic for this consumer. This, or setting multiple topics or a topic pattern, is required.
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// Set the topics for this consumer. This, or setting a single topic or a topic pattern, is required.
    /// </summary>
    public IEnumerable<string> Topics
    {
        get => _topics;
        set
        {
            _topics.Clear();

            foreach (var topic in value)
            {
                _topics.Add(topic);
            }
        }
    }

    /// <summary>
    /// Specify a pattern for topics that this consumer subscribes to. This, or setting a single topic or multiple topics, is required.
    /// </summary>
    public Regex? TopicsPattern { get; set; }

    /// <summary>
    /// Allow out-of-order delivery on key_shared subscriptions. The default is 'false'.
    /// </summary>
    /// <remarks>
    /// https://pulsar.apache.org/docs/3.3.x/concepts-messaging/#preserving-order-of-processing
    /// </remarks>
    public bool AllowOutOfOrderDeliver { get; set; }
}
