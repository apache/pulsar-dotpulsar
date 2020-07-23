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
    using DotPulsar.Abstractions;
    using DotPulsar.Internal;
    using DotPulsar.Internal.Abstractions;
    using DotPulsar.Internal.PulsarApi;
    using Moq;
    using System.Threading;
    using Xunit;
    public class PulsarClientTests
    {
        [Fact]
        public void CreateProducer_GivenPartitionedProducerConfig_SholudReturnPartitionedProducer()
        {
            var topicName = "persistent://public/default/test-topic";

            var connectionMock = new Mock<IConnection>(MockBehavior.Strict);
            _ = connectionMock.Setup(c => c.Send(It.IsAny<CommandPartitionedTopicMetadata>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BaseCommand()
                {
                    CommandType = BaseCommand.Type.PartitionedMetadataResponse,
                    PartitionMetadataResponse = new CommandPartitionedTopicMetadataResponse()
                    {
                        Response = CommandPartitionedTopicMetadataResponse.LookupType.Success,
                        Partitions = 3
                    }
                });
            _ = connectionMock.Setup(c => c.Send(It.IsAny<CommandProducer>(), It.IsAny<Channel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProducerResponse(0, ""));
            var connection = connectionMock.Object;
            var connectionPoolMock = new Mock<IConnectionPool>(MockBehavior.Strict);
            _ = connectionPoolMock.Setup(c => c.FindConnectionForTopic(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(connection);
            var connectionPool = connectionPoolMock.Object;

            var client = new PulsarClient(connectionPoolMock.Object, new ProcessManager(connectionPool), new Mock<IHandleException>().Object);
            var options = new ProducerOptions(topicName);

            var producer = client.CreateProducer(options);

            Assert.IsType<PartitionedProducer>(producer);
        }
    }
}
