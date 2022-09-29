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
using System.Threading;
using System.Threading.Tasks;

public sealed class SubProducer : IEstablishNewChannel, IState<ProducerState>
{
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private IProducerChannel _channel;
    private readonly IExecute _executor;
    private readonly IStateChanged<ProducerState> _state;
    private readonly IProducerChannelFactory _factory;
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
        IProducerChannelFactory factory)
    {
        _correlationId = correlationId;
        ServiceUrl = serviceUrl;
        Topic = topic;
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
        await _channel.ClosedByClient(CancellationToken.None).ConfigureAwait(false);
        await _channel.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask<MessageId> Send(PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> data, CancellationToken cancellationToken)
        => await _executor.Execute(() => InternalSend(metadata, data, cancellationToken), cancellationToken).ConfigureAwait(false);

    public void Enqueue(PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> data, Func<MessageId, ValueTask> onMessageSent, Action<Exception> onError)
    {
        throw new NotImplementedException();
    }

    private async ValueTask<MessageId> InternalSend(PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> data, CancellationToken cancellationToken)
    {
        var response = await _channel.Send(metadata, data, cancellationToken).ConfigureAwait(false);
        return response.MessageId.ToMessageId();
    }

    public async Task EstablishNewChannel(CancellationToken cancellationToken)
    {
        var channel = await _executor.Execute(() => _factory.Create(cancellationToken), cancellationToken).ConfigureAwait(false);

        var oldChannel = _channel;
        if (oldChannel is not null)
            await oldChannel.DisposeAsync().ConfigureAwait(false);

        _channel = channel;
    }
}
