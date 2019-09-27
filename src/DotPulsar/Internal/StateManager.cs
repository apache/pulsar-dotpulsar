using DotPulsar.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class StateManager<TState> : IStateChanged<TState> where TState : notnull
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

        public bool IsFinalState() => IsFinalState(CurrentState);
    }
}
