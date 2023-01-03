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
using NSubstitute;
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
        var sut = new AsyncQueueWithCursor<IDisposable>(10);

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
        var expected = new QueueItem(1);
        var sut = new AsyncQueueWithCursor<QueueItem>(10);
        await sut.Enqueue(expected, CancellationToken.None);

        //Act
        var actual = sut.TryPeek(out QueueItem? result);

        //Assert
        actual.Should().BeTrue();
        result.Should().NotBeNull();
        result!.Should().Be(expected);

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task Dequeue_GivenQueueHasItem_ShouldRemoveItemFromQueue()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<IDisposable>(10);
        await sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);

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
        var sut = new AsyncQueueWithCursor<IDisposable>(10);

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
        var expected = new QueueItem(1);
        var sut = new AsyncQueueWithCursor<QueueItem>(10);

        //Act
        await sut.Enqueue(expected, CancellationToken.None);
        var actual = sut.TryPeek(out QueueItem? result);

        //Assert
        actual.Should().BeTrue();
        result.Should().NotBeNull();
        result!.Should().Be(expected);

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task Enqueue_GivenQueueIsFull_ShouldBlockInsertion()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<IDisposable>(1);
        await sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);

        //Act
        var actual = sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);

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
        var sut = new AsyncQueueWithCursor<IDisposable>(1);
        await sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100);
        var pendingEnqueue = sut.Enqueue(Substitute.For<IDisposable>(), cts.Token);

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
        var sut = new AsyncQueueWithCursor<IDisposable>(1);
        await sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);
        var cts = new CancellationTokenSource();

        //Act
        var pendingEnqueue = sut.Enqueue(Substitute.For<IDisposable>(), cts.Token).AsTask();
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
        var expected1 = new QueueItem(1);
        var expected2 = new QueueItem(2);
        var sut = new AsyncQueueWithCursor<QueueItem>(2);
        await sut.Enqueue(expected1, CancellationToken.None);
        await sut.Enqueue(expected2, CancellationToken.None);

        //Act
        QueueItem actual1 = await sut.NextItem(CancellationToken.None);
        QueueItem actual2 = await sut.NextItem(CancellationToken.None);

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
        var sut = new AsyncQueueWithCursor<IDisposable>(1);
        await sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);
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
        var expected = new QueueItem(1);
        var sut = new AsyncQueueWithCursor<QueueItem>(1);

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
        var expected1 = new QueueItem(1);
        var expected2 = new QueueItem(2);
        var sut = new AsyncQueueWithCursor<QueueItem>(2);
        await sut.Enqueue(expected1, CancellationToken.None);
        await sut.Enqueue(expected2, CancellationToken.None);
        await sut.NextItem(CancellationToken.None);

        //Act
        QueueItem before = await sut.NextItem(CancellationToken.None);
        sut.ResetCursor();
        QueueItem after = await sut.NextItem(CancellationToken.None);

        //Assert
        before.Should().Be(expected2);
        after.Should().Be(expected1);

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task RemoveCurrentItem_GivenQueueEmpty_ShouldThrow()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<IDisposable>(2);

        //Act
        var exception = Record.Exception(() => sut.RemoveCurrentItem());

        //Assert
        exception.Should().BeOfType<AsyncQueueWithCursorNoItemException>();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task RemoveCurrentItem_GivenQueueWithItemButCursorNotMoved_ShouldThrow()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<IDisposable>(2);
        await sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);

        //Act
        var exception = Record.Exception(() => sut.RemoveCurrentItem());

        //Assert
        exception.Should().BeOfType<AsyncQueueWithCursorNoItemException>();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task RemoveCurrentItem_GivenQueueWithCursorOnItem_ShouldRemoveItem()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<IDisposable>(2);
        await sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);
        await sut.NextItem(CancellationToken.None);

        //Act
        sut.RemoveCurrentItem();

        //Assert
        sut.TryPeek(out _).Should().BeFalse();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task RemoveCurrentItem_GivenQueueWithMultipleItems_ShouldMoveCursor()
    {
        //Arrange
        var expected = new QueueItem(3);
        var sut = new AsyncQueueWithCursor<QueueItem>(3);
        await sut.Enqueue(new QueueItem(1), CancellationToken.None);
        await sut.Enqueue(new QueueItem(2), CancellationToken.None);
        await sut.Enqueue(expected, CancellationToken.None);
        await sut.NextItem(CancellationToken.None);
        await sut.NextItem(CancellationToken.None);

        //Act
        sut.RemoveCurrentItem();
        var result = await sut.NextItem(CancellationToken.None);

        //Assert
        result.Should().Be(expected);

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task Dispose_GivenEnqueueWaiting_ShouldCancelTask()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<IDisposable>(1);
        await sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);
        var enqueueTask = sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);

        //Act
        await sut.DisposeAsync();
        var act = async () => await enqueueTask;

        //Assert
        await act.Should().ThrowAsync<TaskCanceledException>();
        enqueueTask.IsCompleted.Should().BeTrue();
        enqueueTask.IsCanceled.Should().BeTrue();
    }

    [Fact]
    public async Task Enqueue_GivenDisposed_ShouldThrow()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<IDisposable>(1);
        await sut.DisposeAsync();

        //Act
        var act = async () => await sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<AsyncLockDisposedException>();
    }

    [Fact]
    public async Task Dispose_GivenNextItemAwaiting_ShouldCancelTask()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<IDisposable>(2);
        var nextItemTask = sut.NextItem(CancellationToken.None);

        //Act
        await sut.DisposeAsync();
        var act = async () => await nextItemTask;

        //Assert
        await act.Should().ThrowAsync<TaskCanceledException>();
        nextItemTask.IsCompleted.Should().BeTrue();
        nextItemTask.IsCanceled.Should().BeTrue();
    }

    [Fact]
    public async Task Dispose_GivenQueueEmptyAwaiting_ShouldCancelTask()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<IDisposable>(1);
        await sut.Enqueue(Substitute.For<IDisposable>(), CancellationToken.None);
        var waitForEmptyTask = sut.WaitForEmpty(CancellationToken.None);

        //Act
        await sut.DisposeAsync();
        var act = async () => await waitForEmptyTask;

        //Assert
        await act.Should().ThrowAsync<TaskCanceledException>();
        waitForEmptyTask.IsCompleted.Should().BeTrue();
        waitForEmptyTask.IsCanceled.Should().BeTrue();
    }

    [Fact]
    public async Task Dispose_GivenQueuedItem_ShouldDisposeItem()
    {
        //Arrange
        var item = Substitute.For<IDisposable>();
        var sut = new AsyncQueueWithCursor<IDisposable>(1);
        await sut.Enqueue(item, CancellationToken.None);

        //Act
        await sut.DisposeAsync();

        //Assert
        item.Received().Dispose();
    }

    private class QueueItem : IDisposable, IEquatable<QueueItem>
    {
        private int Number { get; }

        public QueueItem(int number)
        {
            Number = number;
        }

        public void Dispose() {}
        public bool Equals(QueueItem? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Number == other.Number;
        }
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QueueItem)obj);
        }
        public override int GetHashCode() =>
            Number;
    }
}
