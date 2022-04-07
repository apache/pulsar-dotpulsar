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
using Xunit;

[Trait("Category", "Unit")]
public class SerializerTests
{
    [Fact]
    public void ToBigEndianBytes_GivenUnsignedInteger_ShouldReturnExpectedBytes()
    {
        //Arrange
        uint value = 66051;

        //Act
        var actual = Serializer.ToBigEndianBytes(value);

        //Assert
        var expected = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        actual.Should().Equal(expected);
    }
}
