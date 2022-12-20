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

namespace DotPulsar.Abstractions;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// An abstraction for sending a message.
/// </summary>
public interface ISend<TMessage>
{
    /// <summary>
    /// Sends a message with metadata.
    /// </summary>
    /// <returns>ValueTask which completes when the Broker acknowledgement has been received.</returns>
    ValueTask<MessageId> Send(MessageMetadata metadata, TMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enqueue a message to be sent.
    /// </summary>
    /// <returns>ValueTask which completes when the message is added to the send queue.</returns>
    ValueTask Enqueue(MessageMetadata metadata, TMessage message, Func<MessageId, ValueTask>? onMessageSent = default, CancellationToken cancellationToken = default);
}
