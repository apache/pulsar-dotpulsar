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
        actual.ShouldBeFalse();

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
        var actual = sut.TryPeek(out var result);

        //Assert
        actual.ShouldBeTrue();
        result.ShouldNotBeNull();
        result!.ShouldBe(expected);

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
        actual.ShouldBeFalse();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task Dequeue_GivenQueueIsEmpty_ShouldThrowException()
    {
        //Arrange
        var sut = new AsyncQueueWithCursor<IDisposable>(10);

        //Act
        var act = () => sut.Dequeue();

        //Assert
        act.ShouldThrow<InvalidOperationException>();

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
        var actual = sut.TryPeek(out var result);

        //Assert
        actual.ShouldBeTrue();
        result.ShouldNotBeNull();
        result!.ShouldBe(expected);

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
        actual.IsCompleted.ShouldBeFalse();
        actual.IsFaulted.ShouldBeFalse();
        actual.IsCanceled.ShouldBeFalse();

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
        pendingEnqueue.IsCompleted.ShouldBeTrue();
        pendingEnqueue.IsFaulted.ShouldBeFalse();
        pendingEnqueue.IsCanceled.ShouldBeFalse();

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
        var exception = await Record.ExceptionAsync(() => pendingEnqueue);

        //Assert
        exception.ShouldBeOfType<TaskCanceledException>();
        pendingEnqueue.IsCompleted.ShouldBeTrue();
        pendingEnqueue.IsFaulted.ShouldBeFalse();
        pendingEnqueue.IsCanceled.ShouldBeTrue();

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
        var actual1 = await sut.NextItem(CancellationToken.None);
        var actual2 = await sut.NextItem(CancellationToken.None);

        //Assert
        actual1.ShouldBe(expected1);
        actual2.ShouldBe(expected2);

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
        actual.IsCompleted.ShouldBeFalse();
        actual.IsFaulted.ShouldBeFalse();
        actual.IsCanceled.ShouldBeFalse();

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
        actual.ShouldBe(expected);
        task.IsCompleted.ShouldBeTrue();
        task.IsFaulted.ShouldBeFalse();
        task.IsCanceled.ShouldBeFalse();

        //Annihilate
        await sut.DisposeAsync();
    }

    [Fact]
    public async Task NextItem_WhenCanceled_ShouldThrowException()
    {
        //Arrange
        var uut = new AsyncQueueWithCursor<QueueItem>(1);
        var cts = new CancellationTokenSource();

        //Act
        var task = uut.NextItem(cts.Token);
        cts.Cancel();
        var exception = await Record.ExceptionAsync(() => task.AsTask());

        //Assert
        exception.ShouldBeOfType<TaskCanceledException>();
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
        var before = await sut.NextItem(CancellationToken.None);
        sut.ResetCursor();
        var after = await sut.NextItem(CancellationToken.None);

        //Assert
        before.ShouldBe(expected2);
        after.ShouldBe(expected1);

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
        exception.ShouldBeOfType<AsyncQueueWithCursorNoItemException>();

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
        exception.ShouldBeOfType<AsyncQueueWithCursorNoItemException>();

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
        sut.TryPeek(out _).ShouldBeFalse();

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
        result.ShouldBe(expected);

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
        await act.ShouldThrowAsync<TaskCanceledException>();
        enqueueTask.IsCompleted.ShouldBeTrue();
        enqueueTask.IsCanceled.ShouldBeTrue();
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
        await act.ShouldThrowAsync<AsyncLockDisposedException>();
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
        await act.ShouldThrowAsync<TaskCanceledException>();
        nextItemTask.IsCompleted.ShouldBeTrue();
        nextItemTask.IsCanceled.ShouldBeTrue();
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
        await act.ShouldThrowAsync<TaskCanceledException>();
        waitForEmptyTask.IsCompleted.ShouldBeTrue();
        waitForEmptyTask.IsCanceled.ShouldBeTrue();
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

        public QueueItem(int number) => Number = number;

        public void Dispose() { }

        public bool Equals(QueueItem? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Number == other.Number;
        }

        public override bool Equals(object? obj) => obj is QueueItem qi && Equals(qi);

        public override int GetHashCode() => Number;
    }
}
