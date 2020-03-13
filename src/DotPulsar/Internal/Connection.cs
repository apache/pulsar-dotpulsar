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
            using (await _lock.Lock(cancellationToken))
            {
                return _channelManager.HasChannels();
            }
        }

        public async Task<ProducerResponse> Send(CommandProducer command, IChannel channel, CancellationToken cancellationToken)
        {
            Task<ProducerResponse>? responseTask = null;

            using (await _lock.Lock(cancellationToken))
            {
                var baseCommand = command.AsBaseCommand();
                var requestResponseTask = _requestResponseHandler.Outgoing(baseCommand);
                responseTask = _channelManager.Outgoing(command, requestResponseTask, channel);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence);
            }

            return await responseTask;
        }

        public async Task<SubscribeResponse> Send(CommandSubscribe command, IChannel channel, CancellationToken cancellationToken)
        {
            Task<SubscribeResponse>? responseTask = null;

            using (await _lock.Lock(cancellationToken))
            {
                var baseCommand = command.AsBaseCommand();
                var requestResponseTask = _requestResponseHandler.Outgoing(baseCommand);
                responseTask = _channelManager.Outgoing(command, requestResponseTask, channel);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence);
            }

            return await responseTask;
        }

        public async Task Send(CommandPing command, CancellationToken cancellationToken) =>
            await Send(command.AsBaseCommand(), cancellationToken);

        public async Task Send(CommandPong command, CancellationToken cancellationToken) =>
            await Send(command.AsBaseCommand(), cancellationToken);

        public async Task Send(CommandAck command, CancellationToken cancellationToken) =>
            await Send(command.AsBaseCommand(), cancellationToken);

        public async Task Send(CommandFlow command, CancellationToken cancellationToken) =>
            await Send(command.AsBaseCommand(), cancellationToken);

        public async Task<BaseCommand> Send(CommandUnsubscribe command, CancellationToken cancellationToken)
        {
            Task<BaseCommand>? responseTask = null;

            using (await _lock.Lock(cancellationToken))
            {
                var baseCommand = command.AsBaseCommand();
                responseTask = _requestResponseHandler.Outgoing(baseCommand);
                _channelManager.Outgoing(command, responseTask);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence);
            }

            return await responseTask;
        }

        public async Task<BaseCommand> Send(CommandConnect command, CancellationToken cancellationToken) =>
            await SendRequestResponse(command.AsBaseCommand(), cancellationToken);

        public async Task<BaseCommand> Send(CommandLookupTopic command, CancellationToken cancellationToken) =>
            await SendRequestResponse(command.AsBaseCommand(), cancellationToken);

        public async Task<BaseCommand> Send(CommandSeek command, CancellationToken cancellationToken) =>
            await SendRequestResponse(command.AsBaseCommand(), cancellationToken);

        public async Task<BaseCommand> Send(CommandGetLastMessageId command, CancellationToken cancellationToken) =>
            await SendRequestResponse(command.AsBaseCommand(), cancellationToken);

        public async Task<BaseCommand> Send(CommandCloseProducer command, CancellationToken cancellationToken)
        {
            Task<BaseCommand>? responseTask = null;

            using (await _lock.Lock(cancellationToken))
            {
                var baseCommand = command.AsBaseCommand();
                responseTask = _requestResponseHandler.Outgoing(baseCommand);
                _channelManager.Outgoing(command, responseTask);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence);
            }

            return await responseTask;
        }

        public async Task<BaseCommand> Send(CommandCloseConsumer command, CancellationToken cancellationToken)
        {
            Task<BaseCommand>? responseTask = null;

            using (await _lock.Lock(cancellationToken))
            {
                var baseCommand = command.AsBaseCommand();
                responseTask = _requestResponseHandler.Outgoing(baseCommand);
                _channelManager.Outgoing(command, responseTask);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence);
            }

            return await responseTask;
        }

        public async Task<BaseCommand> Send(SendPackage command, CancellationToken cancellationToken)
        {
            Task<BaseCommand>? response = null;
            using (await _lock.Lock(cancellationToken))
            {
                var baseCommand = command.Command.AsBaseCommand();
                response = _requestResponseHandler.Outgoing(baseCommand);
                var sequence = Serializer.Serialize(baseCommand, command.Metadata, command.Payload);
                await _stream.Send(sequence);
            }
            return await response;
        }

        private async Task<BaseCommand> SendRequestResponse(BaseCommand command, CancellationToken cancellationToken)
        {
            Task<BaseCommand>? response = null;
            using (await _lock.Lock(cancellationToken))
            {
                response = _requestResponseHandler.Outgoing(command);
                var sequence = Serializer.Serialize(command);
                await _stream.Send(sequence);
            }
            return await response;
        }

        private async Task Send(BaseCommand command, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                var sequence = Serializer.Serialize(command);
                await _stream.Send(sequence);
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
            await _lock.DisposeAsync();
            _requestResponseHandler.Dispose();
            _channelManager.Dispose();
            await _stream.DisposeAsync();
        }
    }
}
