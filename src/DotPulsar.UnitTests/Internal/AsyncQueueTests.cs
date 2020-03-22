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

using DotPulsar.Internal;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DotPulsar.UnitTests.Internal
{
    public class AsyncQueueTests
    {
        [Fact]
        public async Task Enqueue_GivenDequeueTaskWasWaiting_ShouldCompleteDequeueTask()
        {
            //Arrange
            const int value = 1;
            var queue = new AsyncQueue<int>();
            var dequeueTask = queue.Dequeue();
            queue.Enqueue(value);

            //Act
            var actual = await dequeueTask;

            //Assert
            Assert.Equal(value, actual);

            //Annihilate
            queue.Dispose();
        }

        [Fact]
        public async Task DequeueAsync_GivenQueueWasNotEmpty_ShouldCompleteDequeueTask()
        {
            //Arrange
            const int value = 1;
            var queue = new AsyncQueue<int>();
            queue.Enqueue(value);

            //Act
            var actual = await queue.Dequeue();

            //Assert
            Assert.Equal(value, actual);

            //Annihilate
            queue.Dispose();
        }

        [Fact]
        public async Task DequeueAsync_GivenMultipleDequeues_ShouldCompleteInOrderedSequence()
        {
            //Arrange
            const int value1 = 1, value2 = 2;
            var queue = new AsyncQueue<int>();
            var dequeue1 = queue.Dequeue();
            var dequeue2 = queue.Dequeue();
            queue.Enqueue(value1);
            queue.Enqueue(value2);

            //Act
            var actual1 = await dequeue1;
            var actual2 = await dequeue2;

            //Assert
            Assert.Equal(value1, actual1);
            Assert.Equal(value2, actual2);

            //Annihilate
            queue.Dispose();
        }

        [Fact]
        public async Task DequeueAsync_GivenSequenceOfInput_ShouldReturnSameSequenceOfOutput()
        {
            //Arrange
            const int value1 = 1, value2 = 2;
            var queue = new AsyncQueue<int>();
            queue.Enqueue(value1);
            queue.Enqueue(value2);

            //Act
            var actual1 = await queue.Dequeue();
            var actual2 = await queue.Dequeue();

            //Assert
            Assert.Equal(value1, actual1);
            Assert.Equal(value2, actual2);

            //Annihilate
            queue.Dispose();
        }

        [Fact]
        public async Task DequeueAsync_GivenTokenIsCanceled_ShouldCancelTask()
        {
            //Arrange
            CancellationTokenSource source1 = new CancellationTokenSource(), source2 = new CancellationTokenSource();
            const int excepted = 1;
            var queue = new AsyncQueue<int>();
            var task1 = queue.Dequeue(source1.Token);
            var task2 = queue.Dequeue(source2.Token);

            //Act
            source1.Cancel();
            queue.Enqueue(excepted);
            var exception = await Record.ExceptionAsync(() => task1.AsTask()); // xUnit can't record ValueTask yet
            await task2;

            //Assert
            Assert.IsType<TaskCanceledException>(exception);
            Assert.Equal(excepted, task2.Result);

            //Annihilate
            source1.Dispose();
            source2.Dispose();
            queue.Dispose();
        }
    }
}
