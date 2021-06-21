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
    using NSubstitute;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Xunit;

    public class PartitionedProducerProcessTests
    {
        [Fact]
        public void TestPartitionedProducerStateManage_WhenSubProducersStateChange_ThenPartitionedProducerStateChangeCorrectly()
        {
            var connectionPool = Substitute.For<IConnectionPool>();
            var establishNewChannel = Substitute.For<IEstablishNewChannel>();
            var producer = Substitute.For<IProducer>();

            var processManager = new ProcessManager(connectionPool);

            var producerGuids = new Dictionary<uint, Guid>(3);
            var producersGroup = new ConcurrentDictionary<uint, IProducer>(Environment.ProcessorCount, 3);
            var partitionedProducerGuid = Guid.NewGuid();

            for (uint i = 0; i < 3; i++)
            {
                var stateManager = new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);
                var correlationId = Guid.NewGuid();
                var process = new ProducerProcess(correlationId, stateManager, establishNewChannel, processManager, partitionedProducerGuid);
                producerGuids[i] = correlationId;
                producersGroup[i] = producer;
                processManager.Add(process);
            }

            var partitionedStateManager =
                new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);

            var producerProcess = new ProducerProcess(partitionedProducerGuid, partitionedStateManager, establishNewChannel, new ProcessManager(connectionPool));
            processManager.Add(producerProcess);
            processManager.Register(new UpdatePartitions(partitionedProducerGuid, (uint) producersGroup.Count));

            // Test initial channel
            processManager.Register(new ChannelDisconnected(producerGuids[0]));
            Assert.Equal(ProducerState.Disconnected, partitionedStateManager.CurrentState);
            processManager.Register(new ChannelDisconnected(producerGuids[1]));
            Assert.Equal(ProducerState.Disconnected, partitionedStateManager.CurrentState);
            processManager.Register(new ChannelDisconnected(producerGuids[2]));
            Assert.Equal(ProducerState.Disconnected, partitionedStateManager.CurrentState);

            // Test connect
            Assert.Equal(ProducerState.Disconnected, partitionedStateManager.CurrentState);
            processManager.Register(new ChannelConnected(producerGuids[0]));
            Assert.Equal(ProducerState.PartiallyConnected, partitionedStateManager.CurrentState);
            processManager.Register(new ChannelConnected(producerGuids[1]));
            Assert.Equal(ProducerState.PartiallyConnected, partitionedStateManager.CurrentState);
            processManager.Register(new ChannelConnected(producerGuids[2]));
            Assert.Equal(ProducerState.Connected, partitionedStateManager.CurrentState);

            // Test disconnect
            processManager.Register(new ChannelDisconnected(producerGuids[1]));
            Assert.Equal(ProducerState.PartiallyConnected, partitionedStateManager.CurrentState);

            // Test reconnect
            processManager.Register(new ChannelConnected(producerGuids[1]));
            Assert.Equal(ProducerState.Connected, partitionedStateManager.CurrentState);

            // Test fault
            processManager.Register(new ExecutorFaulted(producerGuids[1]));
            Assert.Equal(ProducerState.Faulted, partitionedStateManager.CurrentState);
        }

        [Fact]
        public void TestUpdatePartitions_WhenIncreasePartitions_ThenPartitionedProducerStateChangeCorrectly()
        {
            var connectionPool = Substitute.For<IConnectionPool>();
            var processManager = new ProcessManager(connectionPool);
            var establishNewChannel = Substitute.For<IEstablishNewChannel>();

            var guid = Guid.NewGuid();
            var stateManager = new StateManager<ProducerState>(ProducerState.Disconnected, ProducerState.Closed, ProducerState.Faulted);
            var process = new ProducerProcess(guid, stateManager, establishNewChannel, new ProcessManager(connectionPool));
            processManager.Add(process);
            processManager.Register(new UpdatePartitions(guid, 1));

            Assert.Equal(ProducerState.Disconnected, stateManager.CurrentState);
            processManager.Register(new PartitionedSubProducerStateChanged(guid, ProducerState.Connected));
            Assert.Equal(ProducerState.Connected, stateManager.CurrentState);
            processManager.Register(new UpdatePartitions(guid, 2));
            Assert.Equal(ProducerState.PartiallyConnected, stateManager.CurrentState);
            processManager.Register(new PartitionedSubProducerStateChanged(guid, ProducerState.Connected));
            Assert.Equal(ProducerState.Connected, stateManager.CurrentState);
        }
    }
}
