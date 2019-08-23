using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotPulsar.Internal
{
    public sealed class CancelableCompletionSource<T> : IDisposable
    {
        private readonly TaskCompletionSource<T> _source;
        private CancellationTokenRegistration? _registration;

        public CancelableCompletionSource() => _source = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

        public void SetupCancellation(Action callback, CancellationToken token) => _registration = token.Register(() => callback());

        public void SetResult(T result) => _ = _source.TrySetResult(result);

        public void SetException(Exception exception) => _ = _source.TrySetException(exception);

        public Task<T> Task => _source.Task;

        public void Dispose()
        {
            _ = _source.TrySetCanceled();
            _registration?.Dispose();
        }
    }
}
