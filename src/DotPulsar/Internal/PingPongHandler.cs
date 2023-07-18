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
using DotPulsar.Internal.PulsarApi;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public sealed class PingPongHandler : IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly Timer _timer;
    private readonly CommandPing _ping;
    private readonly CommandPong _pong;
    private volatile bool _waitForPongResponse;

    public PingPongHandler(IConnection connection, TimeSpan keepAliveInterval)
    {
        _connection = connection;
        _timer = new Timer(Watch);
        _timer.Change(keepAliveInterval, keepAliveInterval);
        _ping = new CommandPing();
        _pong = new CommandPong();
    }

    public bool Incoming(BaseCommand.Type commandType)
    {
        _waitForPongResponse = false;

        if (commandType == BaseCommand.Type.Ping)
        {
            Task.Factory.StartNew(() => SendPong());
            return true;
        }

        return commandType == BaseCommand.Type.Pong;
    }

    private void Watch(object? state)
    {
        try
        {
            if (_waitForPongResponse)
            {
                _connection.DisposeAsync();
                return;
            }
            Task.Factory.StartNew(() => SendPing());
            _waitForPongResponse = true;
        }
        catch
        {
            // Ignore
        }
    }

    private async Task SendPing()
    {
        try
        {
            await _connection.Send(_ping, default).ConfigureAwait(false);
        }
        catch { }
    }

    private async Task SendPong()
    {
        try
        {
            await _connection.Send(_pong, default).ConfigureAwait(false);
        }
        catch { }
    }

#if NETSTANDARD2_0
    public ValueTask DisposeAsync()
    {
        _timer.Dispose();
        return new ValueTask();
    }
#else
    public async ValueTask DisposeAsync()
    {
        await _timer.DisposeAsync().ConfigureAwait(false);
    }
#endif
}
