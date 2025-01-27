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

#nullable disable

namespace DotPulsar.Tests;

[Trait("Category", "Unit")]
public class MessageIdTests
{
    [Fact]
    public void CompareTo_GivenTheSameValues_ShouldBeEqual()
    {
        var m1 = new MessageId(1, 2, 3, 4);
        var m2 = new MessageId(1, 2, 3, 4);

        m1.CompareTo(m2).ShouldBe(0);
        (m1 < m2).ShouldBeFalse();
        (m1 > m2).ShouldBeFalse();
        (m1 >= m2).ShouldBeTrue();
        (m1 <= m2).ShouldBeTrue();
    }

    [Fact]
    public void CompareTo_GivenTheSameValuesWithTopic_ShouldBeEqual()
    {
        var m1 = new MessageId(1, 2, 3, 4, "persistent://public/default/my-topic-partition-0");
        var m2 = new MessageId(1, 2, 3, 4, "persistent://public/default/my-topic-partition-0");

        m1.CompareTo(m2).ShouldBe(0);
        (m1 < m2).ShouldBeFalse();
        (m1 > m2).ShouldBeFalse();
        (m1 >= m2).ShouldBeTrue();
        (m1 <= m2).ShouldBeTrue();
    }

    [Fact]
    public void CompareTo_GivenAllNull_ShouldBeEqual()
    {
        MessageId m1 = null;
        MessageId m2 = null;

        (m1 < m2).ShouldBeFalse();
        (m1 > m2).ShouldBeFalse();
        (m1 <= m2).ShouldBeTrue();
        (m1 >= m2).ShouldBeTrue();
    }

    [Fact]
    public void CompareTo_GivenOneNull_ShouldFollowNull()
    {
        var m1 = new MessageId(1, 2, 3, 4);
        MessageId m2 = null;

        m1.CompareTo(m2).ShouldBePositive();
        (m1 < m2).ShouldBeFalse();
        (m1 > m2).ShouldBeTrue();
        (m1 <= m2).ShouldBeFalse();
        (m1 >= m2).ShouldBeTrue();

        (m2 < m1).ShouldBeTrue();
        (m2 > m1).ShouldBeFalse();
        (m2 <= m1).ShouldBeTrue();
        (m2 >= m1).ShouldBeFalse();
    }

    [Theory]
    [InlineData(2, 2, 3, 4)] // LegderId is greater
    [InlineData(1, 3, 3, 4)] // EntryId is greater
    [InlineData(1, 2, 4, 4)] // Partition is greater
    [InlineData(1, 2, 3, 5)] // BatchIndex is greater
    public void CompareTo_GivenCurrentFollowsArgument_ShouldReturnPositive(ulong ledgerId, ulong entryId, int partition, int batchIndex)
    {
        var m1 = new MessageId(ledgerId, entryId, partition, batchIndex);
        var m2 = new MessageId(1, 2, 3, 4);

        m1.CompareTo(m2).ShouldBePositive();
        (m1 > m2).ShouldBeTrue();
        (m1 < m2).ShouldBeFalse();
    }

