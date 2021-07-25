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
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Xunit;

    public class ConsumerChannelTests
    {
        [Fact]
        public void GivenNoDecompressorFactoriesAreGiven_ShouldCreate()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            //Act
            var channel = CreateConsumerChannelWithSubstitudes();

            //Assert
            channel.Should().BeAssignableTo<IConsumerChannel<string>>();
        }

        [Fact]
        public void GivenADecompressorFactoryIsGiven_ShouldCreateTheDecompressionFactory()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var decompressionFactory = Substitute.For<IDecompressorFactory>();

            //Act
            _ = CreateConsumerChannelWithSubstitudes(
                decompressorFactories: new IDecompressorFactory[] { decompressionFactory });


            //Assert
            decompressionFactory.Received(1).Create();
        }



        [Fact]
        public async void Send_GivenCommandAckContainsNoBatchIndex_ShouldAcknowledgeTheMessage()
        {
            //Arrange
            var channel = CreateConsumerChannelWithSubstitudes(out IMessageQueue queue);
            var firstMessageIdData = new MessageIdData();
            var commandAck = new CommandAck();
            commandAck.MessageIds.Add(firstMessageIdData);

            //Act
            await channel.Send(commandAck, CancellationToken.None);

            //Assert
            queue.Received(1).Acknowledge(Arg.Is(firstMessageIdData));
        }

        [Fact]
        public void NegativeAcknowledge_ShouldCallNegativeAcknowledgeOnItsQueue()
        {
            //Arrange
            var channel = CreateConsumerChannelWithSubstitudes(out IMessageQueue queue);
            var messageIdData = new MessageIdData();

            //Act
            channel.NegativeAcknowledge(messageIdData);

            //Assert
            queue.Received(1).NegativeAcknowledge(Arg.Is(messageIdData));
        }

        private static ConsumerChannel<string> CreateConsumerChannelWithSubstitudes(out IMessageQueue queue)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            queue = Substitute.For<IMessageQueue>();
            return CreateConsumerChannelWithSubstitudes(queue: queue);
        }

        private static ConsumerChannel<string> CreateConsumerChannelWithSubstitudes(
            ulong id = 1001,
            uint messagePrefetchCount = 1,
            IMessageQueue? queue = null,
            IConnection? connection = null,
            IBatchHandler<string>? batchHandler = null,
            IMessageFactory<string>? messageFactory = null,
            IEnumerable<IDecompressorFactory>? decompressorFactories = null) =>
            new(id, messagePrefetchCount,
                queue ?? Substitute.For<IMessageQueue>(),
                connection ?? Substitute.For<IConnection>(),
                batchHandler ?? Substitute.For<IBatchHandler<string>>(),
                messageFactory ?? Substitute.For<IMessageFactory<string>>(),
                decompressorFactories ?? Array.Empty<IDecompressorFactory>());
    }
}
