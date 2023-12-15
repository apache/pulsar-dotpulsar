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
/// An abstraction for seeking.
/// </summary>
public interface ISeek
{
    /// <summary>
    /// Reset the cursor associated with the consumer or reader to a specific MessageId.
    /// </summary>
    ValueTask Seek(MessageId messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset the cursor associated with the consumer or reader to a specific message publish time using unix time in milliseconds.
    /// </summary>
    ValueTask Seek(ulong publishTime, CancellationToken cancellationToken = default);
}
