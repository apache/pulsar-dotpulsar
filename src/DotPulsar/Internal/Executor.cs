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
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using DotPulsar.Abstractions;
    using Events;

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
            while (true)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                        throw;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public async ValueTask Execute(Func<Task> func, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await func().ConfigureAwait(false);
                    return;
                }
                catch (Exception ex)
                {
                    if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                        throw;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public async ValueTask Execute(Func<ValueTask> func, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await func().ConfigureAwait(false);
                    return;
                }
                catch (Exception ex)
                {
                    if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                        throw;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public async ValueTask<TResult> Execute<TResult>(Func<TResult> func, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                        throw;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public async ValueTask<TResult> Execute<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    return await func().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                        throw;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public async ValueTask<TResult> Execute<TResult>(Func<ValueTask<TResult>> func, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    return await func().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (await Handle(ex, cancellationToken).ConfigureAwait(false))
                        throw;
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
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
}
