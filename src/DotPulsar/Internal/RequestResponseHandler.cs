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
    using PulsarApi;
    using System;
    using System.Threading.Tasks;

    public sealed class RequestResponseHandler : IDisposable
    {
        private const string _connectResponseIdentifier = "Connected";

        private readonly Awaiter<string, BaseCommand> _responses;
        private readonly RequestId _requestId;

        public RequestResponseHandler()
        {
            _responses = new Awaiter<string, BaseCommand>();
            _requestId = new RequestId();
        }

        public void Dispose()
            => _responses.Dispose();

        public Task<BaseCommand> Outgoing(BaseCommand command)
        {
            SetRequestId(command);
            return _responses.CreateTask(GetResponseIdentifier(command));
        }

        public void Incoming(BaseCommand command)
        {
            var identifier = GetResponseIdentifier(command);

            if (identifier is not null)
                _responses.SetResult(identifier, command);
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
                case BaseCommand.Type.PartitionedMetadata:
                    cmd.PartitionMetadata.RequestId = _requestId.FetchNext();
                    return;
            }
        }

        private string GetResponseIdentifier(BaseCommand cmd)
            => cmd.CommandType switch
            {
                BaseCommand.Type.Connect => _connectResponseIdentifier,
                BaseCommand.Type.Connected => _connectResponseIdentifier,
                BaseCommand.Type.Send => $"{cmd.Send.ProducerId}-{cmd.Send.SequenceId}",
                BaseCommand.Type.SendError => $"{cmd.SendError.ProducerId}-{cmd.SendError.SequenceId}",
                BaseCommand.Type.SendReceipt => $"{cmd.SendReceipt.ProducerId}-{cmd.SendReceipt.SequenceId}",
                BaseCommand.Type.Error => !_requestId.IsPastInitialId() ? _connectResponseIdentifier : cmd.Error.RequestId.ToString(),
                BaseCommand.Type.Producer => cmd.Producer.RequestId.ToString(),
                BaseCommand.Type.ProducerSuccess => cmd.ProducerSuccess.RequestId.ToString(),
                BaseCommand.Type.CloseProducer => cmd.CloseProducer.RequestId.ToString(),
                BaseCommand.Type.PartitionedMetadata => cmd.PartitionMetadata.RequestId.ToString(),
                BaseCommand.Type.PartitionedMetadataResponse => cmd.PartitionMetadataResponse.RequestId.ToString(),
                BaseCommand.Type.Lookup => cmd.LookupTopic.RequestId.ToString(),
                BaseCommand.Type.LookupResponse => cmd.LookupTopicResponse.RequestId.ToString(),
                BaseCommand.Type.Unsubscribe => cmd.Unsubscribe.RequestId.ToString(),
                BaseCommand.Type.Subscribe => cmd.Subscribe.RequestId.ToString(),
                BaseCommand.Type.Success => cmd.Success.RequestId.ToString(),
                BaseCommand.Type.Seek => cmd.Seek.RequestId.ToString(),
                BaseCommand.Type.CloseConsumer => cmd.CloseConsumer.RequestId.ToString(),
                BaseCommand.Type.GetLastMessageId => cmd.GetLastMessageId.RequestId.ToString(),
                BaseCommand.Type.GetLastMessageIdResponse => cmd.GetLastMessageIdResponse.RequestId.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(cmd.CommandType), cmd.CommandType, "CommandType not supported as request/response type")
            };
    }
}
