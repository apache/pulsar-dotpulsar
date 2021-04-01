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
    using Abstractions;
    using DotPulsar.Internal;
    using DotPulsar.Internal.Abstractions;
    using DotPulsar.Internal.Events;
    using Moq;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Xunit;

    public class PartitionedProducerProcessTests
    {
        [Fact]
        public void TestPartitionedProducerStateManage()
        {
            var connectionPoolMock = new Mock<IConnectionPool>(MockBehavior.Loose);
            var connectionPool = connectionPoolMock.Object;
            var establishNewChannelMock = new Mock<IEstablishNewChannel>(MockBehavior.Loose);
            var establishNewChannel = establishNewChannelMock.Object;
            var producerMock = new Mock<IProducer>(MockBehavior.Loose);
            var producer = producerMock.Object;

            var processManager = new ProcessManager(connectionPool);

            var producerProcesses = new Dictionary<uint, ProducerProcess>(3);
            var producerGuids = new Dictionary<uint, Guid>(3);
            var producersGroup = new ConcurrentDictionary<uint, IProducer>(Environment.ProcessorCount, 3);
            var partitionedProducerGuid = Guid.NewGuid();

            for (uint i = 0; i < 3; i++)
            {
                var stateManager = new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);
                var correlationId = Guid.NewGuid();
                var process = new ProducerProcess(correlationId, stateManager, establishNewChannel, processManager, partitionedProducerGuid, i);
                producerGuids[i] = correlationId;
                producerProcesses[i] = process;
                producersGroup[i] = producer;
                processManager.Add(process);
            }

            var partitionedStateManager =
                new StateManager<PartitionedProducerState>(PartitionedProducerState.Disconnected, PartitionedProducerState.Closed, PartitionedProducerState.Faulted);
            var partitionedProducerProcess = new PartitionedProducerProcess(partitionedProducerGuid, partitionedStateManager, producersGroup);
            processManager.Add(partitionedProducerProcess);

            // Test connect
            Assert.Equal(PartitionedProducerState.Disconnected, partitionedStateManager.CurrentState);
            processManager.Register(new ChannelConnected(producerGuids[0]));
            Assert.Equal(PartitionedProducerState.PartiallyConnected, partitionedStateManager.CurrentState);
            processManager.Register(new ChannelConnected(producerGuids[1]));
            Assert.Equal(PartitionedProducerState.PartiallyConnected, partitionedStateManager.CurrentState);
            processManager.Register(new ChannelConnected(producerGuids[2]));
            Assert.Equal(PartitionedProducerState.Connected, partitionedStateManager.CurrentState);

            // Test disconnect
            processManager.Register(new ChannelDisconnected(producerGuids[1]));
            Assert.Equal(PartitionedProducerState.PartiallyConnected, partitionedStateManager.CurrentState);

            // Test reconnect
            processManager.Register(new ChannelConnected(producerGuids[1]));
            Assert.Equal(PartitionedProducerState.Connected, partitionedStateManager.CurrentState);

            // Test fault
            processManager.Register(new ExecutorFaulted(producerGuids[1]));
            Assert.Equal(PartitionedProducerState.Faulted, partitionedStateManager.CurrentState);
        }
    }
}
