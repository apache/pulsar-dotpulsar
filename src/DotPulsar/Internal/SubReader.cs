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
using Pulsar.Proto;

public sealed class SubReader<TMessage> : IContainsChannel, IReader<TMessage>
{
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private IConsumerChannel<TMessage> _channel;
    private readonly IExecute _executor;
    private readonly IState<ReaderState> _state;
    private readonly IConsumerChannelFactory<TMessage> _factory;
    private int _isDisposed;
    private Exception? _faultException;

    public Uri ServiceUrl { get; }
    public string Topic { get; }
    public IState<ReaderState> State => _state;

    public SubReader(
        Guid correlationId,
        Uri serviceUrl,
        string topic,
        IRegisterEvent eventRegister,
        IConsumerChannel<TMessage> initialChannel,
        IExecute executor,
        IState<ReaderState> state,
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

    public async ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken)
    {
        var getLastMessageId = new CommandGetLastMessageId();
        return await _executor.Execute(() => InternalGetLastMessageId(getLastMessageId, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<MessageId> InternalGetLastMessageId(CommandGetLastMessageId command, CancellationToken cancellationToken)
    {
        Guard();
        return await _channel.Send(command, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
        => await _executor.Execute(() => InternalReceive(cancellationToken), cancellationToken).ConfigureAwait(false);

    private async ValueTask<IMessage<TMessage>> InternalReceive(CancellationToken cancellationToken)
    {
        Guard();
        return await _channel.Receive(cancellationToken).ConfigureAwait(false);
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

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _eventRegister.Register(new ReaderDisposed(_correlationId));
        await DisposeChannel().ConfigureAwait(false);
    }

    private async ValueTask DisposeChannel()
    {
        await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
        await _channel.DisposeAsync().ConfigureAwait(false);
    }

    private async Task InternalSeek(CommandSeek command, CancellationToken cancellationToken)
    {
        Guard();
        await _channel.Send(command, cancellationToken).ConfigureAwait(false);
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

    private void Guard()
    {
        if (_isDisposed != 0)
            throw new ReaderDisposedException(GetType().FullName!);

        if (_faultException is not null)
            throw _faultException;
    }

    public async ValueTask ChannelFaulted(Exception exception)
    {
        _faultException = exception;
        await DisposeChannel().ConfigureAwait(false);
    }
    public ValueTask<IEnumerable<MessageId>> GetLastMessageIds(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}
