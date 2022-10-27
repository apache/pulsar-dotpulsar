namespace DotPulsar.Internal;

using DotPulsar.Abstractions;
using Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

public class SendChannel<TMessage> : ISendChannel<TMessage>
{
    private readonly Producer<TMessage> _producer;
    private int _isCompleted;

    public SendChannel(Producer<TMessage> producer)
    {
        _producer = producer;
    }

    public async ValueTask Send(MessageMetadata metadata, TMessage message, Func<MessageId, ValueTask>? onMessageSent = default, CancellationToken cancellationToken = default)
    {
        if (_isCompleted != 0) throw new SendChannelCompletedException();
        await _producer.Enqueue(metadata, message, onMessageSent, cancellationToken).ConfigureAwait(false);
    }

    public void Complete()
    {
        _isCompleted = 1;
        //Interlocked.Exchange(ref _isCompleted, 1);
    }

    public async ValueTask Completion(CancellationToken cancellationToken = default)
    {
        await _producer.WaitForSendQueueEmpty(cancellationToken).ConfigureAwait(false);
    }
}
