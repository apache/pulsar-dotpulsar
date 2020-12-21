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
        private int maxTimeoutMs;

        public Tracker(int timeoutMs)
        {
            maxTimeoutMs = timeoutMs;
            _timer = new Stopwatch();
            _timer.Start();
        }

        public bool IsTimedOut() => _timer.ElapsedMilliseconds > maxTimeoutMs;

        public long msTillTimeout => maxTimeoutMs - _timer.ElapsedMilliseconds;

        public void Reset(int newTimeoutMs)
        {
            maxTimeoutMs = newTimeoutMs;
            _timer.Restart();
        }
    }

    // TODO add mechnism to stop tracker when disposed
    public sealed class MessageAcksTracker : IMessageAcksTracker<MessageId>
    {
        private readonly Dictionary<MessageId, Tracker> _trackers;
        private readonly int _unackedTimeoutMs;
        private readonly int _nackTimeoutMs;
        private readonly int _trackerDelayMs;
        public MessageAcksTracker(int unackedTimeoutMs, int nackTimeoutMs, int trackerDelayMs)
        {
            _unackedTimeoutMs = unackedTimeoutMs;
            _nackTimeoutMs = nackTimeoutMs;
            _trackerDelayMs = trackerDelayMs;
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