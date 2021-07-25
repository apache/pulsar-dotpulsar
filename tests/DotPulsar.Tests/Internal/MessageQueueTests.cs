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

namespace DotPulsar.Tests.Internal
{
    using AutoFixture;
    using AutoFixture.AutoNSubstitute;
    using DotPulsar.Internal;
    using DotPulsar.Internal.Abstractions;
    using DotPulsar.Internal.PulsarApi;
    using FluentAssertions;
    using NSubstitute;
    using System.Buffers;
    using Xunit;

    public class MessageQueueTests
    {
        [Fact]
        public async void Dequeue_GivenPackageDataIsDequeued_ShouldAddMessageIdToTrackerAndReturnPackage()
        {
            //Arrange
            var messageIdData = new MessageIdData();
            var package = new MessagePackage(messageIdData, 1, new ReadOnlySequence<byte>());
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var queue = Substitute.For<IAsyncQueue<MessagePackage>>();
            var tracker = Substitute.For<IMessageTracker>();
            var messageQueue = new MessageQueue(queue, tracker);
            queue.Dequeue().ReturnsForAnyArgs(package);

            //Act
            var message = await messageQueue.Dequeue();

            //Assert
            message.Should().Be(package);
            tracker.Received(1).Track(Arg.Is(messageIdData));
        }

        [Fact]
        public void Acknowledge_GivenMessageIdIsPassed_ShouldAcknowledgeTracker()
        {
            //Arrange
            var messageIdData = new MessageIdData();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var queue = Substitute.For<IAsyncQueue<MessagePackage>>();
            var tracker = Substitute.For<IMessageTracker>();
            var messageQueue = new MessageQueue(queue, tracker);

            //Act
            messageQueue.Acknowledge(messageIdData);

            //Assert
            tracker.Received(1).Acknowledge(Arg.Is(messageIdData));
        }

        [Fact]
        public void NegativeAcknowledge_GivenMessageIdIsPassed_ShouldNegativeAcknowledgeTracker()
        {
            //Arrange
            var messageIdData = new MessageIdData();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var queue = Substitute.For<IAsyncQueue<MessagePackage>>();
            var tracker = Substitute.For<IMessageTracker>();
            var messageQueue = new MessageQueue(queue, tracker);

            //Act
            messageQueue.NegativeAcknowledge(messageIdData);

            //Assert
            tracker.Received(1).NegativeAcknowledge(Arg.Is(messageIdData));
        }

        [Fact]
        public void GivenDisposeIsCalled_ShouldDisposeTheQueueAndTracker()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var queue = Substitute.For<IAsyncQueue<MessagePackage>>();
            var tracker = Substitute.For<IMessageTracker>();
            var messageQueue = new MessageQueue(queue, tracker);

            //Act
            messageQueue.Dispose();

            //Assert
            tracker.Received(1).Dispose();
            queue.Received(1).Dispose();
        }

    }
}
