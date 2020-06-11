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
    using System.Buffers;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ProducerChannel : IProducerChannel
    {
        private readonly MessageMetadata _cachedMetadata;
        private readonly ulong _id;
        private readonly SequenceId _sequenceId;
        private readonly IConnection _connection;

        public ProducerChannel(ulong id, string name, SequenceId sequenceId, IConnection connection)
        {
            _cachedMetadata = new MessageMetadata { ProducerName = name };

            var commandSend = new CommandSend { ProducerId = id, NumMessages = 1 };

            _id = id;
            _sequenceId = sequenceId;
            _connection = connection;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _connection.Send(new CommandCloseProducer { ProducerId = _id }, CancellationToken.None).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }
        }

        public Task<CommandSendReceipt> Send(ReadOnlySequence<byte> payload, CancellationToken cancellationToken)
        {
            var package = GetNewSendPackage(payload, new PulsarApi.MessageMetadata());
            return SendPackage(package, cancellationToken);
        }

        public Task<CommandSendReceipt> Send(MessageMetadata metadata, ReadOnlySequence<byte> payload, CancellationToken cancellationToken)
        {
            var package = GetNewSendPackage(payload, metadata);
            return SendPackage(package, cancellationToken);
        }

        private SendPackage GetNewSendPackage(ReadOnlySequence<byte> payload, PulsarApi.MessageMetadata metadata)
        {
            metadata.ProducerName = _cachedMetadata.ProducerName;
            var package = new SendPackage(new CommandSend() { ProducerId = _id, NumMessages = 1 }, metadata);
            package.Payload = payload;

            if (metadata.SequenceId == 0)
            {
                // Auto assign sequence id
                package.Metadata.SequenceId = _sequenceId.FetchNext();
            }

            package.Command.SequenceId = package.Metadata.SequenceId;
            return package;
        }

        private async Task<CommandSendReceipt> SendPackage(SendPackage sendPackage, CancellationToken cancellationToken)
        {
            sendPackage.Metadata.PublishTime = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var response = await _connection.Send(sendPackage, cancellationToken);
            response.Expect(BaseCommand.Type.SendReceipt);

            return response.SendReceipt;
        }
    }
}

