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
    using DotPulsar.Internal.Abstractions;
    using DotPulsar.Internal.Requests;
    using PulsarApi;
    using System;
    using System.Threading.Tasks;

    public sealed class RequestResponseHandler : IDisposable
    {
        private readonly RequestId _requestId;
        private readonly Awaiter<IRequest, BaseCommand> _requests;
        private readonly EnumLookup<BaseCommand.Type, Action<BaseCommand>> _setRequestId;
        private readonly EnumLookup<BaseCommand.Type, Func<BaseCommand, IRequest>> _getResponseIdentifier;

        public RequestResponseHandler()
        {
            _requestId = new RequestId();

            _requests = new Awaiter<IRequest, BaseCommand>();

            _setRequestId = new EnumLookup<BaseCommand.Type, Action<BaseCommand>>(cmd => { });
            _setRequestId.Set(BaseCommand.Type.Seek, cmd => cmd.Seek.RequestId = _requestId.FetchNext());
            _setRequestId.Set(BaseCommand.Type.Error, cmd => cmd.Error.RequestId = _requestId.FetchNext());
            _setRequestId.Set(BaseCommand.Type.Producer, cmd => cmd.Producer.RequestId = _requestId.FetchNext());
            _setRequestId.Set(BaseCommand.Type.Lookup, cmd => cmd.LookupTopic.RequestId = _requestId.FetchNext());
            _setRequestId.Set(BaseCommand.Type.Subscribe, cmd => cmd.Subscribe.RequestId = _requestId.FetchNext());
            _setRequestId.Set(BaseCommand.Type.Unsubscribe, cmd => cmd.Unsubscribe.RequestId = _requestId.FetchNext());
            _setRequestId.Set(BaseCommand.Type.CloseConsumer, cmd => cmd.CloseConsumer.RequestId = _requestId.FetchNext());
            _setRequestId.Set(BaseCommand.Type.CloseProducer, cmd => cmd.CloseProducer.RequestId = _requestId.FetchNext());
            _setRequestId.Set(BaseCommand.Type.GetLastMessageId, cmd => cmd.GetLastMessageId.RequestId = _requestId.FetchNext());
            _setRequestId.Set(BaseCommand.Type.GetOrCreateSchema, cmd => cmd.GetOrCreateSchema.RequestId = _requestId.FetchNext());

            _getResponseIdentifier = new EnumLookup<BaseCommand.Type, Func<BaseCommand, IRequest>>(cmd => throw new ArgumentOutOfRangeException(nameof(cmd.CommandType), cmd.CommandType, "CommandType not supported as request/response type"));
            _getResponseIdentifier.Set(BaseCommand.Type.Connect, cmd => new ConnectRequest());
            _getResponseIdentifier.Set(BaseCommand.Type.Connected, cmd => new ConnectRequest());
            _getResponseIdentifier.Set(BaseCommand.Type.Seek, cmd => new StandardRequest(cmd.Seek.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.Send, cmd => new SendRequest(cmd.Send.ProducerId, cmd.Send.SequenceId));
            _getResponseIdentifier.Set(BaseCommand.Type.SendError, cmd => new SendRequest(cmd.SendError.ProducerId, cmd.SendError.SequenceId));
            _getResponseIdentifier.Set(BaseCommand.Type.SendReceipt, cmd => new SendRequest(cmd.SendReceipt.ProducerId, cmd.SendReceipt.SequenceId));
            _getResponseIdentifier.Set(BaseCommand.Type.Producer, cmd => new StandardRequest(cmd.Producer.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.ProducerSuccess, cmd => new StandardRequest(cmd.ProducerSuccess.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.CloseConsumer, cmd => new StandardRequest(cmd.CloseConsumer.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.CloseProducer, cmd => new StandardRequest(cmd.CloseProducer.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.Lookup, cmd => new StandardRequest(cmd.LookupTopic.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.LookupResponse, cmd => new StandardRequest(cmd.LookupTopicResponse.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.Subscribe, cmd => new StandardRequest(cmd.Subscribe.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.Unsubscribe, cmd => new StandardRequest(cmd.Unsubscribe.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.GetLastMessageId, cmd => new StandardRequest(cmd.GetLastMessageId.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.GetLastMessageIdResponse, cmd => new StandardRequest(cmd.GetLastMessageIdResponse.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.GetOrCreateSchema, cmd => new StandardRequest(cmd.GetOrCreateSchema.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.GetOrCreateSchemaResponse, cmd => new StandardRequest(cmd.GetOrCreateSchemaResponse.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.Success, cmd => new StandardRequest(cmd.Success.RequestId));
            _getResponseIdentifier.Set(BaseCommand.Type.Error, cmd => !_requestId.IsPastInitialId() ? new ConnectRequest() : new StandardRequest(cmd.Error.RequestId));
        }

        public void Dispose()
                => _requests.Dispose();

        public Task<BaseCommand> Outgoing(BaseCommand command)
        {
            _setRequestId.Get(command.CommandType)(command);
            return _requests.CreateTask(_getResponseIdentifier.Get(command.CommandType)(command));
        }

        public void Incoming(BaseCommand command)
        {
            var identifier = _getResponseIdentifier.Get(command.CommandType)(command);

            if (identifier is not null)
                _requests.SetResult(identifier, command);
        }

        public void Incoming(CommandCloseProducer command)
        {
            var requests = _requests.Keys;
            foreach (var request in requests)
            {
                if (request is SendRequest sendRequest && sendRequest.ProducerId == command.ProducerId)
                    _requests.Cancel(request);
            }
        }
    }
}
