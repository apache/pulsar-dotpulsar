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

namespace DotPulsar.Internal.Abstractions;

using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading;
using System.Threading.Tasks;

public interface IConnection : IAsyncDisposable
{
    ValueTask<bool> HasChannels(CancellationToken cancellationToken);

    Task<ProducerResponse> Send(CommandProducer command, IChannel channel, CancellationToken cancellationToken);
    Task<SubscribeResponse> Send(CommandSubscribe command, IChannel channel, CancellationToken cancellationToken);

    Task Send(CommandPing command, CancellationToken cancellationToken);
    Task Send(CommandPong command, CancellationToken cancellationToken);
    Task Send(CommandAck command, CancellationToken cancellationToken);
    Task Send(CommandFlow command, CancellationToken cancellationToken);
    Task Send(CommandRedeliverUnacknowledgedMessages command, CancellationToken cancellationToken);

    Task<BaseCommand> Send(CommandUnsubscribe command, CancellationToken cancellationToken);
    Task<BaseCommand> Send(CommandConnect command, CancellationToken cancellationToken);
    Task<BaseCommand> Send(CommandLookupTopic command, CancellationToken cancellationToken);
    Task<BaseCommand> Send(CommandSeek command, CancellationToken cancellationToken);
    Task<BaseCommand> Send(CommandGetLastMessageId command, CancellationToken cancellationToken);
    Task<BaseCommand> Send(CommandCloseProducer command, CancellationToken cancellationToken);
    Task<BaseCommand> Send(CommandCloseConsumer command, CancellationToken cancellationToken);
    Task Send(SendPackage command, TaskCompletionSource<BaseCommand> responseTcs, CancellationToken cancellationToken);
    Task<BaseCommand> Send(CommandGetOrCreateSchema command, CancellationToken cancellationToken);
    Task<BaseCommand> Send(CommandPartitionedTopicMetadata command, CancellationToken cancellationToken);

    void MarkInactive();
}
