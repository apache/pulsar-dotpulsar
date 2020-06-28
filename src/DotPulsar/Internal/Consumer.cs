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
    using DotPulsar.Exceptions;
    using Events;
    using PulsarApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class Consumer : IConsumer
    {
        private readonly Guid _correlationId;
        private readonly IRegisterEvent _eventRegister;
        private IConsumerChannel _channel;
        private readonly CommandAck _cachedCommandAck;
        private readonly CommandRedeliverUnacknowledgedMessages _cachedCommandRedeliverUnacknowledgedMessages;
        private readonly IExecute _executor;
        private readonly IStateChanged<ConsumerState> _state;
        private int _isDisposed;

        public string Topic { get; }

        public Consumer(
            Guid correlationId,
            string topic,
            IRegisterEvent eventRegister,
            IConsumerChannel initialChannel,
            IExecute executor,
            IStateChanged<ConsumerState> state)
        {
            _correlationId = correlationId;
            Topic = topic;
            _eventRegister = eventRegister;
            _channel = initialChannel;
            _executor = executor;
            _state = state;
            _cachedCommandAck = new CommandAck();
            _cachedCommandRedeliverUnacknowledgedMessages = new CommandRedeliverUnacknowledgedMessages();
            _isDisposed = 0;

            _eventRegister.Register(new ConsumerCreated(_correlationId, this));
        }

        public async ValueTask<ConsumerStateChanged> StateChangedTo(ConsumerState state, CancellationToken cancellationToken)
        {
            var newState = await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);
            return new ConsumerStateChanged(this, newState);
        }

        public async ValueTask<ConsumerStateChanged> StateChangedFrom(ConsumerState state, CancellationToken cancellationToken)
        {
            var newState = await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);
            return new ConsumerStateChanged(this, newState);
        }

        public bool IsFinalState()
            => _state.IsFinalState();

        public bool IsFinalState(ConsumerState state)
            => _state.IsFinalState(state);

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            _eventRegister.Register(new ConsumerDisposed(_correlationId, this));
            await _channel.DisposeAsync().ConfigureAwait(false);
        }

        public async IAsyncEnumerable<Message> Messages([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            while (!cancellationToken.IsCancellationRequested)
                yield return await _executor.Execute(() => _channel.Receive(cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask Acknowledge(Message message, CancellationToken cancellationToken)
            => await Acknowledge(message.MessageId.Data, CommandAck.AckType.Individual, cancellationToken).ConfigureAwait(false);

        public async ValueTask Acknowledge(MessageId messageId, CancellationToken cancellationToken)
            => await Acknowledge(messageId.Data, CommandAck.AckType.Individual, cancellationToken).ConfigureAwait(false);

        public async ValueTask AcknowledgeCumulative(Message message, CancellationToken cancellationToken)
            => await Acknowledge(message.MessageId.Data, CommandAck.AckType.Cumulative, cancellationToken).ConfigureAwait(false);

        public async ValueTask AcknowledgeCumulative(MessageId messageId, CancellationToken cancellationToken)
            => await Acknowledge(messageId.Data, CommandAck.AckType.Cumulative, cancellationToken).ConfigureAwait(false);

        public async ValueTask RedeliverUnacknowledgedMessages(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken)
            => await RedeliverUnacknowledgedMessages(messageIds.Select(m => m.Data).ToList(), cancellationToken).ConfigureAwait(false);

        public async ValueTask RedeliverUnacknowledgedMessages(CancellationToken cancellationToken)
            => await RedeliverUnacknowledgedMessages(Enumerable.Empty<MessageId>(), cancellationToken).ConfigureAwait(false);

        public async ValueTask Unsubscribe(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            _ = await _executor.Execute(() => _channel.Send(new CommandUnsubscribe(), cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask Seek(MessageId messageId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            var seek = new CommandSeek
            {
                MessageId = messageId.Data
            };

            _ = await _executor.Execute(() => _channel.Send(seek, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            var response = await _executor.Execute(() => _channel.Send(new CommandGetLastMessageId(), cancellationToken), cancellationToken).ConfigureAwait(false);

            return new MessageId(response.LastMessageId);
        }

        private async ValueTask Acknowledge(MessageIdData messageIdData, CommandAck.AckType ackType, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            await _executor.Execute(() =>
            {
                _cachedCommandAck.Type = ackType;
                _cachedCommandAck.MessageIds.Clear();
                _cachedCommandAck.MessageIds.Add(messageIdData);
                return _channel.Send(_cachedCommandAck, cancellationToken);
            }, cancellationToken).ConfigureAwait(false);
        }

        private async ValueTask RedeliverUnacknowledgedMessages(List<MessageIdData> messageIds, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            await _executor.Execute(() =>
            {
                _cachedCommandRedeliverUnacknowledgedMessages.MessageIds.Clear();
                _cachedCommandAck.MessageIds.AddRange(messageIds);
                return _channel.Send(_cachedCommandRedeliverUnacknowledgedMessages, cancellationToken);
            }, cancellationToken).ConfigureAwait(false);
        }
        internal async ValueTask SetChannel(IConsumerChannel channel)
        {
            if (_isDisposed != 0)
            {
                await channel.DisposeAsync().ConfigureAwait(false);
                return;
            }

            var oldChannel = _channel;
            _channel = channel;

            if (oldChannel != null)
                await oldChannel.DisposeAsync().ConfigureAwait(false);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new ConsumerDisposedException();
        }
    }
}
