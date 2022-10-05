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

public sealed class AsyncQueueWithCursor<T> : IAsyncDisposable
{
    public AsyncQueueWithCursor(uint maxItems)
    {
    }

    /// <summary>
    /// Enqueue item
    /// </summary>
    public ValueTask Enqueue(T item, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Attempt to retrieve the last item in the queue without removing it
    /// </summary>
    public bool TryPeek(out T item)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Remove the last item from the queue
    /// </summary>
    public void Dequeue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Return the item the cursor is pointing to and move the cursor, to the following item.
    /// </summary>
    public ValueTask<T> NextItem(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reset the cursor back to the last item
    /// </summary>
    public ValueTask ResetCursor(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
        => throw new NotImplementedException();
}
