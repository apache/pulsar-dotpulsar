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

﻿namespace DotPulsar
{
    public sealed class ConsumerOptions
    {
        internal const SubscriptionInitialPosition DefaultInitialPosition = SubscriptionInitialPosition.Latest;
        internal const uint DefaultMessagePrefetchCount = 1000;
        internal const int DefaultPriorityLevel = 0;
        internal const bool DefaultReadCompacted = false;
        internal const SubscriptionType DefaultSubscriptionType = SubscriptionType.Exclusive;

        public ConsumerOptions(string subscriptionName, string topic)
        {
            InitialPosition = DefaultInitialPosition;
            PriorityLevel = DefaultPriorityLevel;
            MessagePrefetchCount = DefaultMessagePrefetchCount;
            ReadCompacted = DefaultReadCompacted;
            SubscriptionType = DefaultSubscriptionType;
            SubscriptionName = subscriptionName;
            Topic = topic;
        }

        public string? ConsumerName { get; set; }
        public SubscriptionInitialPosition InitialPosition { get; set; }
        public int PriorityLevel { get; set; }
        public uint MessagePrefetchCount { get; set; }
        public bool ReadCompacted { get; set; }
        public string SubscriptionName { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public string Topic { get; set; }
    }
}
