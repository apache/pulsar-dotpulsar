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
/// An abstraction for getting the last message ids.
/// </summary>
public interface IGetLastMessageIds
{
    /// <summary>
    /// Get the message ids of the last messages on the topic. Non-partitioned topics will only return one message id.
    /// </summary>
    ValueTask<IEnumerable<MessageId>> GetLastMessageIds(CancellationToken cancellationToken = default);
}
