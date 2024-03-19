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

using DotPulsar.Abstractions;
using DotPulsar.Internal.PulsarApi;

public interface IConsumerChannel<TMessage> : IAsyncDisposable
{
    ValueTask<bool> HasMessageAvailable(CancellationToken cancellationToken);
    Task Send(CommandAck command, CancellationToken cancellationToken);
    Task Send(CommandRedeliverUnacknowledgedMessages command, CancellationToken cancellationToken);
    Task Send(CommandUnsubscribe command, CancellationToken cancellationToken);
    Task Send(CommandSeek command, CancellationToken cancellationToken);
    Task<MessageId> Send(CommandGetLastMessageId command, CancellationToken cancellationToken);
    ValueTask<IMessage<TMessage>> Receive(CancellationToken cancellationToken);
    ValueTask ClosedByClient(CancellationToken cancellationToken);
}
