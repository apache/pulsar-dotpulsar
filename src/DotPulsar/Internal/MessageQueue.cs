namespace DotPulsar.Internal
{
    using Abstractions;
    using DotPulsar.Abstractions;
    using DotPulsar.Exceptions;
    using Events;
    using Microsoft.Extensions.ObjectPool;
    using PulsarApi;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class MessageQueue : IMessageQueue<MessageId>
    {
        private readonly AsyncQueue<MessageId> _queue;
        private readonly IMessageAcksTracker<MessageId> _tracker;
        public MessageQueue(AsyncQueue<MessageId> queue, IMessageAcksTracker<MessageId> tracker)
        {
            _queue = queue;
            _tracker = tracker;
        }
        public async ValueTask<MessageId> Dequeue(CancellationToken cancellationToken = default)
        {
            var message = await _queue.Dequeue(cancellationToken).ConfigureAwait(false);
            return _tracker.Add(message);
        }
        public MessageId Acknowledge(MessageId obj) => _tracker.Ack(obj);
        public MessageId NegativeAcknowledge(MessageId obj) => _tracker.Nack(obj);
    }
}