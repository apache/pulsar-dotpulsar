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
public class AwaiterTests
{
    [Fact]
    public void SetResult_GivenSingleEntry_ShouldCompleteTask()
    {
        //Arrange
        var sut = new Awaiter<string, string>();
        var task = sut.CreateTask("key1");

        //Act
        sut.SetResult("key1", "value1");

        //Assert
        task.IsCompletedSuccessfully.ShouldBeTrue();
        task.Result.ShouldBe("value1");

        //Annihilate
        sut.Dispose();
    }

    [Fact]
    public void Cancel_GivenSingleEntry_ShouldCancelTask()
    {
        //Arrange
        var sut = new Awaiter<string, string>();
        var task = sut.CreateTask("key1");

        //Act
        sut.Cancel("key1");

        //Assert
        task.IsCanceled.ShouldBeTrue();

        //Annihilate
        sut.Dispose();
    }

    [Fact]
    public void SetResult_GivenNoMatchingEntry_ShouldNotThrow()
    {
        //Arrange
        var sut = new Awaiter<string, string>();

        //Act & Assert
        Should.NotThrow(() => sut.SetResult("nonexistent", "value"));

        //Annihilate
        sut.Dispose();
    }

    [Fact]
    public void AddTaskCompletionSource_GivenDuplicateKey_ShouldReplaceOldEntry()
    {
        //Arrange
        var sut = new Awaiter<string, string>();
        var oldTask = sut.CreateTask("key1");
        var newTcs = new TaskCompletionSource<string>();

        //Act
        sut.AddTaskCompletionSource("key1", newTcs);

        //Assert — old TCS should be cancelled since it was replaced
        oldTask.IsCanceled.ShouldBeTrue();

        //Annihilate
        sut.Dispose();
    }

    [Fact]
    public void AddTaskCompletionSource_GivenDuplicateKey_ShouldCompleteNewTcsOnSetResult()
    {
        //Arrange
        var sut = new Awaiter<string, string>();
        _ = sut.CreateTask("key1");
        var newTcs = new TaskCompletionSource<string>();
        sut.AddTaskCompletionSource("key1", newTcs);

        //Act
        sut.SetResult("key1", "receipt");

        //Assert — the NEW TCS should receive the result
        newTcs.Task.IsCompletedSuccessfully.ShouldBeTrue();
        newTcs.Task.Result.ShouldBe("receipt");

        //Annihilate
        sut.Dispose();
    }

    [Fact]
    public void CreateTask_GivenDuplicateKey_ShouldReplaceOldEntry()
    {
        //Arrange
        var sut = new Awaiter<string, string>();
        var oldTask = sut.CreateTask("key1");

        //Act
        var newTask = sut.CreateTask("key1");

        //Assert — old task should be cancelled, new task should be pending
        oldTask.IsCanceled.ShouldBeTrue();
        newTask.IsCompleted.ShouldBeFalse();

        //Annihilate
        sut.Dispose();
    }

    [Fact]
    public void CreateTask_GivenDuplicateKey_ShouldCompleteNewTaskOnSetResult()
    {
        //Arrange
        var sut = new Awaiter<string, string>();
        _ = sut.CreateTask("key1");
        var newTask = sut.CreateTask("key1");

        //Act
        sut.SetResult("key1", "receipt");

        //Assert
        newTask.IsCompletedSuccessfully.ShouldBeTrue();
        newTask.Result.ShouldBe("receipt");

        //Annihilate
        sut.Dispose();
    }

    [Fact]
    public void Dispose_GivenPendingEntries_ShouldCancelAll()
    {
        //Arrange
        var sut = new Awaiter<string, string>();
        var task1 = sut.CreateTask("key1");
        var task2 = sut.CreateTask("key2");

        //Act
        sut.Dispose();

        //Assert
        task1.IsCanceled.ShouldBeTrue();
        task2.IsCanceled.ShouldBeTrue();
    }
}
