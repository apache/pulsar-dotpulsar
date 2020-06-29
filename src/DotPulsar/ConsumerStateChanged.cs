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
    using Abstractions;

    /// <summary>
    /// Representation of a consumer state change.
    /// </summary>
    public sealed class ConsumerStateChanged
    {
        internal ConsumerStateChanged(IConsumer consumer, ConsumerState consumerState)
        {
            Consumer = consumer;
            ConsumerState = consumerState;
        }

        /// <summary>
        /// The consumer that changed state.
        /// </summary>
        public IConsumer Consumer { get; }

        /// <summary>
        /// The state that it changed to.
        /// </summary>
        public ConsumerState ConsumerState { get; }
    }
}
