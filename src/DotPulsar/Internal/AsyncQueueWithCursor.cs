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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public sealed class AsyncQueueWithCursor<T> : IAsyncDisposable
{
    private readonly AsyncLock _pendingLock;
    private readonly SemaphoreSlim _cursorSemaphore;
    private readonly LinkedList<T> _queue;
    private readonly uint _maxItems;
    private IDisposable? _pendingLockGrant;
    private LinkedListNode<T>? _currentNode;
    private TaskCompletionSource<LinkedListNode<T>>? _cursorNextItemTcs;
    private int _isDisposed;

    public AsyncQueueWithCursor(uint maxItems)
    {
        _pendingLock = new AsyncLock();
        _cursorSemaphore = new SemaphoreSlim(1, 1);
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

                if (_cursorNextItemTcs is not null)
                {
                    _cursorNextItemTcs.TrySetResult(node);
                    _cursorNextItemTcs = null;
                }
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
    /// <exception cref="InvalidOperationException">The AsyncQueueWithCursor&lt;T&gt; is empty.</exception>
    public void Dequeue()
    {
        ThrowIfDisposed();

        lock (_queue)
        {
            _queue.RemoveFirst();
            ReleasePendingLockGrant();
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
                _currentNode = _currentNode is null ? _queue.First : _currentNode.Next;

                if (_currentNode is not null) return _currentNode.Value;

                _cursorNextItemTcs = new TaskCompletionSource<LinkedListNode<T>>(TaskCreationOptions.RunContinuationsAsynchronously);
                cancellationToken.Register(() => _cursorNextItemTcs.TrySetCanceled(cancellationToken));
            }

            _currentNode = await _cursorNextItemTcs.Task.ConfigureAwait(false);
            return _currentNode.Value;
        }
        finally
        {
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
            throw new ObjectDisposedException("Queue is disposed"); // TODO: Create specific exception type
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
