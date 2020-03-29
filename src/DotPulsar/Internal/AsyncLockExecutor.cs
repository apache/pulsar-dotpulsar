/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar.Internal
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;

    public sealed class AsyncLockExecutor : IExecute, IAsyncDisposable
    {
        private readonly AsyncLock _lock;
        private readonly IExecute _executor;

        public AsyncLockExecutor(IExecute executor)
        {
            _lock = new AsyncLock();
            _executor = executor;
        }

        public ValueTask DisposeAsync()
            => _lock.DisposeAsync();

        public async ValueTask Execute(Action action, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                await _executor.Execute(action, cancellationToken).ConfigureAwait(false);
            }
        }

        public async ValueTask Execute(Func<Task> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                await _executor.Execute(func, cancellationToken).ConfigureAwait(false);
            }
        }

        public async ValueTask Execute(Func<ValueTask> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                await _executor.Execute(func, cancellationToken).ConfigureAwait(false);
            }
        }

        public async ValueTask<TResult> Execute<TResult>(Func<TResult> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                return await _executor.Execute(func, cancellationToken).ConfigureAwait(false);
            }
        }

        public async ValueTask<TResult> Execute<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                return await _executor.Execute(func, cancellationToken).ConfigureAwait(false);
            }
        }

        public async ValueTask<TResult> Execute<TResult>(Func<ValueTask<TResult>> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken).ConfigureAwait(false))
            {
                return await _executor.Execute(func, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
