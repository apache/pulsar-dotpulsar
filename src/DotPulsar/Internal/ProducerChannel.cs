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
    using DotPulsar.Internal.PulsarApi;
    using Extensions;
    using Microsoft.Extensions.ObjectPool;
    using PulsarApi;
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ProducerChannel : IProducerChannel
    {
        private readonly ObjectPool<MessageMetadata> _messageMetadataPool;
        private readonly ObjectPool<SendPackage> _sendPackagePool;
        private readonly ulong _id;
        private readonly string _name;
        private readonly IConnection _connection;

        public ProducerChannel(ulong id, string name, IConnection connection)
        {
            var messageMetadataPolicy = new DefaultPooledObjectPolicy<MessageMetadata>();
            _messageMetadataPool = new DefaultObjectPool<MessageMetadata>(messageMetadataPolicy);
            var sendPackagePolicy = new DefaultPooledObjectPolicy<SendPackage>();
            _sendPackagePool = new DefaultObjectPool<SendPackage>(sendPackagePolicy);
            _id = id;
            _name = name;
            _connection = connection;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                var closeProducer = new CommandCloseProducer { ProducerId = _id };
                await _connection.Send(closeProducer, CancellationToken.None).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }
        }

        public Task<CommandSendReceipt> Send(ulong sequenceId, ReadOnlySequence<byte> payload, CancellationToken cancellationToken)
        {
            var metadata = _messageMetadataPool.Get();
            metadata.ProducerName = _name;
            metadata.SequenceId = sequenceId;
            try
            {
                return SendPackage(metadata, payload, cancellationToken);
            }
            finally
            {
                _messageMetadataPool.Return(metadata);
            }
        }

        public Task<CommandSendReceipt> Send(MessageMetadata metadata, ReadOnlySequence<byte> payload, CancellationToken cancellationToken)
        {
            metadata.ProducerName = _name;
            return SendPackage(metadata, payload, cancellationToken);
        }

        public async Task<CommandSendReceipt> SendBatchPackage(MessageMetadata metadata, Queue<(SingleMessageMetadata, ReadOnlySequence<byte>)> messages, CancellationToken cancellationToken)
        {
            var batchPackage = new BatchPackage();

            batchPackage.Command = new CommandSend
            {
                ProducerId = _id,
                NumMessages = messages.Count
            };
            batchPackage.Metadata = metadata;
            batchPackage.Messages = messages;

            batchPackage.Command.SequenceId = metadata.SequenceId;
            metadata.PublishTime = (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            try
            {
                var response = await _connection.Send(batchPackage, cancellationToken).ConfigureAwait(false);
                response.Expect(BaseCommand.Type.SendReceipt);
                return response.SendReceipt;
            }
        }

        private async Task<CommandSendReceipt> SendPackage(
            MessageMetadata metadata,
            ReadOnlySequence<byte> payload,
            CancellationToken cancellationToken)
        {
            var sendPackage = _sendPackagePool.Get();

            if (sendPackage.Command is null)
            {
                sendPackage.Command = new CommandSend
                {
                    ProducerId = _id,
                    NumMessages = 1
                };
            }

            sendPackage.Command.SequenceId = metadata.SequenceId;
            sendPackage.Metadata = metadata;
            sendPackage.Payload = payload;
            metadata.PublishTime = (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // TODO Benchmark against StopWatch

            try
            {
                var response = await _connection.Send(sendPackage, cancellationToken).ConfigureAwait(false);
                response.Expect(BaseCommand.Type.SendReceipt);
                return response.SendReceipt;
            }
            finally
            {
                _sendPackagePool.Return(sendPackage);
            }
        }

    }
}
