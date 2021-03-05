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

    public sealed class ReaderBuilder<TMessage> : IReaderBuilder<TMessage>
    {
        private readonly IPulsarClient _pulsarClient;
        private readonly ISchema<TMessage> _schema;
        private string? _readerName;
        private uint _messagePrefetchCount;
        private bool _readCompacted;
        private MessageId? _startMessageId;
        private string? _topic;
        private IHandleStateChanged<ReaderStateChanged>? _stateChangedHandler;

        public ReaderBuilder(IPulsarClient pulsarClient, ISchema<TMessage> schema)
        {
            _pulsarClient = pulsarClient;
            _schema = schema;
            _messagePrefetchCount = ReaderOptions<TMessage>.DefaultMessagePrefetchCount;
            _readCompacted = ReaderOptions<TMessage>.DefaultReadCompacted;
        }

        public IReaderBuilder<TMessage> MessagePrefetchCount(uint count)
        {
            _messagePrefetchCount = count;
            return this;
        }

        public IReaderBuilder<TMessage> ReadCompacted(bool readCompacted)
        {
            _readCompacted = readCompacted;
            return this;
        }

        public IReaderBuilder<TMessage> ReaderName(string name)
        {
            _readerName = name;
            return this;
        }

        public IReaderBuilder<TMessage> StartMessageId(MessageId messageId)
        {
            _startMessageId = messageId;
            return this;
        }

        public IReaderBuilder<TMessage> StateChangedHandler(IHandleStateChanged<ReaderStateChanged> handler)
        {
            _stateChangedHandler = handler;
            return this;
        }

        public IReaderBuilder<TMessage> Topic(string topic)
        {
            _topic = topic;
            return this;
        }

        public IReader<TMessage> Create()
        {
            if (_startMessageId is null)
                throw new ConfigurationException("StartMessageId may not be null");

            if (string.IsNullOrEmpty(_topic))
                throw new ConfigurationException("Topic may not be null or empty");

            var options = new ReaderOptions<TMessage>(_startMessageId, _topic!, _schema)
            {
                MessagePrefetchCount = _messagePrefetchCount,
                ReadCompacted = _readCompacted,
                ReaderName = _readerName,
                StateChangedHandler = _stateChangedHandler
            };

            return _pulsarClient.CreateReader(options);
        }
    }
}
