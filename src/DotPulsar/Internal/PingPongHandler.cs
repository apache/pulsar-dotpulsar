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

namespace DotPulsar.Internal
{
    using Abstractions;
    using DotPulsar.Abstractions;
    using Extensions;
    using PulsarApi;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class PingPongHandler
    {
        private readonly IConnection _connection;
        private readonly CommandPong _pong;
        private readonly IPulsarClientLogger? _logger;

        public PingPongHandler(IConnection connection, IPulsarClientLogger? logger)
        {
            _connection = connection;
            _logger = logger;
            _pong = new CommandPong();
        }

        public void Incoming(CommandPing ping, CancellationToken cancellationToken)
        {
            _logger.Trace(nameof(PingPongHandler), nameof(Incoming), "Received Ping command on {0}, sending Pong", _connection.Id);
            Task.Factory.StartNew(() => SendPong(cancellationToken));
        }

        private async Task SendPong(CancellationToken cancellationToken)
        {
            try
            {
                await _connection.Send(_pong, cancellationToken).ConfigureAwait(false);
                _logger.Trace(nameof(PingPongHandler), nameof(SendPong), "Sent Pong response on {0}", _connection.Id);
            }
            catch (Exception ex)
            {
                _logger.DebugException(nameof(PingPongHandler), nameof(SendPong), ex, "Exception while sending Pong response on {0}", _connection.Id);
            }
        }
    }
}
