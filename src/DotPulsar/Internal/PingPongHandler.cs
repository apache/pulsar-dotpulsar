﻿/*
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

using System.Threading;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;

namespace DotPulsar.Internal
{
    public sealed class PingPongHandler
    {
        private readonly IConnection _connection;
        private readonly CommandPong _pong;

        public PingPongHandler(IConnection connection)
        {
            _connection = connection;
            _pong = new CommandPong();
        }

        public void Incoming(CommandPing ping, CancellationToken cancellationToken)
        {
            _ = _connection.Send(_pong, cancellationToken);
        }
    }
}
