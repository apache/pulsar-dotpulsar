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
    using DotPulsar.Abstractions;
    using Extensions;
    using PulsarApi;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class ConsumerChannel : IConsumerChannel, IReaderChannel
    {
        private readonly ulong _id;
        private readonly AsyncQueue<MessagePackage> _queue;
        private readonly IConnection _connection;
        private readonly BatchHandler _batchHandler;
        private CommandFlow _cachedCommandFirstFlow;
        private CommandFlow _cachedCommandOtherFlow;
        private readonly AsyncLock _lock;
        private uint _sendWhenZero;
        private bool _firstFlow;

        private readonly TimeSpan _negativeAcknowledgeRedeliveryDelay;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ConcurrentDictionary<MessageId, DateTime> _negativelyAcknowledgedMessageIds = new ConcurrentDictionary<MessageId, DateTime>();

        private readonly IPulsarClientLogger? _logger;

        public ConsumerChannel(
            ulong id,
            uint messagePrefetchCount,
            AsyncQueue<MessagePackage> queue,
            IConnection connection,
            BatchHandler batchHandler,
            TimeSpan negativeAcknowledgeRedeliveryDelay,
            IPulsarClientLogger? logger)
        {
            _id = id;
            _queue = queue;
            _logger = logger;
            _connection = connection;
            _batchHandler = batchHandler;
            _negativeAcknowledgeRedeliveryDelay = negativeAcknowledgeRedeliveryDelay;

            _lock = new AsyncLock();

            _cachedCommandFirstFlow = new CommandFlow { ConsumerId = _id, MessagePermits = messagePrefetchCount };
            _cachedCommandOtherFlow = new CommandFlow { ConsumerId = _id, MessagePermits = (uint) Math.Max(Math.Ceiling(messagePrefetchCount * 0.5), 1.0) };

            _sendWhenZero = 0;
            _firstFlow = true;

            if (_negativeAcknowledgeRedeliveryDelay > TimeSpan.Zero)
            {
                var _ = Task.Run(() => RedeliverNegativelyAcknowledgedMessages(_cancellationTokenSource.Token));
            }
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
                _cancellationTokenSource.Dispose();
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

        public void UpdateMessagePrefetchCount(uint messagePrefetchCount, CancellationToken cancellationToken)
        {
            var newCommandFirstFlow = new CommandFlow { ConsumerId = _id, MessagePermits = messagePrefetchCount };
            var newCommandOtherFlow = new CommandFlow { ConsumerId = _id, MessagePermits = (uint) Math.Max(Math.Ceiling(messagePrefetchCount * 0.5), 1.0) };

            _ = Interlocked.Exchange(ref _cachedCommandFirstFlow, newCommandFirstFlow);
            _ = Interlocked.Exchange(ref _cachedCommandOtherFlow, newCommandOtherFlow);
        }

        private async ValueTask SendFlow(CancellationToken cancellationToken)
        {
            //TODO Should sending the flow command be handled on another thread and thereby not slow down the consumer?
            var localFlowReference = _cachedCommandOtherFlow;
            if (_firstFlow)
            {
                await _connection.Send(_cachedCommandFirstFlow, cancellationToken).ConfigureAwait(false);
                _firstFlow = false;
            }
            else
            {
                await _connection.Send(localFlowReference, cancellationToken).ConfigureAwait(false);
            }

            _sendWhenZero = localFlowReference.MessagePermits;
        }

        private async Task RejectPackage(MessagePackage messagePackage, CancellationToken cancellationToken)
        {
            var ack = new CommandAck { Type = CommandAck.AckType.Individual, ValidationError = CommandAck.ValidationErrorType.ChecksumMismatch };

            ack.MessageIds.Add(messagePackage.MessageId);

            await Send(ack, cancellationToken).ConfigureAwait(false);
        }

        public ValueTask NegativeAcknowledge(MessageId messageId, CancellationToken cancellationToken = default)
        {
            if (messageId is null)
            {
                throw new ArgumentNullException(nameof(messageId));
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                if (this._negativeAcknowledgeRedeliveryDelay == TimeSpan.Zero)
                {
                    return RedeliverUnacknowledgedMessages(new MessageId[] { messageId }, cancellationToken);
                }
                else
                {
                    DateTime redeliverDateTime = DateTime.UtcNow + this._negativeAcknowledgeRedeliveryDelay;
                    this._negativelyAcknowledgedMessageIds.AddOrUpdate(messageId, redeliverDateTime, (x, y) => redeliverDateTime);
                }
            }

            return new ValueTask();
        }

        internal async Task RedeliverNegativelyAcknowledgedMessages(CancellationToken cancellationToken)
        {
            // we need to keep track of negatively acknowledged messages in the channel as when channel is closed
            // and we reconnect, all unacknowledged and negatively acknowledged messages will be redelivered by Pulsar
            // (that is we need to forget all requests to redeliver messages during disconnect, as we don't want duplicate deliveries)

            List<KeyValuePair<MessageId, DateTime>> messageIdsToRedeliver = new List<KeyValuePair<MessageId, DateTime>>();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    messageIdsToRedeliver.Clear();

                    DateTime evaluationDateTime = DateTime.UtcNow;

                    // it is important to remove message ids which will be redelivered from _negativelyAcknowledgedMessageIds before requesting redelivery as messages
                    // can arrive after RedeliverUnacknowledgedMessages call succeeded but before we have a chance to remove message ids from _negativelyAcknowledgedMessageIds
                    // in this case message might be processed again, negatively acknowledged, and after this we will delete it from _negativelyAcknowledgedMessageIds resulting
                    // in message which won't be redelivered and will be stuck until consumer restarts

                    foreach (KeyValuePair<MessageId, DateTime> data in _negativelyAcknowledgedMessageIds)
                    {
                        if (data.Value <= evaluationDateTime &&
                            _negativelyAcknowledgedMessageIds.TryRemove(data.Key, out DateTime redeliverDateTime))
                        {
                            // we don't case if time changed to later (double negative acknowledgment) as original request date / time check passed and we are going to resend this
                            // message now.

                            messageIdsToRedeliver.Add(new KeyValuePair<MessageId, DateTime>(data.Key, redeliverDateTime));
                        }
                    }

                    if (messageIdsToRedeliver.Count > 0)
                    {
                        try
                        {
                            // RedeliverUnacknowledgedMessages or Acknowledge both call Send(...) which is single threaded, so we have two posibilities:
                            // 1. RedeliverUnacknowledgedMessages is called before Acknowledge -> can result in redelivery of message which is acknowledged before being processed again
                            //                                                                    I think we can assume one message should not be 1st negativelyAcknowledged and then
                            //                                                                    acknowledged.
                            // 2. Acknowledge is called before RedeliverUnacknowledgedMessages -> messages will never be redelivered
                            await RedeliverUnacknowledgedMessages(messageIdsToRedeliver.Select(x => x.Key), cancellationToken);
                            messageIdsToRedeliver.Clear();
                        }
                        catch (Exception e)
                        {
                            _logger.DebugException(nameof(Consumer), nameof(RedeliverNegativelyAcknowledgedMessages), e, "Failed to redeliver unacknowledged messages");
                        }

                        // make sure to place all the message ids to redeliver back to _negativelyAcknowledgedMessageIds
                        // in case we failed to request redelivery

                        foreach (KeyValuePair<MessageId, DateTime> data in messageIdsToRedeliver)
                        {
                            // we use current time to re-deliver messages as previous attempt failed

                            _negativelyAcknowledgedMessageIds.AddOrUpdate(data.Key, data.Value, (messagedId, redeliverDateTime) => evaluationDateTime);
                        }
                    }

                    await Task.Delay(_negativeAcknowledgeRedeliveryDelay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    _logger.ErrorException(nameof(Consumer), nameof(RedeliverNegativelyAcknowledgedMessages), e, "Unexpected exception in RedeliverNegativelyAcknowledgedMessages task");
                }
            }
        }

        private async ValueTask RedeliverUnacknowledgedMessages(IEnumerable<MessageId> messageIds, CancellationToken cancellationToken)
        {
            var redeliverUnacknowledgedMessages = new CommandRedeliverUnacknowledgedMessages();
            redeliverUnacknowledgedMessages.MessageIds.AddRange(messageIds.Select(m => m.Data).ToList());
            await Send(redeliverUnacknowledgedMessages, cancellationToken);
        }
    }
}
