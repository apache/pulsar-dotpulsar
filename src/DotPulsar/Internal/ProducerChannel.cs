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

namespace DotPulsar.Internal;

using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using Microsoft.Extensions.ObjectPool;
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
    private readonly ICompressorFactory? _compressorFactory;
    private readonly byte[]? _schemaVersion;

    public ProducerChannel(
        ulong id,
        string name,
        IConnection connection,
        ICompressorFactory? compressorFactory,
        byte[]? schemaVersion)
    {
        var sendPackagePolicy = new DefaultPooledObjectPolicy<SendPackage>();
        _sendPackagePool = new DefaultObjectPool<SendPackage>(sendPackagePolicy);
        _id = id;
        _name = name;
        _connection = connection;
        _compressorFactory = compressorFactory;
        _schemaVersion = schemaVersion;
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

    public ValueTask DisposeAsync() => new();

    public async Task Send(MessageMetadata metadata, ReadOnlySequence<byte> payload, TaskCompletionSource<BaseCommand> responseTcs,
        CancellationToken cancellationToken)
    {
        var sendPackage = _sendPackagePool.Get();

        try
        {
            metadata.PublishTime = (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            metadata.ProducerName = _name;

            if (metadata.SchemaVersion is null && _schemaVersion is not null)
                metadata.SchemaVersion = _schemaVersion;

            sendPackage.Command ??= new CommandSend { ProducerId = _id, NumMessages = 1 };

            sendPackage.Command.SequenceId = metadata.SequenceId;
            sendPackage.Metadata = metadata;

            if (_compressorFactory is null)
                sendPackage.Payload = payload;
            else
            {
                sendPackage.Metadata.Compression = _compressorFactory.CompressionType;
                sendPackage.Metadata.UncompressedSize = (uint) payload.Length;
                using var compressor = _compressorFactory.Create();
                sendPackage.Payload = compressor.Compress(payload);
            }

            await _connection.Send(sendPackage, responseTcs, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _sendPackagePool.Return(sendPackage);
        }
    }
}
