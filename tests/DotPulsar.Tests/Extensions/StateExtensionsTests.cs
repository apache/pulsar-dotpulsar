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
using NSubstitute.ExceptionExtensions;

[Trait("Category", "Unit")]
public class StateExtensionsTests
{
    [Theory, Tests.AutoData]
    public async Task OnStateChangeTo_WhenChangingToFinalStateInitially_ShouldReturnFinalState([Frozen] IState<ProducerState> uut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Faulted;
        uut.OnStateChangeTo(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Returns(expected);
        uut.IsFinalState(expected).Returns(true);

        // Act
        var actual = await uut.OnStateChangeTo(ProducerState.Disconnected, TimeSpan.FromSeconds(1));

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeTo_WhenChangingToFinalStateWhileWaitingForDelay_ShouldReturnFinalState([Frozen] IState<ProducerState> uut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Faulted;
        uut.OnStateChangeTo(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Returns(ProducerState.Disconnected);
        uut.IsFinalState(ProducerState.Disconnected).Returns(false);
        uut.OnStateChangeFrom(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Returns(expected);
        uut.IsFinalState(expected).Returns(true);

        // Act
        var actual = await uut.OnStateChangeTo(ProducerState.Disconnected, TimeSpan.FromSeconds(1));

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeTo_WhenTheDelayIsExceeded_ShouldReturnState([Frozen] IState<ProducerState> uut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Disconnected;
        uut.OnStateChangeTo(expected, Arg.Any<CancellationToken>()).Returns(expected);
        uut.IsFinalState(expected).Returns(false);
        uut.OnStateChangeFrom(expected, Arg.Any<CancellationToken>()).Throws<OperationCanceledException>();

        // Act
        var actual = await uut.OnStateChangeTo(expected, TimeSpan.FromSeconds(1));

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeTo_WhenCancellationTokenIsCancelledInitially_ShouldThrowException([Frozen] IState<ProducerState> uut)
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        uut.OnStateChangeTo(ProducerState.Disconnected, cts.Token).Throws<OperationCanceledException>();

        // Act
        var exception = await Record.ExceptionAsync(() => uut.OnStateChangeTo(ProducerState.Disconnected, TimeSpan.FromSeconds(1), cts.Token).AsTask());

        // Assert
        exception.ShouldBeOfType<OperationCanceledException>();
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeTo_WhenCancellationTokenIsCancelledWhileWaitingForDelay_ShouldThrowException([Frozen] IState<ProducerState> uut)
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        uut.OnStateChangeTo(ProducerState.Disconnected, cts.Token).Returns(ProducerState.Disconnected);
        uut.IsFinalState(ProducerState.Disconnected).Returns(false);
        uut.OnStateChangeFrom(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Throws<OperationCanceledException>();

        // Act
        var exception = await Record.ExceptionAsync(() => uut.OnStateChangeTo(ProducerState.Disconnected, TimeSpan.FromSeconds(1), cts.Token).AsTask());

        // Assert
        exception.ShouldBeOfType<OperationCanceledException>();
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeFrom_WhenChangingToFinalStateInitially_ShouldReturnFinalState([Frozen] IState<ProducerState> uut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Faulted;
        uut.OnStateChangeFrom(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Returns(expected);
        uut.IsFinalState(expected).Returns(true);

        // Act
        var actual = await uut.OnStateChangeFrom(ProducerState.Disconnected, TimeSpan.FromSeconds(1));

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeFrom_WhenChangingToFinalStateWhileWaitingForDelay_ShouldReturnFinalState([Frozen] IState<ProducerState> uut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Faulted;
        uut.OnStateChangeFrom(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Returns(ProducerState.Disconnected);
        uut.IsFinalState(ProducerState.Disconnected).Returns(false);
        uut.OnStateChangeTo(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Returns(expected);
        uut.IsFinalState(expected).Returns(true);

        // Act
        var actual = await uut.OnStateChangeFrom(ProducerState.Disconnected, TimeSpan.FromSeconds(1));

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeFrom_WhenTheDelayIsExceeded_ShouldReturnState([Frozen] IState<ProducerState> uut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Disconnected;
        uut.OnStateChangeFrom(ProducerState.Connected, Arg.Any<CancellationToken>()).Returns(expected);
        uut.IsFinalState(expected).Returns(false);
        uut.OnStateChangeTo(ProducerState.Connected, Arg.Any<CancellationToken>()).Throws<OperationCanceledException>();

        // Act
        var actual = await uut.OnStateChangeFrom(ProducerState.Connected, TimeSpan.FromSeconds(1));

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeFrom_WhenCancellationTokenIsCancelledInitially_ShouldThrowException([Frozen] IState<ProducerState> uut)
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        uut.OnStateChangeFrom(ProducerState.Disconnected, cts.Token).Throws<OperationCanceledException>();

        // Act
        var exception = await Record.ExceptionAsync(() => uut.OnStateChangeFrom(ProducerState.Disconnected, TimeSpan.FromSeconds(1), cts.Token).AsTask());

        // Assert
        exception.ShouldBeOfType<OperationCanceledException>();
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeFrom_WhenCancellationTokenIsCancelledWhileWaitingForDelay_ShouldThrowException([Frozen] IState<ProducerState> uut)
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        uut.OnStateChangeFrom(ProducerState.Disconnected, cts.Token).Returns(ProducerState.Disconnected);
        uut.IsFinalState(ProducerState.Disconnected).Returns(false);
        uut.OnStateChangeTo(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Throws<OperationCanceledException>();

        // Act
        var exception = await Record.ExceptionAsync(() => uut.OnStateChangeFrom(ProducerState.Disconnected, TimeSpan.FromSeconds(1), cts.Token).AsTask());

        // Assert
        exception.ShouldBeOfType<OperationCanceledException>();
    }
}
