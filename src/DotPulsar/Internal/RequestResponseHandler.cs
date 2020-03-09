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

using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class RequestResponseHandler : IDisposable
    {
        private const string ConnectResponseIdentifier = "Connected";

        private readonly Awaitor<string, BaseCommand> _responses;
        private SequenceId _requestId;
        private bool _pastInitialRequestId = false;

        public RequestResponseHandler()
        {
            _responses = new Awaitor<string, BaseCommand>();
            _requestId = new SequenceId(1);
        }

        public void Dispose() => _responses.Dispose();

        public Task<BaseCommand> Outgoing(BaseCommand command)
        {
            SetRequestId(command);
            return _responses.CreateTask(GetResponseIdentifier(command));
        }

        public void Incoming(BaseCommand command)
        {
            var identifier = GetResponseIdentifier(command);
            if (identifier != null)
                _responses.SetResult(identifier, command);
        }

        private void SetRequestId(BaseCommand cmd)
        {
            switch (cmd.CommandType)
            {
                case BaseCommand.Type.Seek:
                    cmd.Seek.RequestId = _requestId.FetchNext();
                    _pastInitialRequestId = true;
                    return;
                case BaseCommand.Type.Lookup:
                    cmd.LookupTopic.RequestId = _requestId.FetchNext();
                    _pastInitialRequestId = true;
                    return;
                case BaseCommand.Type.Error:
                    cmd.Error.RequestId = _requestId.FetchNext();
                    _pastInitialRequestId = true;
                    return;
                case BaseCommand.Type.Producer:
                    cmd.Producer.RequestId = _requestId.FetchNext();
                    _pastInitialRequestId = true;
                    return;
                case BaseCommand.Type.CloseProducer:
                    cmd.CloseProducer.RequestId = _requestId.FetchNext();
                    _pastInitialRequestId = true;
                    return;
                case BaseCommand.Type.Subscribe:
                    cmd.Subscribe.RequestId = _requestId.FetchNext();
                    _pastInitialRequestId = true;
                    return;
                case BaseCommand.Type.Unsubscribe:
                    cmd.Unsubscribe.RequestId = _requestId.FetchNext();
                    _pastInitialRequestId = true;
                    return;
                case BaseCommand.Type.CloseConsumer:
                    cmd.CloseConsumer.RequestId = _requestId.FetchNext();
                    _pastInitialRequestId = true;
                    return;
                case BaseCommand.Type.GetLastMessageId:
                    cmd.GetLastMessageId.RequestId = _requestId.FetchNext();
                    _pastInitialRequestId = true;
                    return;
            }
        }

        private string GetResponseIdentifier(BaseCommand cmd)
        {
            switch (cmd.CommandType)
            {
                case BaseCommand.Type.Connect:
                case BaseCommand.Type.Connected: return ConnectResponseIdentifier;
                case BaseCommand.Type.Send: return cmd.Send.ProducerId.ToString() + '-' + cmd.Send.SequenceId.ToString();
                case BaseCommand.Type.SendError: return cmd.SendError.ProducerId.ToString() + '-' + cmd.SendError.SequenceId.ToString();
                case BaseCommand.Type.SendReceipt: return cmd.SendReceipt.ProducerId.ToString() + '-' + cmd.SendReceipt.SequenceId.ToString();
                case BaseCommand.Type.Error: return !_pastInitialRequestId ? ConnectResponseIdentifier : cmd.Error.RequestId.ToString();
                case BaseCommand.Type.Producer: return cmd.Producer.RequestId.ToString();
                case BaseCommand.Type.ProducerSuccess: return cmd.ProducerSuccess.RequestId.ToString();
                case BaseCommand.Type.CloseProducer: return cmd.CloseProducer.RequestId.ToString();
                case BaseCommand.Type.Lookup: return cmd.LookupTopic.RequestId.ToString();
                case BaseCommand.Type.LookupResponse: return cmd.LookupTopicResponse.RequestId.ToString();
                case BaseCommand.Type.Unsubscribe: return cmd.Unsubscribe.RequestId.ToString();
                case BaseCommand.Type.Subscribe: return cmd.Subscribe.RequestId.ToString();
                case BaseCommand.Type.Success: return cmd.Success.RequestId.ToString();
                case BaseCommand.Type.Seek: return cmd.Seek.RequestId.ToString();
                case BaseCommand.Type.CloseConsumer: return cmd.CloseConsumer.RequestId.ToString();
                case BaseCommand.Type.GetLastMessageId: return cmd.GetLastMessageId.RequestId.ToString();
                case BaseCommand.Type.GetLastMessageIdResponse: return cmd.GetLastMessageIdResponse.RequestId.ToString();
                default: throw new ArgumentOutOfRangeException("CommandType", cmd.CommandType, "CommandType not supported as request/response type");
            }
        }
    }
}
