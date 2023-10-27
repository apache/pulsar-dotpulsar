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

        node.Value.CancelableCompletionSource.SetupCancellation(() => RemoveAndDispose(node), cancellationToken);

        return node.Value.CancelableCompletionSource.Task;
    }

    public void CompleteTasksAwaiting(TState state) => SetState(state, true);

    public void CompleteAllTasks(TState state) => SetState(state, false);

    private LinkedListNode<StateTask<TState>>? GetFirst()
    {
        lock (_lock)
        {
            return _awaiters.First;
        }
    }

    private void SetState(TState state, bool onlyAwaiting)
    {
        var awaiter = GetFirst();

        while (awaiter is not null)
        {
            var next = awaiter.Next;

            if (!onlyAwaiting || awaiter.Value.IsAwaiting(state))
            {
                awaiter.Value.CancelableCompletionSource.SetResult(state);
                RemoveAndDispose(awaiter);
            }

            awaiter = next;
        }
    }

    private void RemoveAndDispose(LinkedListNode<StateTask<TState>> node)
    {
        lock (_lock)
        {
            try
            {
                _awaiters.Remove(node);
            }
            catch
            {
                // Ignore
            }
        }

        node.Value.Dispose();
    }
}
