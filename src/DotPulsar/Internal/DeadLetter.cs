namespace DotPulsar.Internal;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;


public class DeadLetterPolicy
{
    public uint MaxRedeliveryCount { get; internal set; }
    public string DeadLetterTopic { get; internal set; } = default!;
    public string RetryLetterTopic { get; internal set; } = default!;

    public DeadLetterPolicy(uint maxRedeliveryCount, string deadLetterTopic, string retryLetterTopic)
    {
        if (string.IsNullOrWhiteSpace(deadLetterTopic))
        {
            throw new ArgumentException($"'{nameof(deadLetterTopic)}' cannot be null or whitespace.", nameof(deadLetterTopic));
        }

        if (string.IsNullOrWhiteSpace(retryLetterTopic))
        {
            throw new ArgumentException($"'{nameof(retryLetterTopic)}' cannot be null or whitespace.", nameof(retryLetterTopic));
        }

        MaxRedeliveryCount = maxRedeliveryCount;
        DeadLetterTopic = deadLetterTopic;
        RetryLetterTopic = retryLetterTopic;
    }
}

public interface IDeadLetterPolicyBuilder<TMessage>
{
    public IDeadLetterPolicyBuilder<TMessage> MaxRedeliveryCount(uint maxRedeliveryCount);
    public IDeadLetterPolicyBuilder<TMessage> DeadLetterTopic(string deadLetterTopic);
    public IDeadLetterPolicyBuilder<TMessage> RetryLetterTopic(string retryLetterTopic);
    public IDeadLetterPolicyBuilder<TMessage> Producer(Action<IProducerBuilder<TMessage>> producerConfig);
}

public class DeadLetterPolicyBuilder<TMessage> : IDeadLetterPolicyBuilder<TMessage>
{
    private uint _maxRedeliveryCount;
    private string? _deadLetterTopic;
    private string? _retryLetterTopic;

    private Action<IProducerBuilder<TMessage>> _producerConfig;

    private readonly IPulsarClient _client;
    private readonly ISchema<TMessage> _schema;

    public DeadLetterPolicyBuilder(IPulsarClient client, ISchema<TMessage> schema)
    {
        _client = client;
        _schema = schema;
    }

    public IDeadLetterPolicyBuilder<TMessage> DeadLetterTopic(string deadLetterTopic)
    {
        _deadLetterTopic = deadLetterTopic;
        return this;
    }

    public IDeadLetterPolicyBuilder<TMessage> Producer(Action<IProducerBuilder<TMessage>> producerConfig)
    {
        _producerConfig = producerConfig;
        return this;
    }

    public IDeadLetterPolicyBuilder<TMessage> MaxRedeliveryCount(uint maxRedeliveryCount)
    {
        _maxRedeliveryCount = maxRedeliveryCount;
        return this;
    }

    public IDeadLetterPolicyBuilder<TMessage> RetryLetterTopic(string retryLetterTopic)
    {
        _retryLetterTopic = retryLetterTopic;
        return this;
    }

    internal (DeadLetterPolicy policy, IProducerBuilder<TMessage> producer) Create()
    {
        var policy = new DeadLetterPolicy(_maxRedeliveryCount, _deadLetterTopic, _retryLetterTopic);
        var producerBuilder = _client.NewProducer<TMessage>(_schema);
        _producerConfig(producerBuilder);
        return (policy, producerBuilder);
    }
}


public interface IDeadLetterProcessor<TMessage>
{
    public uint MaxRedeliveryCount { get; }

    public void ClearMessages();
    public void AddMessage(IMessage<TMessage> message);
    public void RemoveMessage(IMessage<TMessage> message);

    public ValueTask<bool> ProcessMessage(IMessage<TMessage> message);

    public IMessage<TMessage> ReconsumeLater(TimeSpan timeSpan);
}

public class DeadLetterProcessor<T> : IDeadLetterProcessor<T>
{
    private readonly DeadLetterPolicy _policy;

    private readonly Lazy<IProducer<T>> _deadLetterProducer;
    private readonly Lazy<IProducer<T>> _retryProducer;

    private readonly ConcurrentDictionary<MessageId, IMessage<T>> _store;

    public uint MaxRedeliveryCount => _policy.MaxRedeliveryCount;

    public DeadLetterProcessor(DeadLetterPolicy policy, IProducerBuilder<T> producerBuilder)
    {
        if (producerBuilder is null)
        {
            throw new ArgumentNullException(nameof(producerBuilder));
        }

        _policy = policy ?? throw new ArgumentNullException(nameof(policy));

        _store = new ConcurrentDictionary<MessageId, IMessage<T>>();
        _deadLetterProducer = new Lazy<IProducer<T>>(() => producerBuilder.Topic(policy.DeadLetterTopic).Create());
        _retryProducer = new Lazy<IProducer<T>>(() => producerBuilder.Topic(policy.RetryLetterTopic).Create());
    }

    public void AddMessage(IMessage<T> message)
    {
        _store.AddOrUpdate(message.MessageId, message, (id, old) => message);
    }

    public void ClearMessages() => _store.Clear();

    public async ValueTask<bool> ProcessMessage(IMessage<T> message)
    {
        if (message.RedeliveryCount > MaxRedeliveryCount)
        {
            var msgbuilder = new MessageBuilder<T>(_deadLetterProducer.Value);

            msgbuilder.EventTime(message.EventTime)
                .KeyBytes(message.KeyBytes)
                .OrderingKey(message.OrderingKey)
                .SchemaVersion(message.SchemaVersion)
                .SequenceId(message.SequenceId)
                ;

            _ = await _deadLetterProducer.Value.Send(message.Data, msgbuilder._metadata);

            return true;
        }

        return false;
    }

    public IMessage<T> ReconsumeLater(TimeSpan timeSpan)
    {
        throw new NotImplementedException();
    }

    public void RemoveMessage(IMessage<T> message)
    {
        _store.TryRemove(message.MessageId, out _);
    }
}
