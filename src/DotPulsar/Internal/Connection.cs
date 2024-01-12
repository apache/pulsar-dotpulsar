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
using DotPulsar.Extensions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Exceptions;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System.Buffers;

public sealed class Connection : IConnection
{
    private readonly StateManager<ConnectionState> _stateManager;
    private readonly AsyncLock _lock;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly ChannelManager _channelManager;
    private readonly PingPongHandler _pingPongHandler;
    private readonly IPulsarStream _stream;
    private readonly IAuthentication? _authentication;
    private readonly TimeSpan _closeOnInactiveInterval;
    private int _isDisposed;

    private Connection(IPulsarStream stream, IAuthentication? authentication, TimeSpan keepAliveInterval, TimeSpan closeOnInactiveInterval)
    {
        _stateManager = new StateManager<ConnectionState>(ConnectionState.Connected, ConnectionState.Disconnected, ConnectionState.Closed);
        _lock = new AsyncLock();
        _cancellationTokenSource = new CancellationTokenSource();
        _channelManager = new ChannelManager();
        _pingPongHandler = new PingPongHandler(keepAliveInterval);
        _stream = stream;
        _authentication = authentication;
        _closeOnInactiveInterval = closeOnInactiveInterval;
        _ = Task.Factory.StartNew(() => Setup(_cancellationTokenSource.Token));
    }

    public static Connection Connect(
        IPulsarStream stream,
        IAuthentication? authentication,
        TimeSpan keepAliveInterval,
        TimeSpan closeOnInactiveInterval)
    {
        var connection = new Connection(stream, authentication, keepAliveInterval, closeOnInactiveInterval);
        DotPulsarMeter.ConnectionCreated();
        return connection;
    }

    public int MaxMessageSize { get; set; }

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
        if (_authentication is not null)
        {
            command.Response ??= new AuthData();
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

    public async Task Send(SendPackage command, TaskCompletionSource<BaseCommand> responseTcs, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        try
        {
            var sequence = Serializer.Serialize(command.Command!.AsBaseCommand(), command.Metadata!, command.Payload);

            if (sequence.Length > MaxMessageSize)
                throw new TooLargeMessageException((int) sequence.Length, MaxMessageSize);

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                _channelManager.Outgoing(command.Command!, responseTcs);
                await _stream.Send(sequence).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            _ = responseTcs.TrySetCanceled();
            throw;
        }
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

    private async Task Setup(CancellationToken cancellationToken)
    {
        var incoming = ProcessIncomingFrames(cancellationToken);
        var channelManager = _channelManager.OnStateChangeTo(ChannelManagerState.Inactive, _closeOnInactiveInterval, cancellationToken).AsTask();
        var pingPongTimeOut = _pingPongHandler.OnStateChangeTo(PingPongHandlerState.TimedOut, cancellationToken).AsTask();
        _ = Task.Factory.StartNew(async () => await KeepAlive(PingPongHandlerState.Active, cancellationToken).ConfigureAwait(false));
        await Task.WhenAny(incoming, channelManager, pingPongTimeOut).ConfigureAwait(false);
        _stateManager.SetState(ConnectionState.Disconnected);
    }

    private async Task KeepAlive(PingPongHandlerState state, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            state = await _pingPongHandler.OnStateChangeFrom(state, cancellationToken).ConfigureAwait(false);
            if (state == PingPongHandlerState.TimedOut)
                return;
            if (state == PingPongHandlerState.Active)
                continue;
            if (state == PingPongHandlerState.ThresholdExceeded)
                await Send(new CommandPing(), cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessIncomingFrames(CancellationToken cancellationToken)
    {
        await Task.Yield();

        try
        {
            await foreach (var frame in _stream.Frames(cancellationToken).ConfigureAwait(false))
            {
                var commandSize = frame.ReadUInt32(0, true);
                var command = Serializer.Deserialize<BaseCommand>(frame.Slice(4, commandSize));

                _pingPongHandler.Incoming(command.CommandType);

                if (command.CommandType == BaseCommand.Type.Message)
                    _channelManager.Incoming(command.Message, new ReadOnlySequence<byte>(frame.Slice(commandSize + 4).ToArray()));
                else if (command.CommandType == BaseCommand.Type.AuthChallenge)
                    _ = Task.Factory.StartNew(async () => await Send(new CommandAuthResponse(), cancellationToken).ConfigureAwait(false));
                else if (command.CommandType == BaseCommand.Type.Ping)
                    _ = Task.Factory.StartNew(async () => await Send(new CommandPong(), cancellationToken).ConfigureAwait(false));
                else if (command.CommandType == BaseCommand.Type.Pong)
                    continue;
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

        DotPulsarMeter.ConnectionDisposed();

        _stateManager.SetState(ConnectionState.Closed);
        _cancellationTokenSource.Cancel();
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

    public bool IsFinalState() => _stateManager.IsFinalState();

    public bool IsFinalState(ConnectionState state) => _stateManager.IsFinalState(state);

    public async ValueTask<ConnectionState> OnStateChangeTo(ConnectionState state, CancellationToken cancellationToken = default)
        => await _stateManager.StateChangedTo(state, cancellationToken).ConfigureAwait(false);

    public async ValueTask<ConnectionState> OnStateChangeFrom(ConnectionState state, CancellationToken cancellationToken = default)
        => await _stateManager.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);
}
