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
    using Abstractions;
    using DotPulsar.Abstractions;
    using PulsarApi;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public readonly struct AwaitingAck
    {
        public MessageIdData MessageId { get; }
        public long Timestamp { get; }

        public AwaitingAck(MessageIdData messageId)
        {
            MessageId = messageId;
            Timestamp = Stopwatch.GetTimestamp();
        }

        public TimeSpan Elapsed => TimeSpan.FromTicks(
            (long) ((Stopwatch.GetTimestamp() - Timestamp) / (double) Stopwatch.Frequency * TimeSpan.TicksPerSecond));
    }

    public sealed class UnackedMessageTracker : IUnacknowledgedMessageTracker
    {
        private readonly TimeSpan _ackTimeout;
        private readonly TimeSpan _pollingTimeout;
        private readonly ConcurrentQueue<AwaitingAck> _awaitingAcks;
        private readonly List<MessageIdData> _acked;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public UnackedMessageTracker(TimeSpan ackTimeout, TimeSpan pollingTimeout)
        {
            _ackTimeout = ackTimeout;
            _pollingTimeout = pollingTimeout;
            _awaitingAcks = new ConcurrentQueue<AwaitingAck>();
            _acked = new List<MessageIdData>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Add(MessageIdData messageId)
        {
            _awaitingAcks.Enqueue(new AwaitingAck(messageId));
        }

        public void Acknowledge(MessageIdData messageId)
        {
            // We only need to store the highest cumulative ack we see (if there is one)
            // and the MessageIds not included by that cumulative ack.
            _acked.Add(messageId);
        }

        public Task Start(IConsumer consumer, CancellationToken cancellationToken = default)
        {
            CancellationToken token =
              CancellationTokenSource.CreateLinkedTokenSource(
                  _cancellationTokenSource.Token, cancellationToken).Token;

            return Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var messages = CheckUnackedMessages();

                    if (messages.Count() > 0)
                        await consumer.RedeliverUnacknowledgedMessages(messages, token);

                    await Task.Delay(_pollingTimeout, token);
                }
            }, token);
        }

        private IEnumerable<MessageIdData> CheckUnackedMessages()
        {
            var result = new List<MessageIdData>();

            while (_awaitingAcks.TryPeek(out AwaitingAck awaiting)
                && awaiting.Elapsed > _ackTimeout)
            {
                if (_awaitingAcks.TryDequeue(out awaiting))
                {
                    if (!_acked.Contains(awaiting.MessageId))
                        result.Add(awaiting.MessageId);
                    else
                        _acked.Remove(awaiting.MessageId);
                }
            }

            return result;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
