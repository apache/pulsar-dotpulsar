﻿/*
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
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal.Abstractions
{
    public interface IConsumerChannel : IAsyncDisposable
    {
        Task Send(CommandAck command, CancellationToken cancellationToken);
        Task<CommandSuccess> Send(CommandUnsubscribe command, CancellationToken cancellationToken);
        Task<CommandSuccess> Send(CommandSeek command, CancellationToken cancellationToken);
        Task<CommandGetLastMessageIdResponse> Send(CommandGetLastMessageId command, CancellationToken cancellationToken);
        ValueTask<Message> Receive(CancellationToken cancellationToken);
    }
}
