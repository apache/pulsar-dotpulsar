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
using DotPulsar.Internal.Events;
using DotPulsar.Internal.Exceptions;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;

public sealed class SubProducer : IContainsChannel, IStateHolder<ProducerState>
{
    private readonly AsyncQueueWithCursor<SendOp> _sendQueue;
    private CancellationTokenSource? _dispatcherCts;
    private Task? _dispatcherTask;
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private IProducerChannel _channel;
    private readonly IExecute _executor;
    private readonly IState<ProducerState> _state;
    private readonly IProducerChannelFactory _factory;
    private readonly int _partition;
    private int _isDisposed;
    private Exception? _faultException;

    public string Topic { get; }
    public IState<ProducerState> State => _state;

    public SubProducer(
        Guid correlationId,
        IRegisterEvent registerEvent,
        IProducerChannel initialChannel,
        IExecute executor,
        IState<ProducerState> state,
        IProducerChannelFactory factory,
        int partition,
        uint maxPendingMessages,
        string topic)
    {
        _sendQueue = new AsyncQueueWithCursor<SendOp>(maxPendingMessages);
        _correlationId = correlationId;
        _eventRegister = registerEvent;
        _channel = initialChannel;
        _executor = executor;
        _state = state;
        _factory = factory;
        _partition = partition;
        Topic = topic;
        _isDisposed = 0;

        _eventRegister.Register(new ProducerCreated(_correlationId));
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _eventRegister.Register(new ProducerDisposed(_correlationId));
        await InternalDispose().ConfigureAwait(false);
    }

    private async ValueTask InternalDispose()
    {
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
    {
        Guard();
        await _sendQueue.Enqueue(sendOp, cancellationToken).ConfigureAwait(false);
    }

    internal async ValueTask WaitForSendQueueEmpty(CancellationToken cancellationToken)
    {
        Guard();
        await _sendQueue.WaitForEmpty(cancellationToken).ConfigureAwait(false);
    }

    private async Task MessageDispatcher(IProducerChannel channel, CancellationToken cancellationToken)
    {
        using var responseQueue = new AsyncQueue<Task<BaseCommand>>();
        var responseProcessorTask = Task.Run(async () => await ResponseProcessor(responseQueue, cancellationToken), CancellationToken.None);

        _sendQueue.ResetCursor();

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

            if (!success)
            {
                if (!cancellationToken.IsCancellationRequested)
                    _eventRegister.Register(new ChannelDisconnected(_correlationId));
                break;
            }
        }

        await responseProcessorTask.ConfigureAwait(false);
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

                responseTask.Result.Expect(BaseCommand.Types.Type.SendReceipt);
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

        var srMsgId = sendReceipt.MessageId;
        var messageId = new MessageId(srMsgId.LedgerId, srMsgId.EntryId, _partition, srMsgId.BatchIndex, Topic);
        sendOp.ReceiptTcs.TrySetResult(messageId);
    }

    public async Task EstablishNewChannel(CancellationToken cancellationToken)
    {
        try
        {
            if (_dispatcherCts is not null)
            {
                if (!_dispatcherCts.IsCancellationRequested)
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

        _channel = await _executor.Execute(() => _factory.Create(cancellationToken), cancellationToken).ConfigureAwait(false);
        _dispatcherCts = new CancellationTokenSource();
        _dispatcherTask = Task.Run(async () => await MessageDispatcher(_channel, _dispatcherCts.Token), CancellationToken.None);
    }

    public async ValueTask CloseChannel(CancellationToken cancellationToken)
        => await _channel.ClosedByClient(cancellationToken).ConfigureAwait(false);

    public async ValueTask ChannelFaulted(Exception exception)
    {
        _faultException = exception;
        await InternalDispose().ConfigureAwait(false);
    }

    private void Guard()
    {
        if (_isDisposed != 0)
            throw new ProducerDisposedException(GetType().FullName!);

        if (_faultException is ProducerFencedException)
            throw _faultException;
        if (_faultException is not null)
            throw new ProducerFaultedException(_faultException);
    }
}
