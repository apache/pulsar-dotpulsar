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
    using Abstractions;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class StateManager<TState> : IStateManager<TState> where TState : notnull
    {
        private readonly object _lock;
        private readonly StateTaskCollection<TState> _stateTasks;
        private readonly TState[] _finalStates;

        public StateManager(TState initialState, params TState[] finalStates)
        {
            _lock = new object();
            _stateTasks = new StateTaskCollection<TState>();
            _finalStates = finalStates;
            CurrentState = initialState;
        }

        public TState CurrentState { get; private set; }

        public TState SetState(TState state)
        {
            lock (_lock)
            {
                if (IsFinalState(CurrentState) || CurrentState.Equals(state))
                    return CurrentState;

                var formerState = CurrentState;
                CurrentState = state;

                if (IsFinalState(CurrentState))
                    _stateTasks.CompleteAllTasks(CurrentState);
                else
                    _stateTasks.CompleteTasksAwaiting(CurrentState);

                return formerState;
            }
        }

        public ValueTask<TState> StateChangedTo(TState state, CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                if (IsFinalState(CurrentState) || CurrentState.Equals(state))
                    return new ValueTask<TState>(CurrentState);
                return new ValueTask<TState>(_stateTasks.CreateTaskFor(state, StateChanged.To, cancellationToken));
            }
        }

        public ValueTask<TState> StateChangedFrom(TState state, CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                if (IsFinalState(CurrentState) || !CurrentState.Equals(state))
                    return new ValueTask<TState>(CurrentState);
                return new ValueTask<TState>(_stateTasks.CreateTaskFor(state, StateChanged.From, cancellationToken));
            }
        }

        public bool IsFinalState(TState state)
        {
            for (var i = 0; i < _finalStates.Length; ++i)
            {
                if (_finalStates[i].Equals(state))
                    return true;
            }

            return false;
        }

        public bool IsFinalState()
            => IsFinalState(CurrentState);
    }
}
