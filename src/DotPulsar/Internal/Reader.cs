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

﻿using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class Reader : IReader
    {
        private readonly IConsumerStreamFactory _streamFactory;
        private readonly IFaultStrategy _faultStrategy;
        private readonly StateManager<ReaderState> _stateManager;
        private readonly CancellationTokenSource _connectTokenSource;
        private readonly Task _connectTask;
        private Action _throwIfClosedOrFaulted;
        private IConsumerStream Stream { get; set; }

        public Reader(IConsumerStreamFactory streamFactory, IFaultStrategy faultStrategy)
        {
            _stateManager = new StateManager<ReaderState>(ReaderState.Disconnected, ReaderState.Closed, ReaderState.ReachedEndOfTopic, ReaderState.Faulted);
            _streamFactory = streamFactory;
            _faultStrategy = faultStrategy;
            _connectTokenSource = new CancellationTokenSource();
            Stream = new NotReadyStream();
            _connectTask = Connect(_connectTokenSource.Token);
            _throwIfClosedOrFaulted = () => { };
        }

        public async ValueTask<ReaderState> StateChangedTo(ReaderState state, CancellationToken cancellationToken) => await _stateManager.StateChangedTo(state, cancellationToken);
        public async ValueTask<ReaderState> StateChangedFrom(ReaderState state, CancellationToken cancellationToken) => await _stateManager.StateChangedFrom(state, cancellationToken);
        public bool IsFinalState() => _stateManager.IsFinalState();
        public bool IsFinalState(ReaderState state) => _stateManager.IsFinalState(state);

        public async ValueTask DisposeAsync()
        {
            _connectTokenSource.Cancel();
            await _connectTask;
        }

        public async IAsyncEnumerable<Message> Messages([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Message message;

                try
                {
                    message = await Stream.Receive(cancellationToken);
                }
                catch (Exception exception)
                {
                    if (!cancellationToken.IsCancellationRequested)
                        await OnException(exception, cancellationToken);

                    continue;
                }

                yield return message;
            }
        }

        private async Task OnException(Exception exception, CancellationToken cancellationToken)
        {
            _throwIfClosedOrFaulted();

            switch (_faultStrategy.DetermineFaultAction(exception))
            {
                case FaultAction.Retry:
                    await Task.Delay(_faultStrategy.RetryInterval, cancellationToken);
                    break;
                case FaultAction.Relookup:
                    await _stateManager.StateChangedTo(ReaderState.Connected, cancellationToken);
                    break;
                case FaultAction.Fault:
                    HasFaulted(exception);
                    break;
            }

            _throwIfClosedOrFaulted();
        }

        private void HasFaulted(Exception exception)
        {
            _throwIfClosedOrFaulted = () => throw exception;
            _stateManager.SetState(ReaderState.Faulted);
        }

        private void HasClosed()
        {
            _throwIfClosedOrFaulted = () => throw new ReaderClosedException();
            _stateManager.SetState(ReaderState.Closed);
        }

        private async Task Connect(CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                while (true)
                {
                    using (var proxy = new ReaderProxy(_stateManager, new AsyncQueue<MessagePackage>()))
                    await using (Stream = await _streamFactory.CreateStream(proxy, cancellationToken))
                    {
                        proxy.Active();
                        await _stateManager.StateChangedFrom(ReaderState.Connected, cancellationToken);
                        if (_stateManager.IsFinalState())
                            return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                HasClosed();
            }
            catch (Exception exception)
            {
                HasFaulted(exception);
            }
        }
    }
}
