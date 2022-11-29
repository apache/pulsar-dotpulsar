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
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[Trait("Category", "Unit")]
public class StateExtensionsTests
{
    [Theory, Tests.AutoData]
    public async Task OnStateChangeTo_WhenChangingToFinalStateInitially_ShouldReturnFinalState([Frozen] IState<ProducerState> sut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Faulted;
        sut.OnStateChangeTo(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Returns(expected);
        sut.IsFinalState(expected).Returns(true);

        // Act
        var actual = await sut.OnStateChangeTo(ProducerState.Disconnected, TimeSpan.FromSeconds(1));

        // Assert
        actual.Should().Be(expected);
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeTo_WhenChangingToFinalStateWhileWaitingForDelay_ShouldReturnFinalState([Frozen] IState<ProducerState> sut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Faulted;
        sut.OnStateChangeTo(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Returns(ProducerState.Disconnected);
        sut.IsFinalState(ProducerState.Disconnected).Returns(false);
        sut.OnStateChangeFrom(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Returns(expected);
        sut.IsFinalState(expected).Returns(true);

        // Act
        var actual = await sut.OnStateChangeTo(ProducerState.Disconnected, TimeSpan.FromSeconds(1));

        // Assert
        actual.Should().Be(expected);
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeTo_WhenTheDelayIsExceeded_ShouldReturnState([Frozen] IState<ProducerState> sut)
    {
        // Arrange
        const ProducerState expected = ProducerState.Disconnected;
        sut.OnStateChangeTo(expected, Arg.Any<CancellationToken>()).Returns(expected);
        sut.IsFinalState(expected).Returns(false);
        sut.OnStateChangeFrom(expected, Arg.Any<CancellationToken>()).Throws<OperationCanceledException>();

        // Act
        var actual = await sut.OnStateChangeTo(expected, TimeSpan.FromSeconds(1));

        // Assert
        actual.Should().Be(expected);
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeTo_WhenCancellationTokenIsCancelledInitially_ShouldThrowException([Frozen] IState<ProducerState> sut)
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        sut.OnStateChangeTo(ProducerState.Disconnected, cts.Token).Throws<OperationCanceledException>();

        // Act
        var exception = await Record.ExceptionAsync(() => sut.OnStateChangeTo(ProducerState.Disconnected, TimeSpan.FromSeconds(1), cts.Token).AsTask());

        // Assert
        exception.Should().BeOfType<OperationCanceledException>();
    }

    [Theory, Tests.AutoData]
    public async Task OnStateChangeTo_WhenCancellationTokenIsCancelledWhileWaitingForDelay_ShouldThrowException([Frozen] IState<ProducerState> sut)
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        sut.OnStateChangeTo(ProducerState.Disconnected, cts.Token).Returns(ProducerState.Disconnected);
        sut.IsFinalState(ProducerState.Disconnected).Returns(false);
        sut.OnStateChangeFrom(ProducerState.Disconnected, Arg.Any<CancellationToken>()).Throws<OperationCanceledException>();

        // Act
        var exception = await Record.ExceptionAsync(() => sut.OnStateChangeTo(ProducerState.Disconnected, TimeSpan.FromSeconds(1), cts.Token).AsTask());

        // Assert
        exception.Should().BeOfType<OperationCanceledException>();
    }
}
