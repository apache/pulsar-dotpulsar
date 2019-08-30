using System;

namespace DotPulsar.Internal.Abstractions
{
    public interface IFaultStrategy
    {
        FaultAction DetermineFaultAction(Exception exception);
        TimeSpan RetryInterval { get; }
    }
}
