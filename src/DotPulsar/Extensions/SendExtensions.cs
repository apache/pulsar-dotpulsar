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

namespace DotPulsar.Extensions;

using DotPulsar.Abstractions;
using DotPulsar.Internal;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Extensions for ISend.
/// </summary>
public static class SendExtensions
{
    /// <summary>
    /// Sends a message.
    /// </summary>
    public static async ValueTask<MessageId> Send(this ISend<ReadOnlySequence<byte>> sender, byte[] data, CancellationToken cancellationToken = default)
        => await Send(sender, new ReadOnlySequence<byte>(data), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends a message.
    /// </summary>
    public static async ValueTask<MessageId> Send(this ISend<ReadOnlySequence<byte>> sender, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        => await Send(sender, new ReadOnlySequence<byte>(data), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends a message with metadata.
    /// </summary>
    public static async ValueTask<MessageId> Send(this ISend<ReadOnlySequence<byte>> sender, MessageMetadata metadata, byte[] data, CancellationToken cancellationToken = default)
        => await sender.Send(metadata, new ReadOnlySequence<byte>(data), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends a message with metadata.
    /// </summary>
    public static async ValueTask<MessageId> Send(this ISend<ReadOnlySequence<byte>> sender, MessageMetadata metadata, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        => await sender.Send(metadata, new ReadOnlySequence<byte>(data), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends a message without metadata.
    /// </summary>
    public static async ValueTask<MessageId> Send<TMessage>(this ISend<TMessage> sender, TMessage message, CancellationToken cancellationToken = default)
    {
        var metadata = MessageMetadataObjectPool.Get();

        try
        {
            return await sender.Send(metadata, message, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            MessageMetadataObjectPool.Return(metadata);
        }
    }
}
