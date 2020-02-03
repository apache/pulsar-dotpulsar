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
using DotPulsar.Internal.Extensions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Buffers;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class ProducerStream : IProducerStream
    {
        private readonly PulsarApi.MessageMetadata _cachedMetadata;
        private readonly SendPackage _cachedSendPackage;
        private readonly ulong _id;
        private readonly SequenceId _sequenceId;
        private readonly Connection _connection;
        private readonly IFaultStrategy _faultStrategy;
        private readonly IProducerProxy _proxy;

        public ProducerStream(ulong id, string name, SequenceId sequenceId, Connection connection, IFaultStrategy faultStrategy, IProducerProxy proxy)
        {
            _cachedMetadata = new PulsarApi.MessageMetadata
            {
                ProducerName = name
            };

            var commandSend = new CommandSend
            {
                ProducerId = id,
                NumMessages = 1
            };

            _cachedSendPackage = new SendPackage(commandSend, _cachedMetadata);

            _id = id;
            _sequenceId = sequenceId;
            _connection = connection;
            _faultStrategy = faultStrategy;
            _proxy = proxy;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _connection.Send(new CommandCloseProducer { ProducerId = _id });
            }
            catch
            {
                // Ignore
            }
        }

        public async Task<CommandSendReceipt> Send(ReadOnlySequence<byte> payload)
        {
            _cachedSendPackage.Metadata = _cachedMetadata;
            _cachedSendPackage.Payload = payload;
            return await SendPackage(true);
        }

        public async Task<CommandSendReceipt> Send(PulsarApi.MessageMetadata metadata, ReadOnlySequence<byte> payload)
        {
            metadata.ProducerName = _cachedMetadata.ProducerName;
            _cachedSendPackage.Metadata = metadata;
            _cachedSendPackage.Payload = payload;
            return await SendPackage(metadata.SequenceId == 0);
        }

        private async Task<CommandSendReceipt> SendPackage(bool autoAssignSequenceId)
        {
            try
            {
                _cachedSendPackage.Metadata.PublishTime = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                if (autoAssignSequenceId)
                {
                    _cachedSendPackage.Command.SequenceId = _sequenceId.Current;
                    _cachedSendPackage.Metadata.SequenceId = _sequenceId.Current;
                }
                else
                    _cachedSendPackage.Command.SequenceId = _cachedSendPackage.Metadata.SequenceId;

                var response = await _connection.Send(_cachedSendPackage);
                response.Expect(BaseCommand.Type.SendReceipt);

                if (autoAssignSequenceId)
                    _sequenceId.Increment();

                return response.SendReceipt;
            }
            catch (Exception exception)
            {
                if (_faultStrategy.DetermineFaultAction(exception) == FaultAction.Relookup)
                    _proxy.Disconnected();

                throw;
            }
            finally
            {
                if (autoAssignSequenceId)
                    _cachedSendPackage.Metadata.SequenceId = 0; // Reset in case the user reuse the MessageMetadata, but is not explicitly setting the sequenceId
            }
        }
    }
}
