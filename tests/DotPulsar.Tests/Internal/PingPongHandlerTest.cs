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

namespace DotPulsar.Tests.Internal;

using DotPulsar.Internal;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[Trait("Category", "Unit")]
public class PingPongHandlerTest
{
    [Fact]
    public async Task Watch_GivenConnectionNotAlive_ShouldDisposeConnection()
    {
        var connection = Substitute.For<IConnection>();
        var keepAliveInterval = TimeSpan.FromSeconds(1);
        var isActive = true;
        var pingPongHandler = new PingPongHandler(connection, keepAliveInterval, () => isActive = false);

        connection.When(c => c.Send(Arg.Any<CommandPing>(), Arg.Any<CancellationToken>())).Do(c => pingPongHandler.Incoming(BaseCommand.Type.Pong));
        await Task.Delay(3 * keepAliveInterval);
        isActive.Should().BeTrue();

        connection.ClearSubstitute();
        await Task.Delay(3 * keepAliveInterval);
        isActive.Should().BeFalse();
    }
}
