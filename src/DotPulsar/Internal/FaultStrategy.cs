using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Exceptions;
using System;

namespace DotPulsar.Internal
{
    public sealed class FaultStrategy : IFaultStrategy
    {
        public FaultStrategy(int timeToWaitInMilliseconds)
        {
            TimeToWait = TimeSpan.FromMilliseconds(timeToWaitInMilliseconds);
        }

        public TimeSpan TimeToWait { get; }

        public FaultAction DetermineFaultAction(Exception exception)
        {
            switch (exception)
            {
                case TooManyRequestsException _: return FaultAction.Retry;
                case StreamNotReadyException _: return FaultAction.Relookup;
                case ServiceNotReadyException _: return FaultAction.Relookup;
                case DotPulsarException _: return FaultAction.Fault;
            }

            return FaultAction.Relookup;
        }
    }
}
