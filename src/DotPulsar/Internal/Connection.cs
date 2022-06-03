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
using Exceptions;
using Extensions;
using PulsarApi;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

public sealed class Connection : IConnection
{
    private readonly AsyncLock _lock;
    private readonly ChannelManager _channelManager;
    private readonly PingPongHandler _pingPongHandler;
    private readonly IPulsarStream _stream;
    private readonly IAuthentication? _authentication;
    private int _isDisposed;

    public Connection(
        IPulsarStream stream,
        TimeSpan keepAliveInterval,
        TimeSpan serverResponseTimeout,
        IAuthentication? authentication)
    {
        _lock = new AsyncLock();
        _channelManager = new ChannelManager();
        _pingPongHandler = new PingPongHandler(this, keepAliveInterval, serverResponseTimeout);
        _stream = stream;
        _authentication = authentication;
    }

    public async ValueTask<bool> HasChannels(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            return _channelManager.HasChannels();
        }
    }

    public async Task<ProducerResponse> Send(CommandProducer command, IChannel channel, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<ProducerResponse>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command, channel);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    public async Task<SubscribeResponse> Send(CommandSubscribe command, IChannel channel, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<SubscribeResponse>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command, channel);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    private async Task Send(CommandAuthResponse command, CancellationToken cancellationToken)
    {
        await Task.Yield();

        if (_authentication is not null)
        {
            if (command.Response is null)
                command.Response = new AuthData();

            command.Response.AuthMethodName = _authentication.AuthenticationMethodName;
            command.Response.Data = await _authentication.GetAuthenticationData(cancellationToken).ConfigureAwait(false);
        }

        await Send(command.AsBaseCommand(), cancellationToken).ConfigureAwait(false);
    }

    public Task Send(CommandPing command, CancellationToken cancellationToken)
        => Send(command.AsBaseCommand(), cancellationToken);

    public Task Send(CommandPong command, CancellationToken cancellationToken)
        => Send(command.AsBaseCommand(), cancellationToken);

    public Task Send(CommandAck command, CancellationToken cancellationToken)
        => Send(command.AsBaseCommand(), cancellationToken);

    public Task Send(CommandFlow command, CancellationToken cancellationToken)
        => Send(command.AsBaseCommand(), cancellationToken);

    public Task Send(CommandRedeliverUnacknowledgedMessages command, CancellationToken cancellationToken)
        => Send(command.AsBaseCommand(), cancellationToken);

    public async Task<BaseCommand> Send(CommandUnsubscribe command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<BaseCommand>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    public async Task<BaseCommand> Send(CommandConnect command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        if (_authentication is not null)
        {
            command.AuthMethodName = _authentication.AuthenticationMethodName;
            command.AuthData = await _authentication.GetAuthenticationData(cancellationToken).ConfigureAwait(false);
        }

        Task<BaseCommand>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    public async Task<BaseCommand> Send(CommandLookupTopic command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<BaseCommand>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    public async Task<BaseCommand> Send(CommandSeek command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<BaseCommand>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    public async Task<BaseCommand> Send(CommandGetLastMessageId command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<BaseCommand>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    public async Task<BaseCommand> Send(CommandCloseProducer command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<BaseCommand>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    public async Task<BaseCommand> Send(CommandCloseConsumer command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<BaseCommand>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    public async Task<BaseCommand> Send(SendPackage command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<BaseCommand>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command.Command!);
            var sequence = Serializer.Serialize(command.Command!.AsBaseCommand(), command.Metadata!, command.Payload);
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    public async Task<BaseCommand> Send(CommandGetOrCreateSchema command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<BaseCommand>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    public async Task<BaseCommand> Send(CommandPartitionedTopicMetadata command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        Task<BaseCommand>? responseTask;

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            responseTask = _channelManager.Outgoing(command);
            var sequence = Serializer.Serialize(command.AsBaseCommand());
            await _stream.Send(sequence).ConfigureAwait(false);
        }

        return await responseTask.ConfigureAwait(false);
    }

    private async Task Send(BaseCommand command, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        var sequence = Serializer.Serialize(command);

        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            await _stream.Send(sequence).ConfigureAwait(false);
        }
    }

    public async Task ProcessIncommingFrames(CancellationToken cancellationToken)
    {
        await Task.WhenAny(ProcessIncommingFramesImpl(cancellationToken), _pingPongHandler.ServerNotResponding).ConfigureAwait(false);
    }

    public async Task ProcessIncommingFramesImpl(CancellationToken cancellationToken)
    {
        await Task.Yield();

        try
        {
            await foreach (var frame in _stream.Frames(cancellationToken))
            {
                var commandSize = frame.ReadUInt32(0, true);
                var command = Serializer.Deserialize<BaseCommand>(frame.Slice(4, commandSize));

                if (_pingPongHandler.Incoming(command.CommandType))
                    continue;

                if (command.CommandType == BaseCommand.Type.Message)
                    _channelManager.Incoming(command.Message, new ReadOnlySequence<byte>(frame.Slice(commandSize + 4).ToArray()));
                else if (command.CommandType == BaseCommand.Type.AuthChallenge)
                    _ = Send(new CommandAuthResponse(), cancellationToken).ConfigureAwait(false);
                else
                    _channelManager.Incoming(command);
            }
        }
        catch
        {
            // ignored
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        await _pingPongHandler.DisposeAsync().ConfigureAwait(false);
        await _lock.DisposeAsync().ConfigureAwait(false);
        _channelManager.Dispose();
        await _stream.DisposeAsync().ConfigureAwait(false);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
            throw new ConnectionDisposedException();
    }
}
