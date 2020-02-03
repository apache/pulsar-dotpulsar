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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Abstractions
{
    /// <summary>
    /// A consumer abstraction.
    /// </summary>
    public interface IConsumer : IStateChanged<ConsumerState>, IAsyncDisposable
    {
        /// <summary>
        /// Acknowledge the consumption of a single message.
        /// </summary>
        ValueTask Acknowledge(Message message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acknowledge the consumption of a single message using the MessageId.
        /// </summary>
        ValueTask Acknowledge(MessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acknowledge the consumption of all the messages in the topic up to and including the provided message.
        /// </summary>
        ValueTask AcknowledgeCumulative(Message message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acknowledge the consumption of all the messages in the topic up to and including the provided MessageId.
        /// </summary>
        ValueTask AcknowledgeCumulative(MessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the MessageId of the last message on the topic.
        /// </summary>
        ValueTask<MessageId> GetLastMessageId(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get an IAsyncEnumerable for consuming messages
        /// </summary>
        IAsyncEnumerable<Message> Messages(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reset the subscription associated with this consumer to a specific MessageId.
        /// </summary>
        ValueTask Seek(MessageId messageId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unsubscribe the consumer.
        /// </summary>
        ValueTask Unsubscribe(CancellationToken cancellationToken = default);
    }
}
