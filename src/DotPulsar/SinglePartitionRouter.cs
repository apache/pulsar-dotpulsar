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
    using DotPulsar.Internal;
    using System;

    public sealed class SinglePartitionRouter : MessageRouter
    {
        private int? _partitionIndex;

        public SinglePartitionRouter(int? partitionIndex = null)
        {
            _partitionIndex = partitionIndex;
        }

        public override int ChoosePartition(MessageMetadata? messageMetadata, PartitionedTopicMetadata partitionedTopic)
        {
            if (messageMetadata != null && !string.IsNullOrEmpty(messageMetadata.Key))
            {
                return SignSafeMod(Murmur3_32Hash.Instance.MakeHash(messageMetadata.Key!), partitionedTopic.Partitions);
            }
            if (_partitionIndex == null)
            {
                _partitionIndex = new Random().Next(0, partitionedTopic.Partitions);
            }
            return (int) _partitionIndex;
        }
    }
}
