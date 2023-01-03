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

using Abstractions;
using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal.Extensions;
using Events;
using Microsoft.Extensions.ObjectPool;
using PulsarApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed class Consumer<TMessage> : IContainsChannel, IConsumer<TMessage>
{
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private IConsumerChannel<TMessage> _channel;
    private readonly ObjectPool<CommandAck> _commandAckPool;
    private readonly IExecute _executor;
    private readonly IStateChanged<ConsumerState> _state;
    private readonly IConsumerChannelFactory<TMessage> _factory;
    private int _isDisposed;

    public Uri ServiceUrl { get; }
    public string SubscriptionName { get; }
    public string Topic { get; }

    public Consumer(
        Guid correlationId,
        Uri serviceUrl,
        string subscriptionName,
        string topic,
        IRegisterEvent eventRegister,
        IConsumerChannel<TMessage> initialChannel,
        IExecute executor,
        IStateChanged<ConsumerState> state,
        IConsumerChannelFactory<TMessage> factory)
    {
        _correlationId = correlationId;
        ServiceUrl = serviceUrl;
        SubscriptionName = subscriptionName;
        Topic = topic;
        _eventRegister = eventRegister;
        _channel = initialChannel;
        _executor = executor;
        _state = state;
        _factory = factory;
        _commandAckPool = new DefaultObjectPool<CommandAck>(new DefaultPooledObjectPolicy<CommandAck>());
        _isDisposed = 0;

        _eventRegister.Register(new ConsumerCreated(_correlationId));
    }

    public async ValueTask<ConsumerState> OnStateChangeTo(ConsumerState state, CancellationToken cancellationToken)
        => await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);

    public async ValueTask<ConsumerState> OnStateChangeFrom(ConsumerState state, CancellationToken cancellationToken)
        => await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);

    public bool IsFinalState()
        => _state.IsFinalState();

    public bool IsFinalState(ConsumerState state)
        => _state.IsFinalState(state);

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _eventRegister.Register(new ConsumerDisposed(_correlationId));
        await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
        await _channel.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        return await _executor.Execute(() => ReceiveMessage(cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<IMessage<TMessage>> ReceiveMessage(CancellationToken cancellationToken)
        => await _channel.Receive(cancellationToken).ConfigureAwait(false);

    public async ValueTask Acknowledge(MessageId messageId, CancellationToken cancellationToken)
        => await Acknowledge(messageId, CommandAck.AckType.Individual, cancellationToken).ConfigureAwait(false);

    public async ValueTask AcknowledgeCumulative(MessageId messageId, CancellationToken cancellationToken)
        => await Acknowledge(messageId, CommandAck.AckType.Cumulative, cancellationToken).ConfigureAwait(false);

    public async ValueTask RedeliverUnacknowledgedMessages(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var command = new CommandRedeliverUnacknowledgedMessages();
        command.MessageIds.AddRange(messageIds.Select(messageId => messageId.ToMessageIdData()));
        await _executor.Execute(() => RedeliverUnacknowledgedMessages(command, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RedeliverUnacknowledgedMessages(CancellationToken cancellationToken)
        => await RedeliverUnacknowledgedMessages(Enumerable.Empty<MessageId>(), cancellationToken).ConfigureAwait(false);

    public async ValueTask Unsubscribe(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var unsubscribe = new CommandUnsubscribe();
        await _executor.Execute(() => Unsubscribe(unsubscribe, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask Unsubscribe(CommandUnsubscribe command, CancellationToken cancellationToken)
        => await _channel.Send(command, cancellationToken).ConfigureAwait(false);

    public async ValueTask Seek(MessageId messageId, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var seek = new CommandSeek { MessageId = messageId.ToMessageIdData() };
        await _executor.Execute(() => Seek(seek, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask Seek(ulong publishTime, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var seek = new CommandSeek { MessagePublishTime = publishTime };
        await _executor.Execute(() => Seek(seek, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var getLastMessageId = new CommandGetLastMessageId();
        return await _executor.Execute(() => GetLastMessageId(getLastMessageId, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<MessageId> GetLastMessageId(CommandGetLastMessageId command, CancellationToken cancellationToken)
        => await _channel.Send(command, cancellationToken).ConfigureAwait(false);

    private async Task Seek(CommandSeek command, CancellationToken cancellationToken)
        => await _channel.Send(command, cancellationToken).ConfigureAwait(false);

    private async ValueTask Acknowledge(MessageId messageId, CommandAck.AckType ackType, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var commandAck = _commandAckPool.Get();
        commandAck.Type = ackType;
        if (commandAck.MessageIds.Count == 0)
            commandAck.MessageIds.Add(messageId.ToMessageIdData());
        else
            commandAck.MessageIds[0].MapFrom(messageId);

        try
        {
            await _executor.Execute(() => Acknowledge(commandAck, cancellationToken), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _commandAckPool.Return(commandAck);
        }
    }

    private async ValueTask Acknowledge(CommandAck command, CancellationToken cancellationToken)
        => await _channel.Send(command, cancellationToken).ConfigureAwait(false);

    private async ValueTask RedeliverUnacknowledgedMessages(CommandRedeliverUnacknowledgedMessages command, CancellationToken cancellationToken)
        => await _channel.Send(command, cancellationToken).ConfigureAwait(false);

    private void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
            throw new ConsumerDisposedException(GetType().FullName!);
    }

    public async Task EstablishNewChannel(CancellationToken cancellationToken)
    {
        var channel = await _executor.Execute(() => _factory.Create(cancellationToken), cancellationToken).ConfigureAwait(false);

        var oldChannel = _channel;
        if (oldChannel is not null)
            await oldChannel.DisposeAsync().ConfigureAwait(false);

        _channel = channel;
    }

    public async ValueTask CloseChannel(CancellationToken cancellationToken)
    {
        await _channel.ClosedByClient(cancellationToken).ConfigureAwait(false);
    }
}
