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

using DotPulsar.Internal;

/// <summary>
/// Extensions for MessageMetadata.
/// </summary>
public static class MessageMetadataExtensions
{
    /// <summary>
    /// Set the conversation id.
    /// </summary>
    public static void SetConversationId(this MessageMetadata messageMetadata, string conversationId)
        => messageMetadata[Constants.ConversationId] = conversationId;

    /// <summary>
    /// Get the conversation id.
    /// </summary>
    public static string? GetConversationId(this MessageMetadata messageMetadata)
        => messageMetadata[Constants.ConversationId];
}
