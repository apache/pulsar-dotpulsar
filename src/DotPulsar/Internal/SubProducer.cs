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
    private readonly AsyncQueueWithCursor<SendOp> _sendQueue;
    private CancellationTokenSource _dispatcherCts;
    private Task _dispatcherTask;
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
        IProducerChannelFactory factory)
    {
        _sendQueue = new AsyncQueueWithCursor<SendOp>(1000); // TODO: Make size configurable
        _dispatcherCts = new CancellationTokenSource();
        _correlationId = correlationId;
        _eventRegister = registerEvent;
        _channel = initialChannel;
        _executor = executor;
        _state = state;
        _factory = factory;
        _isDisposed = 0;

        _dispatcherTask = MessageDispatcher(_channel, _dispatcherCts.Token);
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
        _dispatcherCts.Cancel();
        await _dispatcherTask;
        await _sendQueue.DisposeAsync();
        await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
        await _channel.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask Send(MessageMetadata metadata, ReadOnlySequence<byte> data, TaskCompletionSource<MessageId> receiptTcs, CancellationToken cancellationToken)
    {
        if (IsFinalState()) throw new ProducerClosedException(); // TODO: This exception might be intended for other purposes.
        SendOp sendOp = new SendOp(metadata, data, receiptTcs);
        await _sendQueue.Enqueue(sendOp, cancellationToken);
    }

    private async Task MessageDispatcher(IProducerChannel channel, CancellationToken cancellationToken)
    {
        ValueTask? responseProcessorTask = null;
        try
        {
            var responseQueue = new AsyncQueue<Task<BaseCommand>>();
            responseProcessorTask = ResponseProcessor(responseQueue, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var tcs = new TaskCompletionSource<BaseCommand>();
                _ = tcs.Task.ContinueWith(task => responseQueue.Enqueue(task),
                    TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously);

                SendOp sendOp = await _sendQueue.NextItem(cancellationToken).ConfigureAwait(false);

                // TODO: What should we do with the exceptions that are raised here.
                bool success = await _executor.TryExecuteOnce(() => channel.Send(sendOp.Metadata, sendOp.Data, tcs, cancellationToken), cancellationToken).ConfigureAwait(false);

                if (!success) StopProcessingAndRetry();
            }
        }
        catch (OperationCanceledException)
        {
            // Ignored
        }
        finally
        {
            // TODO: Would this ever raise another exception.
            await (responseProcessorTask ?? new ValueTask(Task.CompletedTask));
        }
    }

    private async ValueTask ResponseProcessor(IDequeue<Task<BaseCommand>> responseQueue, CancellationToken cancellationToken)
    {
        try
        {
            while (cancellationToken.IsCancellationRequested)
            {
                var responseTask = await responseQueue.Dequeue(cancellationToken);

                bool success = await _executor.TryExecuteOnce(() =>
                {
                    if (responseTask.IsFaulted) throw responseTask.Exception!;
                    responseTask.Result.Expect(BaseCommand.Type.SendReceipt);
                    ProcessReceipt(responseTask.Result.SendReceipt);
                }, cancellationToken).ConfigureAwait(false);

                if (!success) StopProcessingAndRetry();
            }
        }
        catch (OperationCanceledException)
        {
            // Ignored
        }
    }

    private void ProcessReceipt(CommandSendReceipt sendReceipt)
    {
        if (!_sendQueue.TryPeek(out SendOp sendOp)) return;

        ulong receiptSequenceId = sendReceipt.SequenceId;
        ulong expectedSequenceId = sendOp.Metadata.SequenceId;

        // Ignore acknowledgements for messages that have already been removed from the queue
        if (receiptSequenceId < expectedSequenceId) return;

        if (receiptSequenceId != expectedSequenceId)
        {
            throw new ProducerSendReceiptOrderingException($"Received sequenceId {receiptSequenceId}. Expected {expectedSequenceId}");
        }

        _sendQueue.Dequeue();
        sendOp.ReceiptTcs.SetResult(sendReceipt.MessageId.ToMessageId());
    }

    private void StopProcessingAndRetry()
    {
        if (!_dispatcherCts.IsCancellationRequested) _dispatcherCts.Cancel();
        // TODO: decide if we want to close the channel og simply raise disconnect, and let Establish new channel dispose it?
        //await channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
        _eventRegister.Register(new ChannelDisconnected(_correlationId));
    }

    public async Task EstablishNewChannel(CancellationToken cancellationToken)
    {
        // TODO: Is it a problem that this cancellation token is always none
        var channel = await _executor.Execute(() => _factory.Create(cancellationToken), cancellationToken).ConfigureAwait(false);

        await _dispatcherTask;
        await _sendQueue.ResetCursor(cancellationToken).ConfigureAwait(false);

        _dispatcherCts = new CancellationTokenSource();
        _dispatcherTask = MessageDispatcher(channel, _dispatcherCts.Token);

        var oldChannel = _channel;
        _channel = channel;

        // TODO: Why does IProducerChannel.DisposeAsync not do anything?
        await oldChannel.DisposeAsync().ConfigureAwait(false);
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
