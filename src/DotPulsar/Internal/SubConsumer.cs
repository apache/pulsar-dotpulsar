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
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Events;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using Microsoft.Extensions.ObjectPool;

public sealed class SubConsumer<TMessage> : IConsumer<TMessage>, IContainsChannel
{
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private IConsumerChannel<TMessage> _channel;
    private readonly ObjectPool<CommandAck> _commandAckPool;
    private readonly IExecute _executor;
    private readonly IStateChanged<ConsumerState> _state;
    private readonly IConsumerChannelFactory<TMessage> _factory;
    private int _isDisposed;
    private Exception? _faultException;

    public Uri ServiceUrl { get; }
    public string SubscriptionName { get; }
    public SubscriptionType SubscriptionType { get; }
    public string Topic { get; }

    public SubConsumer(
        Guid correlationId,
        Uri serviceUrl,
        string subscriptionName,
        SubscriptionType subscriptionType,
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
        SubscriptionType = subscriptionType;
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
        await DisposeChannel().ConfigureAwait(false);
    }

    private async ValueTask DisposeChannel()
    {
        await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
        await _channel.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
        => await _executor.Execute(() => InternalReceive(cancellationToken), cancellationToken).ConfigureAwait(false);

    public async ValueTask Acknowledge(MessageId messageId, CancellationToken cancellationToken)
        => await InternalAcknowledge(messageId, CommandAck.AckType.Individual, cancellationToken).ConfigureAwait(false);

    public async ValueTask Acknowledge(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken = default)
        => await InternalAcknowledge(messageIds, cancellationToken).ConfigureAwait(false);

    public async ValueTask AcknowledgeCumulative(MessageId messageId, CancellationToken cancellationToken)
        => await InternalAcknowledge(messageId, CommandAck.AckType.Cumulative, cancellationToken).ConfigureAwait(false);

    public async ValueTask RedeliverUnacknowledgedMessages(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken)
    {
        var command = new CommandRedeliverUnacknowledgedMessages();
        command.MessageIds.AddRange(messageIds.Select(messageId => messageId.ToMessageIdData()));
        await _executor.Execute(() => InternalRedeliverUnacknowledgedMessages(command, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RedeliverUnacknowledgedMessages(CancellationToken cancellationToken)
        => await RedeliverUnacknowledgedMessages(Enumerable.Empty<MessageId>(), cancellationToken).ConfigureAwait(false);

    public async ValueTask Unsubscribe(CancellationToken cancellationToken)
    {
        var unsubscribe = new CommandUnsubscribe();
        await _executor.Execute(() => InternalUnsubscribe(unsubscribe, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask Seek(MessageId messageId, CancellationToken cancellationToken)
    {
        var seek = new CommandSeek { MessageId = messageId.ToMessageIdData() };
        await _executor.Execute(() => InternalSeek(seek, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask Seek(ulong publishTime, CancellationToken cancellationToken)
    {
        var seek = new CommandSeek { MessagePublishTime = publishTime };
        await _executor.Execute(() => InternalSeek(seek, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken)
    {
        var getLastMessageId = new CommandGetLastMessageId();
        return await _executor.Execute(() => InternalGetLastMessageId(getLastMessageId, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private void Guard()
    {
        if (_isDisposed != 0)
            throw new ConsumerDisposedException(GetType().FullName!);

        if (_faultException is not null)
            throw _faultException;
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
        => await _channel.ClosedByClient(cancellationToken).ConfigureAwait(false);

    public async ValueTask ChannelFaulted(Exception exception)
    {
        _faultException = exception;
        await DisposeChannel().ConfigureAwait(false);
    }

    private async ValueTask InternalAcknowledge(CommandAck command, CancellationToken cancellationToken)
    {
        Guard();
        await _channel.Send(command, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask InternalRedeliverUnacknowledgedMessages(CommandRedeliverUnacknowledgedMessages command, CancellationToken cancellationToken)
    {
        Guard();
        await _channel.Send(command, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<MessageId> InternalGetLastMessageId(CommandGetLastMessageId command, CancellationToken cancellationToken)
    {
        Guard();
        return await _channel.Send(command, cancellationToken).ConfigureAwait(false);
    }

    private async Task InternalSeek(CommandSeek command, CancellationToken cancellationToken)
    {
        Guard();
        await _channel.Send(command, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<IMessage<TMessage>> InternalReceive(CancellationToken cancellationToken)
    {
        Guard();
        return await _channel.Receive(cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask InternalUnsubscribe(CommandUnsubscribe command, CancellationToken cancellationToken)
    {
        Guard();
        await _channel.Send(command, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask InternalAcknowledge(MessageId messageId, CommandAck.AckType ackType, CancellationToken cancellationToken)
    {
        var commandAck = _commandAckPool.Get();
        commandAck.Type = ackType;
        if (commandAck.MessageIds.Count == 0)
            commandAck.MessageIds.Add(messageId.ToMessageIdData());
        else
            commandAck.MessageIds[0].MapFrom(messageId);

        try
        {
            await _executor.Execute(() => InternalAcknowledge(commandAck, cancellationToken), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _commandAckPool.Return(commandAck);
        }
    }

    private async ValueTask InternalAcknowledge(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken)
    {
        var commandAck = _commandAckPool.Get();
        commandAck.Type = CommandAck.AckType.Individual;
        commandAck.MessageIds.Clear();

        foreach (var messageId in messageIds)
        {
            commandAck.MessageIds.Add(messageId.ToMessageIdData());
        }

        try
        {
            await _executor.Execute(() => InternalAcknowledge(commandAck, cancellationToken), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            commandAck.MessageIds.Clear();
            _commandAckPool.Return(commandAck);
        }
    }

    public ValueTask<IEnumerable<MessageId>> GetLastMessageIds(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}
