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

[Trait("Category", "Unit")]
public class AsyncQueueTests
{
    [Fact]
    public async Task Enqueue_GivenDequeueTaskWasWaiting_ShouldCompleteDequeueTask()
    {
        //Arrange
        const int expected = 1;
        var queue = new AsyncQueue<int>();
        var dequeueTask = queue.Dequeue();
        queue.Enqueue(expected);

        //Act
        var actual = await dequeueTask;

        //Assert
        actual.ShouldBe(expected);

        //Annihilate
        queue.Dispose();
    }

    [Fact]
    public async Task DequeueAsync_GivenQueueWasNotEmpty_ShouldCompleteDequeueTask()
    {
        //Arrange
        const int expected = 1;
        var queue = new AsyncQueue<int>();
        queue.Enqueue(expected);

        //Act
        var actual = await queue.Dequeue();

        //Assert
        actual.ShouldBe(expected);

        //Annihilate
        queue.Dispose();
    }

    [Fact]
    public async Task DequeueAsync_GivenMultipleDequeues_ShouldCompleteInOrderedSequence()
    {
        //Arrange
        const int expected1 = 1, expected2 = 2;
        var queue = new AsyncQueue<int>();
        var dequeue1 = queue.Dequeue();
        var dequeue2 = queue.Dequeue();
        queue.Enqueue(expected1);
        queue.Enqueue(expected2);

        //Act
        var actual1 = await dequeue1;
        var actual2 = await dequeue2;

        //Assert
        actual1.ShouldBe(expected1);
        actual2.ShouldBe(expected2);

        //Annihilate
        queue.Dispose();
    }

    [Fact]
    public async Task DequeueAsync_GivenSequenceOfInput_ShouldReturnSameSequenceOfOutput()
    {
        //Arrange
        const int expected1 = 1, expected2 = 2;
        var queue = new AsyncQueue<int>();
        queue.Enqueue(expected1);
        queue.Enqueue(expected2);

        //Act
        var actual1 = await queue.Dequeue();
        var actual2 = await queue.Dequeue();

        //Assert
        actual1.ShouldBe(expected1);
        actual2.ShouldBe(expected2);

        //Annihilate
        queue.Dispose();
    }

    [Fact]
    public async Task DequeueAsync_GivenTokenIsCanceled_ShouldCancelTask()
    {
        //Arrange
        CancellationTokenSource source1 = new(), source2 = new();
        const int excepted = 1;
        var queue = new AsyncQueue<int>();
        var task1 = queue.Dequeue(source1.Token).AsTask();
        var task2 = queue.Dequeue(source2.Token).AsTask();

        //Act
        source1.Cancel();
        queue.Enqueue(excepted);
        var exception = await Record.ExceptionAsync(() => task1);
        var actual = await task2;

        //Assert
        exception.ShouldBeOfType<TaskCanceledException>();
        actual.ShouldBe(excepted);

        //Annihilate
        source1.Dispose();
        source2.Dispose();
        queue.Dispose();
    }
}
