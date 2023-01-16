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

namespace DotPulsar;

using DotPulsar.Abstractions;

/// <summary>
/// Representation of a reader state change.
/// </summary>
public sealed class ReaderStateChanged
{
    internal ReaderStateChanged(IReader reader, ReaderState readerState)
    {
        Reader = reader;
        ReaderState = readerState;
    }

    /// <summary>
    /// The reader that changed state.
    /// </summary>
    public IReader Reader { get; }

    /// <summary>
    /// The state that it changed to.
    /// </summary>
    public ReaderState ReaderState { get; }
}
