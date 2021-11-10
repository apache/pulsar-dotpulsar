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

using DotPulsar.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class ActionStateChangedHandler<TStateChanged> : IHandleStateChanged<TStateChanged>
{
    private readonly Action<TStateChanged, CancellationToken> _stateChangedHandler;

    public ActionStateChangedHandler(Action<TStateChanged, CancellationToken> stateChangedHandler, CancellationToken cancellationToken)
    {
        _stateChangedHandler = stateChangedHandler;
        CancellationToken = cancellationToken;
    }

    public CancellationToken CancellationToken { get; }

    public ValueTask OnStateChanged(TStateChanged stateChanged, CancellationToken cancellationToken)
    {
        _stateChangedHandler(stateChanged, CancellationToken);
        return new ValueTask();
    }
}
