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
    using System;
    using System.Text;

    /// <summary>
    /// If no key is provided, the producer will randomly pick one single partition and publish all the messages
    /// into that partition. While if a key is specified on the message, the partitioned producer will hash the
    /// key and assign message to a particular partition.
    /// </summary>
    public sealed class SinglePartitionRouter : IMessageRouter
    {
        private int? _partitionIndex;

        internal SinglePartitionRouter(int? partitionIndex = null)
        {
            _partitionIndex = partitionIndex;
        }

        /// <summary>
        /// Choose a partition in single partition routing mode
        /// </summary>
        public int ChoosePartition(MessageMetadata? messageMetadata, int partitionsCount)
        {
            if (messageMetadata != null && !string.IsNullOrEmpty(messageMetadata.Key))
            {
                return (int) MurmurHash3.Hash32(Encoding.UTF8.GetBytes(messageMetadata.Key ?? string.Empty), 0) % partitionsCount;
            }

            _partitionIndex ??= new Random().Next(0, partitionsCount);

            return _partitionIndex.Value;
        }
    }
}
