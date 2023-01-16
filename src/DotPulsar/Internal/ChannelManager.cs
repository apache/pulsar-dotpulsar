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

using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Buffers;
using System.Threading.Tasks;

public sealed class ChannelManager : IDisposable
{
    private readonly RequestResponseHandler _requestResponseHandler;
    private readonly IdLookup<IChannel> _consumerChannels;
    private readonly IdLookup<IChannel> _producerChannels;
    private readonly EnumLookup<BaseCommand.Type, Action<BaseCommand>> _incoming;

    public ChannelManager()
    {
        _requestResponseHandler = new RequestResponseHandler();
        _consumerChannels = new IdLookup<IChannel>();
        _producerChannels = new IdLookup<IChannel>();
        _incoming = new EnumLookup<BaseCommand.Type, Action<BaseCommand>>(cmd => _requestResponseHandler.Incoming(cmd));
        _incoming.Set(BaseCommand.Type.CloseConsumer, cmd => Incoming(cmd.CloseConsumer));
        _incoming.Set(BaseCommand.Type.CloseProducer, cmd => Incoming(cmd.CloseProducer));
        _incoming.Set(BaseCommand.Type.ActiveConsumerChange, cmd => Incoming(cmd.ActiveConsumerChange));
        _incoming.Set(BaseCommand.Type.ReachedEndOfTopic, cmd => Incoming(cmd.ReachedEndOfTopic));
    }

    public bool HasChannels()
        => !_consumerChannels.IsEmpty() || !_producerChannels.IsEmpty();

    public Task<ProducerResponse> Outgoing(CommandProducer command, IChannel channel)
    {
        var producerId = _producerChannels.Add(channel);
        command.ProducerId = producerId;
        var response = _requestResponseHandler.Outgoing(command);

        return response.ContinueWith(result =>
        {
            if (result.Result.CommandType == BaseCommand.Type.Error)
            {
                _ = _producerChannels.Remove(producerId);
                result.Result.Error.Throw();
            }

            channel.Connected();

            return new ProducerResponse(producerId, result.Result.ProducerSuccess.ProducerName);
        }, TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    public Task<SubscribeResponse> Outgoing(CommandSubscribe command, IChannel channel)
    {
        var consumerId = _consumerChannels.Add(channel);
        command.ConsumerId = consumerId;
        var response = _requestResponseHandler.Outgoing(command);

        return response.ContinueWith(result =>
        {
            if (result.Result.CommandType == BaseCommand.Type.Error)
            {
                _ = _consumerChannels.Remove(consumerId);
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
            if (result.Result.CommandType == BaseCommand.Type.Success)
                _ = _consumerChannels.Remove(consumerId);
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
            if (result.Result.CommandType == BaseCommand.Type.Success)
                _ = _producerChannels.Remove(producerId);
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
            if (result.Result.CommandType == BaseCommand.Type.Success)
                _consumerChannels.Remove(consumerId)?.Unsubscribed();
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
        => _incoming.Get(command.CommandType)(command);

    public void Incoming(CommandMessage command, ReadOnlySequence<byte> data)
        => _consumerChannels[command.ConsumerId]?.Received(new MessagePackage(command.MessageId, command.RedeliveryCount, data));

    public void Dispose()
    {
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
        var channel = _consumerChannels.Remove(command.ConsumerId);
        if (channel is null)
            return;

        _requestResponseHandler.Incoming(command);
        channel.ClosedByServer();
    }

    private void Incoming(CommandCloseProducer command)
    {
        var channel = _producerChannels.Remove(command.ProducerId);
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
}
