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
    using Abstractions;
    using HashDepot;
    using System.Threading;

    /// <summary>
    /// Round robin partition messages router
    /// If no key is provided, the producer will publish messages across all partitions in round-robin fashion
    /// to achieve maximum throughput. While if a key is specified on the message, the partitioned producer will
    /// hash the key and assign message to a particular partition.
    /// This is the default mode.
    /// </summary>
    public sealed class RoundRobinPartitionRouter : IMessageRouter
    {
        private int _partitionIndex = -1;

        /// <summary>
        /// Choose a partition in round robin routing mode
        /// </summary>
        public int ChoosePartition(MessageMetadata? messageMetadata, int partitionsCount)
        {
            var keyBytes = messageMetadata?.KeyBytes;
            if (keyBytes is not null)
                return (int) MurmurHash3.Hash32(keyBytes, 0) % partitionsCount;

            return Interlocked.Increment(ref _partitionIndex) % partitionsCount;
        }
    }
}
