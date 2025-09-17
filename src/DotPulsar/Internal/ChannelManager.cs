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
using DotPulsar.Internal.Extensions;
using Pulsar.Proto;
using System.Buffers;

public sealed class ChannelManager : IStateHolder<ChannelManagerState>, IDisposable
{
    private readonly StateManager<ChannelManagerState> _stateManager;
    private readonly RequestResponseHandler _requestResponseHandler;
    private readonly IdLookup<IChannel> _consumerChannels;
    private readonly IdLookup<IChannel> _producerChannels;
    private readonly EnumLookup<BaseCommand.Types.Type, Action<BaseCommand>> _incoming;

    public IState<ChannelManagerState> State => _stateManager;

    public ChannelManager()
    {
        _stateManager = new StateManager<ChannelManagerState>(ChannelManagerState.Inactive, ChannelManagerState.Closed);
        _requestResponseHandler = new RequestResponseHandler();
        _consumerChannels = new IdLookup<IChannel>();
        _producerChannels = new IdLookup<IChannel>();
        _incoming = new EnumLookup<BaseCommand.Types.Type, Action<BaseCommand>>(cmd => _requestResponseHandler.Incoming(cmd));
        _incoming.Set(BaseCommand.Types.Type.CloseConsumer, cmd => Incoming(cmd.CloseConsumer));
        _incoming.Set(BaseCommand.Types.Type.CloseProducer, cmd => Incoming(cmd.CloseProducer));
        _incoming.Set(BaseCommand.Types.Type.ActiveConsumerChange, cmd => Incoming(cmd.ActiveConsumerChange));
        _incoming.Set(BaseCommand.Types.Type.ReachedEndOfTopic, cmd => Incoming(cmd.ReachedEndOfTopic));
    }

