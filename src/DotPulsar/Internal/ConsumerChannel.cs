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
    using Extensions;
    using PulsarApi;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ConsumerChannel : IConsumerChannel, IReaderChannel
    {
        private readonly ulong _id;
        private readonly AsyncQueue<MessagePackage> _queue;
        private readonly IConnection _connection;
        private readonly BatchHandler _batchHandler;
        private readonly CommandFlow _cachedCommandFirstFlow;
        private readonly CommandFlow _cachedCommandOtherFlow;
        private readonly AsyncLock _lock;
        private uint _sendWhenZero;
        private bool _firstFlow;

        public ConsumerChannel(
            ulong id,
            uint messagePrefetchCount,
            AsyncQueue<MessagePackage> queue,
            IConnection connection,
            BatchHandler batchHandler)
        {
            _id = id;
            _queue = queue;
            _connection = connection;
            _batchHandler = batchHandler;

            _lock = new AsyncLock();

            _cachedCommandFirstFlow = new CommandFlow { ConsumerId = id, MessagePermits = messagePrefetchCount };
            _cachedCommandOtherFlow = new CommandFlow { ConsumerId = id, MessagePermits = (uint) Math.Ceiling(messagePrefetchCount * 0.5) };

            _sendWhenZero = 0;
            _firstFlow = true;
        }

        public async ValueTask<Message> Receive(CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                while (true)
                {
                    if (_sendWhenZero == 0)
                        await SendFlow(cancellationToken).ConfigureAwait(false);

                    var message = _batchHandler.GetNext();

                    if (message is not null)
                    {
                        if (message.MessageId.BatchIndex + 1 == message.NumMessagesInBatch)
                        {
                            // only decrease flow counter after all sub-messages in a batch were dequeued

                            _sendWhenZero--;
                        }

                        return message;
                    }

                    // here we are dequeing normal message so we need to decrease flow counter

                    _sendWhenZero--;

                    var messagePackage = await _queue.Dequeue(cancellationToken).ConfigureAwait(false);

                    if (!messagePackage.IsValid())
                    {
                        await RejectPackage(messagePackage, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    var metadataSize = messagePackage.GetMetadataSize();
                    var redeliveryCount = messagePackage.RedeliveryCount;
                    var data = messagePackage.ExtractData(metadataSize);
                    var metadata = messagePackage.ExtractMetadata(metadataSize);
                    var messageId = messagePackage.MessageId;

                    return metadata.ShouldSerializeNumMessagesInBatch()
                        ? _batchHandler.Add(messageId, redeliveryCount, metadata, data)
                        : MessageFactory.Create(new MessageId(messageId), redeliveryCount, metadata, data);
                }
            }
        }

        public async Task Send(CommandAck command, CancellationToken cancellationToken)
        {
            var messageId = command.MessageIds[0];

            if (messageId.BatchIndex != -1)
            {
                var batchMessageId = _batchHandler.Acknowledge(messageId);

                if (batchMessageId is null)
                    return;

                command.MessageIds[0] = batchMessageId;
            }

            command.ConsumerId = _id;
            await _connection.Send(command, cancellationToken).ConfigureAwait(false);
        }

        public async Task Send(CommandRedeliverUnacknowledgedMessages command, CancellationToken cancellationToken)
        {
            command.ConsumerId = _id;
            await _connection.Send(command, cancellationToken).ConfigureAwait(false);
        }

        public async Task<CommandSuccess> Send(CommandUnsubscribe command, CancellationToken cancellationToken)
        {
            command.ConsumerId = _id;
            var response = await _connection.Send(command, cancellationToken).ConfigureAwait(false);
            response.Expect(BaseCommand.Type.Success);
            return response.Success;
        }

        public async Task<CommandSuccess> Send(CommandSeek command, CancellationToken cancellationToken)
        {
            command.ConsumerId = _id;
            var response = await _connection.Send(command, cancellationToken).ConfigureAwait(false);
            response.Expect(BaseCommand.Type.Success);
            _batchHandler.Clear();
            return response.Success;
        }

        public async Task<CommandGetLastMessageIdResponse> Send(CommandGetLastMessageId command, CancellationToken cancellationToken)
        {
            command.ConsumerId = _id;
            var response = await _connection.Send(command, cancellationToken).ConfigureAwait(false);
            response.Expect(BaseCommand.Type.GetLastMessageIdResponse);
            return response.GetLastMessageIdResponse;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                _queue.Dispose();
                await _lock.DisposeAsync();
                var closeConsumer = new CommandCloseConsumer { ConsumerId = _id };
                await _connection.Send(closeConsumer, CancellationToken.None).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }
        }

        public async Task UpdateMessagePrefetchCount(uint messagePrefetchCount, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                _cachedCommandFirstFlow.MessagePermits = messagePrefetchCount;
                _cachedCommandOtherFlow.MessagePermits = (uint) Math.Ceiling(messagePrefetchCount * 0.5);
            }
        }

        private async ValueTask SendFlow(CancellationToken cancellationToken)
        {
            //TODO Should sending the flow command be handled on another thread and thereby not slow down the consumer?

            if (_firstFlow)
            {
                await _connection.Send(_cachedCommandFirstFlow, cancellationToken).ConfigureAwait(false);
                _firstFlow = false;
            }
            else
            {
                await _connection.Send(_cachedCommandOtherFlow, cancellationToken).ConfigureAwait(false);
            }

            _sendWhenZero = _cachedCommandOtherFlow.MessagePermits;
        }

        private async Task RejectPackage(MessagePackage messagePackage, CancellationToken cancellationToken)
        {
            var ack = new CommandAck { Type = CommandAck.AckType.Individual, ValidationError = CommandAck.ValidationErrorType.ChecksumMismatch };

            ack.MessageIds.Add(messagePackage.MessageId);

            await Send(ack, cancellationToken).ConfigureAwait(false);
        }
    }
}
