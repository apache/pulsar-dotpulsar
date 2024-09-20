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

using DotPulsar.Abstractions;
using DotPulsar.Exceptions;
using DotPulsar.Internal;
using System.Threading;

[Trait("Category", "Unit")]
public sealed class MessageProcessorTests : IDisposable
{
    private bool _taskHasStarted;
    private bool _taskHasCompleted;
    private readonly SemaphoreSlim _semaphore;
    private readonly CancellationTokenSource _cts;

    public MessageProcessorTests()
    {
        _taskHasStarted = false;
        _taskHasCompleted = false;
        _semaphore = new SemaphoreSlim(1);
        _cts = new CancellationTokenSource();
    }

    [Theory]
    [InlineAutoData(SubscriptionType.Shared)]
    [InlineAutoData(SubscriptionType.KeyShared)]
    public void Constructor_GivenSharedSubscriptionTypeWithOrderedAcknowledgment_ShouldThrowProcessingException(
        SubscriptionType subscriptionType,
        [AutoFixture.Xunit2.Frozen] IConsumer<byte[]> consumer,
        ProcessingOptions options)
    {
        //Arrange
        consumer.SubscriptionType.Returns(subscriptionType);
        ValueTask ProcessMessage(IMessage<byte[]> _1, CancellationToken _2) => ValueTask.CompletedTask;

        //Act
        var exception = Record.Exception(() => new MessageProcessor<byte[]>(consumer, ProcessMessage, options));

        //Assert
        exception.Should().BeOfType<ProcessingException>();
    }

    [Theory, AutoData]
    public async Task Process_GivenNoShutdownGracePeriod_ShouldNotLetTaskComplete(
        [AutoFixture.Xunit2.Frozen] IConsumer<byte[]> consumer,
        ProcessingOptions options)
    {
        //Arrange
        consumer.Receive(Arg.Any<CancellationToken>()).Returns(_ => NewMessage());
        var uut = new MessageProcessor<byte[]>(consumer, ProcessMessage, options);

        //Act
        await _semaphore.WaitAsync();
        var processTask = uut.Process(_cts.Token).AsTask();
        while (!_taskHasStarted) { }
        _cts.Cancel();
        await processTask;
        _semaphore.Release();

        //Assert
        _taskHasCompleted.Should().BeFalse();
    }

    [Theory, AutoData]
    public async Task Process_GivenShutdownGracePeriod_ShouldLetTaskComplete(
        [AutoFixture.Xunit2.Frozen] IConsumer<byte[]> consumer,
        ProcessingOptions options)
    {
        //Arrange
        options.ShutdownGracePeriod = TimeSpan.FromHours(1);
        consumer.Receive(Arg.Any<CancellationToken>()).Returns(_ => NewMessage());
        var uut = new MessageProcessor<byte[]>(consumer, ProcessMessage, options);

        //Act
        await _semaphore.WaitAsync();
        var processTask = uut.Process(_cts.Token).AsTask();
        while (!_taskHasStarted) { }
        _cts.Cancel();
        _semaphore.Release();
        await processTask;

        //Assert
        _taskHasCompleted.Should().BeTrue();
    }

    private async ValueTask ProcessMessage(IMessage<byte[]> _, CancellationToken token)
    {
        _taskHasStarted = true;
        await _semaphore.WaitAsync(token);
        _semaphore.Release();
        _taskHasCompleted = true;
    }

    private static IMessage<byte[]> NewMessage()
    {
        var message = Substitute.For<IMessage<byte[]>>();
        message.MessageId.Returns(new MessageId(0, 0, 0, 0));
        return message;
    }

    public void Dispose()
    {
        _semaphore.Dispose();
        _cts.Dispose();
    }
}