    public Task<ProducerResponse> Outgoing(CommandProducer command, IChannel channel)
    {
        var producerId = AddProducerChannel(channel);
        command.ProducerId = producerId;
        var response = _requestResponseHandler.Outgoing(command);

        return response.ContinueWith(result =>
        {
            if (result.Result.Type == BaseCommand.Types.Type.Error)
            {
                _ = RemoveProducerChannel(producerId);
                result.Result.Error.Throw();
            }

            if (response.Result.ProducerSuccess.ProducerReady)
            {
                channel.Connected();
            }
            else
            {
                channel.WaitingForExclusive();
                HandleAdditionalProducerSuccess(command, channel.Connected);
            }

            return new ProducerResponse(producerId, response.Result.ProducerSuccess.ProducerName, response.Result.ProducerSuccess.TopicEpoch);
        }, TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    public Task<SubscribeResponse> Outgoing(CommandSubscribe command, IChannel channel)
    {
        var consumerId = AddConsumerChannel(channel);
        command.ConsumerId = consumerId;
        var response = _requestResponseHandler.Outgoing(command);

        return response.ContinueWith(result =>
        {
            if (result.Result.Type == BaseCommand.Types.Type.Error)
            {
                _ = RemoveConsumerChannel(consumerId);
                result.Result.Error.Throw();
            }

            channel.Connected();

            return new SubscribeResponse(consumerId);
        }, TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    public Task<BaseCommand> Outgoing(CommandCloseConsumer command)
    {
        var consumerId = command.ConsumerId;

        Task<BaseCommand> response;

        using (TakeConsumerSenderLock(consumerId))
        {
            response = _requestResponseHandler.Outgoing(command);
        }

        _ = response.ContinueWith(result =>
        {
            if (result.Result.Type == BaseCommand.Types.Type.Success)
                _ = RemoveConsumerChannel(consumerId);
        }, TaskContinuationOptions.OnlyOnRanToCompletion);

        return response;
    }

    public Task<BaseCommand> Outgoing(CommandCloseProducer command)
    {
        var producerId = command.ProducerId;

        Task<BaseCommand> response;

        using (TakeProducerSenderLock(producerId))
        {
            response = _requestResponseHandler.Outgoing(command);
        }

        _ = response.ContinueWith(result =>
        {
            if (result.Result.Type == BaseCommand.Types.Type.Success)
                _ = RemoveProducerChannel(producerId);
        }, TaskContinuationOptions.OnlyOnRanToCompletion);

        return response;
    }

    public Task<BaseCommand> Outgoing(CommandUnsubscribe command)
    {
        var consumerId = command.ConsumerId;

        Task<BaseCommand> response;

        using (TakeConsumerSenderLock(consumerId))
        {
            response = _requestResponseHandler.Outgoing(command);
        }

        _ = response.ContinueWith(result =>
        {
            if (result.Result.Type == BaseCommand.Types.Type.Success)
                RemoveConsumerChannel(consumerId)?.Unsubscribed();
        }, TaskContinuationOptions.OnlyOnRanToCompletion);

        return response;
    }

    public void Outgoing(CommandSend command, TaskCompletionSource<BaseCommand> tcs)
    {
        using (TakeProducerSenderLock(command.ProducerId))
        {
            _requestResponseHandler.Outgoing(command, tcs);
        }
    }

    public Task<BaseCommand> Outgoing(CommandGetOrCreateSchema command)
        => _requestResponseHandler.Outgoing(command);

    public Task<BaseCommand> Outgoing(CommandConnect command)
        => _requestResponseHandler.Outgoing(command);

    public Task<BaseCommand> Outgoing(CommandLookupTopic command)
        => _requestResponseHandler.Outgoing(command);

    public Task<BaseCommand> Outgoing(CommandPartitionedTopicMetadata command)
        => _requestResponseHandler.Outgoing(command);

    public Task<BaseCommand> Outgoing(CommandGetTopicsOfNamespace command)
        => _requestResponseHandler.Outgoing(command);

    public Task<BaseCommand> Outgoing(CommandSeek command)
    {
        using (TakeConsumerSenderLock(command.ConsumerId))
        {
            return _requestResponseHandler.Outgoing(command);
        }
    }

    public Task<BaseCommand> Outgoing(CommandGetLastMessageId command)
    {
        using (TakeConsumerSenderLock(command.ConsumerId))
        {
            return _requestResponseHandler.Outgoing(command);
        }
    }

    public void Incoming(BaseCommand command)
        => _incoming.Get(command.Type)(command);

    public void Incoming(CommandMessage command, ReadOnlySequence<byte> data)
        => _consumerChannels[command.ConsumerId]?.Received(new MessagePackage(command.MessageId, command.RedeliveryCount, data));

    public void Dispose()
    {
        _stateManager.SetState(ChannelManagerState.Closed);
        _requestResponseHandler.Dispose();

        foreach (var channel in _consumerChannels.RemoveAll())
            channel.Disconnected();

        foreach (var channel in _producerChannels.RemoveAll())
            channel.Disconnected();
    }

    private void Incoming(CommandReachedEndOfTopic command)
        => _consumerChannels[command.ConsumerId]?.ReachedEndOfTopic();

    private void Incoming(CommandCloseConsumer command)
    {
        var channel = RemoveConsumerChannel(command.ConsumerId);
        if (channel is null)
            return;

        _requestResponseHandler.Incoming(command);
        channel.ClosedByServer();
    }

    private void Incoming(CommandCloseProducer command)
    {
        var channel = RemoveProducerChannel(command.ProducerId);
        if (channel is null)
            return;

        _requestResponseHandler.Incoming(command);
        channel.ClosedByServer();
    }

    private void Incoming(CommandActiveConsumerChange command)
    {
        var channel = _consumerChannels[command.ConsumerId];

        if (channel is null)
            return;

        if (command.IsActive)
            channel.Activated();
        else
            channel.Deactivated();
    }

    private IDisposable TakeConsumerSenderLock(ulong consumerId)
    {
        var channel = _consumerChannels[consumerId];
        if (channel is null)
            throw new OperationCanceledException();

        return channel.SenderLock();
    }

    private IDisposable TakeProducerSenderLock(ulong producerId)
    {
        var channel = _producerChannels[producerId];
        if (channel is null)
            throw new OperationCanceledException();

        return channel.SenderLock();
    }

    private void HandleAdditionalProducerSuccess(CommandProducer command, Action successAction)
    {
        _ = _requestResponseHandler.ExpectAdditionalResponse(command).ContinueWith(response =>
        {
            if (response.IsCanceled || response.IsFaulted || response.Result.Type == BaseCommand.Types.Type.Error)
            {
                _producerChannels[command.ProducerId]?.Disconnected();
                return;
            }
            if (!response.Result.ProducerSuccess.ProducerReady)
            {
                HandleAdditionalProducerSuccess(command, successAction);
                return;
            }
            successAction.Invoke();
        });
    }

    private ulong AddProducerChannel(IChannel channel) => AddChannel(_producerChannels, channel);

    private ulong AddConsumerChannel(IChannel channel) => AddChannel(_consumerChannels, channel);

    private ulong AddChannel(IdLookup<IChannel> lookup, IChannel channel)
    {
        var id = lookup.Add(channel);
        _stateManager.SetState(ChannelManagerState.Active);
        return id;
    }

    private IChannel? RemoveProducerChannel(ulong producerId) => RemoveChannel(_producerChannels, producerId);

    private IChannel? RemoveConsumerChannel(ulong consumerId) => RemoveChannel(_consumerChannels, consumerId);

    private IChannel? RemoveChannel(IdLookup<IChannel> lookup, ulong id)
    {
        var channel = lookup.Remove(id);
        ChannelRemoved();
        return channel;
    }

    private void ChannelRemoved()
    {
        if (_consumerChannels.IsEmpty() && _producerChannels.IsEmpty())
            _stateManager.SetState(ChannelManagerState.Inactive);
    }
}
