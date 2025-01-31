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

namespace DotPulsar.Internal;

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using System.Text.RegularExpressions;

public sealed class ConsumerBuilder<TMessage> : IConsumerBuilder<TMessage>
{
    private readonly IPulsarClient _pulsarClient;
    private readonly ISchema<TMessage> _schema;
    private string? _consumerName;
    private SubscriptionInitialPosition _initialPosition;
    private int _priorityLevel;
    private uint _messagePrefetchCount;
    private bool _readCompacted;
    private RegexSubscriptionMode _regexSubscriptionMode;
    private bool _replicateSubscriptionState;
    private string? _subscriptionName;
    private readonly Dictionary<string, string> _subscriptionProperties;
    private SubscriptionType _subscriptionType;
    private string _topic;
    private readonly HashSet<string> _topics;
    private Regex? _topicsPattern;
    private IHandleStateChanged<ConsumerStateChanged>? _stateChangedHandler;

    public ConsumerBuilder(IPulsarClient pulsarClient, ISchema<TMessage> schema)
    {
        _schema = schema;
        _pulsarClient = pulsarClient;
        _initialPosition = ConsumerOptions<TMessage>.DefaultInitialPosition;
        _priorityLevel = ConsumerOptions<TMessage>.DefaultPriorityLevel;
        _messagePrefetchCount = ConsumerOptions<TMessage>.DefaultMessagePrefetchCount;
        _readCompacted = ConsumerOptions<TMessage>.DefaultReadCompacted;
        _regexSubscriptionMode = ConsumerOptions<TMessage>.DefaultRegexSubscriptionMode;
        _replicateSubscriptionState = ConsumerOptions<TMessage>.DefaultReplicateSubscriptionState;
        _subscriptionProperties = [];
        _subscriptionType = ConsumerOptions<TMessage>.DefaultSubscriptionType;
        _topic = string.Empty;
        _topics = [];
    }

    public IConsumerBuilder<TMessage> ConsumerName(string name)
    {
        _consumerName = name;
        return this;
    }

    public IConsumerBuilder<TMessage> InitialPosition(SubscriptionInitialPosition initialPosition)
    {
        _initialPosition = initialPosition;
        return this;
    }

    public IConsumerBuilder<TMessage> MessagePrefetchCount(uint count)
    {
        _messagePrefetchCount = count;
        return this;
    }

    public IConsumerBuilder<TMessage> PriorityLevel(int priorityLevel)
    {
        _priorityLevel = priorityLevel;
        return this;
    }

    public IConsumerBuilder<TMessage> ReadCompacted(bool readCompacted)
    {
        _readCompacted = readCompacted;
        return this;
    }

    public IConsumerBuilder<TMessage> RegexSubscriptionMode(RegexSubscriptionMode regexSubscriptionMode)
    {
        _regexSubscriptionMode = regexSubscriptionMode;
        return this;
    }

    public IConsumerBuilder<TMessage> ReplicateSubscriptionState(bool replicateSubscriptionState)
    {
        _replicateSubscriptionState = replicateSubscriptionState;
        return this;
    }

    public IConsumerBuilder<TMessage> StateChangedHandler(IHandleStateChanged<ConsumerStateChanged> handler)
    {
        _stateChangedHandler = handler;
        return this;
    }

    public IConsumerBuilder<TMessage> SubscriptionName(string name)
    {
        _subscriptionName = name;
        return this;
    }

    public IConsumerBuilder<TMessage> SubscriptionProperty(string key, string value)
    {
        _subscriptionProperties[key] = value;
        return this;
    }

    public IConsumerBuilder<TMessage> SubscriptionType(SubscriptionType type)
    {
        _subscriptionType = type;
        return this;
    }

    public IConsumerBuilder<TMessage> Topic(string topic)
    {
        _topic = topic;
        return this;
    }

    public IConsumerBuilder<TMessage> Topics(IEnumerable<string> topics)
    {
        _topics.Clear();

        foreach (var topic in topics)
        {
            _topics.Add(topic);
        }

        return this;
    }

    public IConsumerBuilder<TMessage> TopicsPattern(Regex topicsPattern)
    {
        _topicsPattern = topicsPattern;
        return this;
    }

    public IConsumer<TMessage> Create()
    {
        if (string.IsNullOrEmpty(_subscriptionName))
            throw new ConfigurationException("SubscriptionName may not be null or empty");

        if (string.IsNullOrEmpty(_topic) && _topics.Count == 0 && _topicsPattern is null)
            throw new ConfigurationException("A 'Topic', multiple 'Topics', or a 'TopicsPattern' must be set");

        var options = new ConsumerOptions<TMessage>(_subscriptionName!, _topic, _schema)
        {
            ConsumerName = _consumerName,
            InitialPosition = _initialPosition,
            MessagePrefetchCount = _messagePrefetchCount,
            PriorityLevel = _priorityLevel,
            ReadCompacted = _readCompacted,
            RegexSubscriptionMode = _regexSubscriptionMode,
            ReplicateSubscriptionState = _replicateSubscriptionState,
            StateChangedHandler = _stateChangedHandler,
            SubscriptionProperties = _subscriptionProperties,
            SubscriptionType = _subscriptionType,
            Topics = _topics,
            TopicsPattern = _topicsPattern
        };

        return _pulsarClient.CreateConsumer(options);
    }
}
