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
    using Abstractions;
    using PulsarApi;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ReaderChannelFactory : IReaderChannelFactory
    {
        private readonly Guid _correlationId;
        private readonly IRegisterEvent _eventRegister;
        private readonly IConnectionPool _connectionPool;
        private readonly IExecute _executor;
        private readonly CommandSubscribe _subscribe;
        private readonly uint _messagePrefetchCount;
        private readonly BatchHandler _batchHandler;

        public ReaderChannelFactory(
            Guid correlationId,
            IRegisterEvent eventRegister,
            IConnectionPool connectionPool,
            IExecute executor,
            ReaderOptions options)
        {
            _correlationId = correlationId;
            _eventRegister = eventRegister;
            _connectionPool = connectionPool;
            _executor = executor;
            _messagePrefetchCount = options.MessagePrefetchCount;

            _subscribe = new CommandSubscribe
            {
                ConsumerName = options.ReaderName,
                Durable = false,
                ReadCompacted = options.ReadCompacted,
                StartMessageId = options.StartMessageId.Data,
                Subscription = $"Reader-{Guid.NewGuid():N}",
                Topic = options.Topic
            };

            _batchHandler = new BatchHandler(false);
        }

        public async Task<IReaderChannel> Create(CancellationToken cancellationToken)
            => await _executor.Execute(() => GetChannel(cancellationToken), cancellationToken).ConfigureAwait(false);

        private async ValueTask<IReaderChannel> GetChannel(CancellationToken cancellationToken)
        {
            var connection = await _connectionPool.FindConnectionForTopic(_subscribe.Topic, cancellationToken).ConfigureAwait(false);
            var messageQueue = new AsyncQueue<MessagePackage>();
            var tracker = new InactiveUnackedMessageTracker(); // No tracker for reader since readers don't ack.
            var consumerMessageQueue = new MessageQueue(messageQueue, tracker);
            var channel = new Channel(_correlationId, _eventRegister, messageQueue);
            var response = await connection.Send(_subscribe, channel, cancellationToken).ConfigureAwait(false);
            return new ConsumerChannel(response.ConsumerId, _messagePrefetchCount, consumerMessageQueue, connection, _batchHandler);
        }
    }
}
