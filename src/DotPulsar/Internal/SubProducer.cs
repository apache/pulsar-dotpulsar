﻿/*
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

using Abstractions;
using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using Extensions;
using Events;
using Exceptions;
using PulsarApi;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

public sealed class SubProducer : IEstablishNewChannel, IState<ProducerState>
{
    private readonly SemaphoreSlim _newChannelLock;
    private readonly AsyncQueueWithCursor<SendOp> _sendQueue;
    private CancellationTokenSource? _dispatcherCts;
    private Task? _dispatcherTask;
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private IProducerChannel _channel;
    private readonly IExecute _executor;
    private readonly IStateChanged<ProducerState> _state;
    private readonly IProducerChannelFactory _factory;
    private int _isDisposed;

    public SubProducer(
        Guid correlationId,
        IRegisterEvent registerEvent,
        IProducerChannel initialChannel,
        IExecute executor,
        IStateChanged<ProducerState> state,
        IProducerChannelFactory factory,
        uint maxPendingMessages)
    {
        _newChannelLock = new SemaphoreSlim(1, 1);
        _sendQueue = new AsyncQueueWithCursor<SendOp>(maxPendingMessages);
        _correlationId = correlationId;
        _eventRegister = registerEvent;
        _channel = initialChannel;
        _executor = executor;
        _state = state;
        _factory = factory;
        _isDisposed = 0;

        _eventRegister.Register(new ProducerCreated(_correlationId));
    }

    public async ValueTask<ProducerState> OnStateChangeTo(ProducerState state, CancellationToken cancellationToken)
        => await _state.StateChangedTo(state, cancellationToken).ConfigureAwait(false);

    public async ValueTask<ProducerState> OnStateChangeFrom(ProducerState state, CancellationToken cancellationToken)
        => await _state.StateChangedFrom(state, cancellationToken).ConfigureAwait(false);

    public bool IsFinalState()
        => _state.IsFinalState();

    public bool IsFinalState(ProducerState state)
        => _state.IsFinalState(state);

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _eventRegister.Register(new ProducerDisposed(_correlationId));
        _newChannelLock.Dispose();
        _dispatcherCts?.Cancel();

        try
        {
            await (_dispatcherTask ?? Task.CompletedTask).ConfigureAwait(false);
        }
        catch
        {
            // Ignored
        }
        await _sendQueue.DisposeAsync().ConfigureAwait(false);
        await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
        await _channel.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask Send(MessageMetadata metadata, ReadOnlySequence<byte> data, TaskCompletionSource<MessageId> receiptTcs, CancellationToken cancellationToken)
    {
        if (IsFinalState()) throw new ProducerClosedException(); // TODO: This exception might be intended for other purposes.
        SendOp sendOp = new SendOp(metadata, data, receiptTcs);
        await _sendQueue.Enqueue(sendOp, cancellationToken).ConfigureAwait(false);
    }

    private async Task MessageDispatcher(IProducerChannel channel, CancellationToken cancellationToken)
    {
        var responseQueue = new AsyncQueue<Task<BaseCommand>>();
        var responseProcessorTask = ResponseProcessor(responseQueue, cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var tcs = new TaskCompletionSource<BaseCommand>();

            _ = tcs.Task.ContinueWith(task => responseQueue.Enqueue(task),
                TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously);

            SendOp sendOp = await _sendQueue.NextItem(cancellationToken).ConfigureAwait(false);

            // Use CancellationToken.None here because otherwise it will throw exceptions on all fault actions even retry.
            bool success = await _executor.TryExecuteOnce(() => channel.Send(sendOp.Metadata, sendOp.Data, tcs, cancellationToken), CancellationToken.None).ConfigureAwait(false);

            if (success) continue;
            _eventRegister.Register(new ChannelDisconnected(_correlationId));
            break;
        }

        await responseProcessorTask.ConfigureAwait(false);
    }

    private async ValueTask ResponseProcessor(IDequeue<Task<BaseCommand>> responseQueue, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var responseTask = await responseQueue.Dequeue(cancellationToken).ConfigureAwait(false);

            bool success = await _executor.TryExecuteOnce(() =>
            {
                if (responseTask.IsFaulted) throw responseTask.Exception!;
                responseTask.Result.Expect(BaseCommand.Type.SendReceipt);
                ProcessReceipt(responseTask.Result.SendReceipt);
            }, CancellationToken.None).ConfigureAwait(false); // Use CancellationToken.None here because otherwise it will throw exceptions on all fault actions even retry.

            // TODO: Should we crate a new event instead of channel disconnected?
            if (success) continue;
            _eventRegister.Register(new ChannelDisconnected(_correlationId));
            break;
        }
    }

    private void ProcessReceipt(CommandSendReceipt sendReceipt)
    {
        ulong receiptSequenceId = sendReceipt.SequenceId;

        if (!_sendQueue.TryPeek(out SendOp? sendOp) || sendOp is null)
            throw new ProducerSendReceiptOrderingException($"Received sequenceId {receiptSequenceId} but send queue is empty");

        ulong expectedSequenceId = sendOp.Metadata.SequenceId;

        if (receiptSequenceId != expectedSequenceId)
            throw new ProducerSendReceiptOrderingException($"Received sequenceId {receiptSequenceId}. Expected {expectedSequenceId}");

        _sendQueue.Dequeue();
        sendOp.ReceiptTcs.TrySetResult(sendReceipt.MessageId.ToMessageId());
    }

    public async Task EstablishNewChannel(CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Should more of this run through the executor for exception handling?
            await _newChannelLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            if (_dispatcherCts is not null && !_dispatcherCts.IsCancellationRequested)
            {
                _dispatcherCts.Cancel();
                _dispatcherCts.Dispose();
            }

            try
            {
                await (_dispatcherTask ?? Task.CompletedTask).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // Ignore
            }

            var oldChannel = _channel;
            // TODO: Not sure we need to actually close the channel?
            await oldChannel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
            // TODO: Why does IProducerChannel.DisposeAsync not do anything?
            await oldChannel.DisposeAsync().ConfigureAwait(false);

            _channel = await _executor.Execute(() => _factory.Create(cancellationToken), cancellationToken).ConfigureAwait(false);

            _sendQueue.ResetCursor();
            _dispatcherCts = new CancellationTokenSource();
            _dispatcherTask = MessageDispatcher(_channel, _dispatcherCts.Token);
        }
        catch (Exception)
        {
            // Ignored
        }
        finally
        {
            _newChannelLock.Release();
        }
    }

    private class SendOp
    {
        public SendOp(MessageMetadata metadata, ReadOnlySequence<byte> data, TaskCompletionSource<MessageId> receiptTcs)
        {
            Metadata = metadata;
            Data = data;
            ReceiptTcs = receiptTcs;
        }

        public MessageMetadata Metadata { get; }
        public ReadOnlySequence<byte> Data { get; }
        public TaskCompletionSource<MessageId> ReceiptTcs { get; }
    }
}
