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
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class Producer : IProducer
    {
        private readonly Executor _executor;
        private readonly IProducerStreamFactory _streamFactory;
        private readonly IFaultStrategy _faultStrategy;
        private readonly StateManager<ProducerState> _stateManager;
        private readonly CancellationTokenSource _connectTokenSource;
        private readonly Task _connectTask;
        private Action _throwIfClosedOrFaulted;
        private IProducerStream Stream { get; set; }

        public Producer(IProducerStreamFactory streamFactory, IFaultStrategy faultStrategy)
        {
            _executor = new Executor(ExecutorOnException);
            _streamFactory = streamFactory;
            _faultStrategy = faultStrategy;
            _stateManager = new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);
            _connectTokenSource = new CancellationTokenSource();
            Stream = new NotReadyStream();
            _connectTask = Connect(_connectTokenSource.Token);
            _throwIfClosedOrFaulted = () => { };
        }

        public async ValueTask<ProducerState> StateChangedTo(ProducerState state, CancellationToken cancellationToken) => await _stateManager.StateChangedTo(state, cancellationToken);
        public async ValueTask<ProducerState> StateChangedFrom(ProducerState state, CancellationToken cancellationToken) => await _stateManager.StateChangedFrom(state, cancellationToken);
        public bool IsFinalState() => _stateManager.IsFinalState();
        public bool IsFinalState(ProducerState state) => _stateManager.IsFinalState(state);

        public async ValueTask DisposeAsync()
        {
            await _executor.DisposeAsync();
            _connectTokenSource.Cancel();
            await _connectTask;
        }

        public async ValueTask<MessageId> Send(byte[] data, CancellationToken cancellationToken) => await Send(new ReadOnlySequence<byte>(data), cancellationToken);

        public async ValueTask<MessageId> Send(ReadOnlyMemory<byte> data, CancellationToken cancellationToken) => await Send(new ReadOnlySequence<byte>(data), cancellationToken);

        public async ValueTask<MessageId> Send(ReadOnlySequence<byte> data, CancellationToken cancellationToken)
        {
            var response = await _executor.Execute(() => Stream.Send(data), cancellationToken);
            return new MessageId(response.MessageId);
        }

        public async ValueTask<MessageId> Send(MessageMetadata metadata, byte[] data, CancellationToken cancellationToken)
            => await Send(metadata, new ReadOnlySequence<byte>(data), cancellationToken);

        public async ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
            => await Send(metadata, new ReadOnlySequence<byte>(data), cancellationToken);

        public async ValueTask<MessageId> Send(MessageMetadata metadata, ReadOnlySequence<byte> data, CancellationToken cancellationToken)
        {
            var response = await _executor.Execute(() => Stream.Send(metadata.Metadata, data), cancellationToken);
            return new MessageId(response.MessageId);
        }

        private async Task ExecutorOnException(Exception exception, CancellationToken cancellationToken)
        {
            _throwIfClosedOrFaulted();

            switch (_faultStrategy.DetermineFaultAction(exception))
            {
                case FaultAction.Retry:
                    await Task.Delay(_faultStrategy.RetryInterval, cancellationToken);
                    break;
                case FaultAction.Relookup:
                    await _stateManager.StateChangedTo(ProducerState.Connected, cancellationToken);
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
            _stateManager.SetState(ProducerState.Faulted);
        }

        private void HasClosed()
        {
            _throwIfClosedOrFaulted = () => throw new ProducerClosedException();
            _stateManager.SetState(ProducerState.Closed);
        }

        private async Task Connect(CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                while (true)
                {
                    var proxy = new ProducerProxy(_stateManager);

                    await using (Stream = await _streamFactory.CreateStream(proxy, cancellationToken))
                    {
                        proxy.Connected();
                        await _stateManager.StateChangedFrom(ProducerState.Connected, cancellationToken);
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
