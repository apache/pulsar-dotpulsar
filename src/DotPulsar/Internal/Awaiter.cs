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
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class Awaiter<T, TResult> : IDisposable where T : notnull
    {
        private readonly ConcurrentDictionary<T, (TaskCompletionSource<TResult> tcs, DateTime expiry)> _items;
        private readonly int _timeoutMs;
        private readonly CancellationTokenSource _disposed = new CancellationTokenSource();

        public Awaiter(int timeoutMs)
        {
            _items = new ConcurrentDictionary<T, (TaskCompletionSource<TResult> tcs, DateTime expiry)>();
            _timeoutMs = timeoutMs;
            _ = TimeoutScanner(_disposed.Token);
        }

        public Task<TResult> CreateTask(T item, int? timeoutOverrideMs = null)
        {
            var tcs = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            int thisTimeout = timeoutOverrideMs ?? _timeoutMs;
            DateTime expiry = (thisTimeout > 0) ? DateTime.UtcNow.AddMilliseconds(thisTimeout) : DateTime.MaxValue;
            _ = _items.TryAdd(item, (tcs, expiry));
            return tcs.Task;
        }

        public void SetResult(T item, TResult result)
        {
            if (_items.TryRemove(item, out var tuple))
                tuple.tcs.SetResult(result);
        }

        public void Fault(T item, Exception exceptionToRelay)
        {
            if (_items.TryRemove(item, out var tuple))
                tuple.tcs.SetException(exceptionToRelay);
        }

        public void Dispose()
        {
            _disposed.Cancel();

            foreach (var item in _items.Values)
                item.tcs.SetCanceled();

            _items.Clear();
        }

        private async Task TimeoutScanner(CancellationToken stop)
        {
            try
            {
                await Task.Yield();
                while (!stop.IsCancellationRequested)
                {
                    var thisLoopNow = DateTime.UtcNow;
                    foreach (var itemToCheck in _items)
                    {
                        // If the item is expired, and we could remove it from the collection to operate on
                        if ((itemToCheck.Value.expiry < thisLoopNow) && _items.TryRemove(itemToCheck.Key, out var maybeExpiredItem))
                        {
                            // Make sure it's either the same item we checked, or we're too late to put it back
                            if ((maybeExpiredItem == itemToCheck.Value) || !_items.TryAdd(itemToCheck.Key, maybeExpiredItem))
                            {
                                // In the implementation as of this code change, we do not expect to ever actually hit
                                // either of these conditions, because Awaiters are unique to a Connection, and commands
                                // outside of Connect have unique request IDs within a Connection.
                                // This code is only for safety.  In case these assumptions are invalidated later,
                                // we will cancel this task because it can no longer be tracked correctly due to a race.
                                maybeExpiredItem.tcs.TrySetCanceled();
                            }
                        }
                    }

                    await Task.Delay(1000);
                }
            }
            catch
            {
                // Do nothing but die
            }
        }
    }
}
