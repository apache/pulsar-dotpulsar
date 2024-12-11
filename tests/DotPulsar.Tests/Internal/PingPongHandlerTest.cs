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
using System;

[Trait("Category", "Unit")]
public class PingPongHandlerTest
{
    [Fact]
    public async Task Constructor_GivenNoIncomingCommands_ShouldChangeStateToTimedOut()
    {
        // Arrange
        var expected = PingPongHandlerState.TimedOut;
        var uut = new PingPongHandler(TimeSpan.FromSeconds(1));

        // Act
        var actual = await uut.State.OnStateChangeTo(PingPongHandlerState.TimedOut);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task Incoming_GivenIncomingCommandAfterThresholdExceeded_ShouldChangeStateToActive()
    {
        // Arrange
        var expected = PingPongHandlerState.Active;
        var uut = new PingPongHandler(TimeSpan.FromSeconds(1));

        // Act
        _ = await uut.State.OnStateChangeTo(PingPongHandlerState.ThresholdExceeded);
        uut.Incoming(DotPulsar.Internal.PulsarApi.BaseCommand.Type.Ack);
        var actual = await uut.State.OnStateChangeTo(PingPongHandlerState.Active);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public async Task Dispose_GivenTheStateWasActive_ShouldChangeStateToClosed()
    {
        // Arrange
        var expected = PingPongHandlerState.Closed;
        var uut = new PingPongHandler(TimeSpan.FromSeconds(1));

        // Act
        var actual = uut.State.OnStateChangeTo(PingPongHandlerState.Closed);
        await uut.DisposeAsync();

        // Assert
        (await actual).Should().Be(expected);
    }
}
