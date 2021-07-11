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

    public class NegativeackedMessageStateTests
    {
        [Fact]
        public async void GetMessagesForRedelivery_GivenAMessageIdIsNotAcked_ShouldReturnTheMessageId()
        {
            //Arrange
            var messageId = new MessageIdData();
            var state = new NegativeackedMessageState(
                TimeSpan.FromMilliseconds(10));

            //Act
            state.Add(messageId);
            await Task.Delay(11);
            var result = state.GetMessagesForRedelivery();

            //Assert
            result.Should().Contain(messageId);
        }

        [Fact]
        public void GetMessagesForRedelivery_GivenAMessageIdRedeliveryDelayHasNotPassed_ShouldNotReturnTheMessageId()
        {
            //Arrange
            var state = new NegativeackedMessageState(
                TimeSpan.FromMilliseconds(10));

            //Act
            state.Add(new MessageIdData());
            var result = state.GetMessagesForRedelivery();

            //Assert
            result.Should().BeEmpty();
        }
    }
}
