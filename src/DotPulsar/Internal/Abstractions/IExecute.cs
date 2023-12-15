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

namespace DotPulsar.Internal.Abstractions;

public interface IExecute
{
    ValueTask Execute(Action action, CancellationToken cancellationToken = default);

    ValueTask Execute(Func<Task> func, CancellationToken cancellationToken = default);

    ValueTask Execute(Func<ValueTask> func, CancellationToken cancellationToken = default);

    ValueTask<TResult> Execute<TResult>(Func<TResult> func, CancellationToken cancellationToken = default);

    ValueTask<TResult> Execute<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default);

    ValueTask<TResult> Execute<TResult>(Func<ValueTask<TResult>> func, CancellationToken cancellationToken = default);

    ValueTask<bool> TryExecuteOnce(Action action, CancellationToken cancellationToken = default);

    ValueTask<bool> TryExecuteOnce(Func<Task> func, CancellationToken cancellationToken = default);

    ValueTask<bool> TryExecuteOnce(Func<ValueTask> func, CancellationToken cancellationToken = default);

    ValueTask<ExecutionResult<TResult>> TryExecuteOnce<TResult>(Func<TResult> func, CancellationToken cancellationToken = default);

    ValueTask<ExecutionResult<TResult>> TryExecuteOnce<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default);

    ValueTask<ExecutionResult<TResult>> TryExecuteOnce<TResult>(Func<ValueTask<TResult>> func, CancellationToken cancellationToken = default);
}
