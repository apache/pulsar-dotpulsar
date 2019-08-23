using DotPulsar.Internal;
using Xunit;

namespace DotPulsar.Tests.Internal
{
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
            Assert.Equal(expected, actual);
        }
    }
}
