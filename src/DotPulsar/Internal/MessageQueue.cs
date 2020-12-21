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

    public sealed class MessageQueue : IMessageQueue, IDequeue<MessagePackage>, IDisposable
    {
        private readonly AsyncQueue<MessagePackage> _queue;
        private readonly IMessageAcksTracker<MessageId> _tracker;
        public MessageQueue(AsyncQueue<MessagePackage> queue, IMessageAcksTracker<MessageId> tracker)
        {
            _queue = queue;
            _tracker = tracker;
        }
        public async ValueTask<MessagePackage> Dequeue(CancellationToken cancellationToken = default)
        {
            var message = await _queue.Dequeue(cancellationToken).ConfigureAwait(false);
            _tracker.Add(new MessageId(message.MessageId));
            return message;
        }
        public MessageId Acknowledge(MessageId obj) => _tracker.Ack(obj);
        public MessageId NegativeAcknowledge(MessageId obj) => _tracker.Nack(obj);

        public void Dispose()
        {
            _queue.Dispose();
            // TODO dispose tracker
        }
    }
}