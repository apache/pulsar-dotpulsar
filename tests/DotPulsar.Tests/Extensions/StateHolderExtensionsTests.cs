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

namespace DotPulsar.Tests.Extensions;

using AutoFixture.Xunit2;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using System;

[Trait("Category", "Unit")]
public class StateHolderExtensionsTests
{
    [Theory, Tests.AutoData]
    public async Task DelayedStateMonitor_WhenChangingToFinalStateInitially_ShouldInvokeOnStateLeft([Frozen] IStateHolder<ProducerState> uut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Faulted;
        uut.State.OnStateChangeFrom(ProducerState.Connected, Arg.Any<CancellationToken>()).Returns(expected);
        uut.State.IsFinalState(expected).Returns(true);
        ProducerState? stateLeft = null;
        ProducerState? stateReached = null;
        void onStateLeft(IStateHolder<ProducerState> _, ProducerState state) => stateLeft = state;
        void onStateReached(IStateHolder<ProducerState> _, ProducerState state) => stateReached = state;

        // Act
        await uut.DelayedStateMonitor(ProducerState.Connected, TimeSpan.FromSeconds(1), onStateLeft, onStateReached);

        // Assert
        stateLeft.HasValue.ShouldBeTrue();
        stateLeft!.Value.ShouldBe(expected);
        stateReached.HasValue.ShouldBeFalse();
    }

    [Theory, Tests.AutoData]
    public async Task DelayedStateMonitor_WhenChangingToFinalStateWhileWaiting_ShouldInvokeOnStateLeft([Frozen] IStateHolder<ProducerState> uut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Disconnected;
        uut.State.OnStateChangeFrom(ProducerState.Connected, Arg.Any<CancellationToken>()).Returns(expected);
        uut.State.IsFinalState(ProducerState.Disconnected).Returns(false);
        uut.State.OnStateChangeTo(ProducerState.Connected, Arg.Any<CancellationToken>()).Returns(x => throw new OperationCanceledException(), x => ProducerState.Faulted);
        uut.State.IsFinalState(ProducerState.Faulted).Returns(true);
        ProducerState? stateLeft = null;
        ProducerState? stateReached = null;
        void onStateLeft(IStateHolder<ProducerState> _, ProducerState state) => stateLeft = state;
        void onStateReached(IStateHolder<ProducerState> _, ProducerState state) => stateReached = state;

        // Act
        await uut.DelayedStateMonitor(ProducerState.Connected, TimeSpan.FromSeconds(1), onStateLeft, onStateReached);

        // Assert
        stateLeft.HasValue.ShouldBeTrue();
        stateLeft!.Value.ShouldBe(expected);
        stateReached.HasValue.ShouldBeFalse();
    }
}
