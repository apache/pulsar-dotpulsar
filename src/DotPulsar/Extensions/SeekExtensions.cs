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

/// <summary>
/// Extensions for ISeek.
/// </summary>
public static class SeekExtensions
{
    /// <summary>
    /// Reset the cursor associated with the consumer or reader to a specific message publish time using an UTC DateTime.
    /// </summary>
    public static async ValueTask Seek(this ISeek seeker, DateTime publishTime, CancellationToken cancellationToken = default)
        => await seeker.Seek((ulong) new DateTimeOffset(publishTime).ToUnixTimeMilliseconds(), cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Reset the cursor associated with the consumer or reader to a specific message publish time using a DateTimeOffset.
    /// </summary>
    public static async ValueTask Seek(this ISeek seeker, DateTimeOffset publishTime, CancellationToken cancellationToken = default)
        => await seeker.Seek((ulong) publishTime.ToUnixTimeMilliseconds(), cancellationToken).ConfigureAwait(false);
}
