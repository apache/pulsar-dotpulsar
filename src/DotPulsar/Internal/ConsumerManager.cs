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
using DotPulsar.Internal.PulsarApi;
using System;
using System.Buffers;

namespace DotPulsar.Internal
{
    public sealed class ConsumerManager : IDisposable
    {
        private readonly IdLookup<IConsumerProxy> _proxies;

        public ConsumerManager() => _proxies = new IdLookup<IConsumerProxy>();

        public bool HasConsumers => !_proxies.IsEmpty();

        public void Outgoing(CommandSubscribe subscribe, IConsumerProxy proxy) => subscribe.ConsumerId = _proxies.Add(proxy);

        public void Dispose()
        {
            foreach (var id in _proxies.AllIds())
            {
                RemoveConsumer(id);
            }
        }

        public void Incoming(CommandMessage message, ReadOnlySequence<byte> data)
        {
            var proxy = _proxies[message.ConsumerId];
            proxy?.Enqueue(new MessagePackage(message.MessageId, data));
        }

        public void Incoming(CommandCloseConsumer command) => RemoveConsumer(command.ConsumerId);

        public void Incoming(CommandActiveConsumerChange command)
        {
            var proxy = _proxies[command.ConsumerId];
            if (proxy is null) return;

            if (command.IsActive)
                proxy.Active();
            else
                proxy.Inactive();
        }

        public void Incoming(CommandReachedEndOfTopic command)
        {
            var proxy = _proxies[command.ConsumerId];
            proxy?.ReachedEndOfTopic();
        }

        public void Remove(ulong consumerId) => _proxies.Remove(consumerId);

        private void RemoveConsumer(ulong consumerId)
        {
            var proxy = _proxies[consumerId];
            if (proxy is null) return;
            proxy.Disconnected();
            _proxies.Remove(consumerId);
        }
    }
}
