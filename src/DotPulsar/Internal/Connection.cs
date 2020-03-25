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

using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Exceptions;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class Connection : IConnection
    {
        private readonly AsyncLock _lock;
        private readonly ChannelManager _channelManager;
        private readonly RequestResponseHandler _requestResponseHandler;
        private readonly PingPongHandler _pingPongHandler;
        private readonly IPulsarStream _stream;
        private int _isDisposed;

        public Connection(IPulsarStream stream)
        {
            _lock = new AsyncLock();
            _channelManager = new ChannelManager();
            _requestResponseHandler = new RequestResponseHandler();
            _pingPongHandler = new PingPongHandler(this);
            _stream = stream;
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

            Task<ProducerResponse>? responseTask = null;

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                var baseCommand = command.AsBaseCommand();
                var requestResponseTask = _requestResponseHandler.Outgoing(baseCommand);
                responseTask = _channelManager.Outgoing(command, requestResponseTask, channel);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence).ConfigureAwait(false);
            }

            return await responseTask.ConfigureAwait(false);
        }

        public async Task<SubscribeResponse> Send(CommandSubscribe command, IChannel channel, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            Task<SubscribeResponse>? responseTask = null;

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                var baseCommand = command.AsBaseCommand();
                var requestResponseTask = _requestResponseHandler.Outgoing(baseCommand);
                responseTask = _channelManager.Outgoing(command, requestResponseTask, channel);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence).ConfigureAwait(false);
            }

            return await responseTask.ConfigureAwait(false);
        }

        public async Task Send(CommandPing command, CancellationToken cancellationToken) =>
            await Send(command.AsBaseCommand(), cancellationToken).ConfigureAwait(false);

        public async Task Send(CommandPong command, CancellationToken cancellationToken) =>
            await Send(command.AsBaseCommand(), cancellationToken).ConfigureAwait(false);

        public async Task Send(CommandAck command, CancellationToken cancellationToken) =>
            await Send(command.AsBaseCommand(), cancellationToken).ConfigureAwait(false);

        public async Task Send(CommandFlow command, CancellationToken cancellationToken) =>
            await Send(command.AsBaseCommand(), cancellationToken).ConfigureAwait(false);

        public async Task<BaseCommand> Send(CommandUnsubscribe command, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            Task<BaseCommand>? responseTask = null;

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                var baseCommand = command.AsBaseCommand();
                responseTask = _requestResponseHandler.Outgoing(baseCommand);
                _channelManager.Outgoing(command, responseTask);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence).ConfigureAwait(false);
            }

            return await responseTask.ConfigureAwait(false);
        }

        public async Task<BaseCommand> Send(CommandConnect command, CancellationToken cancellationToken) =>
            await SendRequestResponse(command.AsBaseCommand(), cancellationToken).ConfigureAwait(false);

        public async Task<BaseCommand> Send(CommandLookupTopic command, CancellationToken cancellationToken) =>
            await SendRequestResponse(command.AsBaseCommand(), cancellationToken).ConfigureAwait(false);

        public async Task<BaseCommand> Send(CommandSeek command, CancellationToken cancellationToken) =>
            await SendRequestResponse(command.AsBaseCommand(), cancellationToken).ConfigureAwait(false);

        public async Task<BaseCommand> Send(CommandGetLastMessageId command, CancellationToken cancellationToken) =>
            await SendRequestResponse(command.AsBaseCommand(), cancellationToken).ConfigureAwait(false);

        public async Task<BaseCommand> Send(CommandCloseProducer command, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            Task<BaseCommand>? responseTask = null;

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                var baseCommand = command.AsBaseCommand();
                responseTask = _requestResponseHandler.Outgoing(baseCommand);
                _channelManager.Outgoing(command, responseTask);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence).ConfigureAwait(false);
            }

            return await responseTask.ConfigureAwait(false);
        }

        public async Task<BaseCommand> Send(CommandCloseConsumer command, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            Task<BaseCommand>? responseTask = null;

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                var baseCommand = command.AsBaseCommand();
                responseTask = _requestResponseHandler.Outgoing(baseCommand);
                _channelManager.Outgoing(command, responseTask);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence).ConfigureAwait(false);
            }

            return await responseTask.ConfigureAwait(false);
        }

        public async Task<BaseCommand> Send(SendPackage command, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            Task<BaseCommand>? response = null;

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                var baseCommand = command.Command.AsBaseCommand();
                response = _requestResponseHandler.Outgoing(baseCommand);
                var sequence = Serializer.Serialize(baseCommand, command.Metadata, command.Payload);
                await _stream.Send(sequence).ConfigureAwait(false);
            }

            return await response.ConfigureAwait(false);
        }

        private async Task<BaseCommand> SendRequestResponse(BaseCommand command, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            Task<BaseCommand>? response = null;

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                response = _requestResponseHandler.Outgoing(command);
                var sequence = Serializer.Serialize(command);
                await _stream.Send(sequence).ConfigureAwait(false);
            }

            return await response.ConfigureAwait(false);
        }

        private async Task Send(BaseCommand command, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                var sequence = Serializer.Serialize(command);
                await _stream.Send(sequence).ConfigureAwait(false);
            }
        }

        public async Task ProcessIncommingFrames(CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                await foreach (var frame in _stream.Frames())
                {
                    var commandSize = frame.ReadUInt32(0, true);
                    var command = Serializer.Deserialize<BaseCommand>(frame.Slice(4, commandSize));

                    switch (command.CommandType)
                    {
                        case BaseCommand.Type.Message:
                            _channelManager.Incoming(command.Message, frame.Slice(commandSize + 4));
                            break;
                        case BaseCommand.Type.CloseConsumer:
                            _channelManager.Incoming(command.CloseConsumer);
                            break;
                        case BaseCommand.Type.ActiveConsumerChange:
                            _channelManager.Incoming(command.ActiveConsumerChange);
                            break;
                        case BaseCommand.Type.ReachedEndOfTopic:
                            _channelManager.Incoming(command.ReachedEndOfTopic);
                            break;
                        case BaseCommand.Type.CloseProducer:
                            _channelManager.Incoming(command.CloseProducer);
                            break;
                        case BaseCommand.Type.Ping:
                            _pingPongHandler.Incoming(command.Ping, cancellationToken);
                            break;
                        default:
                            _requestResponseHandler.Incoming(command);
                            break;
                    }
                }
            }
            catch { }
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            await _lock.DisposeAsync().ConfigureAwait(false);
            _requestResponseHandler.Dispose();
            _channelManager.Dispose();
            await _stream.DisposeAsync().ConfigureAwait(false);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new ConnectionDisposedException();
        }
    }
}
