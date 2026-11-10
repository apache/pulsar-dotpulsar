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
using DotPulsar.Internal.PulsarApi;
using Google.Protobuf;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;

[Trait("Category", "Unit")]
public sealed class BatchHandlerTests
{
    [Fact]
    public void Acknowledge_GivenMultipleBatches_ShouldMatchLedgerEntryAndPartition()
    {
        // Arrange
        var handler = CreateHandler();
        var batches = new[]
        {
            CreateId(1, 2, 0),
            CreateId(2, 2, 0),
            CreateId(1, 2, 1),
            CreateId(1, 3, 0)
        };

        foreach (var batch in batches)
        {
            AddBatch(handler, batch, 2);
            handler.Acknowledge(CreateId(batch.LedgerId, batch.EntryId, batch.Partition, 0)).ShouldBeNull();
        }

        // Act and assert
        foreach (var batch in batches.Reverse())
        {
            handler.Acknowledge(CreateId(batch.LedgerId, batch.EntryId, batch.Partition, 1)).ShouldBeSameAs(batch);
            handler.Acknowledge(CreateId(batch.LedgerId, batch.EntryId, batch.Partition, 0)).ShouldBeNull();
        }

        handler.Acknowledge(CreateId(3, 2, 0, 0)).ShouldBeNull();
    }

    [Fact]
    public void Acknowledge_GivenDuplicateAndOutOfOrderIndices_ShouldCompleteOnlyOnce()
    {
        // Arrange
        var handler = CreateHandler();
        var batch = CreateId(1, 2, 0);
        AddBatch(handler, batch, 3);

        // Act and assert
        handler.Acknowledge(CreateId(1, 2, 0, 2)).ShouldBeNull();
        handler.Acknowledge(CreateId(1, 2, 0, 0)).ShouldBeNull();
        handler.Acknowledge(CreateId(1, 2, 0, 2)).ShouldBeNull();
        handler.Acknowledge(CreateId(1, 2, 0, 1)).ShouldBeSameAs(batch);
        handler.Acknowledge(CreateId(1, 2, 0, 1)).ShouldBeNull();
    }

    [Fact]
    public void Add_GivenRedeliveryOfIncompleteBatch_ShouldRetainProgressWithoutTrackingTwice()
    {
        // Arrange
        var handler = CreateHandler();
        var batch = CreateId(1, 2, 0);
        AddBatch(handler, batch, 2);
        handler.Acknowledge(CreateId(1, 2, 0, 0)).ShouldBeNull();

        // Act
        AddBatch(handler, CreateId(1, 2, 0), 2);

        // Assert
        handler.Acknowledge(CreateId(1, 2, 0, 1)).ShouldBeSameAs(batch);
        handler.Acknowledge(CreateId(1, 2, 0, 0)).ShouldBeNull();
        handler.Acknowledge(CreateId(1, 2, 0, 1)).ShouldBeNull();
    }

    [Fact]
    public void Add_GivenRedeliveryOfCompletedBatch_ShouldTrackNewDelivery()
    {
        // Arrange
        var handler = CreateHandler();
        var original = CreateId(1, 2, 0);
        AddBatch(handler, original, 2);
        handler.Acknowledge(CreateId(1, 2, 0, 0)).ShouldBeNull();
        handler.Acknowledge(CreateId(1, 2, 0, 1)).ShouldBeSameAs(original);

        // Act
        var redelivered = CreateId(1, 2, 0);
        AddBatch(handler, redelivered, 2);

        // Assert
        handler.Acknowledge(CreateId(1, 2, 0, 0)).ShouldBeNull();
        handler.Acknowledge(CreateId(1, 2, 0, 1)).ShouldBeSameAs(redelivered);
    }

    [Fact]
    public void Clear_GivenTrackedBatch_ShouldDiscardMessagesAndAcknowledgementProgress()
    {
        // Arrange
        var handler = CreateHandler();
        var batch = CreateId(1, 2, 0);
        AddBatch(handler, batch, 2);
        handler.Acknowledge(CreateId(1, 2, 0, 0)).ShouldBeNull();

        // Act
        handler.Clear();

        // Assert
        handler.GetNext().ShouldBeNull();
        handler.Acknowledge(CreateId(1, 2, 0, 1)).ShouldBeNull();

        AddBatch(handler, batch, 2);
        handler.Acknowledge(CreateId(1, 2, 0, 1)).ShouldBeNull();
        handler.Acknowledge(CreateId(1, 2, 0, 0)).ShouldBeSameAs(batch);
    }

    [Fact]
    public void Add_GivenBatchTrackingDisabled_ShouldStillQueueMessages()
    {
        // Arrange
        var handler = CreateHandler(false);
        var batch = CreateId(1, 2, 0);

        // Act
        var first = AddBatch(handler, batch, 2);

        // Assert
        first.MessageId.BatchIndex.ShouldBe(0);
        handler.GetNext()!.MessageId.BatchIndex.ShouldBe(1);
        handler.Acknowledge(CreateId(1, 2, 0, 0)).ShouldBeNull();
    }

    [Fact]
    public void Acknowledge_GivenConcurrentIndices_ShouldCompleteBatchOnce()
    {
        // Arrange
        const int batchSize = 256;
        var handler = CreateHandler();
        var batch = CreateId(1, 2, 0);
        AddBatch(handler, batch, batchSize);
        var completed = new ConcurrentBag<MessageIdData>();

        // Act
        Parallel.For(0, batchSize, index =>
        {
            var acknowledged = handler.Acknowledge(CreateId(1, 2, 0, index));
            if (acknowledged is not null)
                completed.Add(acknowledged);
        });

        // Assert
        completed.Single().ShouldBeSameAs(batch);
    }

    private static BatchHandler<byte[]> CreateHandler(bool trackBatches = true)
        => new(trackBatches, new MessageFactory<byte[]>(DotPulsar.Schema.ByteArray));

    private static MessageIdData CreateId(ulong ledger, ulong entry, int partition, int batchIndex = -1)
        => new() { LedgerId = ledger, EntryId = entry, Partition = partition, BatchIndex = batchIndex };

    private static DotPulsar.Abstractions.IMessage<byte[]> AddBatch(BatchHandler<byte[]> handler, MessageIdData batch, int count)
    {
        var metadata = new MessageMetadata
        {
            ProducerName = "test",
            NumMessagesInBatch = count,
            PublishTime = 1,
            SequenceId = 1
        };

        var singleMetadata = new SingleMessageMetadata { PayloadSize = 0 }.ToByteArray();
        var data = new byte[count * (4 + singleMetadata.Length)];
        for (var i = 0; i < count; ++i)
        {
            var offset = i * (4 + singleMetadata.Length);
            BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(offset, 4), (uint) singleMetadata.Length);
            singleMetadata.CopyTo(data.AsSpan(offset + 4));
        }

        return handler.Add(batch, 0, metadata, new ReadOnlySequence<byte>(data));
    }
}
