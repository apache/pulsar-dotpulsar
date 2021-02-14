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
    /// Subscription types the consumer can choose from when subscribing.
    /// </summary>
    public enum SubscriptionType : byte
    {
        /// <summary>
        /// There can be only 1 consumer on the same topic with the same subscription name.
        /// </summary>
        Exclusive = 0,

        /// <summary>
        /// Multiple consumers will be able to use the same subscription name and the messages will be dispatched according to a round-robin rotation.
        /// </summary>
        Shared = 1,

        /// <summary>
        /// Multiple consumers will be able to use the same subscription name but only 1 consumer will receive the messages.
        /// </summary>
        Failover = 2,

        /// <summary>
        /// Multiple consumers will be able to use the same subscription name and the messages will be dispatched according to the key.
        /// </summary>
        KeyShared = 3
    }
}
