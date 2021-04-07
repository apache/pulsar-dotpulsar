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
    using System.Text;
    using System.Threading;

    public class RoundRobinPartitionRouter : IMessageRouter
    {
        private int _partitionIndex = 0;

        public int ChoosePartition(MessageMetadata? messageMetadata, int partitionsCount)
        {
            if (messageMetadata != null && !string.IsNullOrEmpty(messageMetadata.Key))
            {
                return (int) MurmurHash3.Hash32(Encoding.UTF8.GetBytes(messageMetadata.Key ?? string.Empty), 0) % partitionsCount;
            }

            return Interlocked.Increment(ref _partitionIndex) % partitionsCount;
        }
    }
}
