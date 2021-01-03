namespace DotPulsar.Internal
{
    using Abstractions;
    using DotPulsar.Abstractions;
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public readonly struct AwaitingAck
    {
        public MessageId MessageId { get; }
        public long Timestamp { get; }

        public AwaitingAck(MessageId messageId)
        {
            MessageId = messageId;
            Timestamp = Stopwatch.GetTimestamp();
        }

        public TimeSpan Elapsed => TimeSpan.FromTicks(
            (long) ((Stopwatch.GetTimestamp() - Timestamp) / (double)Stopwatch.Frequency * TimeSpan.TicksPerSecond));
    }

    public sealed class UnackedMessageTracker : IUnackedMessageTracker
    {
        private readonly TimeSpan _ackTimeout;
        private readonly TimeSpan _pollingTimeout;
        private readonly ConcurrentQueue<AwaitingAck> _awaitingAcks;
        private readonly List<MessageId> _acked;
        private readonly CancellationTokenSource _cancellationTokenSource;

        
        public UnackedMessageTracker(TimeSpan ackTimeout, TimeSpan pollingTimeout)
        {
            _ackTimeout = ackTimeout;
            _pollingTimeout = pollingTimeout;
            _awaitingAcks = new ConcurrentQueue<AwaitingAck>();
            _acked = new List<MessageId>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Add(MessageId messageId)
        {
            _awaitingAcks.Enqueue(new AwaitingAck(messageId));
        }

        public void Ack(MessageId messageId)
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

            return Task.Run(async () => {
                while (!token.IsCancellationRequested)
                {
                    var messages = CheckUnackedMessages();

                    if (messages.Count() > 0)
                        await consumer.RedeliverUnacknowledgedMessages(messages, token);

                    await Task.Delay(_pollingTimeout, token);
                }
            }, token);
        }

        private IEnumerable<MessageId> CheckUnackedMessages()
        {
            var result = new List<MessageId>();

            while (_awaitingAcks.TryPeek(out AwaitingAck awaiting)
                && awaiting.Elapsed > _ackTimeout)
            {
                // Can I safely use Dequeue now instead of TryDequeue?
                if (_awaitingAcks.TryDequeue(out awaiting))
                {
                    //If the MessageId is not acknowledged
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
            this._cancellationTokenSource.Cancel();
        }
    }
}
