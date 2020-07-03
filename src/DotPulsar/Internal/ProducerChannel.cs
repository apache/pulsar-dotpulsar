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
    using Microsoft.Extensions.ObjectPool;
    using PulsarApi;
    using System;
    using System.Buffers;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ProducerChannel : IProducerChannel
    {
        private readonly MessageMetadata _cachedMetadata;
        private readonly ObjectPool<SendPackage> _sendPackagePool;
        private readonly ulong _id;
        private readonly string _name;
        private readonly SequenceId _sequenceId;
        private readonly IConnection _connection;

        public ProducerChannel(ulong id, string name, SequenceId sequenceId, IConnection connection)
        {
            _cachedMetadata = new MessageMetadata { ProducerName = name };
            _sendPackagePool = new DefaultObjectPool<SendPackage>(new DefaultPooledObjectPolicy<SendPackage>());
            _id = id;
            _name = name;
            _sequenceId = sequenceId;
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

        public Task<CommandSendReceipt> Send(ReadOnlySequence<byte> payload, CancellationToken cancellationToken)
            => SendPackage(_cachedMetadata, payload, true, cancellationToken);

        public Task<CommandSendReceipt> Send(MessageMetadata metadata, ReadOnlySequence<byte> payload, CancellationToken cancellationToken)
        {
            metadata.ProducerName = _name;
            return SendPackage(metadata, payload, metadata.SequenceId == 0, cancellationToken);
        }

        private async Task<CommandSendReceipt> SendPackage(
            MessageMetadata metadata,
            ReadOnlySequence<byte> payload,
            bool autoAssignSequenceId,
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

            sendPackage.Metadata = metadata;
            sendPackage.Payload = payload;

            try
            {
                metadata.PublishTime = (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                if (autoAssignSequenceId)
                {
                    var newSequenceId = _sequenceId.FetchNext();
                    sendPackage.Command.SequenceId = newSequenceId;
                    sendPackage.Metadata.SequenceId = newSequenceId;
                }
                else
                    sendPackage.Command.SequenceId = sendPackage.Metadata.SequenceId;

                var response = await _connection.Send(sendPackage, cancellationToken).ConfigureAwait(false);
                response.Expect(BaseCommand.Type.SendReceipt);

                return response.SendReceipt;
            }
            finally
            {
                // Reset in case the user reuse the MessageMetadata, but is not explicitly setting the sequenceId
                if (autoAssignSequenceId)
                    sendPackage.Metadata.SequenceId = 0;

                _sendPackagePool.Return(sendPackage);
            }
        }
    }
}

