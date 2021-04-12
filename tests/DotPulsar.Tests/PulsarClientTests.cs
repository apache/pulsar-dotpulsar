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
    using Moq;
    using System;
    using System.Threading;
    using Xunit;

    public class PulsarClientTests
    {
        [Fact]
        public async void GetPartitions_GivenPartitionedTopic_ShouldReturnPartitionsNumber()
        {
            //Arrange
            var topicName = "persistent://public/default/test-topic";
            uint expectedPartitions = 3;

            var connectionMock = new Mock<IConnection>(MockBehavior.Strict);

            // use saveGetPartitions to assert CommandPartitionedTopicMetadata.
            CommandPartitionedTopicMetadata saveGetPartitions = null;

            _ = connectionMock.Setup(c => c.Send(It.IsAny<CommandPartitionedTopicMetadata>(), It.IsAny<CancellationToken>()))
                .Callback<CommandPartitionedTopicMetadata, CancellationToken>((metadata, cancellationToken) => saveGetPartitions = metadata)
                .ReturnsAsync(new BaseCommand()
                {
                    CommandType = BaseCommand.Type.PartitionedMetadataResponse,
                    PartitionMetadataResponse = new CommandPartitionedTopicMetadataResponse()
                    {
                        Response = CommandPartitionedTopicMetadataResponse.LookupType.Success, Partitions = expectedPartitions
                    }
                });

            var connection = connectionMock.Object;
            var connectionPoolMock = new Mock<IConnectionPool>(MockBehavior.Strict);

            _ = connectionPoolMock.Setup(c => c.FindConnectionForTopic(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(connection);
            var connectionPool = connectionPoolMock.Object;

            var client = new PulsarClient(connectionPoolMock.Object, new ProcessManager(connectionPool), new Mock<IHandleException>().Object, new Uri("pusarl://localhost:6650/"));

            //Act
            uint partitions = await client.GetNumberOfPartitions(topicName, CancellationToken.None).ConfigureAwait(false);

            //Assert
            Assert.NotNull(saveGetPartitions);
            Assert.Equal(saveGetPartitions?.Topic, topicName);
            Assert.Equal(partitions, expectedPartitions);
        }
    }
}
