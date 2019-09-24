using DotPulsar.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DotPulsar.Tests.Internal
{
    public class AsyncLockTests
    {
        [Fact]
        public async Task Lock_GivenLockIsFree_ShouldReturnCompletedTask()
        {
            //Arrange
            var sut = new AsyncLock();

            //Act
            var actual = sut.Lock();

            //Assert
            Assert.True(actual.IsCompleted);

            //Annihilate 
            actual.Result.Dispose();
            await sut.DisposeAsync();
        }

        [Fact]
        public async Task Lock_GivenLockIsTaken_ShouldReturnIncompletedTask()
        {
            //Arrange
            var sut = new AsyncLock();
            var alreadyTaken = await sut.Lock();

            //Act
            var actual = sut.Lock();

            //Assert
            Assert.False(actual.IsCompleted);

            //Annihilate
            alreadyTaken.Dispose();
            actual.Result.Dispose();
            await sut.DisposeAsync();
        }

        [Fact]
        public async Task Lock_GivenLockIsDisposed_ShouldThrowObjectDisposedException()
        {
            //Arrange
            var sut = new AsyncLock();
            await sut.DisposeAsync();

            //Act
            var exception = await Record.ExceptionAsync(() => sut.Lock());

            //Assert
            Assert.IsType<ObjectDisposedException>(exception);
        }

        [Fact]
        public async Task Lock_GivenLockIsDisposedWhileAwaitingLock_ShouldThrowObjectDisposedException()
        {
            //Arrange
            var sut = new AsyncLock();
            var gotLock = await sut.Lock();
            var awaiting = sut.Lock();
            _ = Task.Run(async () => await sut.DisposeAsync());

            //Act
            var exception = await Record.ExceptionAsync(() => awaiting);

            //Assert
            Assert.IsType<ObjectDisposedException>(exception);

            //Annihilate
            await sut.DisposeAsync();
            gotLock.Dispose();
        }

        [Fact]
        public async Task Lock_GivenLockIsTakenAndCancellationTokenIsActivated_ShouldThrowTaskCanceledException()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var sut = new AsyncLock();
            var gotLock = await sut.Lock();
            var awaiting = sut.Lock(cts.Token);

            //Act
            cts.Cancel();
            var exception = await Record.ExceptionAsync(() => awaiting);

            //Assert
            Assert.IsType<TaskCanceledException>(exception);

            //Annihilate
            cts.Dispose();
            gotLock.Dispose();
            await sut.DisposeAsync();
        }

        [Fact]
        public async Task Dispose_GivenLockIsDisposedWhileItIsTaken_ShouldNotCompleteBeforeItIsReleased()
        {
            //Arrange
            var sut = new AsyncLock();
            var gotLock = await sut.Lock();
            var disposeTask = Task.Run(async () => await sut.DisposeAsync());
            Assert.False(disposeTask.IsCompleted);

            //Act
            gotLock.Dispose();
            await disposeTask;

            //Assert
            Assert.True(disposeTask.IsCompleted);

            //Annihilate
            await sut.DisposeAsync();
        }

        [Fact]
        public async Task Dispose_WhenCalledMultipleTimes_ShouldBeSafeToDoSo()
        {
            //Arrange
            var sut = new AsyncLock();

            //Act
            await sut.DisposeAsync();
            var exception = await Record.ExceptionAsync(() => sut.DisposeAsync().AsTask()); // xUnit can't record ValueTask yet

            //Assert
            Assert.Null(exception);
        }
    }
}
