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

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;

public sealed class ConsumerChannel<TMessage> : IConsumerChannel<TMessage>
{
    private readonly ulong _id;
    private readonly AsyncQueue<MessagePackage> _queue;
    private readonly IConnection _connection;
    private readonly BatchHandler<TMessage> _batchHandler;
    private readonly CommandFlow _cachedCommandFlow;
    private readonly IMessageFactory<TMessage> _messageFactory;
    private readonly IDecompress?[] _decompressors;
    private readonly AsyncLock _lock;
    private readonly string _topic;
    private uint _sendWhenZero;
    private bool _firstFlow;

    public ConsumerChannel(
        ulong id,
        uint messagePrefetchCount,
        AsyncQueue<MessagePackage> queue,
        IConnection connection,
        BatchHandler<TMessage> batchHandler,
        IMessageFactory<TMessage> messageFactory,
        IEnumerable<IDecompressorFactory> decompressorFactories,
        string topic)
    {
        _id = id;
        _queue = queue;
        _connection = connection;
        _batchHandler = batchHandler;
        _messageFactory = messageFactory;
        _topic = topic;

        _decompressors = new IDecompress[5];

        foreach (var decompressorFactory in decompressorFactories)
        {
            _decompressors[(int) decompressorFactory.CompressionType] = decompressorFactory.Create();
        }

        _lock = new AsyncLock();

        _cachedCommandFlow = new CommandFlow
        {
            ConsumerId = id,
            MessagePermits = messagePrefetchCount
        };

        _sendWhenZero = 0;
        _firstFlow = true;
    }

    public async ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken)
    {
        using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
        {
            while (true)
            {
                if (_sendWhenZero == 0)
                    await SendFlow(cancellationToken).ConfigureAwait(false);

                _sendWhenZero--;
                try
                {
                    var message = _batchHandler.GetNext();

                    if (message is not null)
                        return message;

                    var messagePackage = await _queue.Dequeue(cancellationToken).ConfigureAwait(false);

                    if (!messagePackage.ValidateMagicNumberAndChecksum())
                    {
                        await RejectPackage(messagePackage, CommandAck.Types.ValidationError.ChecksumMismatch, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    var metadataSize = messagePackage.GetMetadataSize();
                    var metadata = messagePackage.ExtractMetadata(metadataSize);
                    var data = messagePackage.ExtractData(metadataSize);

                    if (metadata.Compression != CompressionType.None)
                    {
                        var decompressor = _decompressors[(int) metadata.Compression];
                        if (decompressor is null)
                            throw new CompressionException($"Support for {metadata.Compression} compression was not found");

                        try
                        {
                            data = decompressor.Decompress(data, (int) metadata.UncompressedSize);
                        }
                        catch
                        {
                            await RejectPackage(messagePackage, CommandAck.Types.ValidationError.DecompressionError, cancellationToken).ConfigureAwait(false);
                            continue;
                        }
                    }

                    var messageId = messagePackage.MessageId;
                    var redeliveryCount = messagePackage.RedeliveryCount;

                    if (metadata.HasNumMessagesInBatch)
                    {
                        try
                        {
                            return _batchHandler.Add(messageId, redeliveryCount, metadata, data);
                        }
                        catch
                        {
                            await RejectPackage(messagePackage, CommandAck.Types.ValidationError.BatchDeSerializeError, cancellationToken).ConfigureAwait(false);
                            continue;
                        }
                    }

                    return _messageFactory.Create(messageId.ToMessageId(_topic), redeliveryCount, data, metadata);
                }
                catch (Exception e) when (e is not CompressionException)
                {
                    // Undo decrementing since we didn't actually receive anything
                    _sendWhenZero++;
                    throw;
                }
            }
        }
    }

    public async Task Send(CommandAck command, CancellationToken cancellationToken)
    {
        var messageId = command.MessageId[0];

        if (messageId.BatchIndex != -1)
        {
            var batchMessageId = _batchHandler.Acknowledge(messageId);

            if (batchMessageId is null)
                return;

            command.MessageId[0] = batchMessageId;
        }

        command.ConsumerId = _id;
        await _connection.Send(command, cancellationToken).ConfigureAwait(false);
    }

    public async Task Send(CommandRedeliverUnacknowledgedMessages command, CancellationToken cancellationToken)
    {
        command.ConsumerId = _id;
        await _connection.Send(command, cancellationToken).ConfigureAwait(false);
    }

    public async Task Send(CommandUnsubscribe command, CancellationToken cancellationToken)
    {
        command.ConsumerId = _id;
        var response = await _connection.Send(command, cancellationToken).ConfigureAwait(false);
        response.Expect(BaseCommand.Types.Type.Success);
    }

    public async Task Send(CommandSeek command, CancellationToken cancellationToken)
    {
        command.ConsumerId = _id;
        var response = await _connection.Send(command, cancellationToken).ConfigureAwait(false);
        response.Expect(BaseCommand.Types.Type.Success);
        _batchHandler.Clear();
    }

    public async Task<MessageId> Send(CommandGetLastMessageId command, CancellationToken cancellationToken)
    {
        command.ConsumerId = _id;
        var response = await _connection.Send(command, cancellationToken).ConfigureAwait(false);
        response.Expect(BaseCommand.Types.Type.GetLastMessageIdResponse);
        var messageIdData = response.GetLastMessageIdResponse.LastMessageId;
        if (messageIdData.LedgerId == MessageId.Earliest.LedgerId &&
            messageIdData.EntryId == MessageId.Earliest.EntryId &&
            messageIdData.Partition == MessageId.Earliest.Partition &&
            messageIdData.BatchIndex == MessageId.Earliest.BatchIndex)
            return MessageId.Earliest;
        return messageIdData.ToMessageId(_topic);
    }

    public async ValueTask DisposeAsync()
    {
        _queue.Dispose();

        for (var i = 0; i < _decompressors.Length; ++i)
        {
            _decompressors[i]?.Dispose();
        }

        await _lock.DisposeAsync().ConfigureAwait(false);
    }

    private async ValueTask SendFlow(CancellationToken cancellationToken)
    {
        await _connection.Send(_cachedCommandFlow, cancellationToken).ConfigureAwait(false);

        if (_firstFlow)
        {
            _cachedCommandFlow.MessagePermits = (uint) Math.Ceiling(_cachedCommandFlow.MessagePermits * 0.5);
            _firstFlow = false;
        }

        _sendWhenZero = _cachedCommandFlow.MessagePermits;
    }

    private async Task RejectPackage(MessagePackage messagePackage, CommandAck.Types.ValidationError validationErrorType, CancellationToken cancellationToken)
    {
        var ack = new CommandAck
        {
            AckType = CommandAck.Types.AckType.Individual,
            ValidationError = validationErrorType
        };

        ack.MessageId.Add(messagePackage.MessageId);

        await Send(ack, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask ClosedByClient(CancellationToken cancellationToken)
    {
        try
        {
            var closeConsumer = new CommandCloseConsumer { ConsumerId = _id };
            await _connection.Send(closeConsumer, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // Ignore
        }
    }
}
