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

namespace DotPulsar.Abstractions;

/// <summary>
/// A channel abstraction for producing messages without waiting for server acknowledgement.
/// </summary>
public interface ISendChannel<in TMessage>
{
    /// <summary>
    /// Sends a message with metadata and an optional callback invoked on server acknowledgement.
    /// </summary>
    ValueTask Send(MessageMetadata metadata, TMessage message, Func<MessageId, ValueTask>? onMessageSent = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Complete the channel blocking further incoming send operations.
    /// </summary>
    void Complete();

    /// <summary>
    /// Wait for server acknowledgement of all outstanding messages.
    /// </summary>
    ValueTask Completion(CancellationToken cancellationToken = default);
}
