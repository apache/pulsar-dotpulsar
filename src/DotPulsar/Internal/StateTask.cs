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
    using System;

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

        public void Dispose()
            => CancelableCompletionSource.Dispose();

        public bool IsAwaiting(TState state)
        {
            if (_change == StateChanged.To)
                return _state.Equals(state);

            return !_state.Equals(state);
        }
    }
}
