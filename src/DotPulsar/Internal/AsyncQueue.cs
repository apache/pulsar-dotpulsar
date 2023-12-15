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

using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Exceptions;

public sealed class AsyncQueue<T> : IEnqueue<T>, IDequeue<T>, IDisposable
{
    private readonly object _lock;
    private readonly Queue<T> _queue;
    private readonly LinkedList<CancelableCompletionSource<T>> _pendingDequeues;
    private int _isDisposed;

    public AsyncQueue()
    {
        _lock = new object();
        _queue = new Queue<T>();
        _pendingDequeues = new LinkedList<CancelableCompletionSource<T>>();
    }

    public void Enqueue(T item)
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            var node = _pendingDequeues.First;
            if (node is not null)
            {
                node.Value.SetResult(item);
                node.Value.Dispose();
                _pendingDequeues.RemoveFirst();
            }
            else
                _queue.Enqueue(item);
        }
    }

    public ValueTask<T> Dequeue(CancellationToken cancellationToken = default)
    {
        LinkedListNode<CancelableCompletionSource<T>>? node = null;

        lock (_lock)
        {
            ThrowIfDisposed();

            if (_queue.Count > 0)
                return new ValueTask<T>(_queue.Dequeue());

            node = _pendingDequeues.AddLast(new CancelableCompletionSource<T>());
        }

        node.Value.SetupCancellation(() => Cancel(node), cancellationToken);
        return new ValueTask<T>(node.Value.Task);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        foreach (var pendingDequeue in _pendingDequeues)
            pendingDequeue.Dispose();

        _pendingDequeues.Clear();
        _queue.Clear();
    }

    private void Cancel(LinkedListNode<CancelableCompletionSource<T>> node)
    {
        lock (_lock)
        {
            try
            {
                node.Value.Dispose();
                _pendingDequeues.Remove(node);
            }
            catch
            {
                // ignored
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
            throw new AsyncQueueDisposedException();
    }
}
