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
using DotPulsar.Internal.Exceptions;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class AsyncLockTests
{
    [Fact]
    public async Task Lock_GivenLockIsFree_ShouldReturnCompletedTask()
    {
        //Arrange
        var sut = new AsyncLock();

        //Act
        var actual = sut.Lock(CancellationToken.None);

        //Assert
        actual.IsCompleted.Should().BeTrue();

        //Annihilate 
        actual.Result.Dispose();
        await sut.DisposeAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task Lock_GivenLockIsTaken_ShouldReturnIncompletedTask()
    {
        //Arrange
        var sut = new AsyncLock();
        var alreadyTaken = await sut.Lock(CancellationToken.None).ConfigureAwait(false);

        //Act
        var actual = sut.Lock(CancellationToken.None);

        //Assert
        actual.IsCompleted.Should().BeFalse();

        //Annihilate
        alreadyTaken.Dispose();
        actual.Result.Dispose();
        await sut.DisposeAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task Lock_GivenLockIsDisposed_ShouldThrowAsyncLockDisposedException()
    {
        //Arrange
        var sut = new AsyncLock();
        await sut.DisposeAsync().ConfigureAwait(false);

        //Act
        var exception = await Record.ExceptionAsync(() => sut.Lock(CancellationToken.None)).ConfigureAwait(false);

        //Assert
        exception.Should().BeOfType<AsyncLockDisposedException>();
    }

    [Fact]
    public async Task Lock_GivenLockIsDisposedWhileAwaitingLock_ShouldThrowTaskCanceledException()
    {
        //Arrange
        var sut = new AsyncLock();
        var gotLock = await sut.Lock(CancellationToken.None).ConfigureAwait(false);
        var awaiting = sut.Lock(CancellationToken.None);
        _ = Task.Run(async () => await sut.DisposeAsync().ConfigureAwait(false));

        //Act
        var exception = await Record.ExceptionAsync(() => awaiting).ConfigureAwait(false);

        //Assert
        exception.Should().BeOfType<TaskCanceledException>();

        //Annihilate
        await sut.DisposeAsync().ConfigureAwait(false);
        gotLock.Dispose();
    }

    [Fact]
    public async Task Lock_GivenLockIsTakenAndCancellationTokenIsActivated_ShouldThrowTaskCanceledException()
    {
        //Arrange
        var cts = new CancellationTokenSource();
        var sut = new AsyncLock();
        var gotLock = await sut.Lock(CancellationToken.None).ConfigureAwait(false);
        var awaiting = sut.Lock(cts.Token);

        //Act
        cts.Cancel();
        var exception = await Record.ExceptionAsync(() => awaiting).ConfigureAwait(false);

        //Assert
        exception.Should().BeOfType<TaskCanceledException>();

        //Annihilate
        cts.Dispose();
        gotLock.Dispose();
        await sut.DisposeAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task Dispose_GivenLockIsDisposedWhileItIsTaken_ShouldNotCompleteBeforeItIsReleased()
    {
        //Arrange
        var sut = new AsyncLock();
        var gotLock = await sut.Lock(CancellationToken.None).ConfigureAwait(false);
        var disposeTask = Task.Run(async () => await sut.DisposeAsync().ConfigureAwait(false));
        Assert.False(disposeTask.IsCompleted);

        //Act
        gotLock.Dispose();
        await disposeTask.ConfigureAwait(false);

        //Assert
        disposeTask.IsCompleted.Should().BeTrue();

        //Annihilate
        await sut.DisposeAsync().ConfigureAwait(false);
    }

    [Fact]
    public async Task Dispose_WhenCalledMultipleTimes_ShouldBeSafeToDoSo()
    {
        //Arrange
        var sut = new AsyncLock();

        //Act
        await sut.DisposeAsync().ConfigureAwait(false);
        var exception = await Record.ExceptionAsync(() => sut.DisposeAsync().AsTask()).ConfigureAwait(false); // xUnit can't record ValueTask yet

        //Assert
        exception.Should().BeNull();
    }
}
