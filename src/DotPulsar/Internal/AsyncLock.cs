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

namespace DotPulsar.Internal;

using DotPulsar.Internal.Exceptions;

public sealed class AsyncLock : IAsyncDisposable
{
    private readonly LinkedList<CancelableCompletionSource<IDisposable>> _pending;
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly Releaser _releaser;
    private readonly Task<IDisposable> _completedTask;
    private int _isDisposed;

    public AsyncLock()
    {
        _pending = new LinkedList<CancelableCompletionSource<IDisposable>>();
        _semaphoreSlim = new SemaphoreSlim(1, 1);
        _releaser = new Releaser(Release);
        _completedTask = Task.FromResult((IDisposable) _releaser);
    }

    public Task<IDisposable> Lock(CancellationToken cancellationToken)
    {
        LinkedListNode<CancelableCompletionSource<IDisposable>>? node = null;

        lock (_pending)
        {
            ThrowIfDisposed();

            if (_semaphoreSlim.CurrentCount == 1) //Lock is free
            {
                _semaphoreSlim.Wait(cancellationToken); //Will never block
                return _completedTask;
            }

            //Lock was not free
            var ccs = new CancelableCompletionSource<IDisposable>();
            node = _pending.AddLast(ccs);
        }

        node.Value.SetupCancellation(() => Cancel(node), cancellationToken);

        return node.Value.Task;
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        IEnumerable<CancelableCompletionSource<IDisposable>> pending;

        lock (_pending)
        {
            pending = _pending.ToArray();
            _pending.Clear();
        }

        foreach (var ccs in pending)
        {
            ccs.Dispose();
        }

        await _semaphoreSlim.WaitAsync().ConfigureAwait(false); //Wait for possible lock-holder to finish

        _semaphoreSlim.Release();
        _semaphoreSlim.Dispose();
    }

    private void Cancel(LinkedListNode<CancelableCompletionSource<IDisposable>> node)
    {
        lock (_pending)
        {
            try
            {
                _pending.Remove(node);
            }
            catch
            {
                // Ignore
            }
        }

        try
        {
            node.Value.Dispose();
        }
        catch
        {
            // Ignore
        }
    }

    private void Release()
    {
        LinkedListNode<CancelableCompletionSource<IDisposable>>? node;

        lock (_pending)
        {
            node = _pending.First;
            if (node is not null)
                _pending.RemoveFirst();
            else if (_semaphoreSlim.CurrentCount == 0)
                _semaphoreSlim.Release();
        }

        if (node is not null)
        {
            node.Value.SetResult(_releaser);
            node.Value.Dispose();
        }
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
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
