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
    using DotPulsar.Abstractions;
    using System;

    /// <summary>
    /// The producer building options.
    /// </summary>
    public sealed class ProducerOptions : ICloneable
    {
        internal const ulong DefaultInitialSequenceId = 0;
        internal const double DefaultAutoUpdatePartitionsIntervalInSecond = 60;

        public ProducerOptions(string topic)
        {
            InitialSequenceId = DefaultInitialSequenceId;
            Topic = topic;
            MessageRouter = new RoundRobinPartitionRouter();
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
        /// Set the message router. The default router is Round Robind routing mode.
        /// </summary>
        public IMessageRouter MessageRouter { get; set; }


        /// <summary>
        /// Whether to auto discover the partition configuration changes
        /// </summary>
        public bool AutoUpdatePartitions { get; set; }

        /// <summary>
        /// The interval of updating partitions
        /// </summary>
        public TimeSpan AutoUpdatePartitionsInterval { get; set; }

        public object Clone()
        {
            return new ProducerOptions(Topic)
            {
                ProducerName = ProducerName,
                InitialSequenceId = InitialSequenceId
            };
        }
    }
}
