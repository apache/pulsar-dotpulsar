/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar.Internal;

using DotPulsar.Abstractions;
using Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class SendChannel<TMessage> : ISendChannel<TMessage>
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
    }

    public async ValueTask Completion(CancellationToken cancellationToken = default)
    {
        await _producer.WaitForSendQueueEmpty(cancellationToken).ConfigureAwait(false);
    }
}
