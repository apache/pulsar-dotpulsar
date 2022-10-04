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
using DotPulsar.Internal.Extensions;
using Events;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public sealed class SubProducer : IEstablishNewChannel, IState<ProducerState>
{
    private readonly AsyncLock _channelLock;
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private IProducerChannel _channel;
    private readonly IExecute _executor;
    private readonly IStateChanged<ProducerState> _state;
    private readonly IProducerChannelFactory _factory;
    private readonly IHandleException _exceptionHandler;
    private readonly SemaphoreSlim _maxPendingMessagesSemaphore;
    private readonly Queue<SendOp> _queue;
    private int _isDisposed;

    public Uri ServiceUrl { get; }
    public string Topic { get; }

    public SubProducer(
        Guid correlationId,
        Uri serviceUrl,
        string topic,
        IRegisterEvent registerEvent,
        IProducerChannel initialChannel,
        IExecute executor,
        IStateChanged<ProducerState> state,
        IProducerChannelFactory factory,
        IHandleException exceptionHandler)
    {
        _channelLock = new AsyncLock();
        _correlationId = correlationId;
        ServiceUrl = serviceUrl;
        Topic = topic;
        _eventRegister = registerEvent;
        _channel = initialChannel;
        _executor = executor;
        _state = state;
        _factory = factory;
        _exceptionHandler = exceptionHandler;
        _queue = new Queue<SendOp>();
        _maxPendingMessagesSemaphore = new SemaphoreSlim(1000, 1000); // TODO: Make size configurable
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
        _maxPendingMessagesSemaphore.Dispose();
        await _channelLock.DisposeAsync();
        await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
        await _channel.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask Enqueue(PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> data, Func<MessageId, ValueTask> onMessageSent, Action<Exception> onError,
    CancellationToken cancellationToken)
    {
        // TODO: This will not guarantee ordering if multiple messages are sent at the same time, but not sure we need to support that.
        await _maxPendingMessagesSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        IDisposable accessGrant;
        try
        {
            accessGrant = await _channelLock.Lock(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            _maxPendingMessagesSemaphore.Release();
            throw;
        }

        SendOp sendOp = new SendOp(metadata, data, onMessageSent);
        try
        {
            var tcs = new TaskCompletionSource<PulsarApi.CommandSendReceipt>(TaskCreationOptions.RunContinuationsAsynchronously);
            _ = tcs.Task.ContinueWith(async task =>
            {
                // TODO: Handle cancellation and fault
                await ProcessResponse(task.Result, cancellationToken);
            }, cancellationToken);
            // TODO: How do we make sure the message is resent if sending fails? Will we always get a new channel afterwards?
            await _channel.Send(metadata, data, tcs, cancellationToken).ConfigureAwait(false);
            _queue.Enqueue(sendOp);
        }
        catch (OperationCanceledException)
        {
            _maxPendingMessagesSemaphore.Release();
            throw;
        }
        catch (Exception e)
        {
            var exceptionContext = new ExceptionContext(e, CancellationToken.None);
            //TODO: The exception handler waits for the retry delay. Should it now be handled in the executor instead?
            await _exceptionHandler.OnException(exceptionContext).ConfigureAwait(false);

            // TODO: What is the difference between Rethrow and ThrowException? Does it have something to do with out the exception is propagated to the user?
            if (exceptionContext.Result != FaultAction.Retry)
            {
                //TODO: Saw something about rethrowing with the initial stacktrace somewhere.
                _maxPendingMessagesSemaphore.Release();
                throw exceptionContext.Exception;
            }
            else
            {
                _queue.Enqueue(sendOp);
            }
        }
        finally
        {
            accessGrant.Dispose();
        }
    }

    private async ValueTask ProcessResponse(PulsarApi.CommandSendReceipt sendReceipt, CancellationToken cancellationToken)
    {
        // TODO: Can we ever get here with a sendReceipt belonging to a different connection? If so how do we handle that?
        // TODO: Cancel properly on shutdown. What token should we use?
        using (await _channelLock.Lock(cancellationToken).ConfigureAwait(false))
        {
            if (_queue.Count == 0) return;

            SendOp sendOp = _queue.Peek();

            // TODO: If ledger id -1 and entry id -1 apparently the message has been dropped. Most likely because dedup is enabled
            // Not sure this can happen, but it is handled this way in the java client. Saying timed out message
            //if (sendOp is null) return;

            // Ignore ack for messages that has already timed out, again not sure what they mean by timeout
            if (sendReceipt.SequenceId < sendOp.Metadata.SequenceId) return;

            if (sendReceipt.SequenceId == sendOp.Metadata.SequenceId)
            {
                try
                {
                    await sendOp.OnSuccess.Invoke(sendReceipt.MessageId.ToMessageId()).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // TODO: Does it make sense to do anything but this?
                    // Ignored
                }
                _queue.Dequeue();
                _maxPendingMessagesSemaphore.Release();
                return;
            }

            await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
        }
    }

    public async Task EstablishNewChannel(CancellationToken cancellationToken)
    {
        var channel = await _executor.Execute(() => _factory.Create(cancellationToken), cancellationToken).ConfigureAwait(false);

        var oldChannel = _channel;

        using (await _channelLock.Lock(cancellationToken).ConfigureAwait(false))
        {
            _channel = channel;
            foreach (SendOp sendOp in _queue)
            {
                try
                {
                    var tcs = new TaskCompletionSource<PulsarApi.CommandSendReceipt>(TaskCreationOptions.RunContinuationsAsynchronously);
                    _ = tcs.Task.ContinueWith(async (task) =>
                    {
                        // TODO: Handle cancellation and fault. Maybe create method which gets task as parameter.
                        await ProcessResponse(task.Result, cancellationToken);
                    }, cancellationToken);
                    await _channel.Send(sendOp.Metadata, sendOp.Data, tcs, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    var exceptionContext = new ExceptionContext(e, cancellationToken);
                    //TODO: The exception handler waits for the retry delay. Should it now be handled in the executor instead?
                    await _exceptionHandler.OnException(exceptionContext).ConfigureAwait(false);

                    // TODO: What is the difference between Rethrow and ThrowException? Does it have something to do with out the exception is propagated to the user?
                    if (exceptionContext.Result != FaultAction.Retry)
                    {
                        //TODO: Saw something about rethrowing with the initial stacktrace somewhere.
                        _maxPendingMessagesSemaphore.Release();
                        throw exceptionContext.Exception;
                    }

                    if (exceptionContext.Result == FaultAction.Retry)
                    {
                        // TODO: No point in continuing resend, but how do we trigger reconnect?
                        await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
                        // We could loop on creating the channel?
                        break;
                    }
                }
            }
        }
        await oldChannel.DisposeAsync().ConfigureAwait(false);
    }

    private class SendOp
    {
        public SendOp(PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> data, Func<MessageId, ValueTask> onSuccess)
        {
            Metadata = metadata;
            Data = data;
            OnSuccess = onSuccess;
        }

        public PulsarApi.MessageMetadata Metadata { get; }
        public ReadOnlySequence<byte> Data { get; }
        public Func<MessageId, ValueTask> OnSuccess { get; }
    }
}
