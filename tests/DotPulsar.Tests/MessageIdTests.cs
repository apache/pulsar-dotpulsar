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

using DotPulsar;
using FluentAssertions;
using Xunit;

public class MessageIdTests
{
    [Fact]
    public void CompareTo_GivenTheSameValues_ShouldBeEqual()
    {
        var m1 = new MessageId(1, 2, 3, 4);
        var m2 = new MessageId(1, 2, 3, 4);

        m1.CompareTo(m2).Should().Be(0);
        (m1 < m2).Should().BeFalse();
        (m1 > m2).Should().BeFalse();
        (m1 >= m2).Should().BeTrue();
        (m1 <= m2).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_GivenAllNull_ShouldBeEqual()
    {
        MessageId m1 = null;
        MessageId m2 = null;

        (m1 < m2).Should().BeFalse();
        (m1 > m2).Should().BeFalse();
        (m1 <= m2).Should().BeTrue();
        (m1 >= m2).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_GivenOneNull_ShouldFollowNull()
    {
        var m1 = new MessageId(1, 2, 3, 4);
        MessageId m2 = null;

        m1.CompareTo(m2).Should().BePositive();
        (m1 < m2).Should().BeFalse();
        (m1 > m2).Should().BeTrue();
        (m1 <= m2).Should().BeFalse();
        (m1 >= m2).Should().BeTrue();

        (m2 < m1).Should().BeTrue();
        (m2 > m1).Should().BeFalse();
        (m2 <= m1).Should().BeTrue();
        (m2 >= m1).Should().BeFalse();
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

        m1.CompareTo(m2).Should().BePositive();
        (m1 > m2).Should().BeTrue();
        (m1 < m2).Should().BeFalse();
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

        m1.CompareTo(m2).Should().BeNegative();
        (m1 < m2).Should().BeTrue();
        (m1 > m2).Should().BeFalse();
    }

    [Fact]
    public void Equals_GivenTheSameObject_ShouldBeEqual()
    {
        var m1 = new MessageId(1, 2, 3, 4);
        var m2 = m1;

        m1.Equals(m2).Should().BeTrue();
        (m1 == m2).Should().BeTrue();
        (m1 != m2).Should().BeFalse();
    }

    [Fact]
    public void Equals_GivenTheSameValues_ShouldBeEqual()
    {
        var m1 = new MessageId(1, 2, 3, 4);
        var m2 = new MessageId(1, 2, 3, 4);

        m1.Equals(m2).Should().BeTrue();
        (m1 == m2).Should().BeTrue();
        (m1 != m2).Should().BeFalse();
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

        m1.Equals(m2).Should().BeFalse();
        (m1 == m2).Should().BeFalse();
        (m1 != m2).Should().BeTrue();
    }

    [Fact]
    public void Equals_GivenAllNull_ShouldBeEqual()
    {
        MessageId m1 = null;
        MessageId m2 = null;

        (m1 == m2).Should().BeTrue();
        (m1 is null).Should().BeTrue();
        (m1 != m2).Should().BeFalse();
    }

    [Fact]
    public void Equals_GivenOneNull_ShouldNotBeEqual()
    {
        var m1 = new MessageId(1, 2, 3, 4);
        MessageId m2 = null;

        (m1 is null).Should().BeFalse();
        (m1 == m2).Should().BeFalse();
        m1.Equals(m2).Should().BeFalse();
        (m1 != m2).Should().BeTrue();
    }
}

#nullable enable
