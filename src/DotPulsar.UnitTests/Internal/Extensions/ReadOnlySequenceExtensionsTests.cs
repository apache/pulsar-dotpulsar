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

using DotPulsar.Internal;
using DotPulsar.Internal.Extensions;
using Xunit;

namespace DotPulsar.UnitTests.Internal.Extensions
{
    public class ReadOnlySequenceExtensionsTests
    {
        [Fact]
        public void StartsWith_GivenToShortSequenceWithSingleSegment_ShouldReturnFalse()
        {
            //Arrange
            var sequence = new SequenceBuilder<byte>().Append(new byte[] { 0x00 }).Build();

            //Act
            var actual = sequence.StartsWith(new byte[] { 0x00, 0x01 });

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public void StartsWith_GivenSequenceWithSingleSegment_ShouldReturnFalse()
        {
            //Arrange
            var sequence = new SequenceBuilder<byte>().Append(new byte[] { 0x00, 0x02, 0x01 }).Build();

            //Act
            var actual = sequence.StartsWith(new byte[] { 0x00, 0x01 });

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public void StartsWith_GivenSequenceWithSingleSegment_ShouldReturnTrue()
        {
            //Arrange
            var sequence = new SequenceBuilder<byte>().Append(new byte[] { 0x00, 0x01, 0x02 }).Build();

            //Act
            var actual = sequence.StartsWith(new byte[] { 0x00, 0x01 });

            //Assert
            Assert.True(actual);
        }

        [Fact]
        public void StartsWith_GivenToShortSequenceWithMultipleSegments_ShouldReturnFalse()
        {
            //Arrange
            var sequence = new SequenceBuilder<byte>().Append(new byte[] { 0x00, 0x01 }).Append(new byte[] { 0x02 }).Build();

            //Act
            var actual = sequence.StartsWith(new byte[] { 0x00, 0x01, 0x02, 0x03 });

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public void StartsWith_GivenSequenceWithMultipleSegments_ShouldReturnFalse()
        {
            //Arrange
            var sequence = new SequenceBuilder<byte>().Append(new byte[] { 0x00, 0x02 }).Append(new byte[] { 0x01, 0x03 }).Build();

            //Act
            var actual = sequence.StartsWith(new byte[] { 0x00, 0x01, 0x02 });

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public void StartsWith_GivenSequenceWithMultipleSegments_ShouldReturnTrue()
        {
            //Arrange
            var sequence = new SequenceBuilder<byte>().Append(new byte[] { 0x00, 0x01 }).Append(new byte[] { 0x02, 0x03 }).Build();

            //Act
            var actual = sequence.StartsWith(new byte[] { 0x00, 0x01, 0x02 });

            //Assert
            Assert.True(actual);
        }

        [Fact]
        public void ReadUInt32_GivenSequenceWithSingleSegment_ShouldGiveExceptedResult()
        {
            //Arrange
            var sequence = new SequenceBuilder<byte>().Append(new byte[] { 0x00, 0x01, 0x02, 0x03 }).Build();

            //Act
            var actual = sequence.ReadUInt32(0, true);

            //Assert
            const uint expected = 66051;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReadUInt32_GivenSequenceWithSingleSegmentAndNonZeroStart_ShouldGiveExceptedResult()
        {
            //Arrange
            var sequence = new SequenceBuilder<byte>().Append(new byte[] { 0x09, 0x00, 0x01, 0x02, 0x03 }).Build();

            //Act
            var actual = sequence.ReadUInt32(1, true);

            //Assert
            const uint expected = 66051;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReadUInt32_GivenSequenceWithMultipleSegments_ShouldGiveExceptedResult()
        {
            //Arrange
            var sequence = new SequenceBuilder<byte>().Append(new byte[] { 0x00, 0x01 }).Append(new byte[] { 0x02, 0x03 }).Build();

            //Act
            var actual = sequence.ReadUInt32(0, true);

            //Assert
            const uint expected = 66051;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReadUInt32_GivenSequenceWithMultipleSegmentsAndNonZeroStart_ShouldGiveExceptedResult()
        {
            //Arrange
            var sequence = new SequenceBuilder<byte>()
                .Append(new byte[] { 0x09, 0x09, 0x09 })
                .Append(new byte[] { 0x09, 0x00, 0x01 })
                .Append(new byte[] { 0x02, 0x03 }).Build();

            //Act
            var actual = sequence.ReadUInt32(4, true);

            //Assert
            const uint expected = 66051;
            Assert.Equal(expected, actual);
        }
    }
}
