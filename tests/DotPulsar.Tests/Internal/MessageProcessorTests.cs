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

[Trait("Category", "Unit")]
public sealed class MessageProcessorTests
{
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

        //Act
        var exception = Record.Exception(() => new MessageProcessor<byte[]>(consumer, ProcessMessage, options));

        //Assert
        exception.Should().BeOfType<ProcessingException>();
    }

    private static ValueTask ProcessMessage(IMessage<byte[]> _1, CancellationToken _2) => ValueTask.CompletedTask;
}
