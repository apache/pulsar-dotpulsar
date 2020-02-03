/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

﻿namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A reader building abstraction
    /// </summary>
    public interface IReaderBuilder
    {
        /// <summary>
        /// Set the reader name. This is optional.
        /// </summary>
        IReaderBuilder ReaderName(string name);

        /// <summary>
        /// Number of messages that will be prefetched. The default is 1000.
        /// </summary>
        IReaderBuilder MessagePrefetchCount(uint count);

        /// <summary>
        /// Whether to read from the compacted topic. The default is 'false'.
        /// </summary>
        IReaderBuilder ReadCompacted(bool readCompacted);

        /// <summary>
        /// The initial reader position is set to the specified message id. This is required.
        /// </summary>
        IReaderBuilder StartMessageId(MessageId messageId);

        /// <summary>
        /// Set the topic for this reader. This is required.
        /// </summary>
        IReaderBuilder Topic(string topic);

        /// <summary>
        /// Create the reader.
        /// </summary>
        IReader Create();
    }
}
