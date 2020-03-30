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
    using System;
    using System.Buffers;
    using System.Threading.Tasks;
    using Abstractions;
    using Extensions;
    using PulsarApi;

    public sealed class ChannelManager : IDisposable
    {
        private readonly IdLookup<IChannel> _consumerChannels;
        private readonly IdLookup<IChannel> _producerChannels;

        public ChannelManager()
        {
            _consumerChannels = new IdLookup<IChannel>();
            _producerChannels = new IdLookup<IChannel>();
        }

        public bool HasChannels()
            => !_consumerChannels.IsEmpty() || !_producerChannels.IsEmpty();

        public Task<ProducerResponse> Outgoing(CommandProducer command, Task<BaseCommand> response, IChannel channel)
        {
            var producerId = _producerChannels.Add(channel);

            command.ProducerId = producerId;

            return response.ContinueWith(result =>
            {
                if (result.Result.CommandType == BaseCommand.Type.Error)
                {
                    _producerChannels.Remove(producerId);
                    result.Result.Error.Throw();
                }

                channel.Connected();

                return new ProducerResponse(producerId, result.Result.ProducerSuccess.ProducerName);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task<SubscribeResponse> Outgoing(CommandSubscribe command, Task<BaseCommand> response, IChannel channel)
        {
            var consumerId = _consumerChannels.Add(channel);

            command.ConsumerId = consumerId;

            return response.ContinueWith(result =>
            {
                if (result.Result.CommandType == BaseCommand.Type.Error)
                {
                    _consumerChannels.Remove(consumerId);
                    result.Result.Error.Throw();
                }

                channel.Connected();
                return new SubscribeResponse(consumerId);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void Outgoing(CommandCloseConsumer command, Task<BaseCommand> response)
        {
            var consumerId = command.ConsumerId;

            _ = response.ContinueWith(result =>
            {
                if (result.Result.CommandType == BaseCommand.Type.Success)
                    _consumerChannels.Remove(consumerId);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void Outgoing(CommandCloseProducer command, Task<BaseCommand> response)
        {
            var producerId = command.ProducerId;

            _ = response.ContinueWith(result =>
            {
                if (result.Result.CommandType == BaseCommand.Type.Success)
                    _producerChannels.Remove(producerId);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void Outgoing(CommandUnsubscribe command, Task<BaseCommand> response)
        {
            var consumerId = command.ConsumerId;

            _ = response.ContinueWith(result =>
            {
                if (result.Result.CommandType == BaseCommand.Type.Success)
                    _consumerChannels.Remove(consumerId)?.Unsubscribed();
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void Incoming(CommandCloseConsumer command)
            => _consumerChannels.Remove(command.ConsumerId)?.ClosedByServer();

        public void Incoming(CommandCloseProducer command)
            => _producerChannels.Remove(command.ProducerId)?.ClosedByServer();

        public void Incoming(CommandActiveConsumerChange command)
        {
            var channel = _consumerChannels[command.ConsumerId];

            if (channel is null)
                return;

            if (command.IsActive)
                channel.Activated();
            else
                channel.Deactivated();
        }

        public void Incoming(CommandReachedEndOfTopic command)
            => _consumerChannels[command.ConsumerId]?.ReachedEndOfTopic();

        public void Incoming(CommandMessage command, ReadOnlySequence<byte> data)
            => _consumerChannels[command.ConsumerId]?.Received(new MessagePackage(command.MessageId, data));

        public void Dispose()
        {
            foreach (var channel in _consumerChannels.RemoveAll())
                channel.Disconnected();

            foreach (var channel in _producerChannels.RemoveAll())
                channel.Disconnected();
        }
    }
}
