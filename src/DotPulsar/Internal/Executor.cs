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
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class Executor : IAsyncDisposable
    {
        private readonly AsyncLock _lock;
        private readonly Func<Exception, CancellationToken, Task> _onException;

        public Executor(Func<Exception, CancellationToken, Task> onException)
        {
            _lock = new AsyncLock();
            _onException = onException;
        }

        public async ValueTask DisposeAsync() => await _lock.DisposeAsync();

        public async Task Execute(Action action, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                while (true)
                {
                    try
                    {
                        action();
                        return;
                    }
                    catch (Exception exception)
                    {
                        await _onException(exception, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        public async Task Execute(Func<Task> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                while (true)
                {
                    try
                    {
                        await func();
                        return;
                    }
                    catch (Exception exception)
                    {
                        await _onException(exception, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        public async Task<TResult> Execute<TResult>(Func<TResult> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                while (true)
                {
                    try
                    {
                        return func();
                    }
                    catch (Exception exception)
                    {
                        await _onException(exception, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        public async Task<TResult> Execute<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                while (true)
                {
                    try
                    {
                        return await func();
                    }
                    catch (Exception exception)
                    {
                        await _onException(exception, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        public async ValueTask<TResult> Execute<TResult>(Func<ValueTask<TResult>> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                while (true)
                {
                    try
                    {
                        return await func();
                    }
                    catch (Exception exception)
                    {
                        await _onException(exception, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
