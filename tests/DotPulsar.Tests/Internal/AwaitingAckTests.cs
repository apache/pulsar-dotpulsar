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
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Xunit;

    public class AwaitingAckTests
    {
        [Fact]
        public async void Elapsed_GivenTimeElapsed_ShoulEqualCorrectElapsedTicks()
        {
            //Arrange
            var messageId = new MessageIdData();
            var sw = Stopwatch.StartNew();

            //Act
            var awaiting = new AwaitingAck(messageId);
            await Task.Delay(TimeSpan.FromMilliseconds(123));
            sw.Stop();

            //Assert
            awaiting.Elapsed.Should().BeCloseTo(sw.Elapsed, 1);
        }
    }
}