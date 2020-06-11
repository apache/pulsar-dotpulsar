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
    using DotPulsar.Internal;
    using Xunit;

    public class Crc32CTests
    {
        [Fact]
        public void Calculate_GivenSequenceWithSingleSegment_ShouldReturnExpectedChecksum()
        {
            //Arrange
            var segment = new byte[] { 0x10, 0x01, 0x18, 0xc9, 0xf8, 0x86, 0x94, 0xeb, 0x2c };

            var sequence = new SequenceBuilder<byte>().Append(segment).Build();

            //Act
            var actual = Crc32C.Calculate(sequence);

            //Assert
            const uint expected = 2355953212;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Calculate_GivenSequenceWithMultipleSegments_ShouldReturnExpectedChecksum()
        {
            //Arrange
            var s1 = new byte[]
            {
                0x0a, 0x0f, 0x73, 0x74, 0x61, 0x6e, 0x64, 0x61, 0x6c, 0x6f,
                0x6e, 0x65, 0x2d, 0x33, 0x30, 0x2d, 0x35, 0x10, 0x00, 0x18,
                0xc7, 0xee, 0xa3, 0x93, 0xeb, 0x2c, 0x58, 0x01
            };

            var s2 = new byte[] { 0x10, 0x01, 0x18, 0xc9, 0xf8, 0x86, 0x94, 0xeb, 0x2c };

            var sequence = new SequenceBuilder<byte>().Append(s1).Append(s2).Build();

            //Act
            var actual = Crc32C.Calculate(sequence);

            //Assert
            const uint expected = 1079987866;

            Assert.Equal(expected, actual);
        }
    }
}
