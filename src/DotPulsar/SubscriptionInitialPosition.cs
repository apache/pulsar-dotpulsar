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
    /// Intial position at which the cursor will be set when subscribing.
    /// </summary>
    public enum SubscriptionInitialPosition : byte
    {
        /// <summary>
        /// Consumption will start at the last message.
        /// </summary>
        Latest = 0,

        /// <summary>
        /// Consumption will start at the first message.
        /// </summary>
        Earliest = 1
    }
}
