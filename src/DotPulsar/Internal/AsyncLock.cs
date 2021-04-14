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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AsyncLock : IAsyncDisposable
    {
        private readonly CancellationTokenSource _disposing;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly Releaser _releaser;

        public AsyncLock()
        {
            _disposing = new CancellationTokenSource();
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _releaser = new Releaser(Release);
        }

        public async Task<IDisposable> Lock(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_disposing.Token, cancellationToken);
            try
            {
                await _semaphoreSlim.WaitAsync(linkedTokenSource.Token);
                return _releaser;
            }
            finally
            {
                linkedTokenSource.Dispose();
            }
        }

        public ValueTask DisposeAsync()
        {
            _disposing.Cancel();
            return default(ValueTask);
        }

        private void Release()
        {
            _semaphoreSlim.Release();
        }

        private void ThrowIfDisposed()
        {
            if (_disposing.IsCancellationRequested)
                throw new AsyncLockDisposedException();
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
