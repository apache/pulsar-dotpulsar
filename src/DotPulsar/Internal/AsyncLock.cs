using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class AsyncLock : IAsyncDisposable
    {
        private readonly LinkedList<CancelableCompletionSource<IDisposable>> _pending;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly Releaser _releaser;
        private readonly Task<IDisposable> _completedTask;
        private bool _isDisposed;

        public AsyncLock()
        {
            _pending = new LinkedList<CancelableCompletionSource<IDisposable>>();
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _releaser = new Releaser(Release);
            _completedTask = Task.FromResult((IDisposable)_releaser);
            _isDisposed = false;
        }

        public Task<IDisposable> Lock(CancellationToken cancellationToken = default)
        {
            LinkedListNode<CancelableCompletionSource<IDisposable>>? node = null;

            lock (_pending)
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(nameof(AsyncLock));

                if (_semaphoreSlim.CurrentCount == 1) //Lock is free
                {
                    _semaphoreSlim.Wait(); //Will never block
                    return _completedTask;
                }

                //Lock was not free
                var ccs = new CancelableCompletionSource<IDisposable>();
                node = _pending.AddLast(ccs);
            }

            cancellationToken.Register(() => Cancel(node));

            return node.Value.Task;
        }

        public async ValueTask DisposeAsync()
        {
            lock (_pending)
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;

                foreach (var pending in _pending)
                {
                    pending.SetException(new ObjectDisposedException(nameof(AsyncLock)));
                    pending.Dispose();
                }

                _pending.Clear();
            }

            await _semaphoreSlim.WaitAsync(); //Wait for possible lock-holder to finish
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
                    node.Value.Dispose();
                }
                catch
                {
                    // Ignore
                }
            }
        }

        private void Release()
        {
            lock (_pending)
            {
                if (_pending.Count > 0)
                {
                    var node = _pending.First;
                    node.Value.SetResult(_releaser);
                    node.Value.Dispose();
                    _pending.RemoveFirst();
                    return;
                }

                if (_semaphoreSlim.CurrentCount == 0)
                    _semaphoreSlim.Release();
            }
        }

        private class Releaser : IDisposable
        {
            private readonly Action _release;

            public Releaser(Action release) => _release = release;

            public void Dispose() => _release();
        }
    }
}
