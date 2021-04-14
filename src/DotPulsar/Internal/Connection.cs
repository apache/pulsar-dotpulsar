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

namespace DotPulsar.Internal
{
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
        internal readonly DateTime CreatedOn;
        public Guid Id { get; }

        private readonly AsyncLock _lock;
        private readonly ChannelManager _channelManager;
        private readonly RequestResponseHandler _requestResponseHandler;
        private readonly PingPongHandler _pingPongHandler;
        private readonly IPulsarStream _stream;
        private readonly IPulsarClientLogger? _logger;
        private int _isDisposed;

        public Connection(IPulsarStream stream, int commandTimeoutMs, IPulsarClientLogger? logger)
        {
            Id = Guid.NewGuid();
            CreatedOn = DateTime.UtcNow;
            _lock = new AsyncLock();
            _channelManager = new ChannelManager();
            _requestResponseHandler = new RequestResponseHandler(commandTimeoutMs, logger, Id);
            _pingPongHandler = new PingPongHandler(this, logger);
            _stream = stream;
            _logger = logger;
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

            Task<SubscribeResponse>? responseTask;

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
                var baseCommand = command.AsBaseCommand();
                responseTask = _requestResponseHandler.Outgoing(baseCommand);
                _channelManager.Outgoing(command, responseTask);
                var sequence = Serializer.Serialize(baseCommand);
                await _stream.Send(sequence).ConfigureAwait(false);
            }

            return await responseTask.ConfigureAwait(false);
        }

        // Special case connect commands as outside the command timeout
        public Task<BaseCommand> Send(CommandConnect command, CancellationToken cancellationToken)
            => SendRequestResponse(command.AsBaseCommand(), cancellationToken, Timeout.Infinite);

        public Task<BaseCommand> Send(CommandPartitionedTopicMetadata command, CancellationToken cancellationToken)
            => SendRequestResponse(command.AsBaseCommand(), cancellationToken);

        public Task<BaseCommand> Send(CommandLookupTopic command, CancellationToken cancellationToken)
            => SendRequestResponse(command.AsBaseCommand(), cancellationToken);

        public Task<BaseCommand> Send(CommandSeek command, CancellationToken cancellationToken)
            => SendRequestResponse(command.AsBaseCommand(), cancellationToken);

        public Task<BaseCommand> Send(CommandGetLastMessageId command, CancellationToken cancellationToken)
            => SendRequestResponse(command.AsBaseCommand(), cancellationToken);

        public async Task<BaseCommand> Send(CommandCloseProducer command, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            Task<BaseCommand>? responseTask;

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

            Task<BaseCommand>? responseTask;

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

            if (command.Command is null)
                throw new ArgumentNullException(nameof(command.Command));

            if (command.Metadata is null)
                throw new ArgumentNullException(nameof(command.Metadata));

            var baseCommand = command.Command.AsBaseCommand();
            var response = _requestResponseHandler.Outgoing(baseCommand);;
            var sequence = Serializer.Serialize(baseCommand, command.Metadata, command.Payload);

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                // By the time we enter this lock, the broker may have closed our producer
                if (!response.IsFaulted)
                {
                    await _stream.Send(sequence).ConfigureAwait(false);
                }
            }

            return await response.ConfigureAwait(false);
        }

        private async Task<BaseCommand> SendRequestResponse(BaseCommand command, CancellationToken cancellationToken, int? timeoutOverrideMs = null)
        {
            ThrowIfDisposed();

            Task<BaseCommand>? response;

            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                response = _requestResponseHandler.Outgoing(command, timeoutOverrideMs);
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
                            _channelManager.Incoming(command.Message, new ReadOnlySequence<byte>(frame.Slice(commandSize + 4).ToArray()));
                            break;
                        case BaseCommand.Type.CloseConsumer:
                            _channelManager.Incoming(command.CloseConsumer);
                            _logger.Debug(nameof(Connection), nameof(ProcessIncommingFrames), "Received CloseConsumer command from broker on {0} for consumer {1}", Id, command.CloseConsumer.ConsumerId);
                            break;
                        case BaseCommand.Type.ActiveConsumerChange:
                            _channelManager.Incoming(command.ActiveConsumerChange);
                            break;
                        case BaseCommand.Type.ReachedEndOfTopic:
                            _channelManager.Incoming(command.ReachedEndOfTopic);
                            break;
                        case BaseCommand.Type.CloseProducer:
                            _channelManager.Incoming(command.CloseProducer);
                            _logger.Debug(nameof(Connection), nameof(ProcessIncommingFrames), "Received CloseProducer command from broker on {0} for producer {1}", Id, command.CloseProducer.ProducerId);
                            // We need to fault all outstanding sends for this producer now also, or else they will hang forever
                            FaultAllOutstandingSendsForProducer(command.CloseProducer.ProducerId, new ServiceNotReadyException("Broker has closed the producer."));
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
            catch (Exception ex)
            {
                // ignored
                _logger.InfoException(nameof(Connection), nameof(ProcessIncommingFrames), ex, "Caught exception during incoming message processing loop for connection {0}", Id);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            _logger.Debug(nameof(Connection), nameof(DisposeAsync), "Disposing of connection {0}", Id);

            await _lock.DisposeAsync().ConfigureAwait(false);
            _requestResponseHandler.Dispose();
            _channelManager.Dispose();
            await _stream.DisposeAsync().ConfigureAwait(false);
        }

        private void FaultAllOutstandingSendsForProducer(ulong producerId, Exception exceptionToRelay)
        {
            _requestResponseHandler.FaultAllOutstandingSendsForProducer(producerId, exceptionToRelay);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new ConnectionDisposedException();
        }
    }
}
