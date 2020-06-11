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

namespace DotPulsar.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A reader abstraction.
    /// </summary>
    public interface IReader : IAsyncDisposable
    {
        /// <summary>
        /// Ask whether the current state is final, meaning that it will never change.
        /// </summary>
        /// <returns>
        /// True if it's final and False if it's not.
        /// </returns>
        bool IsFinalState();

        /// <summary>
        /// Ask whether the provided state is final, meaning that it will never change.
        /// </summary>
        /// <returns>
        /// True if it's final and False if it's not.
        /// </returns>
        bool IsFinalState(ReaderState state);

        /// <summary>
        /// Get an IAsyncEnumerable for reading messages
        /// </summary>
        IAsyncEnumerable<Message> Messages(CancellationToken cancellationToken = default);

        /// <summary>
        /// Wait for the state to change to a specific state.
        /// </summary>
        /// <returns>
        /// The current state.
        /// </returns>
        /// <remarks>
        /// If the state change to a final state, then all awaiting tasks will complete.
        /// </remarks>
        ValueTask<ReaderStateChanged> StateChangedTo(ReaderState state, CancellationToken cancellationToken = default);

        /// <summary>
        /// Wait for the state to change from a specific state.
        /// </summary>
        /// <returns>
        /// The current state.
        /// </returns>
        /// <remarks>
        /// If the state change to a final state, then all awaiting tasks will complete.
        /// </remarks>
        ValueTask<ReaderStateChanged> StateChangedFrom(ReaderState state, CancellationToken cancellationToken = default);

        /// <summary>
        /// The topic of the reader.
        /// </summary>
        string Topic { get; }
    }
}
