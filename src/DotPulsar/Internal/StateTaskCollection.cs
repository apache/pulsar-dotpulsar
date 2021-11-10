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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public sealed class StateTaskCollection<TState> where TState : notnull
{
    private readonly object _lock;
    private readonly LinkedList<StateTask<TState>> _awaiters;

    public StateTaskCollection()
    {
        _lock = new object();
        _awaiters = new LinkedList<StateTask<TState>>();
    }

    public Task<TState> CreateTaskFor(TState state, StateChanged changed, CancellationToken cancellationToken)
    {
        LinkedListNode<StateTask<TState>> node;

        lock (_lock)
        {
            node = _awaiters.AddFirst(new StateTask<TState>(state, changed));
        }

        node.Value.CancelableCompletionSource.SetupCancellation(() => TaskWasCanceled(node), cancellationToken);

        return node.Value.CancelableCompletionSource.Task;
    }

    public void CompleteTasksAwaiting(TState state)
    {
        lock (_lock)
        {
            var awaiter = _awaiters.First;

            while (awaiter is not null)
            {
                var next = awaiter.Next;

                if (awaiter.Value.IsAwaiting(state))
                {
                    _awaiters.Remove(awaiter);
                    awaiter.Value.CancelableCompletionSource.SetResult(state);
                    awaiter.Value.CancelableCompletionSource.Dispose();
                }

                awaiter = next;
            }
        }
    }

    public void CompleteAllTasks(TState state)
    {
        lock (_lock)
        {
            foreach (var awaiter in _awaiters)
            {
                awaiter.CancelableCompletionSource.SetResult(state);
                awaiter.CancelableCompletionSource.Dispose();
            }

            _awaiters.Clear();
        }
    }

    private void TaskWasCanceled(LinkedListNode<StateTask<TState>> node)
    {
        lock (_lock)
        {
            try
            {
                _awaiters.Remove(node);
                node.Value.Dispose();
            }
            catch
            {
                // Ignore
            }
        }
    }
}
