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
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class ConsumerChannelFactoryTests
    {
        [Fact]
        public async void Create_GivenAConnectionIsFoundForTheTopic_ShouldSubscribeAndReturnTheConsumerChannel()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var eventRegister = Substitute.For<IRegisterEvent>();
            var connectionPool = Substitute.For<IConnectionPool>();
            var subscribe = new CommandSubscribe() { Topic = "test-topic" };
            var batchHandler = Substitute.For<IBatchHandler<Schemas.StringSchema>>();
            var messageFactory = Substitute.For<IMessageFactory<Schemas.StringSchema>>();
            var tracker = Substitute.For<IMessageTracker>();
            var consumerChannelFactory = new ConsumerChannelFactory<Schemas.StringSchema>(
                new Guid(),
                eventRegister,
                connectionPool,
                subscribe,
                1,
                batchHandler,
                messageFactory,
                Array.Empty<IDecompressorFactory>(),
                tracker);
            var connection = Substitute.For<IConnection>();
            connection
                .Send(subscribe, Arg.Any<IChannel>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new SubscribeResponse(1001)));
            connectionPool
                .FindConnectionForTopic(subscribe.Topic, Arg.Any<CancellationToken>())
                .Returns(connection);

            //Act
            var channel = await consumerChannelFactory.Create(CancellationToken.None);

            //Assert
            channel.Should().BeAssignableTo<IConsumerChannel<Schemas.StringSchema>>();
        }
    }
}
