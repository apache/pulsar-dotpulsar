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
    using DotPulsar.Abstractions;
    using DotPulsar.Internal;
    using DotPulsar.Internal.Abstractions;
    using DotPulsar.Internal.PulsarApi;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class MessageTrackerTests
    {
        [Fact]
        public async void Start_GivenCancellationIsRequested_ShouldStop()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var consumer = Substitute.For<IConsumer>();
            var ackedState = Substitute.For<IUnackedMessageState>();
            var nackedState = Substitute.For<INegativeackedMessageState>();
            var tracker = new MessageTracker(TimeSpan.FromMilliseconds(0), ackedState, nackedState);

            //Act
            try { await tracker.Start(consumer, cts.Token); }
            catch (TaskCanceledException) { }

            //Assert
            ackedState
                .DidNotReceive()
                .CheckUnackedMessages();
            await consumer
                .DidNotReceive()
                .RedeliverUnacknowledgedMessages(
                    Arg.Any<IEnumerable<MessageIdData>>(),
                    Arg.Any<CancellationToken>());
        }


        [Fact]
        public async void Start_GivenMessageIdsAreUnacked_ShouldRedeliver()
        {
            //Arrange
            var messageId = new MessageIdData();
            var cts = new CancellationTokenSource();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var consumer = Substitute.For<IConsumer>();
            var ackedState = Substitute.For<IUnackedMessageState>();
            ackedState.CheckUnackedMessages()
                .Returns(new MessageIdData[] { messageId })
                .AndDoes((_) => cts.Cancel());
            var nackedState = Substitute.For<INegativeackedMessageState>();
            var tracker = new MessageTracker(TimeSpan.FromMilliseconds(0), ackedState, nackedState);

            //Act
            try { await tracker.Start(consumer, cts.Token); }
            catch (TaskCanceledException) { }

            //Assert
            ackedState
                .Received(1)
                .CheckUnackedMessages();
            await consumer
                .Received(1)
                .RedeliverUnacknowledgedMessages(
                    Arg.Is<IEnumerable<MessageIdData>>((messageIds) => messageIds.Contains(messageId)),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async void Start_GivenNackedMessagesToRedeliver_ShouldRedeliver()
        {
            //Arrange
            var messageId = new MessageIdData();
            var cts = new CancellationTokenSource();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var consumer = Substitute.For<IConsumer>();
            var ackedState = Substitute.For<IUnackedMessageState>();
            var nackedState = Substitute.For<INegativeackedMessageState>();
            nackedState.GetMessagesForRedelivery()
                .Returns(new MessageIdData[] { messageId })
                .AndDoes((_) => cts.Cancel());
            var tracker = new MessageTracker(TimeSpan.FromMilliseconds(0), ackedState, nackedState);

            //Act
            try { await tracker.Start(consumer, cts.Token); }
            catch (TaskCanceledException) { }

            //Assert
            nackedState
                .Received(1)
                .GetMessagesForRedelivery();
            await consumer
                .Received(1)
                .RedeliverUnacknowledgedMessages(
                    Arg.Is<IEnumerable<MessageIdData>>((messageIds) => messageIds.Contains(messageId)),
                    Arg.Any<CancellationToken>());
        }



        [Fact]
        public async void Start_GivenNackedAndAckedMessagesToRedeliver_ShouldRedeliverBoth()
        {
            //Arrange
            var ackedMessageIdToRedeliver = new MessageIdData() { EntryId = 1 };
            var nackedMessageIdToRedeliver = new MessageIdData() { EntryId = 2 };
            var cts = new CancellationTokenSource();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var consumer = Substitute.For<IConsumer>();
            var ackedState = Substitute.For<IUnackedMessageState>();
            ackedState.CheckUnackedMessages()
                .Returns(new MessageIdData[] { ackedMessageIdToRedeliver })
                .AndDoes((_) => cts.Cancel());
            var nackedState = Substitute.For<INegativeackedMessageState>();
            nackedState.GetMessagesForRedelivery()
                .Returns(new MessageIdData[] { nackedMessageIdToRedeliver });
            var tracker = new MessageTracker(TimeSpan.FromMilliseconds(0), ackedState, nackedState);

            //Act
            try { await tracker.Start(consumer, cts.Token); }
            catch (TaskCanceledException) { }

            //Assert
            nackedState
                .Received(1)
                .GetMessagesForRedelivery();
            await consumer
                .Received(1)
                .RedeliverUnacknowledgedMessages(
                    Arg.Is<IEnumerable<MessageIdData>>((messageIds) =>
                        messageIds.Contains(ackedMessageIdToRedeliver)
                        && messageIds.Contains(nackedMessageIdToRedeliver)),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async void Start_GivenNoMessageIdsAreUnacked_ShouldNotRedeliver()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var consumer = Substitute.For<IConsumer>();
            var ackedState = Substitute.For<IUnackedMessageState>();
            var nackedState = Substitute.For<INegativeackedMessageState>();
            ackedState.CheckUnackedMessages()
                .Returns(Enumerable.Empty<MessageIdData>())
                .AndDoes((_) => cts.Cancel());
            var tracker = new MessageTracker(TimeSpan.FromMilliseconds(0), ackedState, nackedState);

            //Act
            try { await tracker.Start(consumer, cts.Token); }
            catch (TaskCanceledException) { }

            //Assert
            ackedState
                .Received(1)
                .CheckUnackedMessages();
            await consumer
                .DidNotReceive()
                .RedeliverUnacknowledgedMessages(
                    Arg.Any<IEnumerable<MessageIdData>>(),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Track_ShouldAddTheMessageToTheAckAndNackedState()
        {
            //Arrange
            var messageId = new MessageIdData();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var ackedState = Substitute.For<IUnackedMessageState>();
            var nackedState = Substitute.For<INegativeackedMessageState>();
            var tracker = new MessageTracker(TimeSpan.FromMilliseconds(0), ackedState, nackedState);

            //Act
            tracker.Track(messageId);

            //Assert
            ackedState
                .Received(1)
                .Add(Arg.Is(messageId));
        }

        [Fact]
        public void Acknowledge_ShouldAcknowledgeTheMessageToTheAckTracker()
        {
            //Arrange
            var messageId = new MessageIdData();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var ackedState = Substitute.For<IUnackedMessageState>();
            var nackedState = Substitute.For<INegativeackedMessageState>();
            var tracker = new MessageTracker(TimeSpan.FromMilliseconds(0), ackedState, nackedState);

            //Act
            tracker.Acknowledge(messageId);

            //Assert
            ackedState
                .Received(1)
                .Acknowledge(Arg.Is(messageId));
        }

        [Fact]
        public void NegativeAcknowledge_ShouldRemoveTheMessageFromTheAckTrackerAndAddItToTheNegativeAckTracker()
        {
            //Arrange
            var messageId = new MessageIdData();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var ackedState = Substitute.For<IUnackedMessageState>();
            var nackedState = Substitute.For<INegativeackedMessageState>();
            var tracker = new MessageTracker(TimeSpan.FromMilliseconds(0), ackedState, nackedState);

            //Act
            tracker.NegativeAcknowledge(messageId);

            //Assert
            nackedState
                .Received(1)
                .Add(Arg.Is(messageId));
            ackedState
                .Received(1)
                .Remove(Arg.Is(messageId));
        }
    }
}
