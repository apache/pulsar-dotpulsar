﻿/*
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
    using Abstractions;
    using DotPulsar.Abstractions;
    using DotPulsar.Internal.PulsarApi;
    using System.Threading;
    using System.Threading.Tasks;

    public class InactiveMessageTracker : IMessageTracker
    {
        public void Acknowledge(MessageIdData messageId)
        {
            return;
        }

        public void Dispose()
        {
            return;
        }

        public void NegativeAcknowledge(MessageIdData messageId)
        {
            return;
        }

        public Task Start(IConsumer consumer, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Track(MessageIdData messageId)
        {
            return;
        }
    }
}
