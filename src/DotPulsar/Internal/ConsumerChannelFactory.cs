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

using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public sealed class ConsumerChannelFactory<TMessage> : IConsumerChannelFactory<TMessage>
{
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private readonly IConnectionPool _connectionPool;
    private readonly CommandSubscribe _subscribe;
    private readonly uint _messagePrefetchCount;
    private readonly BatchHandler<TMessage> _batchHandler;
    private readonly IMessageFactory<TMessage> _messageFactory;
    private readonly IEnumerable<IDecompressorFactory> _decompressorFactories;
    private readonly string _topic;

    public ConsumerChannelFactory(
        Guid correlationId,
        IRegisterEvent eventRegister,
        IConnectionPool connectionPool,
        CommandSubscribe subscribe,
        uint messagePrefetchCount,
        BatchHandler<TMessage> batchHandler,
        IMessageFactory<TMessage> messageFactory,
        IEnumerable<IDecompressorFactory> decompressorFactories,
        string topic)
    {
        _correlationId = correlationId;
        _eventRegister = eventRegister;
        _connectionPool = connectionPool;
        _subscribe = subscribe;
        _messagePrefetchCount = messagePrefetchCount;
        _batchHandler = batchHandler;
        _messageFactory = messageFactory;
        _decompressorFactories = decompressorFactories;
        _topic = topic;
    }

    public async Task<IConsumerChannel<TMessage>> Create(CancellationToken cancellationToken)
    {
        var connection = await _connectionPool.FindConnectionForTopic(_subscribe.Topic, cancellationToken).ConfigureAwait(false);
        var messageQueue = new AsyncQueue<MessagePackage>();
        var channel = new Channel(_correlationId, _eventRegister, messageQueue);
        var response = await connection.Send(_subscribe, channel, cancellationToken).ConfigureAwait(false);
        return new ConsumerChannel<TMessage>(response.ConsumerId, _messagePrefetchCount, messageQueue, connection, _batchHandler, _messageFactory, _decompressorFactories, _topic);
    }
}
