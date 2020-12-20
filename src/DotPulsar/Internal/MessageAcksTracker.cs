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
    using System.Diagnostics;

    internal class Tracker
    {
        private readonly Stopwatch _timer;
        private long maxTimeoutMs;

        public Tracker(long timeoutMs)
        {
            maxTimeoutMs = timeoutMs;
            _timer = new Stopwatch();
            _timer.Start();
        }

        public bool IsTimedOut() => _timer.ElapsedMilliseconds > maxTimeoutMs;

        public long msTillTimeout => maxTimeoutMs - _timer.ElapsedMilliseconds;

        public void Reset(long newTimeoutMs)
        {
            maxTimeoutMs = newTimeoutMs;
            _timer.Restart();
        }
    }

    public sealed class MessageAcksTracker : IMessageAcksTracker<MessageId>
    {
        private readonly Dictionary<MessageId, Tracker> _trackers;
        private long _unackedTimeoutMs;
        private long _nackTimeoutMs;
        private int _trackerDelayMs;
        public MessageAcksTracker()
        {
            _trackers = new Dictionary<MessageId, Tracker>();
        }

        public async Task StartTracker(IConsumer consumer, CancellationToken cancellationToken)
        {
            await Task.Yield();

            while (true)
            {
                await Task.Delay(_trackerDelayMs);

                var messageIds = new List<MessageId>();
                foreach (KeyValuePair<MessageId, Tracker> p in _trackers)
                {
                    if (p.Value.IsTimedOut())
                        messageIds.Add(p.Key);
                }

                if (messageIds.Count() > 0)
                    await consumer.RedeliverUnacknowledgedMessages(messageIds, cancellationToken).ConfigureAwait(false);

            }
        }
        public MessageId Add(MessageId message)
        {
            if (!_trackers.ContainsKey(message))
            {
                _trackers.Add(message, new Tracker(_unackedTimeoutMs));
            }

            return message;
        }
        public MessageId Ack(MessageId message)
        {
            if (_trackers.ContainsKey(message))
                _trackers.Remove(message);
            return message;
        }
        public MessageId Nack(MessageId message)
        {
            if (_trackers.ContainsKey(message))
            {
                var timer = _trackers[message];
                if (timer.msTillTimeout > _nackTimeoutMs)
                    timer.Reset(_nackTimeoutMs);
            }
            else
                _trackers.Add(message, new Tracker(_nackTimeoutMs));
            return message;
        }
    }
}