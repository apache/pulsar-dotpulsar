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
    using DotPulsar.Internal;
    using DotPulsar.Internal.PulsarApi;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class UnackedMessageStateTests
    {
        [Fact]
        public async void CheckUnackedMessages_GivenAMessageIdIsNotAcked_ShouldReturnTheMessageId()
        {
            //Arrange
            var messageId = new MessageIdData();
            var tracker = new UnackedMessageState(
                TimeSpan.FromMilliseconds(10));

            //Act
            tracker.Add(messageId);
            await Task.Delay(11);
            var result = tracker.CheckUnackedMessages();

            //Assert
            result.Should().Contain(messageId);
        }

        [Fact]
        public void CheckUnackedMessages_GivenAMessageIdAckTimeoutHasNotPassed_ShouldNotReturnTheMessageId()
        {
            //Arrange
            var tracker = new UnackedMessageState(
                TimeSpan.FromMilliseconds(10));

            //Act
            tracker.Add(new MessageIdData());
            var result = tracker.CheckUnackedMessages();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async void CheckUnackedMessages_GivenAMessageIdIsAckedWithinTheTimeout_ShouldNotReturnTheMessageId()
        {
            //Arrange
            var messageId = new MessageIdData();
            var tracker = new UnackedMessageState(
                TimeSpan.FromMilliseconds(10));

            //Act
            tracker.Add(messageId);
            await Task.Delay(5);
            tracker.Acknowledge(messageId);
            await Task.Delay(10);
            var result = tracker.CheckUnackedMessages();

            //Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async void CheckUnackedMessages_GivenAMessageIdIsRemovedBeforeTheTimeout_ShouldNotReturnTheMessageId()
        {
            //Arrange
            var messageId = new MessageIdData();
            var tracker = new UnackedMessageState(
                TimeSpan.FromMilliseconds(10));

            //Act
            tracker.Add(messageId);
            await Task.Delay(5);
            tracker.Remove(messageId);
            await Task.Delay(10);
            var result = tracker.CheckUnackedMessages();

            //Assert
            result.Should().BeEmpty();
        }



        [Fact]
        public async void CheckUnackedMessages_GivenAMessageIdIsRemovedBeforeTheTimeout_ShouldNotReturnThatMessageId()
        {
            //Arrange
            var messageId1 = new MessageIdData() { EntryId = 1 };
            var messageId2 = new MessageIdData() { EntryId = 2 };
            var messageId3 = new MessageIdData() { EntryId = 3 };
            var tracker = new UnackedMessageState(
                TimeSpan.FromMilliseconds(10));

            //Act
            tracker.Add(messageId1);
            tracker.Add(messageId2);
            tracker.Add(messageId3);
            await Task.Delay(5);
            tracker.Remove(messageId2);
            await Task.Delay(11);
            var result = tracker.CheckUnackedMessages();

            //Assert
            result.Should().HaveCount(2);
            result.Should().Contain(messageId1);
            result.Should().Contain(messageId3);
        }

        [Fact]
        public async void CheckUnackedMessages_GivenAMessageIdWasReturned_ShouldNotReturnOnSubsequentCalls()
        {
            //Arrange
            var messageId = new MessageIdData();
            var tracker = new UnackedMessageState(
                TimeSpan.FromMilliseconds(10));

            //Act
            tracker.Add(messageId);
            await Task.Delay(11);
            var result1 = tracker.CheckUnackedMessages();
            var result2 = tracker.CheckUnackedMessages();

            //Assert
            result1.Should().Contain(messageId);
            result2.Should().BeEmpty();
        }
    }
}