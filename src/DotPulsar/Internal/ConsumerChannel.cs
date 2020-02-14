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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class ConsumerChannel : IConsumerChannel, IReaderChannel
    {
        private readonly ulong _id;
        private readonly AsyncQueue<MessagePackage> _queue;
        private readonly IConnection _connection;
        private readonly BatchHandler _batchHandler;
        private readonly CommandFlow _cachedCommandFlow;
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
            _cachedCommandFlow = new CommandFlow { ConsumerId = id, MessagePermits = messagePrefetchCount };
            _sendWhenZero = 0;
            _firstFlow = true;
        }

        public async ValueTask<Message> Receive(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (_sendWhenZero == 0)
                    await SendFlow();

                _sendWhenZero--;

                var message = _batchHandler.GetNext();
                if (message != null)
                    return message;

                var messagePackage = await _queue.Dequeue(cancellationToken);

                if (!await Validate(messagePackage))
                    continue;

                var messageId = messagePackage.MessageId;
                var data = messagePackage.Data;

                var metadataSize = data.ReadUInt32(6, true);
                var metadata = Serializer.Deserialize<PulsarApi.MessageMetadata>(data.Slice(10, metadataSize));
                data = data.Slice(10 + metadataSize);

                if (metadata.NumMessagesInBatch == 1)
                    return new Message(new MessageId(messageId), metadata, null, data);

                return _batchHandler.Add(messageId, metadata, data);
            }
        }

        public async Task Send(CommandAck command)
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
            await _connection.Send(command);
        }

        public async Task<CommandSuccess> Send(CommandUnsubscribe command)
        {
            command.ConsumerId = _id;
            var response = await _connection.Send(command);
            response.Expect(BaseCommand.Type.Success);
            return response.Success;
        }

        public async Task<CommandSuccess> Send(CommandSeek command)
        {
            command.ConsumerId = _id;
            var response = await _connection.Send(command);
            response.Expect(BaseCommand.Type.Success);
            _batchHandler.Clear();
            return response.Success;
        }

        public async Task<CommandGetLastMessageIdResponse> Send(CommandGetLastMessageId command)
        {
            command.ConsumerId = _id;
            var response = await _connection.Send(command);
            response.Expect(BaseCommand.Type.GetLastMessageIdResponse);
            return response.GetLastMessageIdResponse;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                _queue.Dispose();
                await _connection.Send(new CommandCloseConsumer { ConsumerId = _id });
            }
            catch
            {
                // Ignore
            }
        }

        private async ValueTask SendFlow()
        {
            await _connection.Send(_cachedCommandFlow); //TODO Should sending the flow command be handled on another thread and thereby not slow down the consumer?

            if (_firstFlow)
            {
                _cachedCommandFlow.MessagePermits = (uint)Math.Ceiling(_cachedCommandFlow.MessagePermits * 0.5);
                _firstFlow = false;
            }

            _sendWhenZero = _cachedCommandFlow.MessagePermits;
        }

        private async ValueTask<bool> Validate(MessagePackage messagePackage)
        {
            var magicNumberMatches = messagePackage.Data.StartsWith(Constants.MagicNumber);
            var expectedChecksum = messagePackage.Data.ReadUInt32(2, true);
            var actualChecksum = Crc32C.Calculate(messagePackage.Data.Slice(6));
            if (!magicNumberMatches || expectedChecksum != actualChecksum)
            {
                var ack = new CommandAck
                {
                    Type = CommandAck.AckType.Individual,
                    validation_error = CommandAck.ValidationError.ChecksumMismatch
                };
                ack.MessageIds.Add(messagePackage.MessageId);
                await Send(ack);
                return false;
            }

            return true;
        }
    }
}
