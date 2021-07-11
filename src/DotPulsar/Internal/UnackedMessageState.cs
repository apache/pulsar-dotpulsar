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
    using PulsarApi;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public sealed class UnackedMessageState : IUnackedMessageState
    {
        private readonly TimeSpan _ackTimeout;
        private readonly ConcurrentQueue<AwaitingAck> _awaitingAcks;
        private readonly List<MessageIdData> _ackedOrRemoved;

        public UnackedMessageState(TimeSpan ackTimeout)
        {
            _ackTimeout = ackTimeout;
            _awaitingAcks = new ConcurrentQueue<AwaitingAck>();
            _ackedOrRemoved = new List<MessageIdData>();
        }

        public void Add(MessageIdData messageId)
        {
            _awaitingAcks.Enqueue(new AwaitingAck(messageId));
        }

        public void Remove(MessageIdData messageId)
        {
            _ackedOrRemoved.Add(messageId);
        }

        public void Acknowledge(MessageIdData messageId)
        {
            // We only need to store the highest cumulative ack we see (if there is one)
            // and the MessageIds not included by that cumulative ack.
            _ackedOrRemoved.Add(messageId);
        }

        public IEnumerable<MessageIdData> CheckUnackedMessages()
        {
            var result = new List<MessageIdData>();

            while (_awaitingAcks.TryPeek(out AwaitingAck awaiting)
                && awaiting.Elapsed > _ackTimeout)
            {
                if (_awaitingAcks.TryDequeue(out awaiting))
                {
                    if (!_ackedOrRemoved.Contains(awaiting.MessageId))
                        result.Add(awaiting.MessageId);
                    else
                        _ackedOrRemoved.Remove(awaiting.MessageId);
                }
            }

            return result;
        }
    }
}