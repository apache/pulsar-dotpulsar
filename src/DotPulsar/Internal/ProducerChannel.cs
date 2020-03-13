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
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class ProducerChannel : IProducerChannel
    {
        private readonly PulsarApi.MessageMetadata _cachedMetadata;
        private readonly SendPackage _cachedSendPackage;
        private readonly ulong _id;
        private readonly SequenceId _sequenceId;
        private readonly IConnection _connection;

        public ProducerChannel(ulong id, string name, SequenceId sequenceId, IConnection connection)
        {
            _cachedMetadata = new PulsarApi.MessageMetadata
            {
                ProducerName = name
            };

            var commandSend = new CommandSend
            {
                ProducerId = id,
                NumMessages = 1
            };

            _cachedSendPackage = new SendPackage(commandSend, _cachedMetadata);

            _id = id;
            _sequenceId = sequenceId;
            _connection = connection;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _connection.Send(new CommandCloseProducer { ProducerId = _id }, CancellationToken.None);
            }
            catch
            {
                // Ignore
            }
        }

        public async Task<CommandSendReceipt> Send(ReadOnlySequence<byte> payload, CancellationToken cancellationToken)
        {
            _cachedSendPackage.Metadata = _cachedMetadata;
            _cachedSendPackage.Payload = payload;
            return await SendPackage(true, cancellationToken);
        }

        public async Task<CommandSendReceipt> Send(PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> payload, CancellationToken cancellationToken)
        {
            metadata.ProducerName = _cachedMetadata.ProducerName;
            _cachedSendPackage.Metadata = metadata;
            _cachedSendPackage.Payload = payload;
            return await SendPackage(metadata.SequenceId == 0, cancellationToken);
        }

        private async Task<CommandSendReceipt> SendPackage(bool autoAssignSequenceId, CancellationToken cancellationToken)
        {
            try
            {
                _cachedSendPackage.Metadata.PublishTime = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                if (autoAssignSequenceId)
                {
                    _cachedSendPackage.Command.SequenceId = _sequenceId.Current;
                    _cachedSendPackage.Metadata.SequenceId = _sequenceId.Current;
                }
                else
                    _cachedSendPackage.Command.SequenceId = _cachedSendPackage.Metadata.SequenceId;

                var response = await _connection.Send(_cachedSendPackage, cancellationToken);
                response.Expect(BaseCommand.Type.SendReceipt);

                if (autoAssignSequenceId)
                    _sequenceId.Increment();

                return response.SendReceipt;
            }
            finally
            {
                if (autoAssignSequenceId)
                    _cachedSendPackage.Metadata.SequenceId = 0; // Reset in case the user reuse the MessageMetadata, but is not explicitly setting the sequenceId
            }
        }
    }
}
