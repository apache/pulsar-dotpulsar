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
    using System.Threading.Tasks;

    public sealed class Awaiter<T, TResult> : IDisposable
    {
        private readonly ConcurrentDictionary<T, TaskCompletionSource<TResult>> _items;

        public Awaiter()
            => _items = new ConcurrentDictionary<T, TaskCompletionSource<TResult>>();

        public Task<TResult> CreateTask(T item)
        {
            var tcs = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            _ = _items.TryAdd(item, tcs);
            return tcs.Task;
        }

        public void SetResult(T item, TResult result)
        {
            if (_items.TryRemove(item, out var tcs))
                tcs.SetResult(result);
        }

        public void Dispose()
        {
            foreach (var item in _items.Values)
                item.SetCanceled();

            _items.Clear();
        }
    }
}
