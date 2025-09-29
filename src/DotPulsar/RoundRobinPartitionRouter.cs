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

namespace DotPulsar;

using DotPulsar.Abstractions;
using DotPulsar.Internal;

/// <summary>
/// The round-robin partition messages router, which is the default router.
/// If a key is provided, the producer will hash the key and publish the message to a particular partition.
/// If a key is not provided, the producer will publish messages across all partitions in a round-robin fashion to achieve maximum throughput.
/// </summary>
public sealed class RoundRobinPartitionRouter : IMessageRouter
{
    private int _partitionIndex;

    /// <summary>
    /// Initializes a new instance of the round-robin partition router
    /// </summary>
    public RoundRobinPartitionRouter()
    {
        _partitionIndex = -1;
    }

    /// <summary>
    /// Choose a partition in round-robin routing mode
    /// </summary>
    public int ChoosePartition(MessageMetadata messageMetadata, int numberOfPartitions)
    {
        var keyBytes = messageMetadata.KeyBytes;
        if (keyBytes is not null && keyBytes.Length > 0)
            return (int) (MurmurHash3.Hash32(keyBytes, 0) % numberOfPartitions);

        return Interlocked.Increment(ref _partitionIndex) % numberOfPartitions;
    }
}
