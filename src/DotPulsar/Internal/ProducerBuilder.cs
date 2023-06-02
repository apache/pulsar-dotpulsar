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

public sealed class ProducerBuilder<TMessage> : IProducerBuilder<TMessage>
{
    private readonly IPulsarClient _pulsarClient;
    private readonly ISchema<TMessage> _schema;
    private string? _producerName;
    private ProducerAccessMode _producerAccessMode;
    private bool _attachTraceInfoToMessages;
    private CompressionType _compressionType;
    private ulong _initialSequenceId;
    private string? _topic;
    private IHandleStateChanged<ProducerStateChanged>? _stateChangedHandler;
    private IMessageRouter? _messageRouter;
    private uint _maxPendingMessages;

    public ProducerBuilder(IPulsarClient pulsarClient, ISchema<TMessage> schema)
    {
        _pulsarClient = pulsarClient;
        _schema = schema;
        _attachTraceInfoToMessages = false;
        _compressionType = ProducerOptions<TMessage>.DefaultCompressionType;
        _initialSequenceId = ProducerOptions<TMessage>.DefaultInitialSequenceId;
        _maxPendingMessages = 500;
        _producerAccessMode = ProducerOptions<TMessage>.DefaultProducerAccessMode;
    }

    public IProducerBuilder<TMessage> AttachTraceInfoToMessages(bool attachTraceInfoToMessages)
    {
        _attachTraceInfoToMessages = attachTraceInfoToMessages;
        return this;
    }

    public IProducerBuilder<TMessage> CompressionType(CompressionType compressionType)
    {
        _compressionType = compressionType;
        return this;
    }

    public IProducerBuilder<TMessage> InitialSequenceId(ulong initialSequenceId)
    {
        _initialSequenceId = initialSequenceId;
        return this;
    }

    public IProducerBuilder<TMessage> ProducerAccessMode(ProducerAccessMode producerAccessMode)
    {
        _producerAccessMode = producerAccessMode;
        return this;
    }

    public IProducerBuilder<TMessage> ProducerName(string name)
    {
        _producerName = name;
        return this;
    }

    public IProducerBuilder<TMessage> StateChangedHandler(IHandleStateChanged<ProducerStateChanged> handler)
    {
        _stateChangedHandler = handler;
        return this;
    }

    public IProducerBuilder<TMessage> Topic(string topic)
    {
        _topic = topic;
        return this;
    }

    public IProducerBuilder<TMessage> MessageRouter(IMessageRouter messageRouter)
    {
        _messageRouter = messageRouter;
        return this;
    }

    public IProducerBuilder<TMessage> MaxPendingMessages(uint maxPendingMessages)
    {
        _maxPendingMessages = maxPendingMessages;
        return this;
    }

    public IProducer<TMessage> Create()
    {
        if (string.IsNullOrEmpty(_topic))
            throw new ConfigurationException("ProducerOptions.Topic may not be null or empty");

        if (_maxPendingMessages == 0)
            throw new ConfigurationException("ProducerOptions.MaxPendingMessages must be greater than 0");

        var options = new ProducerOptions<TMessage>(_topic!, _schema)
        {
            ProducerAccessMode = _producerAccessMode,
            AttachTraceInfoToMessages = _attachTraceInfoToMessages,
            CompressionType = _compressionType,
            InitialSequenceId = _initialSequenceId,
            ProducerName = _producerName,
            StateChangedHandler = _stateChangedHandler,
            MaxPendingMessages = _maxPendingMessages
        };

        if (_messageRouter is not null)
            options.MessageRouter = _messageRouter;

        return _pulsarClient.CreateProducer(options);
    }
}
