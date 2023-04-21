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
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Events;
using DotPulsar.Internal.Exceptions;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class SubProducer : IContainsProducerChannel, IState<ProducerState>
{
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
    private ulong? _topicEpoch;

    public SubProducer(
        Guid correlationId,
        IRegisterEvent registerEvent,
        IProducerChannel initialChannel,
        IExecute executor,
        IStateChanged<ProducerState> state,
        IProducerChannelFactory factory,
        uint maxPendingMessages)
    {
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

        try
        {
            _dispatcherCts?.Cancel();
            _dispatcherCts?.Dispose();
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

    public async ValueTask Send(SendOp sendOp, CancellationToken cancellationToken)
        => await _sendQueue.Enqueue(sendOp, cancellationToken).ConfigureAwait(false);

    internal async ValueTask WaitForSendQueueEmpty(CancellationToken cancellationToken)
        => await _sendQueue.WaitForEmpty(cancellationToken).ConfigureAwait(false);

    private async Task MessageDispatcher(IProducerChannel channel, CancellationToken cancellationToken)
    {
        var responseQueue = new AsyncQueue<Task<BaseCommand>>();
        var responseProcessorTask = ResponseProcessor(responseQueue, cancellationToken);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var sendOp = await _sendQueue.NextItem(cancellationToken).ConfigureAwait(false);

                if (sendOp.CancellationToken.IsCancellationRequested)
                {
                    _sendQueue.RemoveCurrentItem();
                    continue;
                }

                var tcs = new TaskCompletionSource<BaseCommand>();
                _ = tcs.Task.ContinueWith(task =>
                {
                    try
                    {
                        responseQueue.Enqueue(task);
                    }
                    catch
                    {
                        // Ignore
                    }
                }, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously);

                // Use CancellationToken.None here because otherwise it will throw exceptions on all fault actions even retry.
                var success = await _executor.TryExecuteOnce(() => channel.Send(sendOp.Metadata, sendOp.Data, tcs, cancellationToken), CancellationToken.None).ConfigureAwait(false);

                if (success)
                    continue;

                _eventRegister.Register(new ChannelDisconnected(_correlationId));
                break;
            }

            await responseProcessorTask.ConfigureAwait(false);
        }
        finally
        {
            responseQueue.Dispose();
        }
    }

    private async ValueTask ResponseProcessor(IDequeue<Task<BaseCommand>> responseQueue, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var responseTask = await responseQueue.Dequeue(cancellationToken).ConfigureAwait(false);

            var success = await _executor.TryExecuteOnce(() =>
            {
                if (responseTask.IsFaulted)
                    throw responseTask.Exception!;

                responseTask.Result.Expect(BaseCommand.Type.SendReceipt);
                ProcessReceipt(responseTask.Result.SendReceipt);
            }, CancellationToken.None).ConfigureAwait(false); // Use CancellationToken.None here because otherwise it will throw exceptions on all fault actions even retry.

            if (success)
                continue;

            _eventRegister.Register(new SendReceiptWrongOrdering(_correlationId));
            break;
        }
    }

    private void ProcessReceipt(CommandSendReceipt sendReceipt)
    {
        var receiptSequenceId = sendReceipt.SequenceId;

        if (!_sendQueue.TryPeek(out var sendOp) || sendOp is null)
            throw new ProducerSendReceiptOrderingException($"Received sequenceId {receiptSequenceId} but send queue is empty");

        var expectedSequenceId = sendOp.Metadata.SequenceId;

        if (receiptSequenceId != expectedSequenceId)
            throw new ProducerSendReceiptOrderingException($"Received sequenceId {receiptSequenceId}. Expected {expectedSequenceId}");

        _sendQueue.Dequeue();
        sendOp.ReceiptTcs.TrySetResult(sendReceipt.MessageId.ToMessageId());
    }

    public async Task EstablishNewChannel(CancellationToken cancellationToken)
    {
        try
        {
            if (_dispatcherCts is not null && !_dispatcherCts.IsCancellationRequested)
            {
                _dispatcherCts.Cancel();
                _dispatcherCts.Dispose();
            }
        }
        catch (Exception)
        {
            // Ignored
        }

        await _executor.TryExecuteOnce(() => _dispatcherTask ?? Task.CompletedTask, cancellationToken).ConfigureAwait(false);

        try
        {
            var oldChannel = _channel;
            await oldChannel.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            // Ignored
        }

        _channel = await _executor.Execute(() => _factory.Create(_topicEpoch, cancellationToken), cancellationToken).ConfigureAwait(false);
    }

    public async Task ActivateChannel(ulong topicEpoch, CancellationToken cancellationToken)
    {
        _topicEpoch = topicEpoch;
        _dispatcherCts = new CancellationTokenSource();
        await _executor.Execute(() =>
        {
            _sendQueue.ResetCursor();
            _dispatcherTask = MessageDispatcher(_channel, _dispatcherCts.Token);
        }, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask CloseChannel(CancellationToken cancellationToken)
        => await _channel.ClosedByClient(cancellationToken).ConfigureAwait(false);

    public ValueTask ChannelFaulted(Exception exception)
    {
        return new ValueTask();
    }
}
