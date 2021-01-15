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
    using DotPulsar.Internal.PulsarApi;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class UnacknowledgedMessageTrackerTests
    {
        [Fact]
        public async void Start_GivenAMessageIdIsNotAcked_ShouldRedeliver()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var consumer = Substitute.For<IConsumer>();
            var messageId = new MessageIdData();
            var cts = new CancellationTokenSource();
            var tracker = new UnackedMessageTracker(
                TimeSpan.FromMilliseconds(10),
                TimeSpan.FromMilliseconds(1));

            //Act
            tracker.Add(messageId);
            cts.CancelAfter(20);
            try { await tracker.Start(consumer, cts.Token); }
            catch (TaskCanceledException) { }

            //Assert
            await consumer
                .Received(1)
                .RedeliverUnacknowledgedMessages(
                    Arg.Is(EquivalentTo(new List<MessageIdData>() { messageId })),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async void Start_GivenAMessageIdIsAckedWithinTimeout_ShouldNotRedeliver()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var consumer = Substitute.For<IConsumer>();
            var messageId = new MessageIdData();
            var cts = new CancellationTokenSource();
            var tracker = new UnackedMessageTracker(
                TimeSpan.FromMilliseconds(10),
                TimeSpan.FromMilliseconds(1));

            //Act
            tracker.Add(messageId);
            cts.CancelAfter(20);
            var _ = Task.Delay(5).ContinueWith(_ => tracker.Acknowledge(messageId));
            try { await tracker.Start(consumer, cts.Token); }
            catch (TaskCanceledException) { }

            //Assert
            await consumer
                .DidNotReceive()
                .RedeliverUnacknowledgedMessages(
                    Arg.Any<IEnumerable<MessageIdData>>(),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async void Start_GivenAMessageIdIsNotAckedWithinTimeout_ShouldRedeliver()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var consumer = Substitute.For<IConsumer>();
            var messageId = new MessageIdData();
            var cts = new CancellationTokenSource();
            var tracker = new UnackedMessageTracker(
                TimeSpan.FromMilliseconds(10),
                TimeSpan.FromMilliseconds(1));

            //Act
            tracker.Add(messageId);
            cts.CancelAfter(20);

            var _ = Task.Delay(15).ContinueWith(_ => tracker.Acknowledge(messageId));
            try { await tracker.Start(consumer, cts.Token); }
            catch (TaskCanceledException) { }

            //Assert
            await consumer
                .Received(1)
                .RedeliverUnacknowledgedMessages(
                    Arg.Any<IEnumerable<MessageIdData>>(),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async void Start_GivenAMessageIdIsNotAckedWithinTimeout_ShouldRedeliverOnlyOnce()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var consumer = Substitute.For<IConsumer>();
            var messageId = new MessageIdData();
            var cts = new CancellationTokenSource();
            var tracker = new UnackedMessageTracker(
                TimeSpan.FromMilliseconds(10),
                TimeSpan.FromMilliseconds(5));

            //Act
            tracker.Add(messageId);
            cts.CancelAfter(50);
            try { await tracker.Start(consumer, cts.Token); }
            catch (TaskCanceledException) { }

            //Assert
            await consumer
                .Received(1)
                .RedeliverUnacknowledgedMessages(
                    Arg.Any<IEnumerable<MessageIdData>>(),
                    Arg.Any<CancellationToken>());
        }

        private Expression<Predicate<IEnumerable<T>>> EquivalentTo<T>(IEnumerable<T> enumerable) =>
            x => IsEquivalentIEnumerable(enumerable, x);

        private bool IsEquivalentIEnumerable<T>(IEnumerable<T> a, IEnumerable<T> b) =>
            a.Count() == b.Count() && a.Zip(b, (a_, b_) => a_.Equals(b_)).All(_ => _);
    }
}
