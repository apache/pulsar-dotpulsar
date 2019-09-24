using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class StateTaskCollection<TState> where TState : notnull
    {
        private readonly object _lock;
        private readonly LinkedList<StateTask<TState>> _awaitors;

        public StateTaskCollection()
        {
            _lock = new object();
            _awaitors = new LinkedList<StateTask<TState>>();
        }

        public Task<TState> CreateTaskFor(TState state, StateChanged changed, CancellationToken cancellationToken)
        {
            LinkedListNode<StateTask<TState>> node;

            lock (_lock)
            {
                node = _awaitors.AddFirst(new StateTask<TState>(state, changed));
            }

            node.Value.CancelableCompletionSource.SetupCancellation(() => TaskWasCanceled(node), cancellationToken);
            return node.Value.CancelableCompletionSource.Task;
        }

        public void CompleteTasksAwaiting(TState state)
        {
            lock (_lock)
            {
                var awaitor = _awaitors.First;
                while (awaitor != null)
                {
                    var next = awaitor.Next;
                    if (awaitor.Value.IsAwaiting(state))
                    {
                        _awaitors.Remove(awaitor);
                        awaitor.Value.CancelableCompletionSource.SetResult(state);
                        awaitor.Value.CancelableCompletionSource.Dispose();
                    }
                    awaitor = next;
                }
            }
        }

        public void CompleteAllTasks(TState state)
        {
            lock (_lock)
            {
                foreach (var awaitor in _awaitors)
                {
                    awaitor.CancelableCompletionSource.SetResult(state);
                    awaitor.CancelableCompletionSource.Dispose();
                }
                _awaitors.Clear();
            }
        }

        private void TaskWasCanceled(LinkedListNode<StateTask<TState>> node)
        {
            lock (_lock)
            {
                try
                {
                    _awaitors.Remove(node);
                    node.Value.Dispose();
                }
                catch
                {
                    // Ignore
                }
            }
        }
    }
}
