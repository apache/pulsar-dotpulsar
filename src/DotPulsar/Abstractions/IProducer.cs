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

﻿using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A producer abstraction.
    /// </summary>
    public interface IProducer : IStateChanged<ProducerState>, IAsyncDisposable
    {
        /// <summary>
        /// Sends a message.
        /// </summary>
        ValueTask<MessageId> Send(byte[] data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message.
        /// </summary>
        ValueTask<MessageId> Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message.
        /// </summary>
        ValueTask<MessageId> Send(ReadOnlySequence<byte> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message with metadata.
        /// </summary>
        ValueTask<MessageId> Send(MessageMetadata metadata, byte[] data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message with metadata.
        /// </summary>
        ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a message with metadata.
        /// </summary>
        ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlySequence<byte> data, CancellationToken cancellationToken = default);
    }
}
