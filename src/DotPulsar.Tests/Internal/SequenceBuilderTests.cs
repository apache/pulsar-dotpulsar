using DotPulsar.Internal;
using System.Buffers;
using Xunit;

namespace DotPulsar.Tests.Internal
{
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
            var sequence = builder.Build();

            //Assert
            var array = sequence.ToArray();
            for (byte i = 0; i < array.Length; ++i)
                Assert.Equal(i, array[i]);
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
            var sequence = builder.Build();

            //Assert
            var array = sequence.ToArray();
            for (byte i = 0; i < array.Length; ++i)
                Assert.Equal(i, array[i]);
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
            var sequence = builder.Build();

            //Assert
            var array = sequence.ToArray();
            for (byte i = 0; i < array.Length; ++i)
                Assert.Equal(i, array[i]);
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
            var sequence = builder.Build();

            //Assert
            var array = sequence.ToArray();
            for (byte i = 0; i < array.Length; ++i)
                Assert.Equal(i, array[i]);
        }
    }
}
