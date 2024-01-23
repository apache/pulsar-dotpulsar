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

using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Events;

public sealed class Channel : IChannel
{
    private readonly Lock _senderLock;
    private readonly Guid _correlationId;
    private readonly IRegisterEvent _eventRegister;
    private readonly IEnqueue<MessagePackage> _enqueue;

    public Channel(Guid correlationId, IRegisterEvent eventRegister, IEnqueue<MessagePackage> enqueue)
    {
        _senderLock = new Lock();
        _correlationId = correlationId;
        _eventRegister = eventRegister;
        _enqueue = enqueue;
    }

    public void Received(MessagePackage message)
    {
        try
        {
            _enqueue.Enqueue(message);
        }
        catch
        {
            // Ignore
        }
    }

    public void Activated()
        => _eventRegister.Register(new ChannelActivated(_correlationId));

    public void ClosedByServer()
    {
        _senderLock.Disable();
        _eventRegister.Register(new ChannelClosedByServer(_correlationId));
    }

    public void WaitingForExclusive()
        => _eventRegister.Register(new ProducerWaitingForExclusive(_correlationId));

    public void Connected()
        => _eventRegister.Register(new ChannelConnected(_correlationId));

    public void Deactivated()
        => _eventRegister.Register(new ChannelDeactivated(_correlationId));

    public void Disconnected()
    {
        _senderLock.Disable();
        _eventRegister.Register(new ChannelDisconnected(_correlationId));
    }

    public void ReachedEndOfTopic()
        => _eventRegister.Register(new ChannelReachedEndOfTopic(_correlationId));

    public void Unsubscribed()
        => _eventRegister.Register(new ChannelUnsubscribed(_correlationId));

    public IDisposable SenderLock()
        => _senderLock.Enter();

    private sealed class Lock : IDisposable
    {
        private readonly object _lock;
        private bool _canSend;

        public Lock()
        {
            _lock = new object();
            _canSend = true;
        }

        public void Disable()
        {
            Monitor.Enter(_lock);
            _canSend = false;
            Monitor.Exit(_lock);
        }

        public IDisposable Enter()
        {
            Monitor.Enter(_lock);

            if (_canSend)
                return this;

            Monitor.Exit(_lock);
            throw new OperationCanceledException();
        }

        public void Dispose()
            => Monitor.Exit(_lock);
    }
}
