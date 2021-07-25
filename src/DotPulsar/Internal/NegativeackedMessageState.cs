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

namespace DotPulsar.Internal
{
    using DotPulsar.Internal.Abstractions;
    using DotPulsar.Internal.PulsarApi;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class NegativeackedMessageState : INegativeackedMessageState
    {
        private readonly TimeSpan _nackRedeliveryDelay;
        private readonly ConcurrentQueue<AwaitingAck> _awaitingRedelivery;

        public NegativeackedMessageState(TimeSpan nackRedeliveryDelay)
        {
            _nackRedeliveryDelay = nackRedeliveryDelay;
            _awaitingRedelivery = new ConcurrentQueue<AwaitingAck>();
        }

        public void Add(MessageIdData messageId)
        {
            _awaitingRedelivery.Enqueue(new AwaitingAck(messageId));
        }

        public IEnumerable<MessageIdData> GetMessagesForRedelivery()
        {
            var result = new List<MessageIdData>();

            while (_awaitingRedelivery.TryPeek(out AwaitingAck awaiting)
                && awaiting.Elapsed > _nackRedeliveryDelay)
            {
                if (_awaitingRedelivery.TryDequeue(out awaiting))
                {
                    result.Add(awaiting.MessageId);
                }
            }

            return result;
        }
    }
}
