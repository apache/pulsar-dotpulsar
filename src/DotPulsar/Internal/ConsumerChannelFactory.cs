﻿/*
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
    using DotPulsar.Abstractions;
    using PulsarApi;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ConsumerChannelFactory : IConsumerChannelFactory
    {
        private uint _messagePrefetchCount;

        private readonly Guid _correlationId;
        private readonly IRegisterEvent _eventRegister;
        private readonly IConnectionPool _connectionPool;
        private readonly IExecute _executor;
        private readonly CommandSubscribe _subscribe;
        private readonly BatchHandler _batchHandler;
        private readonly TimeSpan _negativeAcknowledgeRedeliveryDelay;

        private readonly IPulsarClientLogger? _logger;

        public ConsumerChannelFactory(
            Guid correlationId,
            IRegisterEvent eventRegister,
            IConnectionPool connectionPool,
            IExecute executor,
            BatchHandler batchHandler,
            ConsumerOptions options,
            IPulsarClientLogger? logger)
        {
            _correlationId = correlationId;
            _eventRegister = eventRegister;
            _connectionPool = connectionPool;
            _executor = executor;
            _batchHandler = batchHandler;
            _messagePrefetchCount = options.MessagePrefetchCount;
            _negativeAcknowledgeRedeliveryDelay = options.NegativeAcknowledgeRedeliveryDelay;
            _logger = logger;

            _subscribe = new CommandSubscribe
            {
                ConsumerName = options.ConsumerName ?? GenerateRandomConsumerName(),
                InitialPosition = (CommandSubscribe.InitialPositionType) options.InitialPosition,
                PriorityLevel = options.PriorityLevel,
                ReadCompacted = options.ReadCompacted,
                Subscription = options.SubscriptionName,
                Topic = options.Topic,
                Type = (CommandSubscribe.SubType) options.SubscriptionType
            };

            if (options.SubscriptionType == SubscriptionType.KeyShared && options.KeySharedPolicy != null)
            {
                _subscribe.KeySharedMeta = new KeySharedMeta { allowOutOfOrderDelivery = options.KeySharedPolicy.AllowOutOfOrderDelivery, KeySharedMode = KeySharedMode.AutoSplit };
            }
        }

        private static string GenerateRandomConsumerName()
        {
            try
            {
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    var bytes = new byte[4];
                    rng.GetBytes(bytes);
                    return BitConverter.ToString(bytes).Replace("-", String.Empty);
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        public void UpdateMessagePrefetchCount(uint messagePrefetchCount)
        {
            _messagePrefetchCount = messagePrefetchCount;
        }

        public async Task<IConsumerChannel> Create(CancellationToken cancellationToken)
            => await _executor.Execute(() => GetChannel(cancellationToken), cancellationToken).ConfigureAwait(false);

        private async ValueTask<IConsumerChannel> GetChannel(CancellationToken cancellationToken)
        {
            var connection = await _connectionPool.FindConnectionForTopic(_subscribe.Topic, cancellationToken).ConfigureAwait(false);
            var messageQueue = new AsyncQueue<MessagePackage>();
            var channel = new Channel(_correlationId, _eventRegister, messageQueue);
            var response = await connection.Send(_subscribe, channel, cancellationToken).ConfigureAwait(false);
            return new ConsumerChannel(response.ConsumerId, _messagePrefetchCount, messageQueue, connection, _batchHandler, _negativeAcknowledgeRedeliveryDelay, _logger);
        }
    }
}
