﻿/*
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

    /// <summary>
    /// The producer building options.
    /// </summary>
    public sealed class ProducerOptions
    {
        /// <summary>
        /// The default initial sequence id.
        /// </summary>
        public static readonly ulong DefaultInitialSequenceId = 0;

        /// <summary>
        /// Initializes a new instance using the specified topic.
        /// </summary>
        public ProducerOptions(string topic)
        {
            InitialSequenceId = DefaultInitialSequenceId;
            Topic = topic;
        }

        /// <summary>
        /// Set the initial sequence id. The default is 0.
        /// </summary>
        public ulong InitialSequenceId { get; set; }

        /// <summary>
        /// Set the producer name. This is optional.
        /// </summary>
        public string? ProducerName { get; set; }

        /// <summary>
        /// Register a state changed handler. This is optional.
        /// </summary>
        public IHandleStateChanged<ProducerStateChanged>? StateChangedHandler { get; set; }

        /// <summary>
        /// Set the topic for this producer. This is required.
        /// </summary>
        public string Topic { get; set; }
    }
}
