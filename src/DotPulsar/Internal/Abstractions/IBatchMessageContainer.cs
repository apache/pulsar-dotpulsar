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

namespace DotPulsar.Internal.Abstractions
{
    using System.Collections.Generic;

    interface IBatchMessageContainer
    {
        MessageMetadata MessageMetadata { get; }
        Queue<Message> Messages { get; }

        /// <summary>
        /// Add message to the batch message container.
        /// </summary>
        /// <param name="message">The message will be added to the batch message container</param>
        /// <returns>True if the batch is full, otherwise false</returns>
        bool Add(Message message);

        /// <summary>
        /// Check the batch message container have enough space for the message want to add.
        /// </summary>
        /// <param name="message">the message want to add</param>
        /// <returns>return true if the container have enough space for the specific message, otherwise return false.</returns>
        bool HaveEnoughSpace(Message message);

        /// <summary>
        /// Get batched messages and metadata in this container.
        /// It will automatically clear the container.
        /// If the container is empty, return (null,null).
        /// </summary>
        (Queue<Message>?, MessageMetadata?) GetBatchedMessagesAndMetadata();

        /// <summary>
        /// Check the message batch container is empty.
        /// </summary>
        /// <returns>return true if empty, otherwise return false.</returns>
        bool IsEmpty();

        /// <summary>
        /// Clear the message batch container.
        /// </summary>
        void Clear();
    }
}
