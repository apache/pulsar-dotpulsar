namespace DotPulsar.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    public class Watchdog: IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly TimeSpan _timeout;
        private readonly Timer _timer;

        public Watchdog(TimeSpan timeout)
        {
            _timeout = timeout;
            _timer = new Timer(OnTimeout);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public CancellationToken CancellationToken
        {
            get => _cancellationTokenSource.Token;
        }

        public void GotMessage()
        {
            ResetTimer();
        }

        public void Enable()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource = new CancellationTokenSource();
            ResetTimer();
        }

        public void Disable()
        {
            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private void ResetTimer()
        {
            _timer.Change(_timeout, Timeout.InfiniteTimeSpan);
        }

        private void OnTimeout(object state)
        {
            Console.WriteLine("Watchdog timeout");
            Disable();
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            _timer.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }
}
