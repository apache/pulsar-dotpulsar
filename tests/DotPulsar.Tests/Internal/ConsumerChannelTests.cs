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
using Google.Protobuf;
using System.Buffers;
using System.Buffers.Binary;

[Trait("Category", "Unit")]
public sealed class ConsumerChannelTests
{
    [Fact]
    public async Task Send_GivenEntireProducerBatchInOneAck_ShouldAcknowledgeBatch()
    {
        // Arrange
        var (batchHandler, messageFactory, first, second) = CreateBatch();
        var connection = Substitute.For<IConnection>();
        connection.Send(Arg.Any<CommandAck>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        await using var channel = CreateChannel(connection, batchHandler, messageFactory);

        var command = new CommandAck { AckType = CommandAck.Types.AckType.Individual };
        command.MessageId.Add(ToProtocolId(first.MessageId));
        command.MessageId.Add(ToProtocolId(second.MessageId));

        // Act
        await channel.Send(command, Current.CancellationToken);

        // Assert
        await connection.Received(1).Send(
            Arg.Is<CommandAck>(ack => ack.MessageId.Count == 1
                && ack.MessageId[0].LedgerId == first.MessageId.LedgerId
                && ack.MessageId[0].EntryId == first.MessageId.EntryId
                && ack.MessageId[0].BatchIndex == -1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Send_GivenIncompleteBatchBeforeUnbatchedMessage_ShouldAcknowledgeUnbatchedMessage()
    {
        // Arrange
        var (batchHandler, messageFactory, first, _) = CreateBatch();
        var connection = Substitute.For<IConnection>();
        connection.Send(Arg.Any<CommandAck>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        await using var channel = CreateChannel(connection, batchHandler, messageFactory);

        var unbatched = new MessageIdData { LedgerId = 1, EntryId = 3, Partition = 0, BatchIndex = -1 };
        var command = new CommandAck { AckType = CommandAck.Types.AckType.Individual };
        command.MessageId.Add(ToProtocolId(first.MessageId));
        command.MessageId.Add(unbatched);

        // Act
        await channel.Send(command, Current.CancellationToken);

        // Assert
        await connection.Received(1).Send(
            Arg.Is<CommandAck>(ack => ack.MessageId.Count == 1
                && ack.MessageId[0].LedgerId == unbatched.LedgerId
                && ack.MessageId[0].EntryId == unbatched.EntryId
                && ack.MessageId[0].BatchIndex == -1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Send_GivenUnbatchedMessageBeforeEntireProducerBatch_ShouldAcknowledgeBoth()
    {
        // Arrange
        var (batchHandler, messageFactory, first, second) = CreateBatch();
        var connection = Substitute.For<IConnection>();
        connection.Send(Arg.Any<CommandAck>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        await using var channel = CreateChannel(connection, batchHandler, messageFactory);

        var unbatched = new MessageIdData { LedgerId = 1, EntryId = 3, Partition = 0, BatchIndex = -1 };
        var command = new CommandAck { AckType = CommandAck.Types.AckType.Individual };
        command.MessageId.Add(unbatched);
        command.MessageId.Add(ToProtocolId(first.MessageId));
        command.MessageId.Add(ToProtocolId(second.MessageId));

        // Act
        await channel.Send(command, Current.CancellationToken);

        // Assert
        await connection.Received(1).Send(
            Arg.Is<CommandAck>(ack => ack.MessageId.Count == 2
                && ack.MessageId[0].EntryId == unbatched.EntryId
                && ack.MessageId[1].EntryId == first.MessageId.EntryId
                && ack.MessageId[1].BatchIndex == -1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Send_GivenUnbatchedMessages_ShouldAcknowledgeAllInOneCommand()
    {
        // Arrange
        var (batchHandler, messageFactory, _, _) = CreateBatch();
        var connection = Substitute.For<IConnection>();
        connection.Send(Arg.Any<CommandAck>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        await using var channel = CreateChannel(connection, batchHandler, messageFactory);

        var command = new CommandAck { AckType = CommandAck.Types.AckType.Individual };
        command.MessageId.Add(new MessageIdData { LedgerId = 1, EntryId = 3, Partition = 0, BatchIndex = -1 });
        command.MessageId.Add(new MessageIdData { LedgerId = 1, EntryId = 4, Partition = 0, BatchIndex = -1 });

        // Act
        await channel.Send(command, Current.CancellationToken);

        // Assert
        await connection.Received(1).Send(
            Arg.Is<CommandAck>(ack => ack.MessageId.Count == 2
                && ack.MessageId[0].EntryId == 3
                && ack.MessageId[1].EntryId == 4),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Send_GivenEmptyAcknowledgements_ShouldNotSend()
    {
        // Arrange
        var (batchHandler, messageFactory, _, _) = CreateBatch();
        var connection = Substitute.For<IConnection>();
        await using var channel = CreateChannel(connection, batchHandler, messageFactory);

        // Act
        await channel.Send(new CommandAck { AckType = CommandAck.Types.AckType.Individual }, Current.CancellationToken);

        // Assert
        await connection.DidNotReceive().Send(Arg.Any<CommandAck>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Send_GivenPartialBatch_ShouldWaitForRemainingMessageBeforeAcknowledging()
    {
        // Arrange
        var (batchHandler, messageFactory, first, second) = CreateBatch();
        var connection = Substitute.For<IConnection>();
        connection.Send(Arg.Any<CommandAck>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        await using var channel = CreateChannel(connection, batchHandler, messageFactory);

        var firstCommand = new CommandAck { AckType = CommandAck.Types.AckType.Individual };
        firstCommand.MessageId.Add(ToProtocolId(first.MessageId));
        var secondCommand = new CommandAck { AckType = CommandAck.Types.AckType.Individual };
        secondCommand.MessageId.Add(ToProtocolId(second.MessageId));

        // Act
        await channel.Send(firstCommand, Current.CancellationToken);
        await connection.DidNotReceive().Send(Arg.Any<CommandAck>(), Arg.Any<CancellationToken>());
        await channel.Send(secondCommand, Current.CancellationToken);

        // Assert
        await connection.Received(1).Send(
            Arg.Is<CommandAck>(ack => ack.MessageId.Count == 1
                && ack.MessageId[0].EntryId == first.MessageId.EntryId
                && ack.MessageId[0].BatchIndex == -1),
            Arg.Any<CancellationToken>());
    }

    private static (BatchHandler<byte[]> Handler, MessageFactory<byte[]> Factory, DotPulsar.Abstractions.IMessage<byte[]> First, DotPulsar.Abstractions.IMessage<byte[]> Second) CreateBatch()
    {
        var messageFactory = new MessageFactory<byte[]>(DotPulsar.Schema.ByteArray);
        var batchHandler = new BatchHandler<byte[]>(true, messageFactory);
        var batchId = new MessageIdData { LedgerId = 1, EntryId = 2, Partition = 0, BatchIndex = -1 };
        var metadata = new MessageMetadata
        {
            ProducerName = "test",
            NumMessagesInBatch = 2,
            PublishTime = 1,
            SequenceId = 1
        };

        var singleMessageMetadata = new SingleMessageMetadata { PayloadSize = 0 }.ToByteArray();
        var batchData = new byte[2 * (4 + singleMessageMetadata.Length)];
        for (var i = 0; i < 2; ++i)
        {
            var offset = i * (4 + singleMessageMetadata.Length);
            BinaryPrimitives.WriteUInt32BigEndian(batchData.AsSpan(offset, 4), (uint) singleMessageMetadata.Length);
            singleMessageMetadata.CopyTo(batchData.AsSpan(offset + 4));
        }

        var first = batchHandler.Add(batchId, 0, metadata, new ReadOnlySequence<byte>(batchData));
        var second = batchHandler.GetNext();
        second.ShouldNotBeNull();
        first.MessageId.BatchIndex.ShouldBe(0);
        second.MessageId.BatchIndex.ShouldBe(1);

        return (batchHandler, messageFactory, first, second);
    }

    private static ConsumerChannel<byte[]> CreateChannel(
        IConnection connection,
        BatchHandler<byte[]> batchHandler,
        MessageFactory<byte[]> messageFactory)
        => new(1, 100, new AsyncQueue<MessagePackage>(), connection, batchHandler, messageFactory, [], "persistent://public/default/test");

    private static MessageIdData ToProtocolId(MessageId messageId)
        => new()
        {
            LedgerId = messageId.LedgerId,
            EntryId = messageId.EntryId,
            Partition = messageId.Partition,
            BatchIndex = messageId.BatchIndex
        };
}
