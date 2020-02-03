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

﻿using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Exceptions;
using System;
using System.Net.Sockets;

namespace DotPulsar.Internal
{
    public sealed class FaultStrategy : IFaultStrategy
    {
        public FaultStrategy(TimeSpan retryInterval)
        {
            RetryInterval = retryInterval;
        }

        public TimeSpan RetryInterval { get; }

        public FaultAction DetermineFaultAction(Exception exception)
        {
            switch (exception)
            {
                case TooManyRequestsException _: return FaultAction.Retry;
                case StreamNotReadyException _: return FaultAction.Relookup;
                case ServiceNotReadyException _: return FaultAction.Relookup;
                case DotPulsarException _: return FaultAction.Fault;
                case SocketException socketException:
                    switch (socketException.SocketErrorCode)
                    {
                        case SocketError.HostNotFound:
                        case SocketError.HostUnreachable:
                        case SocketError.NetworkUnreachable:
                            return FaultAction.Fault;
                    }
                    break;
            }

            return FaultAction.Relookup;
        }
    }
}
