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
/// The Access modes that can be set on a producer.
/// </summary>
public enum ProducerAccessMode : byte
{
    /// <summary>
    /// Shared Access Mode. Multiple producers can publish on a topic.
    /// </summary>
    Shared = 0,

    /// <summary>
    /// Only one producer can publish on a topic.
    ///
    /// If there is already a producer connected, other producers trying to
    /// publish on this topic get errors immediately.
    ///
    /// The "old" producer is evicted and a "new" producer is selected to be
    /// the next exclusive producer if the "old" producer experiences a
    /// network partition with the broker.
    /// </summary>
    Exclusive = 1,

    /// <summary>
    /// If there is already a producer connected, the producer creation is
    /// pending (rather than timing out) until the producer gets the
    /// Exclusive access.
    ///
    /// The producer that succeeds in becoming the exclusive one is treated
    /// as the leader. Consequently, if you want to implement a leader
    /// election scheme for your application, you can use this access mode.
    /// Note that the leader pattern scheme mentioned refers to using Pulsar
    /// as a Write-Ahead Log (WAL) which means the leader writes its
    /// "decisions" to the topic. On error cases, the leader will get
    /// notified it is no longer the leader only when it tries to write a
    /// message and fails on appropriate error, by the broker.
    /// </summary>
    WaitForExclusive = 2,

    /// <summary>
    /// Only one producer can publish on a topic.
    ///
    /// If there is already a producer connected, it will be removed and
    /// invalidated immediately.
    /// </summary>
    ExclusiveWithFencing = 3
}
