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
    using Exceptions;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AsyncLock : IAsyncDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private int _isDisposed;
        private readonly CancellationTokenSource _disposedCancellation;

        public AsyncLock()
        {
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _isDisposed = 0;
            _disposedCancellation = new CancellationTokenSource();
        }

        public async Task<IDisposable> Lock(CancellationToken cancellationToken)
        {
            try
            {
                // This is a bit ugly and wasteful of allocations; it would be better to be able to dispose the semaphore without waiting
                var requestToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _disposedCancellation.Token).Token;

                await _semaphoreSlim.WaitAsync(requestToken);
                return new Releaser(() => _semaphoreSlim.Release());
            }
            catch (ObjectDisposedException)
            {
                throw new AsyncLockDisposedException();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
            {
                _disposedCancellation.Cancel();
                await _semaphoreSlim.WaitAsync().ConfigureAwait(false); //Wait for possible lock-holder to finish
                _semaphoreSlim.Dispose();
            }
        }

        private class Releaser : IDisposable
        {
            private readonly Action _release;

            public Releaser(Action release)
                => _release = release;

            public void Dispose()
                => _release();
        }
    }
}
