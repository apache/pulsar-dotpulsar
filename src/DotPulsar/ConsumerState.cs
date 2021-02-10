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

namespace DotPulsar
{
    /// <summary>
    /// The possible states a consumer can be in.
    /// </summary>
    public enum ConsumerState : byte
    {
        /// <summary>
        /// The consumer is connected and active. The subscription type is 'Failover' and this consumer is the active consumer.
        /// </summary>
        Active,

        /// <summary>
        /// The consumer is closed. This is a final state.
        /// </summary>
        Closed,

        /// <summary>
        /// The consumer is disconnected.
        /// </summary>
        Disconnected,

        /// <summary>
        /// The consumer is faulted. This is a final state.
        /// </summary>
        Faulted,

        /// <summary>
        /// The consumer is connected and inactive. The subscription type is 'Failover' and this consumer is not the active consumer.
        /// </summary>
        Inactive,

        /// <summary>
        /// The consumer has reached the end of the topic. This is a final state.
        /// </summary>
        ReachedEndOfTopic,

        /// <summary>
        /// The consumer has unsubscribed. This is a final state.
        /// </summary>
        Unsubscribed
    }
}
