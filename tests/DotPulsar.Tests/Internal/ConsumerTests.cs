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
    using DotPulsar.Internal.Extensions;
    using DotPulsar.Internal.PulsarApi;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class ConsumerTests
    {
        [Fact]
        public void GivenAConsumerIsCreated_ShouldSetItsServiceUrlSubscriptionAndTopic()
        {
            //Arrange
            var serviceUrl = new Uri("test-service:6650");
            var subscriptionName = "test-sub";
            var topic = "test-topic";
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            //Act
            var consumer = CreateConsumerWithSubstitudes(serviceUrl: serviceUrl, subscriptionName: subscriptionName, topicName: topic);

            //Assert
            consumer.ServiceUrl.Should().Be(serviceUrl);
            consumer.SubscriptionName.Should().Be(subscriptionName);
            consumer.Topic.Should().Be(topic);
        }

        [Fact]
        public void GivenAConsumerIsCreated_ShouldRegisterConsumerCreatedWithItsCorrelationId()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var eventRegister = Substitute.For<IRegisterEvent>();

            //Act
            var consumer = CreateConsumerWithSubstitudes(correlationId: correlationId, eventRegister: eventRegister);

            //Assert
            eventRegister
                .Received(1)
                .Register(Arg.Is((IEvent x) => x.CorrelationId.Equals(correlationId)));
        }

        [Fact]
        public async void DisposeAsync_GivenNotDisposed_ShouldRegisterConsumerDisposedWithItsCorrelationId()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var eventRegister = Substitute.For<IRegisterEvent>();
            var initialChannel = Substitute.For<IConsumerChannel<string>>();
            var consumer = CreateConsumerWithSubstitudes(correlationId: correlationId, eventRegister: eventRegister, initialChannel: initialChannel);

            //Act
            await consumer.DisposeAsync();

            //Assert
            eventRegister
                .Received(1)
                .Register(Arg.Is((IEvent x) =>
                    x.CorrelationId.Equals(correlationId) &&
                    x.GetType().Name.Equals("ConsumerDisposed")));
            await initialChannel
                .Received(1)
                .ClosedByClient(Arg.Is(CancellationToken.None));
            await initialChannel
                .Received(1)
                .DisposeAsync();
        }

        [Fact]
        public async void RedeliverUnacknowledgedMessages_GivenConsumerIsDisposed_ShouldThrowAnObjectDisposedException()
        {
            //Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            var consumer = CreateConsumerWithSubstitudes();
            await consumer.DisposeAsync();

            //Act
            Func<Task> act = async () => { await consumer.RedeliverUnacknowledgedMessages(new List<MessageId>(), CancellationToken.None); };

            //Assert
            await act.Should()
                .ThrowAsync<ObjectDisposedException>()
                .WithMessage("*Object name:*DotPulsar.Internal.Consumer*");
        }

        [Fact]
        public async void RedeliverUnacknowledgedMessages_GivenAMessageIdIsPassed_ShouldSendCommandRedeliverUnacknowledgedMessages()
        {
            //Arrange
            var consumer = CreateConsumerWithSubstitudes(out IConsumerChannel<string> initialChannel, out IExecute executor);
            executor
                .When(x => x.Execute(Arg.Any<Func<ValueTask>>(), Arg.Any<CancellationToken>()))
                .Do(x => x.ArgAt<Func<ValueTask>>(0)());
            var messageId = new MessageId(1, 2, 3, 4);

            //Act
            await consumer.RedeliverUnacknowledgedMessages(new List<MessageId>() { messageId }, CancellationToken.None);

            //Assert
            await executor
                .Received(1)
                .Execute(Arg.Any<Func<ValueTask>>(), Arg.Any<CancellationToken>());
            await initialChannel
                .Received(1)
                .Send(Arg.Is<CommandRedeliverUnacknowledgedMessages>(x =>
                    x.MessageIds.Count() == 1 &&
                    x.MessageIds.First().ToMessageId().Equals(messageId)),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async void RedeliverUnacknowledgedMessages_GivenNoMessages_ShouldSendEmptyCommandRedeliverUnacknowledgedMessages()
        {
            //Arrange
            var consumer = CreateConsumerWithSubstitudes(out IConsumerChannel<string> initialChannel, out IExecute executor);
            executor
                .When(x => x.Execute(Arg.Any<Func<ValueTask>>(), Arg.Any<CancellationToken>()))
                .Do(x => x.ArgAt<Func<ValueTask>>(0)());

            //Act
            await consumer.RedeliverUnacknowledgedMessages(CancellationToken.None);

            //Assert
            await executor
                .Received(1)
                .Execute(Arg.Any<Func<ValueTask>>(), Arg.Any<CancellationToken>());
            await initialChannel
                .Received(1)
                .Send(Arg.Is<CommandRedeliverUnacknowledgedMessages>(x =>
                    x.MessageIds.Count() == 0),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async void RedeliverUnacknowledgedMessages_GivenMessageIdDataIsPassed_ShouldSendMessageIdDataInCommandRedeliverUnacknowledgedMessages()
        {
            //Arrange
            var consumer = CreateConsumerWithSubstitudes(out IConsumerChannel<string> initialChannel, out IExecute executor);
            executor
                .When(x => x.Execute(Arg.Any<Func<ValueTask>>(), Arg.Any<CancellationToken>()))
                .Do(x => x.ArgAt<Func<ValueTask>>(0)());
            var messageIdData = new MessageIdData();

            //Act
            await consumer.RedeliverUnacknowledgedMessages(new MessageIdData[] { messageIdData }, CancellationToken.None);

            //Assert
            await executor
                .Received(1)
                .Execute(Arg.Any<Func<ValueTask>>(), Arg.Any<CancellationToken>());
            await initialChannel
                .Received(1)
                .Send(Arg.Is<CommandRedeliverUnacknowledgedMessages>(x =>
                    x.MessageIds.Count() == 1 && x.MessageIds.First().Equals(messageIdData)),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public void NegativeAcknowledge_GivenMessageIdDataIsPassed_ShouldNegativeAcknowledgeOnTheChannel()
        {
            //Arrange
            var consumer = CreateConsumerWithSubstitudes(out IConsumerChannel<string> initialChannel);
            var messageId = new MessageId(1, 2, 3, 4);

            //Act
            consumer.NegativeAcknowledge(messageId);

            //Assert
            initialChannel
                .Received(1)
                .NegativeAcknowledge(
                    Arg.Is<MessageIdData>(data => data.ToMessageId().Equals(messageId)));
        }

        private static Consumer<string> CreateConsumerWithSubstitudes(out IConsumerChannel<string> initialChannel)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            initialChannel = Substitute.For<IConsumerChannel<string>>();
            return CreateConsumerWithSubstitudes(initialChannel: initialChannel);
        }

        private static Consumer<string> CreateConsumerWithSubstitudes(out IConsumerChannel<string> initialChannel, out IExecute executor)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            initialChannel = Substitute.For<IConsumerChannel<string>>();
            executor = Substitute.For<IExecute>();
            return CreateConsumerWithSubstitudes(initialChannel: initialChannel, executor: executor);
        }


        private static Consumer<string> CreateConsumerWithSubstitudes(
            Uri? serviceUrl = null,
            string subscriptionName = "foo",
            string topicName = "bar",
            Guid? correlationId = null,
            IRegisterEvent? eventRegister = null,
            IConsumerChannel<string>? initialChannel = null,
            IExecute? executor = null,
            IStateChanged<ConsumerState>? state = null,
            IConsumerChannelFactory<string>? factory = null
        ) =>
            new(
                correlationId ?? Guid.NewGuid(),
                serviceUrl ?? new Uri("localhost:6650"),
                subscriptionName,
                topicName,
                eventRegister ?? Substitute.For<IRegisterEvent>(),
                initialChannel ?? Substitute.For<IConsumerChannel<string>>(),
                executor ?? Substitute.For<IExecute>(),
                state ?? Substitute.For<IStateChanged<ConsumerState>>(),
                factory ?? Substitute.For<IConsumerChannelFactory<string>>());
    }
}
