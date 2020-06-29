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

namespace DotPulsar
{
    /// <summary>
    /// The reader building options.
    /// </summary>
    public sealed class ReaderOptions
    {
        internal const uint DefaultMessagePrefetchCount = 1000;
        internal const bool DefaultReadCompacted = false;

        public ReaderOptions(MessageId startMessageId, string topic)
        {
            MessagePrefetchCount = DefaultMessagePrefetchCount;
            ReadCompacted = DefaultReadCompacted;
            StartMessageId = startMessageId;
            Topic = topic;
        }

        /// <summary>
        /// Set the reader name. This is optional.
        /// </summary>
        public string? ReaderName { get; set; }

        /// <summary>
        /// Number of messages that will be prefetched. The default is 1000.
        /// </summary>
        public uint MessagePrefetchCount { get; set; }

        /// <summary>
        /// Whether to read from the compacted topic. The default is 'false'.
        /// </summary>
        public bool ReadCompacted { get; set; }

        /// <summary>
        /// The initial reader position is set to the specified message id. This is required.
        /// </summary>
        public MessageId StartMessageId { get; set; }

        /// <summary>
        /// Set the topic for this reader. This is required.
        /// </summary>
        public string Topic { get; set; }
    }
}
