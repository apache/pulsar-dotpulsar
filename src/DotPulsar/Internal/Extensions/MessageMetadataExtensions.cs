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
using Metadata = DotPulsar.Internal.PulsarApi.MessageMetadata;

namespace DotPulsar.Internal.Extensions
{
    public static class MessageMetadataExtensions
    {
        public static DateTimeOffset GetDeliverAtTimeAsDateTimeOffset(this Metadata metadata)
            => DateTimeOffset.FromUnixTimeMilliseconds(metadata.DeliverAtTime);

        public static void SetDeliverAtTime(this Metadata metadata, DateTimeOffset timestamp)
            => metadata.DeliverAtTime = timestamp.ToUnixTimeMilliseconds();

        public static DateTimeOffset GetEventTimeAsDateTimeOffset(this Metadata metadata)
            => DateTimeOffset.FromUnixTimeMilliseconds((long)metadata.EventTime);

        public static void SetEventTime(this Metadata metadata, DateTimeOffset timestamp)
            => metadata.EventTime = (ulong)timestamp.ToUnixTimeMilliseconds();

        public static byte[]? GetKeyAsBytes(this Metadata metadata)
            => metadata.PartitionKeyB64Encoded ? Convert.FromBase64String(metadata.PartitionKey) : null;

        public static void SetKey(this Metadata metadata, string? key)
        {
            metadata.PartitionKey = key;
            metadata.PartitionKeyB64Encoded = false;
        }

        public static void SetKey(this Metadata metadata, byte[]? key)
        {
            if (key is null)
                return;

            metadata.PartitionKey = Convert.ToBase64String(key);
            metadata.PartitionKeyB64Encoded = true;
        }
    }
}
