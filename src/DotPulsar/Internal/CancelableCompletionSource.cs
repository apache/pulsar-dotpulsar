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

using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class CancelableCompletionSource<T> : IDisposable
{
    private readonly TaskCompletionSource<T> _source;
    private CancellationTokenRegistration? _registration;
    private int _isDisposed;

    public CancelableCompletionSource()
        => _source = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

    public void SetupCancellation(Action callback, CancellationToken token)
    {
        _registration = token.Register(callback);
        if (_isDisposed == 1)
            _registration?.Dispose();
    }

    public void SetResult(T result)
        => _ = _source.TrySetResult(result);

    public void SetException(Exception exception)
        => _ = _source.TrySetException(exception);

    public Task<T> Task => _source.Task;

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            return;

        _ = _source.TrySetCanceled();
        _registration?.Dispose();
    }
}
