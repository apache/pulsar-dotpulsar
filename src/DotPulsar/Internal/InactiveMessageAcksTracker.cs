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

    public sealed class InactiveMessageAcksTracker : IMessageAcksTracker<MessageId>
    {
        public InactiveMessageAcksTracker() { }

        public async Task StartTracker(IConsumer consumer, CancellationToken cancellationToken)
        {
            await Task.Yield();
        }

        public MessageId Add(MessageId message) => message;
        public MessageId Ack(MessageId message) => message;
        public MessageId Nack(MessageId message) => message;
    }
}