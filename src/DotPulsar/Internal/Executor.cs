using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class Executor : IAsyncDisposable
    {
        private readonly AsyncLock _lock;
        private readonly Func<Exception, CancellationToken, Task> _onException;

        public Executor(Func<Exception, CancellationToken, Task> onException)
        {
            _lock = new AsyncLock();
            _onException = onException;
        }

        public async ValueTask DisposeAsync() => await _lock.DisposeAsync();

        public async Task Execute(Action action, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                while (true)
                {
                    try
                    {
                        action();
                        return;
                    }
                    catch (Exception exception)
                    {
                        await _onException(exception, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        public async Task Execute(Func<Task> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                while (true)
                {
                    try
                    {
                        await func();
                        return;
                    }
                    catch (Exception exception)
                    {
                        await _onException(exception, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        public async Task<TResult> Execute<TResult>(Func<TResult> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                while (true)
                {
                    try
                    {
                        return func();
                    }
                    catch (Exception exception)
                    {
                        await _onException(exception, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        public async Task<TResult> Execute<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken)
        {
            using (await _lock.Lock(cancellationToken))
            {
                while (true)
                {
                    try
                    {
                        return await func();
                    }
                    catch (Exception exception)
                    {
                        await _onException(exception, cancellationToken);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
