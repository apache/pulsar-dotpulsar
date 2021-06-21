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

namespace DotPulsar.Internal
{
    using DotPulsar.Abstractions;
    using DotPulsar.Exceptions;

    public sealed class ProducerBuilder<TMessage> : IProducerBuilder<TMessage>
    {
        private readonly IPulsarClient _pulsarClient;
        private readonly ISchema<TMessage> _schema;
        private string? _producerName;
        private CompressionType _compressionType;
        private ulong _initialSequenceId;
        private string? _topic;
        private IHandleStateChanged<ProducerStateChanged>? _stateChangedHandler;
        private IMessageRouter? _messageRouter;

        public ProducerBuilder(IPulsarClient pulsarClient, ISchema<TMessage> schema)
        {
            _pulsarClient = pulsarClient;
            _schema = schema;
            _compressionType = ProducerOptions<TMessage>.DefaultCompressionType;
            _initialSequenceId = ProducerOptions<TMessage>.DefaultInitialSequenceId;
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

        public IProducer<TMessage> Create()
        {
            if (string.IsNullOrEmpty(_topic))
                throw new ConfigurationException("ProducerOptions.Topic may not be null or empty");

            var options = new ProducerOptions<TMessage>(_topic!, _schema)
            {
                CompressionType = _compressionType,
                InitialSequenceId = _initialSequenceId,
                ProducerName = _producerName,
                StateChangedHandler = _stateChangedHandler
            };

            if (_messageRouter is not null)
                options.MessageRouter = _messageRouter;

            return _pulsarClient.CreateProducer<TMessage>(options);
        }
    }
}
