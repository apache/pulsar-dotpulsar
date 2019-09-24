using System;

namespace DotPulsar.Internal
{
    public sealed class StateTask<TState> : IDisposable where TState : notnull
    {
        private readonly TState _state;
        private readonly StateChanged _change;

        public StateTask(TState state, StateChanged change)
        {
            _state = state;
            _change = change;
            CancelableCompletionSource = new CancelableCompletionSource<TState>();
        }

        public CancelableCompletionSource<TState> CancelableCompletionSource { get; }

        public void Dispose() => CancelableCompletionSource.Dispose();

        public bool IsAwaiting(TState state)
        {
            if (_change == StateChanged.To)
                return _state.Equals(state);

            return !_state.Equals(state);
        }
    }
}
