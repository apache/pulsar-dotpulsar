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
    using DotPulsar.Exceptions;
    using DotPulsar.Internal;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using Xunit;

    public class ConsumerBuilderTests
    {
        [Fact]
        public void GivenNoSubscriptionNameIsConfigured_WhenCallingCreate_ShouldThrowAConfigurationException()
        {
            //Arrange
            new Fixture().Customize(new AutoNSubstituteCustomization());
            var pulsarClient = Substitute.For<IPulsarClient>();
            var schema = Substitute.For<ISchema<string>>();
            var builder = new ConsumerBuilder<string>(pulsarClient, schema);

            //Act
            Func<IConsumer<string>> act = () => builder.Create();

            //Assert
            act.Should()
                .Throw<ConfigurationException>()
                .WithMessage("SubscriptionName may not be null or empty");
        }

        [Fact]
        public void GivenNoTopcIsConfigured_WhenCallingCreate_ShouldThrowAConfigurationException()
        {
            //Arrange
            var subscriptionName = "foo";
            new Fixture().Customize(new AutoNSubstituteCustomization());
            var pulsarClient = Substitute.For<IPulsarClient>();
            var schema = Substitute.For<ISchema<string>>();
            var builder = new ConsumerBuilder<string>(pulsarClient, schema);

            //Act
            Func<IConsumer<string>> act = () => builder.SubscriptionName(subscriptionName).Create();

            //Assert
            act.Should()
                .Throw<ConfigurationException>()
                .WithMessage("Topic may not be null or empty");
        }

        [Fact]
        public void GivenNoBuilderFunctionsAreCalled_WhenCallingCreate_ShouldCreateAConsumerWithTheRightDefaults()
        {
            //Arrange
            var subscriptionName = "foo";
            var topic = "bar";
            new Fixture().Customize(new AutoNSubstituteCustomization());
            var pulsarClient = Substitute.For<IPulsarClient>();
            var schema = Substitute.For<ISchema<string>>();
            var builder = new ConsumerBuilder<string>(pulsarClient, schema);
            var defaultOptions = new ConsumerOptions<string>(subscriptionName, topic, schema);

            //Act
            var _ = builder
                .SubscriptionName(subscriptionName)
                .Topic(topic)
                .Create();

            //Assert
            pulsarClient
                .Received(1)
                .CreateConsumer(IsExpectedOptions(defaultOptions));
        }

        [Fact]
        public void GivenOptionsAreConfigured_WhenCallingCreate_ShouldCreateAConsumerWithTheRightValues()
        {
            //Arrange
            new Fixture().Customize(new AutoNSubstituteCustomization());
            var pulsarClient = Substitute.For<IPulsarClient>();
            var schema = Substitute.For<ISchema<string>>();
            var builder = new ConsumerBuilder<string>(pulsarClient, schema);
            var stateChangedHandler = Substitute.For<IHandleStateChanged<ConsumerStateChanged>>();
            var expectedOptions = new ConsumerOptions<string>("foo", "bar", schema)
            {
                ConsumerName = "baz",
                InitialPosition = SubscriptionInitialPosition.Earliest,
                MessagePrefetchCount = 11,
                PriorityLevel = 42,
                ReadCompacted = true,
                StateChangedHandler = stateChangedHandler,
                SubscriptionType = SubscriptionType.Failover,
                AcknowledgementTimeout = TimeSpan.FromSeconds(10),
                NegativeAcknowledgementRedeliveryDelay = TimeSpan.FromMinutes(5)
            };

            //Act
            var _ = builder
                .SubscriptionName(expectedOptions.SubscriptionName)
                .Topic(expectedOptions.Topic)
                .ConsumerName(expectedOptions.ConsumerName)
                .InitialPosition(expectedOptions.InitialPosition)
                .MessagePrefetchCount(expectedOptions.MessagePrefetchCount)
                .PriorityLevel(expectedOptions.PriorityLevel)
                .ReadCompacted(expectedOptions.ReadCompacted)
                .StateChangedHandler(expectedOptions.StateChangedHandler)
                .SubscriptionType(expectedOptions.SubscriptionType)
                .AcknowledgementTimeout(expectedOptions.AcknowledgementTimeout)
                .NegativeAcknowledgementRedeliveryDelay(expectedOptions.NegativeAcknowledgementRedeliveryDelay)
                .Create();

            //Assert
            pulsarClient
                .Received(1)
                .CreateConsumer(IsExpectedOptions(expectedOptions));
        }

        private static ConsumerOptions<string> IsExpectedOptions(ConsumerOptions<string> defaultOptions) =>
            Arg.Is((ConsumerOptions<string> opts) =>
                opts.SubscriptionName == defaultOptions.SubscriptionName
                && opts.Topic == defaultOptions.Topic
                && opts.InitialPosition == defaultOptions.InitialPosition
                && opts.MessagePrefetchCount == defaultOptions.MessagePrefetchCount
                && opts.PriorityLevel == defaultOptions.PriorityLevel
                && opts.ReadCompacted == defaultOptions.ReadCompacted
                && opts.StateChangedHandler == defaultOptions.StateChangedHandler
                && opts.SubscriptionType == defaultOptions.SubscriptionType
                && opts.AcknowledgementTimeout == defaultOptions.AcknowledgementTimeout
            );
    }
}
