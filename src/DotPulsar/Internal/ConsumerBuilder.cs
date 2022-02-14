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

using System;

public sealed class ConsumerBuilder<TMessage> : IConsumerBuilder<TMessage>
{
    private readonly IPulsarClient _pulsarClient;
    private readonly ISchema<TMessage> _schema;
    private string? _consumerName;
    private SubscriptionInitialPosition _initialPosition;
    private int _priorityLevel;
    private uint _messagePrefetchCount;
    private bool _readCompacted;
    private string? _subscriptionName;
    private SubscriptionType _subscriptionType;
    private string? _topic;
    private IHandleStateChanged<ConsumerStateChanged>? _stateChangedHandler;
    private Action<IDeadLetterPolicyBuilder<TMessage>>? _deadLetterConfig;

    public ConsumerBuilder(IPulsarClient pulsarClient, ISchema<TMessage> schema)
    {
        _schema = schema;
        _pulsarClient = pulsarClient;
        _initialPosition = ConsumerOptions<TMessage>.DefaultInitialPosition;
        _priorityLevel = ConsumerOptions<TMessage>.DefaultPriorityLevel;
        _messagePrefetchCount = ConsumerOptions<TMessage>.DefaultMessagePrefetchCount;
        _readCompacted = ConsumerOptions<TMessage>.DefaultReadCompacted;
        _subscriptionType = ConsumerOptions<TMessage>.DefaultSubscriptionType;
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

    public IConsumerBuilder<TMessage> DeadLetterPolicy(Action<IDeadLetterPolicyBuilder<TMessage>> action)
    {
        _deadLetterConfig = action;
        return this;
    }

    public IConsumer<TMessage> Create()
    {
        if (string.IsNullOrEmpty(_subscriptionName))
            throw new ConfigurationException("SubscriptionName may not be null or empty");

        if (string.IsNullOrEmpty(_topic))
            throw new ConfigurationException("Topic may not be null or empty");

        var options = new ConsumerOptions<TMessage>(_subscriptionName!, _topic!, _schema)
        {
            ConsumerName = _consumerName,
            InitialPosition = _initialPosition,
            MessagePrefetchCount = _messagePrefetchCount,
            PriorityLevel = _priorityLevel,
            ReadCompacted = _readCompacted,
            StateChangedHandler = _stateChangedHandler,
            SubscriptionType = _subscriptionType
        };

        if (_deadLetterConfig is not null)
        {
            var config = new DeadLetterPolicyBuilder<TMessage>(_pulsarClient, _schema);
            _deadLetterConfig(config);
            (var policy, var dlProducerBuilder) = config.Create();
            options.DeadLetterPolicy = policy;
            options.DeadLetterProcessor = new DeadLetterProcessor<TMessage>(policy, dlProducerBuilder);
        }

        return _pulsarClient.CreateConsumer(options);
    }
}
