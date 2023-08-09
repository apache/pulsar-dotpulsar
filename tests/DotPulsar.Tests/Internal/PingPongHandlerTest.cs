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

namespace DotPulsar.Tests.Internal;

using DotPulsar.Internal;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[Trait("Category", "Unit")]
public class PingPongHandlerTest
{
    [Fact]
    public async Task Watch_GivenConnectionNotAlive_ShouldDisposeConnection()
    {
        var countDown = new CountdownEvent(1);
        var connection = new MockConnection();
        var keepAliveInterval = TimeSpan.FromSeconds(1);
        connection.PingCallback = () => countDown.Signal();
        var pingPongHandler = new PingPongHandler(connection, keepAliveInterval);

        // Wait for the first Ping
        // The ping arrive time should be at (keepAliveInterval, 2*keepAliveInterval)
        countDown.Wait(2 * keepAliveInterval);
        pingPongHandler.Incoming(BaseCommand.Type.Pong);
        Assert.False(connection.IsDispose);

        // Wait for the second Ping, but we don't reply with Pong.
        // The connection disposed time should be at (2*keepAliveInterval, 3*keepAliveInterval)
        await Task.Delay(3 * keepAliveInterval);
        Assert.True(connection.IsDispose);
    }

    private class MockConnection : IConnection
    {
        public volatile bool IsDispose;
        public Action PingCallback = (() => { });
        public ValueTask DisposeAsync() => throw new System.NotImplementedException();
        public ValueTask<bool> HasChannels(CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<ProducerResponse> Send(CommandProducer command, IChannel channel, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<SubscribeResponse> Send(CommandSubscribe command, IChannel channel, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task Send(CommandPing command, CancellationToken cancellationToken)
        {
            PingCallback();
            return Task.CompletedTask;
        }
        public Task Send(CommandPong command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task Send(CommandAck command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task Send(CommandFlow command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task Send(CommandRedeliverUnacknowledgedMessages command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<BaseCommand> Send(CommandUnsubscribe command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<BaseCommand> Send(CommandConnect command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<BaseCommand> Send(CommandLookupTopic command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<BaseCommand> Send(CommandSeek command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<BaseCommand> Send(CommandGetLastMessageId command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<BaseCommand> Send(CommandCloseProducer command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<BaseCommand> Send(CommandCloseConsumer command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task Send(SendPackage command, TaskCompletionSource<BaseCommand> responseTcs, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<BaseCommand> Send(CommandGetOrCreateSchema command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public Task<BaseCommand> Send(CommandPartitionedTopicMetadata command, CancellationToken cancellationToken) =>
            throw new System.NotImplementedException();
        public void MarkInactive()
        {
            this.IsDispose = true;
        }
    }
}
