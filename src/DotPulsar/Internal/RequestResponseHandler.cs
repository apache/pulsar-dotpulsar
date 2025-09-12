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
using DotPulsar.Internal.PulsarApi;
using DotPulsar.Internal.Requests;

public sealed class RequestResponseHandler : IDisposable
{
    private readonly ConnectRequest _connectRequest;
    private readonly RequestId _requestId;
    private readonly Awaiter<IRequest, BaseCommand> _requests;
    private readonly EnumLookup<BaseCommand.Types.Type, Func<BaseCommand, IRequest>> _getResponseIdentifier;

    public RequestResponseHandler()
    {
        _connectRequest = new ConnectRequest();
        _requestId = new RequestId();
        _requests = new Awaiter<IRequest, BaseCommand>();

        _getResponseIdentifier = new EnumLookup<BaseCommand.Types.Type, Func<BaseCommand, IRequest>>(cmd => throw new Exception($"CommandType '{cmd.Type}' not supported as request/response type"));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.Connected, cmd => _connectRequest);
        _getResponseIdentifier.Set(BaseCommand.Types.Type.SendError, cmd => new SendRequest(cmd.SendError.ProducerId, cmd.SendError.SequenceId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.SendReceipt, cmd => new SendRequest(cmd.SendReceipt.ProducerId, cmd.SendReceipt.SequenceId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.ProducerSuccess, cmd => StandardRequest.WithRequestId(cmd.ProducerSuccess.RequestId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.CloseConsumer, cmd => StandardRequest.WithConsumerId(cmd.CloseConsumer.RequestId, cmd.CloseConsumer.ConsumerId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.CloseProducer, cmd => StandardRequest.WithProducerId(cmd.CloseProducer.RequestId, cmd.CloseProducer.ProducerId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.LookupResponse, cmd => StandardRequest.WithRequestId(cmd.LookupTopicResponse.RequestId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.PartitionedMetadataResponse, cmd => StandardRequest.WithRequestId(cmd.PartitionMetadataResponse.RequestId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.GetTopicsOfNamespaceResponse, cmd => StandardRequest.WithRequestId(cmd.GetTopicsOfNamespaceResponse.RequestId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.GetLastMessageIdResponse, cmd => StandardRequest.WithRequestId(cmd.GetLastMessageIdResponse.RequestId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.GetOrCreateSchemaResponse, cmd => StandardRequest.WithRequestId(cmd.GetOrCreateSchemaResponse.RequestId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.Success, cmd => StandardRequest.WithRequestId(cmd.Success.RequestId));
        _getResponseIdentifier.Set(BaseCommand.Types.Type.Error, cmd => _requestId.IsPastInitialId() ? StandardRequest.WithRequestId(cmd.Error.RequestId) : _connectRequest);
    }

    public void Dispose()
        => _requests.Dispose();

    public Task<BaseCommand> Outgoing(CommandProducer command)
    {
        command.RequestId = _requestId.FetchNext();
        var request = StandardRequest.WithProducerId(command.RequestId, command.ProducerId);
        return _requests.CreateTask(request);
    }

    public Task<BaseCommand> ExpectAdditionalResponse(CommandProducer command)
    {
        var request = StandardRequest.WithProducerId(command.RequestId, command.ProducerId);
        return _requests.CreateTask(request);
    }

    public Task<BaseCommand> Outgoing(CommandCloseProducer command)
    {
        command.RequestId = _requestId.FetchNext();
        var request = StandardRequest.WithProducerId(command.RequestId, command.ProducerId);
        return _requests.CreateTask(request);
    }

    public Task<BaseCommand> Outgoing(CommandSubscribe command)
    {
        command.RequestId = _requestId.FetchNext();
        var request = StandardRequest.WithConsumerId(command.RequestId, command.ConsumerId);
        return _requests.CreateTask(request);
    }

    public Task<BaseCommand> Outgoing(CommandUnsubscribe command)
    {
        command.RequestId = _requestId.FetchNext();
        var request = StandardRequest.WithConsumerId(command.RequestId, command.ConsumerId);
        return _requests.CreateTask(request);
    }

    public Task<BaseCommand> Outgoing(CommandCloseConsumer command)
    {
        command.RequestId = _requestId.FetchNext();
        var request = StandardRequest.WithConsumerId(command.RequestId, command.ConsumerId);
        return _requests.CreateTask(request);
    }

    public void Outgoing(CommandSend command, TaskCompletionSource<BaseCommand> tcs)
    {
        var request = new SendRequest(command.ProducerId, command.SequenceId);
        _requests.AddTaskCompletionSource(request, tcs);
    }

    public Task<BaseCommand> Outgoing(CommandGetOrCreateSchema command)
    {
        command.RequestId = _requestId.FetchNext();
        var request = StandardRequest.WithRequestId(command.RequestId);
        return _requests.CreateTask(request);
    }

    public Task<BaseCommand> Outgoing(CommandConnect _)
        => _requests.CreateTask(new ConnectRequest());

    public Task<BaseCommand> Outgoing(CommandLookupTopic command)
    {
        command.RequestId = _requestId.FetchNext();
        return _requests.CreateTask(StandardRequest.WithRequestId(command.RequestId));
    }

    public Task<BaseCommand> Outgoing(CommandPartitionedTopicMetadata command)
    {
        command.RequestId = _requestId.FetchNext();
        return _requests.CreateTask(StandardRequest.WithRequestId(command.RequestId));
    }

    public Task<BaseCommand> Outgoing(CommandGetTopicsOfNamespace command)
    {
        command.RequestId = _requestId.FetchNext();
        return _requests.CreateTask(StandardRequest.WithRequestId(command.RequestId));
    }

    public Task<BaseCommand> Outgoing(CommandSeek command)
    {
        command.RequestId = _requestId.FetchNext();
        return _requests.CreateTask(StandardRequest.WithConsumerId(command.RequestId, command.ConsumerId, BaseCommand.Types.Type.Seek));
    }

    public Task<BaseCommand> Outgoing(CommandGetLastMessageId command)
    {
        command.RequestId = _requestId.FetchNext();
        return _requests.CreateTask(StandardRequest.WithRequestId(command.RequestId));
    }

    public void Incoming(BaseCommand command)
    {
        var identifier = _getResponseIdentifier.Get(command.Type)(command);

        if (identifier is not null)
            _requests.SetResult(identifier, command);
    }

    public void Incoming(CommandCloseConsumer command)
    {
        var requests = _requests.Keys;
        foreach (var request in requests)
        {
            if (request.SenderIsConsumer(command.ConsumerId))
            {
                if (request.IsCommandType(BaseCommand.Types.Type.Seek))
                    _requests.SetResult(request, new BaseCommand { Type = BaseCommand.Types.Type.Success });
                else
                    _requests.Cancel(request);
            }
        }
    }

    public void Incoming(CommandCloseProducer command)
    {
        var requests = _requests.Keys;
        foreach (var request in requests)
        {
            if (request.SenderIsProducer(command.ProducerId))
                _requests.Cancel(request);
        }
    }
}
