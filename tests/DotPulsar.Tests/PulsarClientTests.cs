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
    using NSubstitute;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;
    using Schema = DotPulsar.Schema;

    public class PulsarClientTests
    {
        [Fact]
        public async Task GetPartitionedProducer_GivenPartitionedTopic_ShouldReturnPartitionProducer()
        {
            //Arrange
            var topicName = "persistent://public/default/test-topic";
            uint expectedPartitions = 3;

            var connection = Substitute.For<IConnection>();

            // use saveGetPartitions to assert CommandPartitionedTopicMetadata.
            CommandPartitionedTopicMetadata? saveGetPartitions = null;

            connection.Send(Arg.Any<CommandPartitionedTopicMetadata>(), Arg.Any<CancellationToken>())
                .Returns(new BaseCommand()
                {
                    CommandType = BaseCommand.Type.PartitionedMetadataResponse,
                    PartitionMetadataResponse = new CommandPartitionedTopicMetadataResponse()
                    {
                        Response = CommandPartitionedTopicMetadataResponse.LookupType.Success, Partitions = expectedPartitions
                    }
                })
                .AndDoes(info =>
                {
                    saveGetPartitions = (CommandPartitionedTopicMetadata) info[0];
                });

            var connectionPool = Substitute.For<IConnectionPool>();
            connectionPool.FindConnectionForTopic(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(connection);

            var client = PulsarClientFactory.CreatePulsarClient(connectionPool, new ProcessManager(connectionPool), Substitute.For<IHandleException>(),new Uri
            ("pusarl://localhost:6650/"));

            //Act
            await using var producer = client.NewProducer(Schema.String).Topic(topicName).Create();

            //Assert
            Assert.NotNull(saveGetPartitions);
            Assert.Equal(saveGetPartitions?.Topic, topicName);
            Assert.IsType<PartitionedProducer<string>>(producer);
        }
    }
}
