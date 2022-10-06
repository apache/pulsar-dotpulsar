namespace DotPulsar.Tests.Internal;

using DotPulsar.Internal;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[Trait("Category", "Unit")]
public class AsyncQueueWithCursorTests
{
    [Fact]
    public async Task TryPeek_GivenQueueIsEmpty_ShouldReturnFalse()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<int>(10);

        //Act
        var actual = sut.TryPeek(out _);

        //Assert
        actual.Should().BeFalse();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task TryPeek_GivenQueueHasItem_ShouldReturnTrueAndItem()
    {
        //Arrange
        const int expected = 1;
        var sut = new AsyncQueueWithCursor<int>(10);
        await sut.Enqueue(expected, CancellationToken.None);

        //Act
        var actual = sut.TryPeek(out int result);

        //Assert
        actual.Should().BeTrue();
        result.Should().Be(expected);

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task Dequeue_GivenQueueHasItem_ShouldRemoveItemFromQueue()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<int>(10);
        await sut.Enqueue(1, CancellationToken.None);

        //Act
        sut.Dequeue();
        var actual = sut.TryPeek(out _);

        //Assert
        actual.Should().BeFalse();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task Dequeue_GivenQueueIsEmpty_ShouldThrowException()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<int>(10);

        //Act
        Action act = () => sut.Dequeue();

        //Assert
        act.Should().Throw<InvalidOperationException>();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task Enqueue_GivenQueueIsEmpty_ShouldAddItem()
    {
        //Arrange
        const int expected = 1;
        var sut = new AsyncQueueWithCursor<int>(10);

        //Act
        await sut.Enqueue(expected, CancellationToken.None);
        var actual = sut.TryPeek(out int result);

        //Assert
        actual.Should().BeTrue();
        result.Should().Be(expected);

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task Enqueue_GivenQueueIsFull_ShouldBlockInsertion()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<int>(1);
        await sut.Enqueue(1, CancellationToken.None);

        //Act
        var actual = sut.Enqueue(2, CancellationToken.None);

        //Assert
        actual.IsCompleted.Should().BeFalse();
        actual.IsFaulted.Should().BeFalse();
        actual.IsCanceled.Should().BeFalse();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task Dequeue_GivenQueueIsFullAndEnqueuePending_ShouldDequeueAndEnqueue()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<int>(1);
        await sut.Enqueue(1, CancellationToken.None);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100);
        var pendingEnqueue = sut.Enqueue(2, cts.Token);

        //Act
        sut.Dequeue();
        await pendingEnqueue;

        //Assert
        pendingEnqueue.IsCompleted.Should().BeTrue();
        pendingEnqueue.IsFaulted.Should().BeFalse();
        pendingEnqueue.IsCanceled.Should().BeFalse();

        //Annihilate
        cts.Dispose();
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task Enqueue_GivenQueueIsFullAndTokenCanceled_ShouldCancelTask()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<int>(1);
        await sut.Enqueue(1, CancellationToken.None);
        var cts = new CancellationTokenSource();

        //Act
        var pendingEnqueue = sut.Enqueue(2, cts.Token).AsTask();
        cts.Cancel();
        var exception = await Record.ExceptionAsync(() => pendingEnqueue).ConfigureAwait(false);

        //Assert
        exception.Should().BeOfType<TaskCanceledException>();
        pendingEnqueue.IsCompleted.Should().BeTrue();
        pendingEnqueue.IsFaulted.Should().BeFalse();
        pendingEnqueue.IsCanceled.Should().BeTrue();

        //Annihilate
        cts.Dispose();
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task NextItem_GivenQueueHasItems_ShouldReturnItems()
    {
        //Arrange
        const int expected1 = 1;
        const int expected2 = 2;
        var sut = new AsyncQueueWithCursor<int>(2);
        await sut.Enqueue(expected1, CancellationToken.None);
        await sut.Enqueue(expected2, CancellationToken.None);

        //Act
        int actual1 = await sut.NextItem(CancellationToken.None);
        int actual2 = await sut.NextItem(CancellationToken.None);

        //Assert
        actual1.Should().Be(expected1);
        actual2.Should().Be(expected2);

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task NextItem_GivenQueueIfFull_ShouldBlock()
    {
        //Arrange
        const int expected1 = 1;
        var sut = new AsyncQueueWithCursor<int>(1);
        await sut.Enqueue(expected1, CancellationToken.None);
        await sut.NextItem(CancellationToken.None);

        //Act
        var actual = sut.NextItem(CancellationToken.None);

        //Assert
        actual.IsCompleted.Should().BeFalse();
        actual.IsFaulted.Should().BeFalse();
        actual.IsCanceled.Should().BeFalse();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task NextItem_GivenItemInserted_ShouldReturnInsertedItem()
    {
        //Arrange
        const int expected = 1;
        var sut = new AsyncQueueWithCursor<int>(1);

        //Act
        var task = sut.NextItem(CancellationToken.None);
        await sut.Enqueue(expected, CancellationToken.None);
        var actual = await task;

        //Assert
        actual.Should().Be(expected);
        task.IsCompleted.Should().BeTrue();
        task.IsFaulted.Should().BeFalse();
        task.IsCanceled.Should().BeFalse();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task ResetCursor_GivenCursorNotAtFirstElement_ShouldMoveToFirst()
    {
        //Arrange
        const int expected1 = 1;
        const int expected2 = 2;
        var sut = new AsyncQueueWithCursor<int>(2);
        await sut.Enqueue(expected1, CancellationToken.None);
        await sut.Enqueue(expected2, CancellationToken.None);
        await sut.NextItem(CancellationToken.None);

        //Act
        int before = await sut.NextItem(CancellationToken.None);
        sut.ResetCursor();
        int after = await sut.NextItem(CancellationToken.None);

        //Assert
        before.Should().Be(expected2);
        after.Should().Be(expected1);

        //Annihilate
        await sut.DisposeAsync();
    }
}
