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
    using Abstractions;
    using DotPulsar.Abstractions;
    using Exceptions;
    using PulsarApi;
    using System;
    using System.Buffers;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class NotReadyChannel<TMessage> : IConsumerChannel<TMessage>, IProducerChannel
    {
        public ValueTask DisposeAsync()
            => new();

        public ValueTask ClosedByClient(CancellationToken cancellationToken)
            => new();

        public ValueTask<IMessage<TMessage>> Receive(string topic, CancellationToken cancellationToken = default)
            => throw GetException();

        public Task<CommandSendReceipt> Send(MessageMetadata metadata, ReadOnlySequence<byte> payload, CancellationToken cancellationToken)
            => throw GetException();

        public Task Send(CommandAck command, CancellationToken cancellationToken)
            => throw GetException();

        public Task Send(CommandRedeliverUnacknowledgedMessages command, CancellationToken cancellationToken)
            => throw GetException();

        public Task Send(CommandUnsubscribe command, CancellationToken cancellationToken)
            => throw GetException();

        public Task Send(CommandSeek command, CancellationToken cancellationToken)
            => throw GetException();

        public Task<MessageId> Send(string topic, CommandGetLastMessageId command, CancellationToken cancellationToken)
            => throw GetException();

        private static Exception GetException()
            => new ChannelNotReadyException();
    }
}
