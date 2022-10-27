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

using Exceptions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public sealed class AsyncQueueWithCursor<T> : IAsyncDisposable
{
    private readonly AsyncLock _pendingLock;
    private readonly SemaphoreSlim _cursorSemaphore;
    private readonly LinkedList<T> _queue;
    private readonly IList<TaskCompletionSource<object>> _queueEmptyTcs;
    private readonly uint _maxItems;
    private IDisposable? _pendingLockGrant;
    private LinkedListNode<T>? _currentNode;
    private TaskCompletionSource<LinkedListNode<T>>? _cursorNextItemTcs;
    private int _isDisposed;

    public AsyncQueueWithCursor(uint maxItems)
    {
        _pendingLock = new AsyncLock();
        _cursorSemaphore = new SemaphoreSlim(1, 1);
        // TODO: Figure out if we can use https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskcompletionsource?view=net-6.0
        _queueEmptyTcs = new List<TaskCompletionSource<object>>();
        _queue = new LinkedList<T>();
        _maxItems = maxItems;
    }

    /// <summary>
    /// Enqueue item
    /// </summary>
    public async ValueTask Enqueue(T item, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        try
        {
            var grant = await _pendingLock.Lock(cancellationToken).ConfigureAwait(false);
            lock (_pendingLock)
            {
                _pendingLockGrant = grant;
            }
        }
        catch (Exception)
        {
            ReleasePendingLockGrant();
            throw;
        }

        lock (_queue)
        {
            if (_queue.Count < _maxItems)
            {
                var node = _queue.AddLast(item);
                _cursorNextItemTcs?.TrySetResult(node);
            }

            if (_queue.Count < _maxItems)
            {
                ReleasePendingLockGrant();
            }
        }
    }

    /// <summary>
    /// Attempt to retrieve the last item in the queue without removing it
    /// </summary>
    public bool TryPeek(out T? item)
    {
        ThrowIfDisposed();

        lock (_queue)
        {
            var node = _queue.First;
            if (node is not null)
            {
                item = node.Value;
                return true;
            }

            item = default;
            return false;
        }
    }

    /// <summary>
    /// Remove the last item from the queue
    /// </summary>
    public void Dequeue()
    {
        ThrowIfDisposed();

        lock (_queue)
        {
            if (_currentNode == _queue.First)
                _currentNode = null;

            try
            {
                _queue.RemoveFirst();
            }
            catch (InvalidOperationException e)
            {
                throw new AsyncQueueWithCursorNoItemException(e);
            }
            if (_queue.Count == 0) NotifyQueueEmptyAwaiters();
            ReleasePendingLockGrant();
        }
    }

    /// <summary>
    /// Remove the current item the cursor is pointing to if applicable.
    /// </summary>
    public void RemoveCurrentItem()
    {
        ThrowIfDisposed();

        lock (_queue)
        {
            if (_currentNode is null) throw new AsyncQueueWithCursorNoItemException();
            var newCurrent = _currentNode.Previous;
            _queue.Remove(_currentNode);
            _currentNode = newCurrent;
            if (_queue.Count == 0) NotifyQueueEmptyAwaiters();
        }
    }

    /// <summary>
    /// Return the item the cursor is pointing to and move the cursor, to the following item.
    /// Ordering is not supported for multiple concurrent awaiters.
    /// </summary>
    /// <exception cref="OperationCanceledException">If cancellation requested</exception>
    public async ValueTask<T> NextItem(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        await _cursorSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            lock (_queue)
            {
                _currentNode = _currentNode is null || _currentNode.List is null ? _queue.First : _currentNode.Next;

                if (_currentNode is not null) return _currentNode.Value;

                var tcs = new TaskCompletionSource<LinkedListNode<T>>(TaskCreationOptions.RunContinuationsAsynchronously);
                _cursorNextItemTcs = tcs;
                cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            }

            var result = await _cursorNextItemTcs.Task.ConfigureAwait(false);

            lock (_queue)
            {
                _currentNode = result;
            }
            return _currentNode.Value;
        }
        finally
        {
            lock (_queue)
            {
                _cursorNextItemTcs = null;
            }
            _cursorSemaphore.Release();
        }
    }

    /// <summary>
    /// Reset the cursor back to the last item, and cancel any waiting NextItem tasks
    /// </summary>
    public void ResetCursor()
    {
        ThrowIfDisposed();

        lock (_queue)
        {
            _currentNode = null;
            _cursorNextItemTcs?.TrySetCanceled();
        }
    }

    /// <summary>
    /// Wait for the queue to become empty and return. Note that this will not block for enqueue operations.
    /// </summary>
    public async Task WaitForEmpty(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        var tcs = new TaskCompletionSource<object>();
        lock (_queue)
        {
            if (_queue.Count == 0) return;

            cancellationToken.Register(() => tcs.TrySetCanceled());
            _queueEmptyTcs.Add(tcs);
        }

        await tcs.Task.ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _cursorSemaphore.Dispose();
        ValueTask disposeLock = _pendingLock.DisposeAsync();
        ReleasePendingLockGrant();
        await disposeLock.ConfigureAwait(false);
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
            throw new AsyncQueueWithCursorDisposedException();
    }

    private void NotifyQueueEmptyAwaiters()
    {
        foreach (TaskCompletionSource<object>? tcs in _queueEmptyTcs)
        {
            tcs.SetResult(0);
        }
        _queueEmptyTcs.Clear();
    }

    private void ReleasePendingLockGrant()
    {
        lock (_pendingLock)
        {
            if (_pendingLockGrant is null) return;
            _pendingLockGrant.Dispose();
            _pendingLockGrant = null;
        }
    }
}
