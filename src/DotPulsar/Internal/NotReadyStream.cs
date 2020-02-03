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

﻿using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Exceptions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class NotReadyStream : IConsumerStream, IProducerStream
    {
        public ValueTask DisposeAsync() => new ValueTask();

        public ValueTask<Message> Receive(CancellationToken cancellationToken) => throw GetException();

        public Task Send(CommandAck command) => throw GetException();

        public Task<CommandSuccess> Send(CommandUnsubscribe command) => throw GetException();

        public Task<CommandSuccess> Send(CommandSeek command) => throw GetException();

        public Task<CommandGetLastMessageIdResponse> Send(CommandGetLastMessageId command) => throw GetException();

        public Task<CommandSendReceipt> Send(byte[] payload) => throw GetException();

        public Task<CommandSendReceipt> Send(ReadOnlyMemory<byte> payload) => throw GetException();

        public Task<CommandSendReceipt> Send(ReadOnlySequence<byte> payload) => throw GetException();

        public Task<CommandSendReceipt> Send(PulsarApi.MessageMetadata metadata, byte[] payload) => throw GetException();

        public Task<CommandSendReceipt> Send(PulsarApi.MessageMetadata metadata, ReadOnlyMemory<byte> payload) => throw GetException();

        public Task<CommandSendReceipt> Send(PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> payload) => throw GetException();

        private Exception GetException() => new StreamNotReadyException();
    }
}
