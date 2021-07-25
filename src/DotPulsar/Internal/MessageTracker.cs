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
    using DotPulsar.Abstractions;
    using DotPulsar.Internal.Abstractions;
    using DotPulsar.Internal.PulsarApi;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class MessageTracker : IMessageTracker
    {
        private readonly TimeSpan _pollingTimeout;
        private readonly IUnackedMessageState _ackState;
        private readonly INegativeackedMessageState _nackState;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public MessageTracker(TimeSpan pollingTimeout, IUnackedMessageState ackedState, INegativeackedMessageState nackedState)
        {
            _pollingTimeout = pollingTimeout;
            _ackState = ackedState;
            _nackState = nackedState;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Start(IConsumer consumer, CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            CancellationToken token =
              CancellationTokenSource.CreateLinkedTokenSource(
                  _cancellationTokenSource.Token, cancellationToken).Token;

            while (!token.IsCancellationRequested)
            {
                var toRedeliver = _ackState.CheckUnackedMessages()
                    .Concat(_nackState.GetMessagesForRedelivery()).ToList();

                if (toRedeliver.Count() > 0)
                    await consumer.RedeliverUnacknowledgedMessages(toRedeliver, token);

                await Task.Delay(_pollingTimeout, token);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Track(MessageIdData messageId)
        {
            _ackState.Add(messageId);
        }

        public void Acknowledge(MessageIdData messageId)
        {
            _ackState.Acknowledge(messageId);
        }

        public void NegativeAcknowledge(MessageIdData messageId)
        {
            _nackState.Add(messageId);
            _ackState.Remove(messageId);
        }
    }
}
