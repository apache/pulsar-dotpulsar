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

﻿using DotPulsar.Internal.Extensions;
using System;

namespace DotPulsar
{
    public sealed class MessageMetadata
    {
        public MessageMetadata() => Metadata = new Internal.PulsarApi.MessageMetadata();

        internal Internal.PulsarApi.MessageMetadata Metadata;

        public long DeliverAtTime
        {
            get => Metadata.DeliverAtTime;
            set => Metadata.DeliverAtTime = value;
        }

        public DateTimeOffset DeliverAtTimeAsDateTimeOffset
        {
            get => Metadata.GetDeliverAtTimeAsDateTimeOffset();
            set => Metadata.SetDeliverAtTime(value);
        }

        public ulong EventTime
        {
            get => Metadata.EventTime;
            set => Metadata.EventTime = value;
        }

        public DateTimeOffset EventTimeAsDateTimeOffset
        {
            get => Metadata.GetEventTimeAsDateTimeOffset();
            set => Metadata.SetEventTime(value);
        }

        public string? Key
        {
            get => Metadata.PartitionKey;
            set => Metadata.SetKey(value);
        }

        public byte[]? KeyBytes
        {
            get => Metadata.GetKeyAsBytes();
            set => Metadata.SetKey(value);
        }

        public byte[]? OrderingKey
        {
            get => Metadata.OrderingKey;
            set => Metadata.OrderingKey = value;
        }

        public string? this[string key]
        {
            get
            {
                for (var i = 0; i < Metadata.Properties.Count; ++i)
                {
                    var keyValye = Metadata.Properties[i];
                    if (keyValye.Key == key)
                        return keyValye.Value;
                }

                return null;
            }
            set
            {
                for (var i = 0; i < Metadata.Properties.Count; ++i)
                {
                    var keyValye = Metadata.Properties[i];
                    if (keyValye.Key != key)
                        continue;
                    keyValye.Value = value;
                    return;
                }

                Metadata.Properties.Add(new Internal.PulsarApi.KeyValue { Key = key, Value = value });
            }
        }

        public ulong SequenceId
        {
            get => Metadata.SequenceId;
            set => Metadata.SequenceId = value;
        }
    }
}
