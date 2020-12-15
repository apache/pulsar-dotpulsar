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
        private readonly ObjectPool<SendPackage> _sendPackagePool;
        private readonly ulong _id;
        private readonly string _name;
        private readonly IConnection _connection;

        public ProducerChannel(ulong id, string name, IConnection connection)
        {
            var sendPackagePolicy = new DefaultPooledObjectPolicy<SendPackage>();
            _sendPackagePool = new DefaultObjectPool<SendPackage>(sendPackagePolicy);
            _id = id;
            _name = name;
            _connection = connection;
        }

        public async ValueTask ClosedByClient(CancellationToken cancellationToken)
        {
            try
            {
                var closeProducer = new CommandCloseProducer { ProducerId = _id };
                await _connection.Send(closeProducer, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Ignore
            }
        }

        public ValueTask DisposeAsync() => new ValueTask();

        public async Task<CommandSendReceipt> Send(MessageMetadata metadata, ReadOnlySequence<byte> payload, CancellationToken cancellationToken)
        {
            var sendPackage = _sendPackagePool.Get();

            try
            {
                metadata.PublishTime = (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                metadata.ProducerName = _name;

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
