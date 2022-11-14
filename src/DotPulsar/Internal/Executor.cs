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

using Abstractions;
using DotPulsar.Abstractions;
using Events;
using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class Executor : IExecute
{
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private readonly IHandleException _exceptionHandler;

    public Executor(Guid correlationId, IRegisterEvent eventRegister, IHandleException exceptionHandler)
    {
        _correlationId = correlationId;
        _eventRegister = eventRegister;
        _exceptionHandler = exceptionHandler;
    }

    public async ValueTask Execute(Action action, CancellationToken cancellationToken)
    {
        while (!await TryExecuteOnce(action, cancellationToken).ConfigureAwait(false)) { }
    }

    public async ValueTask Execute(Func<Task> func, CancellationToken cancellationToken)
    {
        while (!await TryExecuteOnce(func, cancellationToken).ConfigureAwait(false)) { }
    }

    public async ValueTask Execute(Func<ValueTask> func, CancellationToken cancellationToken)
    {
        while (!await TryExecuteOnce(func, cancellationToken).ConfigureAwait(false)) { }
    }

    public async ValueTask<TResult> Execute<TResult>(Func<TResult> func, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await TryExecuteOnce(func, cancellationToken).ConfigureAwait(false);
            if (result.Success) return result.Result!;
        }
    }

    public async ValueTask<TResult> Execute<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await TryExecuteOnce(func, cancellationToken).ConfigureAwait(false);
            if (result.Success) return result.Result!;
        }
    }

    public async ValueTask<TResult> Execute<TResult>(Func<ValueTask<TResult>> func, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await TryExecuteOnce(func, cancellationToken).ConfigureAwait(false);
            if (result.Success) return result.Result!;
        }
    }

    public async ValueTask<bool> TryExecuteOnce(Action action, CancellationToken cancellationToken = default)
    {
        try
        {
            action();
            return true;
        }
        catch (Exception ex)
        {
            if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                throw;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return false;
    }

    public async ValueTask<bool> TryExecuteOnce(Func<Task> func, CancellationToken cancellationToken = default)
    {
        try
        {
            await func().ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                throw;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return false;
    }

    public async ValueTask<bool> TryExecuteOnce(Func<ValueTask> func, CancellationToken cancellationToken = default)
    {
        try
        {
            await func().ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                throw;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return false;
    }

    public async ValueTask<ExecutionResult<TResult>> TryExecuteOnce<TResult>(Func<TResult> func, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = func();
            return new ExecutionResult<TResult>(true, result);
        }
        catch (Exception ex)
        {
            if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                throw;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return new ExecutionResult<TResult>(false);
    }

    public async ValueTask<ExecutionResult<TResult>> TryExecuteOnce<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await func().ConfigureAwait(false);
            return new ExecutionResult<TResult>(true, result);
        }
        catch (Exception ex)
        {
            if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                throw;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return new ExecutionResult<TResult>(false);
    }

    public async ValueTask<ExecutionResult<TResult>> TryExecuteOnce<TResult>(Func<ValueTask<TResult>> func, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await func().ConfigureAwait(false);
            return new ExecutionResult<TResult>(true, result);
        }
        catch (Exception ex)
        {
            if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                throw;
        }

        cancellationToken.ThrowIfCancellationRequested();
        return new ExecutionResult<TResult>(false);
    }

    private async ValueTask<bool> Handle(Exception exception, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return true;

        var context = new ExceptionContext(exception, cancellationToken);

        await _exceptionHandler.OnException(context).ConfigureAwait(false);

        if (context.Result != FaultAction.Retry)
            _eventRegister.Register(new ExecutorFaulted(_correlationId));

        return context.Result == FaultAction.ThrowException
            ? throw context.Exception
            : context.Result == FaultAction.Rethrow;
    }
}
