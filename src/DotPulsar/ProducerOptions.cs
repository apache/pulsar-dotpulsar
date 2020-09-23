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
    using System;

    /// <summary>
    /// The producer building options.
    /// </summary>
    public sealed class ProducerOptions
    {
        internal const ulong DefaultInitialSequenceId = 0;
        internal const int DefaultMaxMessagesPerBatch = 1000;
        internal const int DefaultMaxBathingBytes = 128 * 1024;
        internal readonly static TimeSpan DefaultMaxPublishDelay = TimeSpan.FromMilliseconds(1);

        public ProducerOptions(string topic)
        {
            InitialSequenceId = DefaultInitialSequenceId;
            Topic = topic;
        }

        /// <summary>
        /// Set the producer name. This is optional.
        /// </summary>
        public string? ProducerName { get; set; }

        /// <summary>
        /// Set the initial sequence id. The default is 0.
        /// </summary>
        public ulong InitialSequenceId { get; set; }

        /// <summary>
        /// Set the topic for this producer. This is required.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Set the maximum number of messages permitted in a batch. The default is 1000.
        /// </summary>
        public int BatchingMaxMessagesPerBatch { get; set; }

        /// <summary>
        /// Set the time period within which the messages sent will be batched. The default is 1 ms.
        /// </summary>
        public TimeSpan BatchingMaxPublishDelay { get; set; }

        /// <summary>
        /// Set the maximum number of bytes permitted in a batch. The default is 128KB.
        /// </summary>
        public int BatchingMaxBytes { get; set; }

        /// <summary>
        /// Control whether automatic batching of messages is enabled for the producer. The default is 'false'.
        /// </summary>
        public bool BatchingEnabled { get; set; }
    }
}
