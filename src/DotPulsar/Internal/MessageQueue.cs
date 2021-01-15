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
    using PulsarApi;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class MessageQueue : IMessageQueue
    {
        private readonly AsyncQueue<MessagePackage> _queue;
        private readonly IUnacknowledgedMessageTracker _tracker;

        public MessageQueue(AsyncQueue<MessagePackage> queue, IUnacknowledgedMessageTracker tracker)
        {
            _queue = queue;
            _tracker = tracker;
        }

        public async ValueTask<MessagePackage> Dequeue(CancellationToken cancellationToken = default)
        {
            var message = await _queue.Dequeue(cancellationToken).ConfigureAwait(false);
            _tracker.Add(message.MessageId);
            return message;
        }

        public void Acknowledge(MessageIdData obj) => _tracker.Acknowledge(obj);

        public void NegativeAcknowledge(MessageIdData obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _queue.Dispose();
            _tracker.Dispose();
        }
    }
}
