namespace DotPulsar.Tests.Internal;

[Trait("Category", "Unit")]
public class SinglePartitionRouterTests
{
    [Fact]
    public void ChoosePartition_GivenBytes_ShouldReturnNonNegativeInteger()
    {
        // Arrange
        var router = new SinglePartitionRouter();
        var messageMetadata = new MessageMetadata
        {
            KeyBytes = [13, 144, 79, 245] //-179335155
        };

        //Act
        var result = router.ChoosePartition(messageMetadata, 4);

        //Assert
        result.ShouldBeInRange(0, 3);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void ChoosePartition_GivenNoMetadata_ShouldReturnNonNegativeInteger(int numberOfPartitions)
    {
        // Arrange
        var router = new SinglePartitionRouter();
        var messageMetadata = new MessageMetadata();

        //Act
        var result = router.ChoosePartition(messageMetadata, numberOfPartitions);

        //Assert
        result.ShouldBeInRange(0, numberOfPartitions);
    }
}
