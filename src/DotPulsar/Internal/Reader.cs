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
using DotPulsar.Internal.PulsarApi;
using Events;
using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class Reader<TMessage> : IEstablishNewChannel, IReader<TMessage>
{
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private IConsumerChannel<TMessage> _channel;
    private readonly IExecute _executor;
    private readonly IStateChanged<ReaderState> _state;
    private readonly IConsumerChannelFactory<TMessage> _factory;
    private int _isDisposed;

    public Uri ServiceUrl { get; }
    public string Topic { get; }

    public Reader(
        Guid correlationId,
        Uri serviceUrl,
        string topic,
        IRegisterEvent eventRegister,
        IConsumerChannel<TMessage> initialChannel,
        IExecute executor,
        IStateChanged<ReaderState> state,
        IConsumerChannelFactory<TMessage> factory)
    {
        _correlationId = correlationId;
        ServiceUrl = serviceUrl;
        Topic = topic;
        _eventRegister = eventRegister;
        _channel = initialChannel;
        _executor = executor;
        _state = state;
        _factory = factory;
        _isDisposed = 0;

        _eventRegister.Register(new ReaderCreated(_correlationId));
    }

    public async ValueTask<ReaderState> OnStateChangeTo(ReaderState state, CancellationToken cancellationToken)
        => await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);

    public async ValueTask<ReaderState> OnStateChangeFrom(ReaderState state, CancellationToken cancellationToken)
        => await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);

    public bool IsFinalState()
        => _state.IsFinalState();

    public bool IsFinalState(ReaderState state)
        => _state.IsFinalState(state);

    public async ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var getLastMessageId = new CommandGetLastMessageId();
        return await _executor.Execute(() => GetLastMessageId(getLastMessageId, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<MessageId> GetLastMessageId(CommandGetLastMessageId command, CancellationToken cancellationToken)
        => await _channel.Send(command, cancellationToken).ConfigureAwait(false);

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        return await _executor.Execute(() => ReceiveMessage(cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<IMessage<TMessage>> ReceiveMessage(CancellationToken cancellationToken)
        => await _channel.Receive(cancellationToken).ConfigureAwait(false);

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

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _eventRegister.Register(new ReaderDisposed(_correlationId));
        await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
        await _channel.DisposeAsync().ConfigureAwait(false);
    }

    private async Task Seek(CommandSeek command, CancellationToken cancellationToken)
        => await _channel.Send(command, cancellationToken).ConfigureAwait(false);

    public async Task EstablishNewChannel(CancellationToken cancellationToken)
    {
        var channel = await _executor.Execute(() => _factory.Create(cancellationToken), cancellationToken).ConfigureAwait(false);

        var oldChannel = _channel;
        _channel = channel;

        if (oldChannel is not null)
            await oldChannel.DisposeAsync().ConfigureAwait(false);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
            throw new ReaderDisposedException(GetType().FullName!);
    }
}
