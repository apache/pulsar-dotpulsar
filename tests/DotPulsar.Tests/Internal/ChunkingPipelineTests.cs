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
    using FluentAssertions;
    using System;
    using System.Buffers;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ChunkingPipelineTests
    {
        [Fact]
        public async Task Send_GivenSequenceIsUnderChunkSize_ShouldWriteArrayOnce()
        {
            //Arrange
            var a = new byte[] { 0x00, 0x01, 0x02, 0x03 };
            var b = new byte[] { 0x04, 0x05, 0x06, 0x07 };
            var sequence = new SequenceBuilder<byte>().Append(a).Append(b).Build();
            var mockStream = new MockStream();
            var sut = new ChunkingPipeline(mockStream, 9);

            //Act
            await sut.Send(sequence);

            //Assert
            var expected = sequence.ToArray();
            var actual = mockStream.GetReadOnlySequence();
            actual.ToArray().Should().Equal(expected);
            actual.IsSingleSegment.Should().BeTrue();
        }

        [Theory]
        [InlineData(4, 6, 3, 4, 6, 3)]     // No segments can be merged
        [InlineData(1, 6, 4, 7, 4, null)]  // Can merge a and b
        [InlineData(4, 6, 1, 4, 7, null)]  // Can merge b and c
        public async Task Send_GivenSequenceIsOverChunkSize_ShouldWriteMultipleArrays(int length1, int length2, int length3, int expected1, int expected2, int? expected3)
        {
            //Arrange
            var a = Enumerable.Range(0, length1).Select(i => (byte) i).ToArray();
            var b = Enumerable.Range(length1, length2).Select(i => (byte) i).ToArray();
            var c = Enumerable.Range(length1 + length2, length3).Select(i => (byte) i).ToArray();
            var sequence = new SequenceBuilder<byte>().Append(a).Append(b).Append(c).Build();
            var mockStream = new MockStream();
            var sut = new ChunkingPipeline(mockStream, 8);

            //Act
            await sut.Send(sequence);

            //Assert
            var expected = sequence.ToArray();
            var actual = mockStream.GetReadOnlySequence();
            actual.ToArray().Should().Equal(expected);
            GetNumberOfSegments(actual).Should().Be(expected3.HasValue ? 3 : 2);

            var segmentNumber = 0;
            foreach (var segment in actual)
            {
                switch (segmentNumber)
                {
                    case 0:
                        segment.Length.Should().Be(expected1);
                        break;
                    case 1:
                        segment.Length.Should().Be(expected2);
                        break;
                    case 2:
                        expected3.Should().NotBeNull();
                        segment.Length.Should().Be(expected3);
                        break;
                }
                ++segmentNumber;
            }
        }

        private static int GetNumberOfSegments(ReadOnlySequence<byte> sequence)
        {
            var numberOfSegments = 0;
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
                ++numberOfSegments;
            return numberOfSegments;
        }

        private class MockStream : Stream
        {
            private readonly SequenceBuilder<byte> _builder;

            public MockStream() => _builder = new SequenceBuilder<byte>();
            public override bool CanRead => throw new NotImplementedException();
            public override bool CanSeek => throw new NotImplementedException();
            public override bool CanWrite => true;
            public override long Length => throw new NotImplementedException();
            public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public override void Flush() => throw new NotImplementedException();
            public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
            public override void SetLength(long value) => throw new NotImplementedException();
            public override void Write(byte[] buffer, int offset, int count) => _builder.Append(new ReadOnlyMemory<byte>(buffer, offset, count));
            public ReadOnlySequence<byte> GetReadOnlySequence() => _builder.Build();
        }
    }
}
