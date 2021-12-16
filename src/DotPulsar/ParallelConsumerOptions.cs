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
    /// Parallel consumer building options.
    /// </summary>
    public class ParallelConsumerOptions : ConsumerOptions
    {
        /// <summary>
        /// The default number of partitions to prefetch messages from.
        /// </summary>
        public static readonly uint DefaultPartitionPrefetchCount = 2;

        /// <summary>
        /// The default max number of parallel workers to use when processing messages.
        /// </summary>
        public static readonly uint DefaultMaxParallelWorkersCount = 10;

        public ParallelConsumerOptions(string subscriptionName, string topic)
            : base(subscriptionName, topic)
        {
            PartitionPrefetchCount = DefaultPartitionPrefetchCount;
            MaxParallelWorkersCount = DefaultMaxParallelWorkersCount;
        }

        public ParallelConsumerOptions(ParallelConsumerOptions options, string topic, uint messagePrefetchCount)
            : base(options, topic, messagePrefetchCount)
        {
            PartitionPrefetchCount = options.PartitionPrefetchCount;
            MaxParallelWorkersCount = options.MaxParallelWorkersCount;
        }

        /// <summary>
        /// Number of partitions to prefetch messages from.
        /// Number of messages which will be prefetched is MessagePrefetchCount / PartitionPrefetchCount.
        /// </summary>
        public uint PartitionPrefetchCount { get; set; }

        /// <summary>
        /// Max number of parallel workers to use when processing messages.
        /// </summary>
        public uint MaxParallelWorkersCount { get; set; }
    }
}
