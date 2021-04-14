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
    using DotPulsar.Abstractions;
    using Extensions;
    using PulsarApi;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ProducerChannelFactory : IProducerChannelFactory
    {
        private readonly Guid _correlationId;
        private readonly IRegisterEvent _eventRegister;
        private readonly IConnectionPool _connectionPool;
        private readonly IExecute _executor;
        private readonly CommandProducer _commandProducer;
        private readonly IPulsarClientLogger? _logger;

        public ProducerChannelFactory(
            Guid correlationId,
            IRegisterEvent eventRegister,
            IConnectionPool connectionPool,
            IExecute executor,
            ProducerOptions options,
            IPulsarClientLogger? logger)
        {
            _correlationId = correlationId;
            _eventRegister = eventRegister;
            _connectionPool = connectionPool;
            _executor = executor;
            _logger = logger;

            _commandProducer = new CommandProducer
            {
                ProducerName = options.ProducerName,
                Topic = options.Topic
            };
        }

        public async Task<IProducerChannel> Create(CancellationToken cancellationToken)
            => await _executor.Execute(() => GetChannel(cancellationToken), cancellationToken).ConfigureAwait(false);

        private async ValueTask<IProducerChannel> GetChannel(CancellationToken cancellationToken)
        {
            _logger.Debug(nameof(ProducerChannelFactory), nameof(GetChannel), "Creating producer channel for topic {0}", _commandProducer.Topic);
            var connection = await _connectionPool.FindConnectionForTopic(_commandProducer.Topic, cancellationToken).ConfigureAwait(false);
            var channel = new Channel(_correlationId, _eventRegister, new AsyncQueue<MessagePackage>());
            _logger.Trace(nameof(ProducerChannelFactory), nameof(GetChannel), "Sending Producer command on {0} for topic {1}", connection.Id, _commandProducer.Topic);
            var response = await connection.Send(_commandProducer, channel, cancellationToken).ConfigureAwait(false);
            _logger.Trace(nameof(ProducerChannelFactory), nameof(GetChannel), "ProducerChannel created with producer id {0} (name {1}) on {2} for topic {3}", response.ProducerId, response.ProducerName, connection.Id, _commandProducer.Topic);
            return new ProducerChannel(response.ProducerId, response.ProducerName, connection);
        }
    }
}
