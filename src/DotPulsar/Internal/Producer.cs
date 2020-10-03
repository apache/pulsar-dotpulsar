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
    using DotPulsar.Exceptions;
    using Events;
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class Producer : IProducer
    {
        private readonly Guid _correlationId;
        private readonly IRegisterEvent _eventRegister;
        private IProducerChannel _channel;
        private readonly IExecute _executor;
        private readonly IStateChanged<ProducerState> _state;
        private readonly SequenceId _sequenceId;
        private int _isDisposed;
        private readonly ProducerOptions _options;
        private readonly IBatchMessageContainer? _batchMessageContainer;
        private readonly Awaiter<Message, MessageId> _batchMessageAwaiter = new Awaiter<Message, MessageId>();
        private readonly CancellationTokenSource _cancellationTokenSource;

        public string Topic { get; }

        public Producer(
            Guid correlationId,
            string topic,
            ulong initialSequenceId,
            IRegisterEvent registerEvent,
            IProducerChannel initialChannel,
            IExecute executor,
            IStateChanged<ProducerState> state,
            ProducerOptions options)
        {
            _correlationId = correlationId;
            Topic = topic;
            _sequenceId = new SequenceId(initialSequenceId);
            _eventRegister = registerEvent;
            _channel = initialChannel;
            _executor = executor;
            _state = state;
            _options = options;
            _isDisposed = 0;

            _cancellationTokenSource = new CancellationTokenSource();

            _eventRegister.Register(new ProducerCreated(_correlationId, this));

            if (_options.BatchingEnabled)
            {
                _batchMessageContainer = new BatchMessageContainer(_options.BatchingMaxMessagesPerBatch, options.BatchingMaxBytes);
                _ = BatchMessageAndSend(_cancellationTokenSource.Token);
            }
        }

        private async Task BatchMessageAndSend(CancellationToken cancellationToken = default)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(_options.BatchingMaxPublishDelay);
                    var (messages, metadata) = _batchMessageContainer!.GetBatchedMessagesAndMetadata();
                    if (messages == null || metadata == null) return;
                    await SendBatchedMessages(messages, metadata).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        async Task SendBatchedMessages(Queue<Message> messages, MessageMetadata metadata)
        {
            var payload = Serializer.Serialize(messages);
            var response = await _executor.Execute(() => _channel.Send(metadata.Metadata, payload, _cancellationTokenSource.Token), _cancellationTokenSource.Token).ConfigureAwait(false);
            var batchIndex = 0;
            foreach (var message in messages)
            {
                var messageId = new MessageId(response.MessageId.LedgerId, response.MessageId.EntryId, response.MessageId.Partition, batchIndex++);
                _batchMessageAwaiter.SetResult(message, messageId);
            }
        }

        private void DoBatchSendAndAdd(Message message)
        {
            var (messages, metadata) = _batchMessageContainer!.GetBatchedMessagesAndMetadata();
            if (messages != null && metadata != null)
                _ = SendBatchedMessages(messages, metadata);
            _batchMessageContainer!.Add(message);
        }

        public async ValueTask<ProducerStateChanged> StateChangedTo(ProducerState state, CancellationToken cancellationToken)
        {
            var newState = await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);
            return new ProducerStateChanged(this, newState);
        }

        public async ValueTask<ProducerStateChanged> StateChangedFrom(ProducerState state, CancellationToken cancellationToken)
        {
            var newState = await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);
            return new ProducerStateChanged(this, newState);
        }

        public bool IsFinalState()
            => _state.IsFinalState();

        public bool IsFinalState(ProducerState state)
            => _state.IsFinalState(state);

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            _eventRegister.Register(new ProducerDisposed(_correlationId, this));

            _batchMessageAwaiter.Dispose();

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            await _channel.DisposeAsync().ConfigureAwait(false);
        }

        public ValueTask<MessageId> Send(byte[] data, CancellationToken cancellationToken)
            => Send(new ReadOnlySequence<byte>(data), cancellationToken);

        public ValueTask<MessageId> Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            => Send(new ReadOnlySequence<byte>(data), cancellationToken);

        public async ValueTask<MessageId> Send(ReadOnlySequence<byte> data, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (_options.BatchingEnabled)
            {
                return await Send(new MessageMetadata(), data, cancellationToken);
            }
            var sequenceId = _sequenceId.FetchNext();
            var response = await _executor.Execute(() => _channel.Send(sequenceId, data, cancellationToken), cancellationToken).ConfigureAwait(false);
            return new MessageId(response.MessageId);
        }

        public ValueTask<MessageId> Send(MessageMetadata metadata, byte[] data, CancellationToken cancellationToken)
            => Send(metadata, new ReadOnlySequence<byte>(data), cancellationToken);

        public ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            => Send(metadata, new ReadOnlySequence<byte>(data), cancellationToken);

        public async ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlySequence<byte> data, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            var autoAssignSequenceId = metadata.SequenceId == 0;
            if (autoAssignSequenceId)
                metadata.SequenceId = _sequenceId.FetchNext();

            // If a message has a delayed delivery time, we'll always send it individually
            if (_options.BatchingEnabled && metadata.DeliverAtTime == 0)
            {
                var message = new Message(metadata.Metadata, data);
                if (_batchMessageContainer!.HaveEnoughSpace(message))
                {
                    // handle boundary cases where message being added would exceed
                    // batch size and/or max message size
                    var isFull = _batchMessageContainer.Add(message);
                    if (isFull)
                    {
                        BatchMessageAndSend();
                    }
                }
                else
                {
                    DoBatchSendAndAdd(message);
                }
                return await _batchMessageAwaiter.CreateTask(message).ConfigureAwait(false);
            }
            else
            {
                try
                {
                    var response = await _executor.Execute(() => _channel.Send(metadata.Metadata, data, cancellationToken), cancellationToken).ConfigureAwait(false);
                    return new MessageId(response.MessageId);
                }
                finally
                {
                    if (autoAssignSequenceId)
                        metadata.SequenceId = 0;
                }
            }
        }

        internal async ValueTask SetChannel(IProducerChannel channel)
        {
            if (_isDisposed != 0)
            {
                await channel.DisposeAsync().ConfigureAwait(false);
                return;
            }

            var oldChannel = _channel;
            _channel = channel;

            if (oldChannel != null)
                await oldChannel.DisposeAsync().ConfigureAwait(false);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed != 0)
                throw new ProducerDisposedException();
        }
    }
}