    [Theory]
    [InlineData(0, 2, 3, 4)] // LegderId is less
    [InlineData(1, 1, 3, 4)] // EntryId is less
    [InlineData(1, 2, 2, 4)] // Partition is less
    [InlineData(1, 2, 3, 3)] // BatchIndex is less
    public void CompareTo_GivenCurrentPrecedesArgument_ShouldReturnNegative(ulong ledgerId, ulong entryId, int partition, int batchIndex)
    {
        var m1 = new MessageId(ledgerId, entryId, partition, batchIndex);
        var m2 = new MessageId(1, 2, 3, 4);

        m1.CompareTo(m2).ShouldBeNegative();
        (m1 < m2).ShouldBeTrue();
        (m1 > m2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_GivenTheSameObject_ShouldBeEqual()
    {
        var m1 = new MessageId(1, 2, 3, 4);
        var m2 = m1;

        m1.Equals(m2).ShouldBeTrue();
        (m1 == m2).ShouldBeTrue();
        (m1 != m2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_GivenTheSameValues_ShouldBeEqual()
    {
        var m1 = new MessageId(1, 2, 3, 4);
        var m2 = new MessageId(1, 2, 3, 4);

        m1.Equals(m2).ShouldBeTrue();
        (m1 == m2).ShouldBeTrue();
        (m1 != m2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_GivenTheSameValuesWithTopic_ShouldBeEqual()
    {
        var m1 = new MessageId(1, 2, 3, 4, "persistent://public/default/my-topic-partition-0");
        var m2 = new MessageId(1, 2, 3, 4, "persistent://public/default/my-topic-partition-0");

        m1.Equals(m2).ShouldBeTrue();
        (m1 == m2).ShouldBeTrue();
        (m1 != m2).ShouldBeFalse();
    }

    [Theory]
    [InlineData(0, 2, 3, 4)] // LegerId not the same
    [InlineData(1, 0, 3, 4)] // EntryId not the same
    [InlineData(1, 2, 0, 4)] // Partition not the same
    [InlineData(1, 2, 3, 0)] // BatchIndex not the same
    public void Equals_GivenDifferentValues_ShouldNotBeEqual(ulong ledgerId, ulong entryId, int partition, int batchIndex)
    {
        var m1 = new MessageId(ledgerId, entryId, partition, batchIndex);
        var m2 = new MessageId(1, 2, 3, 4);

        m1.Equals(m2).ShouldBeFalse();
        (m1 == m2).ShouldBeFalse();
        (m1 != m2).ShouldBeTrue();
    }

    [Theory]
    [InlineData(0, 2, 3, 4, "persistent://public/default/my-topic-partition-1")] // LegerId not the same
    [InlineData(1, 0, 3, 4, "persistent://public/default/my-topic-partition-1")] // EntryId not the same
    [InlineData(1, 2, 0, 4, "persistent://public/default/my-topic-partition-1")] // Partition not the same
    [InlineData(1, 2, 3, 0, "persistent://public/default/my-topic-partition-1")] // BatchIndex not the same
    [InlineData(1, 2, 3, 4, "persistent://public/default/my-topic-partition-0")] // Topic not the same
    public void Equals_GivenDifferentValuesWithTopic_ShouldNotBeEqual(ulong ledgerId, ulong entryId, int partition, int batchIndex, string topic)
    {
        var m1 = new MessageId(ledgerId, entryId, partition, batchIndex, topic);
        var m2 = new MessageId(1, 2, 3, 4, "persistent://public/default/my-topic-partition-1");

        m1.Equals(m2).ShouldBeFalse();
        (m1 == m2).ShouldBeFalse();
        (m1 != m2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_GivenAllNull_ShouldBeEqual()
    {
        MessageId m1 = null;
        MessageId m2 = null;

        (m1 == m2).ShouldBeTrue();
        (m1 is null).ShouldBeTrue();
        (m1 != m2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_GivenOneNull_ShouldNotBeEqual()
    {
        var m1 = new MessageId(1, 2, 3, 4);
        MessageId m2 = null;

        (m1 is null).ShouldBeFalse();
        (m1 == m2).ShouldBeFalse();
        m1.Equals(m2).ShouldBeFalse();
        (m1 != m2).ShouldBeTrue();
    }

    [Theory]
    [InlineData("0.0.0.0")] // Wrong separator
    [InlineData("A:0:0:0")] // Ledger id is not a number
    [InlineData("0:A:0:0")] // Entry id is not a number
    [InlineData("0:0:A:0")] // Partition id is not a number
    [InlineData("0:0:0:A")] // Batch index is not a number
    [InlineData(":::")] // Missing all
    [InlineData(":0:0:0")] // Missing ledger id
    [InlineData("0::0:0")] // Missing entry id
    [InlineData("0:0::0")] // Missing partion
    [InlineData("0:0:0:")] // Missing batch index
    public void TryParse_GivenInvalidString_ShouldReturnFalse(string input)
    {
        // Arrange
        var expected = MessageId.Earliest;

        // Act
        var result = MessageId.TryParse(input, out var actual);

        // Assert
        result.ShouldBeFalse();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void TryParse_GivenValidString_ShouldReturnTrue()
    {
        // Arrange
        var expected = new MessageId(1, 2, 3, 4, "topic");

        // Act
        var result = MessageId.TryParse(expected.ToString(), out var actual);

        // Assert
        result.ShouldBeTrue();
        actual.ShouldBe(expected);
    }
}

#nullable enable
