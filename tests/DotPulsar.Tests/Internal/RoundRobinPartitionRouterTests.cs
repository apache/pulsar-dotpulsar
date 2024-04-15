namespace DotPulsar.Tests.Internal;

public class RoundRobinPartitionRouterTests
{
    [Fact]
    public void ChoosePartition_GivenBytes_ShouldReturnNonNegativeInteger()
    {
        // Arrange
        var router = new RoundRobinPartitionRouter();
        var messageMetadata = new MessageMetadata
        {
            KeyBytes = [13, 144, 79, 245] //-179335155
        };

        //Act
        var result = router.ChoosePartition(messageMetadata, 4);

        //Assert
        result.Should().BeInRange(0, 3);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void ChoosePartition_GivenNoMetadata_ShouldReturnNonNegativeInteger(int numberOfPartitions)
    {
        // Arrange
        var router = new RoundRobinPartitionRouter();
        var messageMetadata = new MessageMetadata();

        //Act
        var result = router.ChoosePartition(messageMetadata, numberOfPartitions);

        //Assert
        result.Should().BeInRange(0, numberOfPartitions);
    }
}
