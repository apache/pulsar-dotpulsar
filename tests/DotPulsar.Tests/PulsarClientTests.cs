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

namespace DotPulsar.Tests
{
    using Abstractions;
    using DotPulsar.Internal;
    using DotPulsar.Internal.Abstractions;
    using DotPulsar.Internal.PulsarApi;
    using Extensions;
    using FluentAssertions;
    using NSubstitute;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using Schema = Schema;

    public class PulsarClientTests
    {
        [Fact]
        public async Task NewProducer_GivenPartitionedTopic_ShouldReturnPartitionProducer()
        {
            //Arrange
            const string topicName = "persistent://public/default/test-topic";
            const uint expectedPartitions = 3;

            CommandPartitionedTopicMetadata? saveGetPartitions = null;  // use saveGetPartitions to assert CommandPartitionedTopicMetadata.

            var connection = Substitute.For<IConnection>();
            connection.Send(Arg.Any<CommandPartitionedTopicMetadata>(), Arg.Any<CancellationToken>())
                .Returns(new BaseCommand
                {
                    CommandType = BaseCommand.Type.PartitionedMetadataResponse,
                    PartitionMetadataResponse = new CommandPartitionedTopicMetadataResponse
                    {
                        Response = CommandPartitionedTopicMetadataResponse.LookupType.Success,
                        Partitions = expectedPartitions
                    }
                })
                .AndDoes(info => saveGetPartitions = (CommandPartitionedTopicMetadata) info[0]);

            var connectionPool = Substitute.For<IConnectionPool>();
            connectionPool.FindConnectionForTopic(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(connection);

            var client = PulsarClientFactory.CreatePulsarClient(connectionPool, new ProcessManager(connectionPool), Substitute.For<IHandleException>(), new Uri("pusarl://localhost:6650/"));

            //Act
            await using var producer = client.NewProducer(Schema.String).Topic(topicName).Create();
            await ((IEstablishNewChannel) producer).EstablishNewChannel(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            //Assert
            saveGetPartitions.Should().NotBeNull();
            saveGetPartitions!.Topic.Should().Be(topicName);
            producer.Should().BeOfType<Producer<string>>();
        }
    }
}
