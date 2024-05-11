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
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;

public sealed class ProducerChannelFactory : IProducerChannelFactory
{
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private readonly IConnectionPool _connectionPool;
    private readonly CommandProducer _commandProducer;
    private readonly ICompressorFactory? _compressorFactory;
    private readonly IEncryptorFactory _encryptorFactory;
    private readonly Schema? _schema;
    private ulong? _topicEpoch;

    public ProducerChannelFactory(Guid correlationId,
        IRegisterEvent eventRegister,
        IConnectionPool connectionPool,
        string topic,
        string? producerName,
        ProducerAccessMode producerAccessMode,
        SchemaInfo schemaInfo,
        ICompressorFactory? compressorFactory,
        IEncryptorFactory encryptorFactory)
    {
        _correlationId = correlationId;
        _eventRegister = eventRegister;
        _connectionPool = connectionPool;

        _commandProducer = new CommandProducer
        {
            ProducerName = producerName,
            ProducerAccessMode = producerAccessMode,
            Topic = topic
        };

        _compressorFactory = compressorFactory;
        _encryptorFactory = encryptorFactory;
        _schema = schemaInfo.PulsarSchema;
    }

    public async Task<IProducerChannel> Create(CancellationToken cancellationToken)
    {
        if (_topicEpoch.HasValue)
        {
            if (_commandProducer.ProducerAccessMode != ProducerAccessMode.Shared)
                _commandProducer.ProducerAccessMode = ProducerAccessMode.Exclusive;
            _commandProducer.TopicEpoch = _topicEpoch.Value;
        }
        else
            _commandProducer.ResetTopicEpoch();

        var connection = await _connectionPool.FindConnectionForTopic(_commandProducer.Topic, cancellationToken).ConfigureAwait(false);
        var channel = new Channel(_correlationId, _eventRegister, new AsyncQueue<MessagePackage>());
        var response = await connection.Send(_commandProducer, channel, cancellationToken).ConfigureAwait(false);
        _topicEpoch = response.TopicEpoch;
        var schemaVersion = await GetSchemaVersion(connection, cancellationToken).ConfigureAwait(false);
        return new ProducerChannel(response.ProducerId, response.ProducerName, connection, _compressorFactory, _encryptorFactory, schemaVersion);
    }

    private async ValueTask<byte[]?> GetSchemaVersion(IConnection connection, CancellationToken cancellationToken)
    {
        if (_schema is null || _schema.Type == Schema.SchemaType.None)
            return null;

        var command = new CommandGetOrCreateSchema
        {
            Schema = _schema,
            Topic = _commandProducer.Topic
        };

        var response = await connection.Send(command, cancellationToken).ConfigureAwait(false);

        response.Expect(BaseCommand.Type.GetOrCreateSchemaResponse);
        if (response.GetOrCreateSchemaResponse.ShouldSerializeErrorCode())
            response.GetOrCreateSchemaResponse.Throw();

        return response.GetOrCreateSchemaResponse.SchemaVersion;
    }
}
