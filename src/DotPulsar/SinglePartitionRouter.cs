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

    /// <summary>
    /// The single partition messages router.
    /// If a key is provided, the producer will hash the key and publish the message to a particular partition.
    /// If a key is not provided, the producer will randomly pick one single partition and publish all messages to that partition.
    /// </summary>
    public sealed class SinglePartitionRouter : IMessageRouter
    {
        private int _partitionIndex;

        /// <summary>
        /// Initializes a new instance of the single partition router that will randomly select a partition
        /// </summary>
        public SinglePartitionRouter()
        {
            _partitionIndex = -1;
        }

        /// <summary>
        /// Initializes a new instance of the single partition router that will publish all messages to the given partition
        /// </summary>
        public SinglePartitionRouter(int partitionIndex)
        {
            _partitionIndex = partitionIndex;
        }

        /// <summary>
        /// Choose a partition in single partition routing mode
        /// </summary>
        public int ChoosePartition(MessageMetadata messageMetadata, int numberOfPartitions)
        {
            var keyBytes = messageMetadata.KeyBytes;
            if (keyBytes is not null && keyBytes.Length > 0)
                return (int) MurmurHash3.Hash32(keyBytes, 0) % numberOfPartitions;

            if (_partitionIndex == -1)
                _partitionIndex = new Random().Next(0, numberOfPartitions);

            return _partitionIndex;
        }
    }
}
