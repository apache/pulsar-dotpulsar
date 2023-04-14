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

/// <summary>
/// The possible states a producer can be in.
/// </summary>
public enum ProducerState : byte
{
    /// <summary>
    /// The producer is closed. This is a final state.
    /// </summary>
    Closed,

    /// <summary>
    /// The producer is connected.
    /// </summary>
    Connected,

    /// <summary>
    /// The producer is connected but waiting for exclusive access.
    /// </summary>
    WaitingForExclusive,

    /// <summary>
    /// The producer is disconnected.
    /// </summary>
    Disconnected,

    /// <summary>
    /// The producer is faulted. This is a final state.
    /// </summary>
    Faulted,

    /// <summary>
    /// Some of the sub-producers are disconnected.
    /// </summary>
    PartiallyConnected
}
