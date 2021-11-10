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
using FluentAssertions;
using System.Buffers;
using System.Linq;
using Xunit;

public class SequenceBuilderTests
{
    [Fact]
    public void Append_GivenMultipleArrayInputs_ShouldArrangeInCorrectOrder()
    {
        //Arrange
        var a = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        var b = new byte[] { 0x04, 0x05, 0x06, 0x07, 0x08 };
        var c = new byte[] { 0x09 };

        var builder = new SequenceBuilder<byte>().Append(a).Append(b).Append(c);

        //Act
        var actual = builder.Build().ToArray();

        //Assert
        var expected = CreateRange(0, 10);
        actual.Should().Equal(expected);
    }

    [Fact]
    public void Append_GivenMultipleArrayAndSequenceInputs_ShouldArrangeInCorrectOrder()
    {
        //Arrange
        var a = new byte[] { 0x00, 0x01 };
        var b = new byte[] { 0x02, 0x03 };
        var c = new byte[] { 0x04, 0x05 };
        var d = new byte[] { 0x06, 0x07 };
        var e = new byte[] { 0x08, 0x09 };

        var seq = new SequenceBuilder<byte>().Append(b).Append(c).Append(d).Build();
        var builder = new SequenceBuilder<byte>().Append(a).Append(seq).Append(e);

        //Act
        var actual = builder.Build().ToArray();

        //Assert
        var expected = CreateRange(0, 10);
        actual.Should().Equal(expected);
    }

    [Fact]
    public void Prepend_GivenMultipleArrayInputs_ShouldArrangeInCorrectOrder()
    {
        //Arrange
        var a = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        var b = new byte[] { 0x04, 0x05, 0x06, 0x07, 0x08 };
        var c = new byte[] { 0x09 };

        var builder = new SequenceBuilder<byte>().Prepend(c).Prepend(b).Prepend(a);

        //Act
        var actual = builder.Build().ToArray();

        //Assert
        var expected = CreateRange(0, 10);
        actual.Should().Equal(expected);
    }

    [Fact]
    public void Prepend_GivenMultipleArrayAndSequenceInputs_ShouldArrangeInCorrectOrder()
    {
        //Arrange
        var a = new byte[] { 0x00, 0x01 };
        var b = new byte[] { 0x02, 0x03 };
        var c = new byte[] { 0x04, 0x05 };
        var d = new byte[] { 0x06, 0x07 };
        var e = new byte[] { 0x08, 0x09 };

        var seq = new SequenceBuilder<byte>().Prepend(d).Prepend(c).Prepend(b).Build();
        var builder = new SequenceBuilder<byte>().Prepend(e).Prepend(seq).Prepend(a);

        //Act
        var actual = builder.Build().ToArray();

        //Assert
        var expected = CreateRange(0, 10);
        actual.Should().Equal(expected);
    }

    [Fact]
    public void Build_GivenMultipleInvocations_ShouldCreateIdenticalSequences()
    {
        //Arrange
        var a = new byte[] { 0x00, 0x01 };
        var b = new byte[] { 0x02, 0x03 };

        var builder = new SequenceBuilder<byte>().Append(a).Append(b);

        //Act
        var actual1 = builder.Build().ToArray();
        var actual2 = builder.Build().ToArray();

        //Assert
        var expected = CreateRange(0, 4);
        actual1.Should().Equal(expected);
        actual2.Should().Equal(expected);
    }

    private static byte[] CreateRange(int start, int count)
    {
        var bytes = new byte[count];

        for (var i = 0; i < count; ++i)
            bytes[i] = System.Convert.ToByte(start + i);

        return bytes;
    }
}
