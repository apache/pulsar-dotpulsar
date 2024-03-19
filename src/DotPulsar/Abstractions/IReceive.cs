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
/// An abstraction for receiving a single message.
/// </summary>
public interface IReceive<TMessage>
{
    /// <summary>
    /// Return true if the topic was terminated and this reader has reached the end of the topic
    /// </summary>
    /// <remarks>
    /// Note that this only applies to a "terminated" topic (where the topic is "sealed" and no
    /// more messages can be published) and not just that the reader is simply caught up with
    /// the publishers. Use <see cref="HasMessageAvailable" /> to check for that.
    /// </remarks>
    bool HasReachedEndOfTopic();

    /// <summary>
    /// Check if there is any message available to read from the current position
    /// </summary>
    /// <remarks>
    /// This check can be used by an application to scan through a topic and
    /// stop when the reader reaches the current last published message.
    /// </remarks>
    ValueTask<bool> HasMessageAvailable(CancellationToken cancellationToken = default);

    /// <summary>
    /// Receive a single message.
    /// </summary>
    ValueTask<TMessage> Receive(CancellationToken cancellationToken = default);
}
