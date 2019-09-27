using DotPulsar.Internal.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class AsyncQueue<T> : IEnqueue<T>, IDequeue<T>, IDisposable
    {
        private readonly object _lock;
        private readonly Queue<T> _queue;
        private readonly LinkedList<CancelableCompletionSource<T>> _pendingDequeues;

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
                if (_pendingDequeues.Count > 0)
                {
                    var tcs = _pendingDequeues.First;
                    _pendingDequeues.RemoveFirst();
                    tcs.Value.SetResult(item);
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
                if (_queue.Count > 0)
                    return new ValueTask<T>(_queue.Dequeue());

                node = _pendingDequeues.AddLast(new CancelableCompletionSource<T>());
            }

            node.Value.SetupCancellation(() => Cancel(node), cancellationToken);
            return new ValueTask<T>(node.Value.Task);
        }

        public void Dispose()
        {
            lock (_lock)
            {
                foreach (var pendingDequeue in _pendingDequeues)
                    pendingDequeue.Dispose();

                _pendingDequeues.Clear();
                _queue.Clear();
            }
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
                catch { }
            }
        }
    }
}
