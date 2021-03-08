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
        private readonly Awaiter<IRequest, BaseCommand> _requests;
        private readonly RequestId _requestId;

        public RequestResponseHandler()
        {
            _requests = new Awaiter<IRequest, BaseCommand>();
            _requestId = new RequestId();
        }

        public void Dispose()
            => _requests.Dispose();

        public Task<BaseCommand> Outgoing(BaseCommand command)
        {
            SetRequestId(command);
            return _requests.CreateTask(GetResponseIdentifier(command));
        }

        public void Incoming(BaseCommand command)
        {
            var identifier = GetResponseIdentifier(command);

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

        private void SetRequestId(BaseCommand cmd)
        {
            switch (cmd.CommandType)
            {
                case BaseCommand.Type.Seek:
                    cmd.Seek.RequestId = _requestId.FetchNext();
                    return;
                case BaseCommand.Type.Lookup:
                    cmd.LookupTopic.RequestId = _requestId.FetchNext();
                    return;
                case BaseCommand.Type.Error:
                    cmd.Error.RequestId = _requestId.FetchNext();
                    return;
                case BaseCommand.Type.Producer:
                    cmd.Producer.RequestId = _requestId.FetchNext();
                    return;
                case BaseCommand.Type.CloseProducer:
                    cmd.CloseProducer.RequestId = _requestId.FetchNext();
                    return;
                case BaseCommand.Type.Subscribe:
                    cmd.Subscribe.RequestId = _requestId.FetchNext();
                    return;
                case BaseCommand.Type.Unsubscribe:
                    cmd.Unsubscribe.RequestId = _requestId.FetchNext();
                    return;
                case BaseCommand.Type.CloseConsumer:
                    cmd.CloseConsumer.RequestId = _requestId.FetchNext();
                    return;
                case BaseCommand.Type.GetLastMessageId:
                    cmd.GetLastMessageId.RequestId = _requestId.FetchNext();
                    return;
                case BaseCommand.Type.GetOrCreateSchema:
                    cmd.GetOrCreateSchema.RequestId = _requestId.FetchNext();
                    return;
            }
        }

        private IRequest GetResponseIdentifier(BaseCommand cmd)
            => cmd.CommandType switch
            {
                BaseCommand.Type.Send => new SendRequest(cmd.Send.ProducerId, cmd.Send.SequenceId),
                BaseCommand.Type.SendReceipt => new SendRequest(cmd.SendReceipt.ProducerId, cmd.SendReceipt.SequenceId),
                BaseCommand.Type.SendError => new SendRequest(cmd.SendError.ProducerId, cmd.SendError.SequenceId),
                BaseCommand.Type.Connect => new ConnectRequest(),
                BaseCommand.Type.Connected => new ConnectRequest(),
                BaseCommand.Type.Error => !_requestId.IsPastInitialId() ? new ConnectRequest() : new StandardRequest(cmd.Error.RequestId),
                BaseCommand.Type.Producer => new StandardRequest(cmd.Producer.RequestId),
                BaseCommand.Type.ProducerSuccess => new StandardRequest(cmd.ProducerSuccess.RequestId),
                BaseCommand.Type.CloseProducer => new StandardRequest(cmd.CloseProducer.RequestId),
                BaseCommand.Type.Lookup => new StandardRequest(cmd.LookupTopic.RequestId),
                BaseCommand.Type.LookupResponse => new StandardRequest(cmd.LookupTopicResponse.RequestId),
                BaseCommand.Type.Unsubscribe => new StandardRequest(cmd.Unsubscribe.RequestId),
                BaseCommand.Type.Subscribe => new StandardRequest(cmd.Subscribe.RequestId),
                BaseCommand.Type.Success => new StandardRequest(cmd.Success.RequestId),
                BaseCommand.Type.Seek => new StandardRequest(cmd.Seek.RequestId),
                BaseCommand.Type.CloseConsumer => new StandardRequest(cmd.CloseConsumer.RequestId),
                BaseCommand.Type.GetLastMessageId => new StandardRequest(cmd.GetLastMessageId.RequestId),
                BaseCommand.Type.GetLastMessageIdResponse => new StandardRequest(cmd.GetLastMessageIdResponse.RequestId),
                BaseCommand.Type.GetOrCreateSchema => new StandardRequest(cmd.GetOrCreateSchema.RequestId),
                BaseCommand.Type.GetOrCreateSchemaResponse => new StandardRequest(cmd.GetOrCreateSchemaResponse.RequestId),
                _ => throw new ArgumentOutOfRangeException(nameof(cmd.CommandType), cmd.CommandType, "CommandType not supported as request/response type")
            };
    }
}
