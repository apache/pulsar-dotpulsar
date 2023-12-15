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

namespace DotPulsar.Extensions;

using DotPulsar.Abstractions;
using DotPulsar.Internal;
using System.Buffers;

/// <summary>
/// Extensions for ISendChannel.
/// </summary>
public static class SendChannelExtensions
{
    /// <summary>
    /// Sends a message.
    /// </summary>
    public static async ValueTask Send(this ISendChannel<ReadOnlySequence<byte>> sender, byte[] data, Func<MessageId, ValueTask>? onMessageSent = default, CancellationToken cancellationToken = default)
        => await Send(sender, new ReadOnlySequence<byte>(data), onMessageSent, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends a message.
    /// </summary>
    public static async ValueTask Send(this ISendChannel<ReadOnlySequence<byte>> sender, ReadOnlyMemory<byte> data, Func<MessageId, ValueTask>? onMessageSent = default, CancellationToken cancellationToken = default)
        => await Send(sender, new ReadOnlySequence<byte>(data), onMessageSent, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends a message with metadata.
    /// </summary>
    public static async ValueTask Send(this ISendChannel<ReadOnlySequence<byte>> sender, MessageMetadata metadata, byte[] data, Func<MessageId, ValueTask>? onMessageSent = default, CancellationToken cancellationToken = default)
        => await sender.Send(metadata, new ReadOnlySequence<byte>(data), onMessageSent, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends a message with metadata.
    /// </summary>
    public static async ValueTask Send(this ISendChannel<ReadOnlySequence<byte>> sender, MessageMetadata metadata, ReadOnlyMemory<byte> data, Func<MessageId, ValueTask>? onMessageSent = default, CancellationToken cancellationToken = default)
        => await sender.Send(metadata, new ReadOnlySequence<byte>(data), onMessageSent, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Sends a message without metadata.
    /// </summary>
    public static async ValueTask Send<TMessage>(this ISendChannel<TMessage> sender, TMessage message, Func<MessageId, ValueTask>? onMessageSent = default, CancellationToken cancellationToken = default)
    {
        var metadata = MessageMetadataObjectPool.Get();

        async ValueTask ReleaseMetadataAndCallCallback(MessageId id)
        {
            MessageMetadataObjectPool.Return(metadata);

            if (onMessageSent is not null)
                await onMessageSent(id).ConfigureAwait(false);
        }

        await sender.Send(metadata, message, ReleaseMetadataAndCallCallback, cancellationToken).ConfigureAwait(false);
    }
}
